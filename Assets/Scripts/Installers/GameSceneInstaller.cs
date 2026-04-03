using UnityEngine;
using Zenject;

public class GameSceneInstaller : MonoInstaller
{
    [Header("Views")]
    [SerializeField] private HandView handView;
    [SerializeField] private CharacterView characterView;
    [SerializeField] private FaceZoneView faceZoneView;
    [SerializeField] private SpongeView spongeView;
    [SerializeField] private MakeupItemView[] makeupItems;

    [Header("Settings")]
    [SerializeField] private GameObject dragControllerObject;

    public override void InstallBindings()
    {
        Container.Bind<CharacterModel>().AsSingle();
        Container.Bind<HandModel>().AsSingle();

        Container.BindInterfacesAndSelfTo<MakeupController>().AsSingle();
        Container.BindInterfacesAndSelfTo<AnimationController>().AsSingle();

        Container.BindInstance(handView).AsSingle();
        Container.BindInstance(characterView).AsSingle();
        Container.BindInstance(faceZoneView).AsSingle();
        Container.BindInstance(spongeView).AsSingle();

        foreach (var item in makeupItems)
        {
            Container.BindInstance(item).WithId(item.ItemType).AsCached();
        }

        if (dragControllerObject != null)
        {
            Container.BindInstance(dragControllerObject.GetComponent<DragController>()).AsSingle();
        }
    }
}
