using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Piece : MonoBehaviour
{
    // https://stackoverflow.com/questions/1431359/event-action-vs-event-eventhandler
    public event Action Initialized;
    public event Action Moved;

    public Board board { get; set; }
    public TetrominoData data { get; set; }
    public Vector3Int position { get; set; }

    public Vector3Int[] cells { get; set; }
    public Vector3Int[] proposedCells { get; set; }

    public void Initialize(Board board, TetrominoData data, Vector3Int position)
    {
        this.board = board;
        this.data = data;
        this.position = position;

        // use proposedCells until the piece is determined to be a valid spawn, then set this.cells.
        this.proposedCells = data.shape
            .Select(v => (Vector3Int)v)
            .ToArray();

        Raise(Initialized);
    }

    private void Raise(Action eventName)
    {
        if (eventName != null)
        {
            eventName();
        }
    }

    public bool Move(Vector2Int translation)
    {
        this.proposedCells = this.cells; // shape and rotation have NOT changed
        bool valid = MoveProposed(translation, out Vector3Int newPosition);

        if (valid)
        {
            this.board.Clear(this);
            this.position = newPosition;
            this.board.Set(this);
            Raise(Moved);
        }

        return valid;
    }

    /// <summary> moves cells already existing in this.proposedCells. </summary>
    public bool MoveProposed(Vector2Int translation, out Vector3Int newPosition)
    {
        newPosition = this.position + (Vector3Int)translation;
        return this.board.IsValidToHave(this, at: newPosition);
    }

    internal void HardDrop()
    {
        while (Move(Vector2Int.down))
        {
            continue;
        }

        Lock();
    }

    /// <summary> The piece has finalized placement. It's now locked into position. </summary>
    public void Lock()
    {
        this.board.Set(this);
        this.board.ClearLines();
        this.board.SpawnPiece();
    }

    public bool Rotate(int direction)
    {
        rotateCellsQuaternion(direction);
        bool valid = isValidRotation(out Vector3Int newPosition);  // is able to rotate piece to a valid position.

        if (valid)
        {
            this.board.Clear(this);
            this.cells = proposedCells;
            this.position = newPosition;
            this.board.Set(this);
            Raise(Moved);
        }

        return valid;
    }

    private void rotateCellsQuaternion(int direction)
    {
        // setup different rotation for I & O than other tetrominos. I & O use an offset pivot
        Vector3[] cellsAsFloat;
        Func<float, int> Round;
        switch (this.data.tetromino)
        {
            case Tetromino.I:
            case Tetromino.O:
                cellsAsFloat = this.cells.Select(c => new Vector3(c.x - 0.5f, c.y - 0.5f, 0)).ToArray();
                Round = Mathf.CeilToInt;
                break;
            case Tetromino.T:
            case Tetromino.J:
            case Tetromino.L:
            case Tetromino.S:
            case Tetromino.Z:
                cellsAsFloat = Array.ConvertAll(this.cells, c => (Vector3)c);
                Round = Mathf.RoundToInt;
                break;
            default:
                throw new NotImplementedException();
        }

        // apply rotation
        this.proposedCells = new Vector3Int[cells.Length];
        for (int i = 0; i < cellsAsFloat.Length; i++)
        {
            var rotatedCell = Quaternion.Euler(0, 0, -90 * direction) * cellsAsFloat[i];
            this.proposedCells[i] = rotatedCell.Scale(Round);
        }
    }

    /// <returns> false if unable to fix rotation causing any tile of the piece to go out of left or right bounds of the board. </returns>
    private bool isValidRotation(out Vector3Int newPosition)
    {
        var xMin = this.proposedCells.Min(c => c.x + position.x);
        var xMax = this.proposedCells.Max(c => c.x + position.x) + 1;
        var xMinBounds = this.board.Bounds.xMin;
        var xMaxBounds = this.board.Bounds.xMax;
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
        newPosition = this.position;
        return true;
    }
}
