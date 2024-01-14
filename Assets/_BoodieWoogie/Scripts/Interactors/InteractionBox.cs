using UnityEngine;

public class InteractionBox : MonoBehaviour, IInteractor
{
    [SerializeField]
    private EInteractorType type = EInteractorType.Blue;
    public EInteractorType GetInteractionType()
    {
        return type;
    }
}
