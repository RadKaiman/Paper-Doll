using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Zenject;
using UniRx;
using System.Collections.Generic;

public class EyeshadowPaletteView : MonoBehaviour
{
    [SerializeField] private Transform brushObject;
    [SerializeField] private Image brushImage;
    [SerializeField] private List<EyeshadowSlot> eyeshadowSlots;

    private MakeupController makeupController;
    private HandModel handModel;
    private HandView handView;
    private RectTransform brushRect;
    private CompositeDisposable disposables = new CompositeDisposable();

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
        brushRect = brushObject.GetComponent<RectTransform>();

        foreach (var slot in eyeshadowSlots)
        {
            if (slot.eyeshadowData != null && slot.colorPosition != null)
            {
                var button = slot.colorPosition.GetComponent<Button>();
                var capturedSlot = slot;
                button.onClick.AddListener(() => OnColorClicked(capturedSlot));
            }
        }

        handView.OnMoveToItemComplete
            .Subscribe(_ => OnHandReachedBrush())
            .AddTo(disposables);

        handModel.CurrentState
            .Where(state => state == HandState.Returning && handModel.HeldItemType.Value == MakeupItemType.Eyeshadow)
            .Subscribe(_ => OnBrushReturned())
            .AddTo(disposables);
    }

    private void OnColorClicked(EyeshadowSlot slot)
    {
        if (handModel.CurrentState.Value != HandState.Idle) return;

        slot.colorPosition.DOScale(0.9f, 0.1f).OnComplete(() =>
            slot.colorPosition.DOScale(1f, 0.1f));

        makeupController.SelectEyeshadow(
            slot.eyeshadowData,
            brushRect.anchoredPosition,
            slot.colorPosition.anchoredPosition + new Vector2(0, -300));
    }

    private void OnHandReachedBrush()
    {
        if (handModel.HeldItemType.Value == MakeupItemType.Eyeshadow)
        {
            brushImage.enabled = false;
        }
    }

    private void OnBrushReturned()
    {
        brushImage.enabled = true;
    }

    private void OnDestroy()
    {
        disposables.Dispose();
    }
}

[System.Serializable]
public class EyeshadowSlot
{
    public MakeupItemData eyeshadowData;
    public RectTransform colorPosition;
}