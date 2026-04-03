using UnityEngine;
using System;
using Zenject;
using UniRx;

public class MakeupController : IInitializable, IDisposable
{
    private readonly CharacterModel characterModel;
    private readonly HandModel handModel;
    private readonly AnimationController animationController;
    private readonly HandView handView;
    private readonly CompositeDisposable disposables = new CompositeDisposable();
    private MakeupItemData currentMakeupData;

    [Inject]
    public MakeupController(CharacterModel characterModelRef, HandModel handModelRef, AnimationController animController, HandView handViewRef)
    {
        characterModel = characterModelRef;
        handModel = handModelRef;
        animationController = animController;
        handView = handViewRef;
    }

    public void Initialize()
    {
        handView.OnMoveToItemComplete
            .Subscribe(_ => OnHandReachedItem())
            .AddTo(disposables);

        handView.OnMoveToHoldingPositionComplete
            .Subscribe(_ => OnHandReachedHoldingPosition())
            .AddTo(disposables);

        handView.OnEyeshadowPickComplete
            .Subscribe(_ => OnEyeshadowPickComplete())
            .AddTo(disposables);

        animationController.OnApplyComplete
            .Subscribe(_ => OnApplicationComplete())
            .AddTo(disposables);
    }

    public void SelectCream(Vector2 itemPosition)
    {
        if (handModel.CurrentState.Value != HandState.Idle) return;

        currentMakeupData = null;
        handModel.StartMoveToItem(MakeupItemType.Cream, null, itemPosition);
    }

    public void SelectLipstick(MakeupItemData lipstickData, Vector2 itemPosition)
    {
        if (handModel.CurrentState.Value != HandState.Idle) return;

        currentMakeupData = lipstickData;
        handModel.StartMoveToItem(MakeupItemType.Lipstick, lipstickData, itemPosition);
    }

    public void SelectEyeshadow(MakeupItemData eyeshadowData, Vector2 brushPosition, Vector2 palettePosition)
    {
        if (handModel.CurrentState.Value != HandState.Idle) return;

        currentMakeupData = eyeshadowData;
        handModel.StartEyeshadowProcess(eyeshadowData, brushPosition, palettePosition);
    }

    private void OnHandReachedItem()
    {
        handModel.OnReachedItem();
    }

    private void OnEyeshadowPickComplete()
    {
        handModel.OnEyeshadowPicked();
    }

    private void OnHandReachedHoldingPosition()
    {
        handModel.OnReachedHoldingPosition();
    }

    public void ApplyCurrentItem()
    {
        switch (handModel.HeldItemType.Value)
        {
            case MakeupItemType.Cream:
                characterModel.ApplyCream();
                break;
            case MakeupItemType.Eyeshadow:
                if (currentMakeupData != null)
                {
                    characterModel.ApplyEyeshadow(currentMakeupData.makeupSprite);
                }
                break;
            case MakeupItemType.Lipstick:
                if (currentMakeupData != null)
                {
                    characterModel.ApplyLipstick(currentMakeupData.makeupSprite);
                }
                break;
        }

        handModel.CurrentState.Value = HandState.Applying;
    }

    public void ClearAllMakeup()
    {
        characterModel.ClearAll();
    }

    private void OnApplicationComplete()
    {
        handModel.CurrentState.Value = HandState.Returning;
    }

    public void Dispose()
    {
        disposables.Dispose();
    }
}
