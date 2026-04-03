using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Zenject;
using UniRx;

public class HandView : MonoBehaviour
{
    [SerializeField] private RectTransform lips;
    [SerializeField] private RectTransform eyes;
    [SerializeField] private RectTransform handRect;
    [SerializeField] private Image handImage;
    [SerializeField] private Image brushImage;
    [SerializeField] private Image brushTipImage;
    [SerializeField] private Image creamImage;
    [SerializeField] private Image lipstickImage;


    private Sprite currentBrushTipSprite;
    private Sprite currentLipstickSprite;
    private MakeupItemType itemType;


    [SerializeField] private float moveDuration = 0.5f;
    [SerializeField] private Ease moveEase = Ease.InOutQuad;
    [SerializeField] private Vector2 holdingPositionOffset = new Vector2(0, 100);
    [SerializeField] private int eyeshadowDipCount = 3;
    [SerializeField] private float eyeshadowDipDuration = 0.2f;

    private HandModel model;
    private AnimationController animationController;
    private Tween currentTween;
    private CompositeDisposable disposables = new CompositeDisposable();

    public Subject<Unit> OnMoveToItemComplete { get; } = new Subject<Unit>();
    public Subject<Unit> OnMoveToHoldingPositionComplete { get; } = new Subject<Unit>();
    public Subject<Unit> OnEyeshadowPickComplete { get; } = new Subject<Unit>();

    private Vector2 _targetItemPosition;
    private Vector2 _targetPalettePosition;

    [Inject]
    public void Construct(HandModel handModel, AnimationController animController)
    {
        model = handModel;
        animationController = animController;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        model.DefaultPosition = handRect.anchoredPosition;
        model.HoldingPosition = model.DefaultPosition + holdingPositionOffset;

        model.CurrentState
            .Subscribe(OnStateChanged)
            .AddTo(disposables);

        model.HeldItemType
            .Subscribe(UpdateType)
            .AddTo(disposables);

        model.CurrentLipstickSprite
            .Subscribe(sprite => UpdateCurrentLipstickSprite(sprite))
            .AddTo(disposables);

        model.CurrentBrushTipSprite
            .Subscribe(sprite => UpdateCurrentBrushTipSprite(sprite))
            .AddTo(disposables);

        model.Position
            .Subscribe(UpdatePosition)
            .AddTo(disposables);

        model.ItemTargetPosition
            .Subscribe(pos => _targetItemPosition = pos)
            .AddTo(disposables);

        model.EyeshadowPalettePosition
            .Subscribe(pos => _targetPalettePosition = pos)
            .AddTo(disposables);

        model.CurrentMakeupData
            .Subscribe(data =>
            {
                if (data != null && data.itemType == MakeupItemType.Eyeshadow)
                {
                    brushTipImage.enabled = true;
                    brushTipImage.sprite = data.brushColor;
                }
                else
                {
                    brushTipImage.enabled = false;
                }
            })
            .AddTo(disposables);
    }

    private void OnStateChanged(HandState state)
    {
        switch (state)
        {
            case HandState.MovingToItem:
                AnimateToItem();
                break;

            case HandState.MovingToBrush:
                AnimateToBrush();
                break;

            case HandState.MovingToPalette:
                AnimateToPalette();
                break;

            case HandState.PickingEyeshadow:
                AnimatePickEyeshadow();
                break;

            case HandState.MovingToHoldingPosition:
                AnimateToHoldingPosition();
                break;

            case HandState.Holding:
                
                break;

            case HandState.Applying:
                AnimateApply();
                break;

            case HandState.Returning:
                AnimateReturnToDefault();
                break;

            case HandState.Idle:
                AnimateToPosition(model.DefaultPosition, 0.3f);
                break;
        }
    }

    private void AnimateToItem()
    {
        currentTween?.Kill();

        currentTween = handRect.DOAnchorPos(_targetItemPosition, moveDuration)
            .SetEase(moveEase)
            .OnComplete(() =>
            {
                UpdateHand();
                OnMoveToItemComplete.OnNext(Unit.Default);
            });
    }

    private void AnimateToBrush()
    {
        currentTween?.Kill();

        currentTween = handRect.DOAnchorPos(_targetItemPosition, moveDuration)
            .SetEase(moveEase)
            .OnComplete(() =>
            {
                UpdateHand();
                OnMoveToItemComplete.OnNext(Unit.Default);
            });
    }

