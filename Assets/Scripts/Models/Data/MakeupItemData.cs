using UnityEngine;

[CreateAssetMenu(fileName = "MakeupItem", menuName = "Scriptable Objects/MakeupItem")]
public class MakeupItemData : ScriptableObject
{
    public MakeupItemType itemType;
    public string itemName;

    public Sprite makeupSprite;
    public Sprite itemSprite;
    public Sprite brushColor;
    public float animationDuration = 0.3f;
}
