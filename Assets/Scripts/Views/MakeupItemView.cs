using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using Zenject;
using UniRx;

public class MakeupItemView : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private MakeupItemData itemData;
    [SerializeField] private Image itemImage;

    private MakeupController makeupController;
    private HandModel handModel;
    private HandView handView;
    private RectTransform rectTransform;
    private Tween clickTween;
    private CompositeDisposable disposables = new CompositeDisposable();

    public MakeupItemType ItemType => itemData.itemType;
    public Vector2 Position => rectTransform.anchoredPosition;

    [Inject]
    public void Construct(MakeupController makeupCtrl, HandModel handModelRef, HandView handViewRef)
    {
        makeupController = makeupCtrl;
        handModel = handModelRef;
        handView = handViewRef;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();

        handView.OnMoveToItemComplete
            .Subscribe(_ => OnHandReachedItem())
            .AddTo(disposables);

        handModel.CurrentState
            .Where(state => state == HandState.Returning && handModel.HeldItemType.Value == ItemType)
            .Subscribe(_ => OnItemReturned())
            .AddTo(disposables);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        if (handModel.CurrentState.Value != HandState.Idle) return;

        AnimateClick();

        switch (ItemType)
        {
            case MakeupItemType.Cream:
                makeupController.SelectCream(Position);
                break;
            case MakeupItemType.Lipstick:
                if (itemData != null)
                {
                    makeupController.SelectLipstick(itemData, Position);
                }
                break;
        }
    }

    private void OnHandReachedItem()
    {
        if (handModel.HeldItemType.Value == ItemType)
        {
            itemImage.enabled = false;
        }
    }

    private void OnItemReturned()
    {
        itemImage.enabled = true;
    }

    private void AnimateClick()
    {
        clickTween?.Kill();
        clickTween = transform
            .DOScale(0.9f, 0.1f)
            .SetEase(Ease.OutQuad)
            .OnComplete(() => transform.DOScale(1f, 0.1f));
    }

    private void OnDestroy()
    {
        disposables.Dispose();
    }
}
