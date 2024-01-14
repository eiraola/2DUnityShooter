using UnityEngine;
using UnityEngine.InputSystem;
using System;
using static Input;
[CreateAssetMenu(fileName = "New Input Reader,", menuName = "Input/InputReader")]

public class PlayerInput : ScriptableObject ,IPlayerActions 
{
    private Input input;
    public event Action<Vector2> OnMoveEvent;
    public event Action OnJumpEvent;
    public event Action OnJumpStopEvent;
    public event Action<EInterchangableSide> OnSelectEvent;
    public event Action OnInterchangeEvent;
    public event Action OnStartEvent;

    public void OnInterchange(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            OnInterchangeEvent?.Invoke();
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            OnJumpEvent?.Invoke();
        }
        if (context.canceled)
        {
            OnJumpStopEvent?.Invoke();
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        OnMoveEvent?.Invoke(context.ReadValue<Vector2>());
    }

    public void OnSelectL(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            OnSelectEvent?.Invoke(EInterchangableSide.L);
        }
    }

    public void OnSelectR(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            OnSelectEvent?.Invoke(EInterchangableSide.R);
        }
    }

    public void OnStart(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            OnStartEvent?.Invoke();
        }
    }

    private void OnEnable()
    {
        if (input == null)
        {
            input = new Input();
            input.Player.SetCallbacks(this);
        }
        input.Player.Enable();
    }
}
