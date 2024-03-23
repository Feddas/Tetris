using System;
using System.Linq;
using UnityEngine;

/// <summary> Relocates an active tetromino that is either falling or recieving player button input. </summary>
[RequireComponent(typeof(Piece))]
public class Relocates : MonoBehaviour
{
    [SerializeField]
    private TetrominoSpawner spawner;

    public event Action Moved;

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

    public Board board
    {
        get
        {
            return piece.board;
        }
    }

    /// <summary> returns void to enable being invoked by a UnityEvent </summary>
    public void MoveVoid(Vector2Int tranlation)
    {
        Move(tranlation);
    }

    public bool Move(Vector2Int translation)
    {
        piece.proposedCells = piece.cells; // shape and rotation have NOT changed
        bool valid = MoveProposed(translation, out Vector3Int newPosition);

        if (valid)
        {
            board.Clear(piece);
            piece.position = newPosition;
            board.Set(piece);
            Moved?.Invoke();
        }

        return valid;
    }

    public void HardDrop()
    {
        while (Move(Vector2Int.down))
        {
            continue;
        }

        Lock();
    }

    /// <summary> returns void to enable being invoked by a UnityEvent </summary>
    public void RotateVoid(int direction)
    {
        Rotate(direction);
    }

    public bool Rotate(int direction)
    {
        rotateCellsQuaternion(direction);
        bool valid = isValidRotation(out Vector3Int newPosition);  // is able to rotate piece to a valid position.

        if (valid)
        {
            board.Clear(piece);
            piece.cells = piece.proposedCells;
            piece.position = newPosition;
            board.Set(piece);
            Moved?.Invoke();
        }

        return valid;
    }

    /// <summary> The piece has finalized placement. It's now locked into position. </summary>
    public void Lock()
    {
        board.Set(piece);
        board.ClearLines();
        this.spawner.SpawnPiece();
    }

    /// <summary> moves cells already existing in this.proposedCells. </summary>
    private bool MoveProposed(Vector2Int translation, out Vector3Int newPosition)
    {
        newPosition = piece.position + (Vector3Int)translation;
        return board.IsValidToHave(piece, at: newPosition);
    }

    private void rotateCellsQuaternion(int direction)
    {
        // setup different rotation for I & O than other tetrominos. I & O use an offset pivot
        Vector3[] cellsAsFloat;
        Func<float, int> Round;
        switch (piece.shape.tetromino)
        {
            case Tetromino.I:
            case Tetromino.O:
                cellsAsFloat = piece.cells.Select(c => new Vector3(c.x - 0.5f, c.y - 0.5f, 0)).ToArray();
                Round = Mathf.CeilToInt;
                break;
            case Tetromino.T:
            case Tetromino.J:
            case Tetromino.L:
            case Tetromino.S:
            case Tetromino.Z:
                cellsAsFloat = Array.ConvertAll(piece.cells, c => (Vector3)c);
                Round = Mathf.RoundToInt;
                break;
            default:
                throw new NotImplementedException();
        }

        // apply rotation
        piece.proposedCells = new Vector3Int[piece.cells.Length];
        for (int i = 0; i < cellsAsFloat.Length; i++)
        {
            var rotatedCell = Quaternion.Euler(0, 0, -90 * direction) * cellsAsFloat[i];
            piece.proposedCells[i] = rotatedCell.Scale(Round);
        }
    }

    /// <returns> false if unable to fix rotation causing any tile of the piece to go out of left or right bounds of the board. </returns>
    private bool isValidRotation(out Vector3Int newPosition)
    {
        var xMin = piece.proposedCells.Min(c => c.x + piece.position.x);
        var xMax = piece.proposedCells.Max(c => c.x + piece.position.x) + 1;
        var xMinBounds = board.Bounds.xMin;
        var xMaxBounds = board.Bounds.xMax;
        if (xMin < xMinBounds)
        {
            // Debug.LogFormat("min {0},{1}  {2},{3}", xMin, xMax, xMinBounds, xMaxBounds);
            int howMuch = xMinBounds - xMin;
            return MoveProposed(Vector2Int.right * howMuch, out newPosition);
        }
        else if (xMax > xMaxBounds)
        {
            // Debug.LogFormat("max {0},{1}  {2},{3}", xMin, xMax, xMinBounds, xMaxBounds);
            int howMuch = xMax - xMaxBounds;
            return MoveProposed(Vector2Int.left * howMuch, out newPosition);
        }

        // piece never touched the bounds of the game board
        newPosition = piece.position;
        return true;
    }
}
