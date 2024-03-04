using System;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary> Controls. How player button inputs affect the active tetromino. </summary>
[RequireComponent(typeof(Relocates))]
public class InputBinding : MonoBehaviour
{
    public Relocates activePiece
    {
        get
        {
            if (_relocates == null)
            {
                _relocates = this.GetComponent<Relocates>();
            }
            return _relocates;
        }
    }
    private Relocates _relocates;

    public void MoveDown(InputAction.CallbackContext context)
    {
        perform(context, () => activePiece.Move(Vector2Int.down));
    }

    public void MoveHorizontal(InputAction.CallbackContext context)
    {
        Vector2Int direction = new Vector2Int((int)context.ReadValue<float>(), 0);
        perform(context, () => activePiece.Move(direction));
    }

    public void Rotate(InputAction.CallbackContext context)
    {
        // 1 = clockwise, -1 = counter-clockwise
        int direction = (int)context.ReadValue<float>();
        perform(context, () => activePiece.Rotate(direction));
    }

    public void HardDrop(InputAction.CallbackContext context)
    {
        perform(context, () => activePiece.HardDrop());
    }

    private void perform(InputAction.CallbackContext context, Action action)
    {
        if (context.phase != InputActionPhase.Performed)
        {
            return;
        }

        action?.Invoke();
    }
}
