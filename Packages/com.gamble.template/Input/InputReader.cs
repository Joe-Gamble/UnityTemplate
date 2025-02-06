using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using static InputActions;

public interface IInputReader
{
    Vector2 MoveDirection { get; }
    void EnablePlayerActions();
}

[CreateAssetMenu(fileName = "InputReader", menuName = "Scriptable Objects/InputReader")]
public class InputReader : ScriptableObject, IPlayerInputActions, IInputReader
{
    public event UnityAction<Vector2> Move = delegate { };
    public event UnityAction<Vector2> Look = delegate { };
    public event UnityAction<bool> Jump = delegate { };

    public InputActions inputActions;

    public Vector2 MoveDirection => inputActions.PlayerInput.Move.ReadValue<Vector2>();
    public Vector2 MouseDirection => inputActions.PlayerInput.Move.ReadValue<Vector2>();
    public bool IsJumpPressed => inputActions.PlayerInput.Jump.IsPressed();

    public void EnablePlayerActions()
    {
        if (inputActions == null)
        {
            inputActions = new InputActions();
            inputActions.PlayerInput.SetCallbacks(this);
        }

        inputActions.Enable();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Started:
                Jump.Invoke(true); 
                break;

            case InputActionPhase.Canceled:
                Jump.Invoke(false);
                break;
        }
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        Look.Invoke(context.ReadValue<Vector2>());
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        Move.Invoke(context.ReadValue<Vector2>());
    }
}
