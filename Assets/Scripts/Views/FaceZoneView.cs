using UnityEngine;
using UniRx;

public class FaceZoneView : MonoBehaviour
{
    [SerializeField] private RectTransform zoneRect;
    [SerializeField] private Canvas canvas;

    private CompositeDisposable disposables = new CompositeDisposable();

    public bool IsPointerOverZone(Vector2 screenPosition)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            zoneRect,
            screenPosition,
            null,
            out Vector2 localPoint);

        return zoneRect.rect.Contains(localPoint);
    }

    private void OnDestroy()
    {
        disposables.Dispose();
    }
}
