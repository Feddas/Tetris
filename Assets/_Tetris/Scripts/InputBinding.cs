using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        if (Input.GetKeyDown(KeyCode.A))
        {
            piece.Move(Vector2Int.left);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            piece.Move(Vector2Int.right);
        }

        if (Input.GetKeyDown(KeyCode.S))
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
        else if (Input.GetKeyDown(KeyCode.E))
        {
            piece.Rotate(1);
        }
    }
}