    private void AnimateToPalette()
    {
        currentTween?.Kill();

        currentTween = handRect.DOAnchorPos(_targetPalettePosition, moveDuration)
            .SetEase(moveEase)
            .OnComplete(() =>
            {
                model.OnReachedPalette();
            });
    }

    private void AnimatePickEyeshadow()
    {
        var sequence = DOTween.Sequence();

        for (int i = 0; i < eyeshadowDipCount; i++)
        {
            sequence.Append(handRect.DOAnchorPos(_targetPalettePosition + new Vector2(0, -20), eyeshadowDipDuration)
                .SetEase(Ease.InQuad));
            sequence.Append(handRect.DOAnchorPos(_targetPalettePosition, eyeshadowDipDuration)
                .SetEase(Ease.OutQuad));
        }

        sequence.AppendInterval(0.1f);

        sequence.OnComplete(() =>
        {
            OnEyeshadowPickComplete.OnNext(Unit.Default);
        });
    }

    private void AnimateToHoldingPosition()
    {
        currentTween?.Kill();

        currentTween = handRect.DOAnchorPos(model.HoldingPosition, moveDuration)
            .SetEase(moveEase)
            .OnComplete(() =>
            {
                OnMoveToHoldingPositionComplete.OnNext(Unit.Default);
            });
    }

    private void AnimateReturnToDefault()
    {
        currentTween?.Kill();

        currentTween = handRect.DOAnchorPos(_targetItemPosition, moveDuration)
            .SetEase(moveEase)
            .OnComplete(() =>
            {
                UpdateHand();

                handRect.DOAnchorPos(model.DefaultPosition, moveDuration)
                    .SetEase(Ease.OutBack)
                    .OnComplete(() =>
                    {
                        model.ReturnItem();
                        model.CurrentState.Value = HandState.Idle;
                    });
            });
    }

    private void UpdateType(MakeupItemType _itemType)
    {
        itemType = _itemType;

        if (itemType == MakeupItemType.None)
        {
            UpdateHand();
        }
    }

    private void UpdateHand()
    {
        switch (itemType)
        {
            case MakeupItemType.Cream:
                creamImage.enabled = true;
                brushImage.enabled = false;
                brushTipImage.enabled = false;
                lipstickImage.enabled = false;
                break;

            case MakeupItemType.Eyeshadow:
                creamImage.enabled = false;
                brushImage.enabled = true;
                brushTipImage.enabled = true;
                lipstickImage.enabled = false;
                brushTipImage.sprite = currentBrushTipSprite;
                break;

            case MakeupItemType.Lipstick:
                creamImage.enabled = false;
                brushImage.enabled = false;
                brushTipImage.enabled = false;
                lipstickImage.enabled = true;

                lipstickImage.sprite = currentLipstickSprite;
                break;

            default:
                creamImage.enabled = false;
                brushImage.enabled = false;
                brushTipImage.enabled = false;
                lipstickImage.enabled = false;
                break;
        }
    }

    private void UpdatePosition(Vector2 position)
    {
        if (model.CurrentState.Value == HandState.Dragging)
        {
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                handRect.parent as RectTransform,
                position,
                null,
                out Vector2 localPoint))
            {
                handRect.anchoredPosition = localPoint;
            }
        }
    }

    private void AnimateApply()
    {
        Vector2 originalPosition = itemType switch
        {
            MakeupItemType.Lipstick => lips.anchoredPosition,
            MakeupItemType.Eyeshadow => eyes.anchoredPosition,
            _ => handRect.anchoredPosition
        };

        animationController.PlayApplyAnimation(itemType, handRect, originalPosition);
    }

    private Tween AnimateToPosition(Vector2 targetPosition, float duration)
    {
        currentTween?.Kill();
        currentTween = handRect.DOAnchorPos(targetPosition, duration)
            .SetEase(Ease.OutCubic);
        return currentTween;
    }

    private void UpdateCurrentBrushTipSprite(Sprite sprite)
    {
        currentBrushTipSprite = sprite;
    }

    private void UpdateCurrentLipstickSprite(Sprite sprite)
    {
        currentLipstickSprite = sprite;
    }

    private void OnDestroy()
    {
        disposables.Dispose();
        OnMoveToItemComplete.Dispose();
        OnMoveToHoldingPositionComplete.Dispose();
        OnEyeshadowPickComplete.Dispose();
    }
}
