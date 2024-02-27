using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[DefaultExecutionOrder(100)] // ensure lockTime is incremented before InputBinding's Update
public class Piece : MonoBehaviour
{
    public Board board { get; set; }
    public TetrominoData data { get; set; }
    public Vector3Int position { get; set; }
    public int rotationIndex { get; set; } = 0;

    public Vector3Int[] cells { get; set; }
    public Vector3Int[] proposedCells { get; set; }

    public float stepDelay = 1f;
    public float lockDelay = 0.5f;

    private float stepTime;
    private float lockTime;

    private void Update()
    {
        this.lockTime += Time.deltaTime;
        if (Time.time >= this.stepTime)
        {
            Step();
        }
    }

    public void Initialize(Board board, TetrominoData data, Vector3Int position)
    {
        this.board = board;
        this.data = data;
        this.position = position;
        this.rotationIndex = 0;
        this.stepTime = Time.time + this.stepDelay;
        this.lockTime = 0f;

        this.cells = data.shape
            .Select(v => (Vector3Int)v)
            .ToArray();
    }

    public bool Move(Vector2Int translation)
    {
        this.proposedCells = this.cells; // shape and rotation have NOT changed
        bool valid = MoveProposed(translation, out Vector3Int newPosition);

        if (valid)
        {
            this.board.Clear(this);
            this.position = newPosition;
            this.lockTime = 0f;
            this.board.Set(this);
        }

        return valid;
    }

    /// <summary> moves cells already existing in this.proposedCells. </summary>
    public bool MoveProposed(Vector2Int translation, out Vector3Int newPosition)
    {
        newPosition = this.position + (Vector3Int)translation;
        return this.board.IsValidMove(this, newPosition);
    }

    public void Step()
    {
        this.stepTime = Time.time + this.stepDelay;
        Move(Vector2Int.down);

        if (this.lockTime >= this.lockDelay)
        {
            Lock();
        }
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
    private void Lock()
    {
        this.board.Set(this);
        this.board.SpawnPiece();
    }

    public bool Rotate(int direction)
    {
        // rotationIndex is not used yet
        this.rotationIndex = Wrap(this.rotationIndex + direction, 0, 4);

        rotateCellsQuaternion(direction);
        bool valid = isValidRotation(out Vector3Int newPosition);  // is able to rotate piece to a valid position.

        if (valid)
        {
            this.board.Clear(this);
            this.cells = proposedCells;
            this.position = newPosition;
            this.lockTime = 0f;
            this.board.Set(this);
        }

        return valid;
    }

    private void rotateCells(int direction)
    {
        for (int i = 0; i < this.cells.Length; i++)
        {
            Vector3 cell = this.cells[i];
            Func<float, int> Round;

            // I & O use an offset pivot
            switch (this.data.tetromino)
            {
                case Tetromino.I:
                case Tetromino.O:
                    cell.x -= 0.5f;
                    cell.y -= 0.5f;
                    Round = Mathf.CeilToInt;
                    break;
                case Tetromino.T:
                case Tetromino.J:
                case Tetromino.L:
                case Tetromino.S:
                case Tetromino.Z:
                    Round = Mathf.RoundToInt;
                    break;
                default:
                    throw new NotImplementedException();
            }

            // rotate the cell. AKA cell = Round(Quaternion.Euler(0, 0, 90 * direction) * cell);
            int x, y;
            x = Round(
                (cell.x * Data.RotationMatrix[0] * direction)
                + (cell.y * Data.RotationMatrix[1] * direction)
                );
            y = Round(
                (cell.x * Data.RotationMatrix[2] * direction)
                + (cell.y * Data.RotationMatrix[3] * direction)
                );

            this.proposedCells[i] = new Vector3Int(x, y, 0);
        }
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
            this.proposedCells[i] = Scale(rotatedCell, Round);
        }
    }

    /// <summary> Applies the same function to every component of a vector. </summary>
    public Vector3Int Scale(Vector3 input, Func<float, int> scalingFunc)
    {
        return new Vector3Int(scalingFunc(input.x), scalingFunc(input.y), scalingFunc(input.z));
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

    /// <summary>
    /// https://stackoverflow.com/questions/14415753/wrap-value-into-range-min-max-without-division
    /// </summary>
    /// <returns> the value of <paramref name="input"/> wrapped into the range of <paramref name="x_min"/> and <paramref name="x_max"/>. </returns>
    private int Wrap(int input, int x_min, int x_max)
    {
        if (input < x_min)
            return x_max - (x_min - input) % (x_max - x_min);
        else
            return x_min + (input - x_min) % (x_max - x_min);
    }
}
