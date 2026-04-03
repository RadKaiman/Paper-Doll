using UnityEngine;
using UniRx;

public class HandModel
{
    public IReactiveProperty<HandState> CurrentState { get; } = new ReactiveProperty<HandState>(HandState.Idle);
    public IReactiveProperty<MakeupItemType> HeldItemType { get; } = new ReactiveProperty<MakeupItemType>(MakeupItemType.None);
    public IReactiveProperty<MakeupItemData> CurrentMakeupData { get; } = new ReactiveProperty<MakeupItemData>(null);
    public IReactiveProperty<Sprite> CurrentBrushTipSprite { get; } = new ReactiveProperty<Sprite>(null);
    public IReactiveProperty<Sprite> CurrentLipstickSprite { get; } = new ReactiveProperty<Sprite>(null);
    public IReactiveProperty<Vector2> Position { get; } = new ReactiveProperty<Vector2>(Vector2.zero);
    public IReactiveProperty<Vector2> ItemTargetPosition { get; } = new ReactiveProperty<Vector2>(Vector2.zero);
    public IReactiveProperty<Vector2> EyeshadowPalettePosition { get; } = new ReactiveProperty<Vector2>(Vector2.zero);
    public IReactiveProperty<bool> IsDragging { get; } = new ReactiveProperty<bool>(false);

    public Vector2 DefaultPosition { get; set; }
    public Vector2 HoldingPosition { get; set; }

    public void StartMoveToItem(MakeupItemType itemType, MakeupItemData makeupData, Vector2 itemPosition)
    {
        CurrentMakeupData.Value = makeupData;
        if (itemType != MakeupItemType.Cream)
            CurrentLipstickSprite.Value = makeupData.itemSprite;
        HeldItemType.Value = itemType;
        ItemTargetPosition.Value = itemPosition;
        CurrentState.Value = HandState.MovingToItem;
    }

    public void StartEyeshadowProcess(MakeupItemData eyeshadowData, Vector2 brushPosition, Vector2 palettePosition)
    {
        CurrentMakeupData.Value = eyeshadowData;
        CurrentBrushTipSprite.Value = eyeshadowData.itemSprite;
        HeldItemType.Value = MakeupItemType.Eyeshadow;
        ItemTargetPosition.Value = brushPosition;
        EyeshadowPalettePosition.Value = palettePosition;
        CurrentState.Value = HandState.MovingToBrush;
    }

    public void OnReachedItem()
    {
        if (CurrentState.Value == HandState.MovingToItem)
        {
            CurrentState.Value = HandState.MovingToHoldingPosition;
        }
        else if (CurrentState.Value == HandState.MovingToBrush)
        {
            CurrentState.Value = HandState.MovingToPalette;
        }
    }

    public void OnReachedPalette()
    {
        if (CurrentState.Value == HandState.MovingToPalette)
        {
            CurrentState.Value = HandState.PickingEyeshadow;
        }
    }

    public void OnEyeshadowPicked()
    {
        if (CurrentState.Value == HandState.PickingEyeshadow)
        {
            CurrentState.Value = HandState.MovingToHoldingPosition;
        }
    }

    public void OnReachedHoldingPosition()
    {
        if (CurrentState.Value == HandState.MovingToHoldingPosition)
        {
            CurrentState.Value = HandState.Holding;
        }
    }

    public void StartDrag()
    {
        if (CurrentState.Value == HandState.Holding)
        {
            IsDragging.Value = true;
            CurrentState.Value = HandState.Dragging;
        }
    }

    public void EndDrag(bool success)
    {
        IsDragging.Value = false;
        CurrentState.Value = success ? HandState.Applying : HandState.Returning;
    }

    public void ReturnItem()
    {
        HeldItemType.Value = MakeupItemType.None;
        CurrentMakeupData.Value = null;
    }
}

public enum HandState
{
    Idle,                       
    MovingToItem,               
    MovingToBrush,
    MovingToPalette, 
    PickingEyeshadow,
    MovingToHoldingPosition, 
    Holding, 
    Dragging,
    Applying, 
    Returning 
}

public enum MakeupItemType
{
    None,
    Cream,
    Eyeshadow,
    Lipstick
}
