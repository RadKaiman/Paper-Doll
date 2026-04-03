using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using Zenject;

public class SpongeView : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Image spongeImage;

    private MakeupController makeupController;
    private Tween clickTween;

    [Inject]
    public void Construct(MakeupController makeupCtrl)
    {
        makeupController = makeupCtrl;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        AnimateClick();
        makeupController.ClearAllMakeup();
    }

    private void AnimateClick()
    {
        clickTween?.Kill();
        clickTween = transform
            .DOScale(0.85f, 0.1f)
            .SetEase(Ease.OutQuad)
            .OnComplete(() => transform.DOScale(1f, 0.15f).SetEase(Ease.OutBack));
    }
}
