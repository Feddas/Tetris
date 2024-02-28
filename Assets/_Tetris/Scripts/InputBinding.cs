using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> Controls. How player button inputs affect the active tetromino. </summary>
[DefaultExecutionOrder(200)] // after pieces have fallen, before ghost is drawn
[RequireComponent(typeof(Piece))]
public class InputBinding : MonoBehaviour
{
    public Piece piece
    {
        get
        {
            if (_piece == null)
            {
                _piece = this.GetComponent<Piece>();
            }
            return _piece;
        }
    }
    private Piece _piece;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            piece.Move(Vector2Int.left);
        }
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            piece.Move(Vector2Int.right);
        }

        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            piece.Move(Vector2Int.down);
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            piece.HardDrop();
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            piece.Rotate(-1);
        }
        else if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            piece.Rotate(1);
        }
    }
}
