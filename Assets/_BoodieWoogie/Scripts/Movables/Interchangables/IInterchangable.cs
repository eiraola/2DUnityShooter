using UnityEngine;
public interface IInterchangable 
{
    public void Select(EInterchangableSide side);
    public void Unselect();
    public bool IsSelected();
    public void Interchange(Vector2 position);
    public GameObject GetGameObject();
}
