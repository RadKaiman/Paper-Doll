using UnityEngine;
using UniRx;

public class CharacterModel
{
    public IReactiveProperty<MakeupState> CurrentMakeupState { get; } = new ReactiveProperty<MakeupState>(MakeupState.WithAcne);
    public IReactiveProperty<bool> HasAcne { get; } = new ReactiveProperty<bool>(true);
    public IReactiveProperty<bool> HasEyeshadow { get; } = new ReactiveProperty<bool>(false);
    public IReactiveProperty<bool> HasLipstick { get; } = new ReactiveProperty<bool>(false);
    public IReactiveProperty<Sprite> CurrentEyeshadowSprite { get; } = new ReactiveProperty<Sprite>(null);
    public IReactiveProperty<Sprite> CurrentLipstickSprite { get; } = new ReactiveProperty<Sprite>(null);

    public void ApplyCream()
    {
        HasAcne.Value = false;
        UpdateState();
    }

    public void ApplyEyeshadow(Sprite eyeshadowSprite)
    {
        HasEyeshadow.Value = true;
        CurrentEyeshadowSprite.Value = eyeshadowSprite;
        UpdateState();
    }

    public void ApplyLipstick(Sprite lipstickSprite)
    {
        HasLipstick.Value = true;
        CurrentLipstickSprite.Value = lipstickSprite;
        UpdateState();
    }

    public void ClearAll()
    {
        HasAcne.Value = true;
        HasEyeshadow.Value = false;
        HasLipstick.Value = false;
        CurrentEyeshadowSprite.Value = null;
        CurrentLipstickSprite.Value = null;
        UpdateState();
    }

    private void UpdateState()
    {
        CurrentMakeupState.Value = MakeupState.None;
        if (HasAcne.Value)
        {
            CurrentMakeupState.Value = MakeupState.WithAcne;
        }
        else if (!HasEyeshadow.Value && !HasLipstick.Value)
        {
            CurrentMakeupState.Value = MakeupState.Clean;
        }
        else if (HasEyeshadow.Value && !HasLipstick.Value)
        {
            CurrentMakeupState.Value = MakeupState.WithEyeshadow;
        }
        else if (!HasEyeshadow.Value && HasLipstick.Value)
        {
            CurrentMakeupState.Value = MakeupState.WithLipstick;
        }
        else
        {
            CurrentMakeupState.Value = MakeupState.MakeupApplied;
        }
    }
}

public enum MakeupState
{
    None,
    WithAcne,
    Clean,
    WithEyeshadow,
    WithLipstick,
    MakeupApplied
}
