using UnityEngine;
using System;
using UniRx;
using DG.Tweening;

public class AnimationController : IDisposable
{
    public Subject<Unit> OnApplyComplete { get; } = new Subject<Unit>();

    private readonly CompositeDisposable disposables = new CompositeDisposable();

    private MakeupItemType itemType;

    public void PlayApplyAnimation(MakeupItemType _itemType, RectTransform handRect, Vector2 originalPosition)
    {
        itemType = _itemType;

        Sequence applySequence = DOTween.Sequence();

        switch (itemType)
        {
            case MakeupItemType.Cream:
                applySequence.Append(handRect.DORotate(new Vector3(0, 0, -30), 0.2f)
                    .SetEase(Ease.InOutQuad));
                applySequence.Append(handRect.DORotate(new Vector3(0, 0, 0), 0.2f)
                    .SetEase(Ease.InOutQuad));
                break;
            case MakeupItemType.Lipstick:
                applySequence.Append(handRect.DOAnchorPos(originalPosition, 0.2f)
                    .SetEase(Ease.InOutQuad));
                applySequence.Append(handRect.DOAnchorPos(originalPosition + new Vector2(-20, 0), 0.15f)
                    .SetEase(Ease.InOutQuad));
                applySequence.Append(handRect.DOAnchorPos(originalPosition + new Vector2(20, 0), 0.15f)
                .SetEase(Ease.InOutQuad));
                applySequence.Append(handRect.DOAnchorPos(originalPosition + new Vector2(-20, 0), 0.1f)
                    .SetEase(Ease.InOutQuad));
                applySequence.Append(handRect.DOAnchorPos(originalPosition + new Vector2(20, 0), 0.1f)
                    .SetEase(Ease.InOutQuad));
                break;
            case MakeupItemType.Eyeshadow:
                applySequence.Append(handRect.DOAnchorPos(originalPosition, 0.15f)
                    .SetEase(Ease.InOutQuad));
                applySequence.Append(handRect.DOAnchorPos(originalPosition + new Vector2(-100, 0), 0.2f)
                    .SetEase(Ease.InOutQuad));
                applySequence.Append(handRect.DOAnchorPos(originalPosition + new Vector2(100, 0), 0.2f)
                    .SetEase(Ease.InOutQuad));
                applySequence.Append(handRect.DOAnchorPos(originalPosition + new Vector2(-80, 0), 0.15f)
                    .SetEase(Ease.InOutQuad));
                applySequence.Append(handRect.DOAnchorPos(originalPosition + new Vector2(80, 0), 0.15f)
                    .SetEase(Ease.InOutQuad));
                applySequence.Append(handRect.DOAnchorPos(originalPosition + new Vector2(-60, 0), 0.1f)
                    .SetEase(Ease.InOutQuad));
                applySequence.Append(handRect.DOAnchorPos(originalPosition + new Vector2(60, 0), 0.1f)
                    .SetEase(Ease.InOutQuad));
                break;
        }

        applySequence.OnComplete(() => OnApplyAnimationComplete());
    }

    public void OnApplyAnimationComplete()
    {
        OnApplyComplete.OnNext(Unit.Default);
    }

    public void Dispose()
    {
        OnApplyComplete.Dispose();
        disposables.Dispose();
    }
}