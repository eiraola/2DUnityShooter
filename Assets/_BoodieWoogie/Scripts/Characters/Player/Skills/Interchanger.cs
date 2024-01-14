using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EInterchangableSide
{
    R,
    L
}
public struct InterchangableSlot
{
    public int interchangableIndex;
    public IInterchangable interchangable;
    public EInterchangableSide side;
    public Vector3 Position
    {
        get {
            if (!IsValid())
            {
                Debug.LogError("[InterchangableSlot: Position] Error: No interchangable selected.");
            }
            return interchangable.GetGameObject().transform.position; 
        }
    }
    public InterchangableSlot(EInterchangableSide side){
        interchangableIndex = 0;
        interchangable = null;
        this.side = side;
    }
    public void SelectInterChangable(List<IInterchangable> interchangables )
    {
        if (interchangables.Count == 0)
        {
            return;
        }

        interchangableIndex++;
        if (interchangable == null || interchangableIndex >= interchangables.Count)
        {
            interchangableIndex = 0;
        }

        if (interchangable != null)
        {
            interchangable.Unselect();
        }

        interchangable = GetFirstValidInterchangable(interchangables);

        if (interchangable != null)
        {
            interchangable.Select(side);
        }
    }
    public IInterchangable GetFirstValidInterchangable(List<IInterchangable> interchangables)
    {
        int currentCheckingIndex = interchangableIndex;
        if (!interchangables[currentCheckingIndex].IsSelected())
        {
            return interchangables[currentCheckingIndex];
        }
        currentCheckingIndex++;
        while (currentCheckingIndex != interchangableIndex)
        {
            if (currentCheckingIndex >= interchangables.Count)
            {
                currentCheckingIndex = 0;
            }
            if (!interchangables[currentCheckingIndex].IsSelected())
            {
                interchangableIndex = currentCheckingIndex;
                return interchangables[currentCheckingIndex];
            }
            currentCheckingIndex++;
        }
        return null;
        
    }
    public bool IsValid()
    {
        return interchangable != null;
    }
    public void Interchange(Vector3 newPos)
    {
        if (!IsValid())
        {
            return;
        }
        interchangable.Interchange(GetValidPosition(newPos));
        interchangable.Unselect();
        interchangable = null;
       
    }
    private Vector3 GetValidPosition(Vector3 interchangePoint)
    {
        Vector3 origin = interchangable.GetGameObject().transform.position;
        Vector3 direction = interchangePoint - origin;
        LayerMask lm = LayerMask.NameToLayer(Constants.LAYER_TINTERRUPTOR);
        RaycastHit2D ray = Physics2D.Raycast(origin, direction.normalized, direction.magnitude * 1.1F, 1 << lm);
        Debug.DrawRay(origin,  direction, side == EInterchangableSide.L?Color.black:Color.blue, 10.0f);
        if (ray.collider == null)
        {
            return interchangePoint;
        }
        if (ray.distance < direction.magnitude)
        {
            return CorrectAppearancePositionAfterInterruption(ray.point, (direction.x * Vector3.right).normalized);
        }
        return interchangePoint;
    }
    private Vector3 CorrectAppearancePositionAfterInterruption(Vector3 impactPoint, Vector3 direction)
    {
        Collider2D collider = interchangable.GetGameObject().GetComponent<Collider2D>();
        float colliderSize = Mathf.Abs(collider.bounds.min.x - collider.bounds.max.x)/2;
        return impactPoint - direction * (colliderSize + 0.1f);
    }
    public void Reset()
    {
        if (!IsValid())
        {
            return;
        }
        interchangable.Unselect();
        interchangable = null;
    }
    
}
public class Interchanger : MonoBehaviour
{
    [SerializeField]
    private PlayerInput playerInput;
    private List<IInterchangable> interchangables = new List<IInterchangable>();
    private IInterchangable playerInterchangable;
    private InterchangableSlot interchangableL;
    private InterchangableSlot interchangableR;

    void Start()
    {
        GetInterchangables();
        interchangableL = new InterchangableSlot(EInterchangableSide.L);
        interchangableR = new InterchangableSlot(EInterchangableSide.R);
    }
    private void OnEnable()
    {
        playerInput.OnSelectEvent += SelectInterChangable;
        playerInput.OnInterchangeEvent += Interchange;
        playerInput.OnStartEvent += CancelInterchange;
    }
    private void OnDisable()
    {
        playerInput.OnSelectEvent -= SelectInterChangable;
        playerInput.OnInterchangeEvent -= Interchange;
        playerInput.OnStartEvent -= CancelInterchange;
    }
    private void GetInterchangables()
    {
        GameObject[] interchangablesGO = GameObject.FindGameObjectsWithTag(Constants.TAG_INTERCHANGABLE);
        GameObject playerGO = GameObject.FindGameObjectWithTag(Constants.TAG_PLAYER);
        interchangables = new List<IInterchangable>();
        if (playerGO.TryGetComponent<IInterchangable>(out IInterchangable interchangablePlayer))
        {
            playerInterchangable = interchangablePlayer;
            interchangables.Add(interchangablePlayer);
        }
        for (int i = 0; i < interchangablesGO.Length; i++)
        {

            if (interchangablesGO[i].TryGetComponent<IInterchangable>(out IInterchangable interchangable))
            {
                interchangables.Add(interchangable);
            }
        }
    }
    void SelectInterChangable(EInterchangableSide side)
    {
        GameManager.Instance.StopGame();
        if (side == EInterchangableSide.R)
        {
            interchangableL.SelectInterChangable(interchangables);
            return;
        }
        interchangableR.SelectInterChangable(interchangables);
    }
    private void Interchange()
    {
        if (!interchangableR.IsValid() && !interchangableL.IsValid())
        {
            return;
        }
        if (!interchangableL.IsValid())
        {
            interchangableL.interchangable = playerInterchangable;
        }
        if (!interchangableR.IsValid())
        {
            interchangableR.interchangable = playerInterchangable;
        }
        Vector3 auxMovablePosition = interchangableL.Position;
        interchangableL.Interchange(interchangableR.Position);
        interchangableR.Interchange(auxMovablePosition);
        GameManager.Instance.RunGame();
    }
    private void CancelInterchange()
    {
        interchangableL.Reset();
        interchangableR.Reset();
        GameManager.Instance.RunGame();
    }
}
