using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;
using UniRx;

public class DragController : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private HandModel handModel;
    private FaceZoneView faceZone;
    private MakeupController makeupController;
    private CompositeDisposable disposables = new CompositeDisposable();
    private bool isDraggingEnabled = false;

    [Inject]
    public void Construct(HandModel handModelRef, FaceZoneView faceZoneRef, MakeupController makeupCtrl)
    {
        handModel = handModelRef;
        faceZone = faceZoneRef;
        makeupController = makeupCtrl;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        handModel.IsDragging
        .Subscribe(isDragging =>
        {
            isDraggingEnabled = isDragging;
            Debug.Log(isDragging);
        })
        .AddTo(disposables);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        handModel.StartDrag();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDraggingEnabled) return;

        handModel.Position.Value = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isDraggingEnabled) return;

        bool success = faceZone.IsPointerOverZone(eventData.position);
        if (success)
        {
            handModel.IsDragging.Value = false;
            makeupController.ApplyCurrentItem();
        }
    }

    private void OnDestroy()
    {
        disposables.Dispose();
    }
}
