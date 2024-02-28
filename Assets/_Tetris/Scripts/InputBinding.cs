using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> Controls. How player button inputs affect the active tetromino. </summary>
[DefaultExecutionOrder(200)] // after pieces have fallen, before ghost is drawn
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

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            activePiece.Move(Vector2Int.left);
        }
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            activePiece.Move(Vector2Int.right);
        }

        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            activePiece.Move(Vector2Int.down);
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            activePiece.HardDrop();
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            activePiece.Rotate(-1);
        }
        else if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            activePiece.Rotate(1);
        }
    }
}
