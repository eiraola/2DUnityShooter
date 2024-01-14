using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
[System.Serializable]

public class OnPlatePressed: UnityEvent<bool> { }
public class PressPlate : MonoBehaviour, IInteractable
{
    [SerializeField]
    private OnPlatePressed OnPlateInteractionEvent = new OnPlatePressed();
    [SerializeField]
    private GameObject InactiveSprite;
    [SerializeField]
    private GameObject ActiveSprite;
    [SerializeField]
    private EInteractorType type = EInteractorType.Blue;
    private int numberOfElementsIn = 0;

    public void Activate()
    {
        throw new System.NotImplementedException();
    }

    public void Deactivate()
    {
        throw new System.NotImplementedException();
    }

    public void Interact(bool state)
    {
        ActiveSprite.SetActive(state);
        InactiveSprite.SetActive(!state);
        TriggerInteractables(state);
    }

    public void TriggerInteractables(bool state)
    {
        OnPlateInteractionEvent?.Invoke(state);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.gameObject.TryGetComponent<IInteractor>(out IInteractor interactor))
        {
            return;
        }
        if (interactor.GetInteractionType().Equals(type))
        {
            numberOfElementsIn++;
        }
        if (numberOfElementsIn == 1)
        {
            Interact(true);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.gameObject.TryGetComponent<IInteractor>(out IInteractor interactor))
        {
            return;
        }
        if (interactor.GetInteractionType().Equals(type))
        {
            numberOfElementsIn--;
        }
        if (numberOfElementsIn == 0)
        {
            Interact(false);
        }
    }
}
