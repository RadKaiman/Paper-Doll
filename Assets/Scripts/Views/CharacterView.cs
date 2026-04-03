using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Zenject;
using UniRx;

public class CharacterView : MonoBehaviour
{
    [SerializeField] private Image characterImage;
    [SerializeField] private Image acneImage;
    [SerializeField] private Image eyeshadowImage;
    [SerializeField] private Image lipstickImage;

    private CharacterModel charModel;
    private Sprite currentEyeshadowSprite;
    private Sprite currentLipstickSprite;

    private Tween currentTween;
    private CompositeDisposable disposables = new CompositeDisposable();

    [Inject]
    public void Construct(CharacterModel characterModel)
    {
        charModel = characterModel;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        charModel.CurrentEyeshadowSprite
            .Subscribe(sprite => UpdateCurrentEyeshadowSprite(sprite))
            .AddTo(disposables);

        charModel.CurrentLipstickSprite
            .Subscribe(sprite => UpdateCurrentLipstickSprite(sprite))
            .AddTo(disposables);

        charModel.CurrentMakeupState
            .Subscribe(state => UpdateSprite(state))
            .AddTo(disposables);

        charModel.HasAcne
            .Skip(1)
            .Subscribe(_ => AnimateChange())
            .AddTo(disposables);

        UpdateSprite(charModel.CurrentMakeupState.Value);
    }

    private void UpdateSprite(MakeupState state)
    {
        switch (state)
        {
            case MakeupState.WithAcne:
                acneImage.enabled = true;
                eyeshadowImage.enabled = false;
                lipstickImage.enabled = false;
                break;
            case MakeupState.Clean:
                acneImage.enabled = false;
                eyeshadowImage.enabled = false;
                lipstickImage.enabled = false;
                break;
            case MakeupState.WithEyeshadow:
                eyeshadowImage.enabled = true;
                eyeshadowImage.sprite = currentEyeshadowSprite;
                break;
            case MakeupState.WithLipstick:
                lipstickImage.enabled = true;
                lipstickImage.sprite = currentLipstickSprite;
                break;
            case MakeupState.MakeupApplied:
                if (eyeshadowImage.enabled == true)
                {
                    lipstickImage.enabled = true;
                    lipstickImage.sprite = currentLipstickSprite;
                    eyeshadowImage.enabled = true;
                    eyeshadowImage.sprite = currentEyeshadowSprite;
                }
                else if (lipstickImage.enabled == true)
                {
                    eyeshadowImage.enabled = true;
                    eyeshadowImage.sprite = currentEyeshadowSprite;
                    lipstickImage.enabled = true;
                    lipstickImage.sprite = currentLipstickSprite;
                }
                break;
        }
        AnimateChange();
    }

    private void UpdateCurrentEyeshadowSprite(Sprite sprite)
    {
        currentEyeshadowSprite = sprite;
    }

    private void UpdateCurrentLipstickSprite(Sprite sprite)
    {
        currentLipstickSprite = sprite;
    }

    private void AnimateChange()
    {
        currentTween?.Kill();
        currentTween = characterImage.transform.DOPunchScale(Vector3.one * 0.15f, 0.25f, 2, 0).SetEase(Ease.OutBack);
    }

    private void OnDestroy()
    {
        disposables.Dispose();
    }
}
