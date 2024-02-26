using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Piece : MonoBehaviour
{
    public Board board { get; set; }
    public TetrominoData data { get; set; }
    public Vector3Int position { get; set; }
    public int rotationIndex { get; set; } = 0;

    public Vector3Int[] cells { get; set; }

    public void Initialize(Board board, TetrominoData data, Vector3Int position)
    {
        this.board = board;
        this.data = data;
        this.position = position;
        this.rotationIndex = 0;

        this.cells = data.shape
            .Select(v => (Vector3Int)v)
            .ToArray();
    }

    public bool Move(Vector2Int translation)
    {
        Vector3Int newPosition = this.position + (Vector3Int)translation;

        bool valid = this.board.IsValidPosition(this, newPosition);

        if (valid)
        {
            this.board.Clear(this);
            this.position = newPosition;
            this.board.Set(this);
        }

        return valid;
    }

    internal void HardDrop()
    {
        while (Move(Vector2Int.down))
        {
            continue;
        }
    }

    public void Rotate(int direction)
    {
        // rotationIndex is not used yet
        this.rotationIndex = Wrap(this.rotationIndex + direction, 0, 4);

        this.board.Clear(this);
        rotateCellsQuaternion(direction);
        this.board.Set(this);
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

            this.cells[i] = new Vector3Int(x, y, 0);
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
        for (int i = 0; i < cellsAsFloat.Length; i++)
        {
            var rotatedCell = Quaternion.Euler(0, 0, -90 * direction) * cellsAsFloat[i];
            this.cells[i] = Scale(rotatedCell, Round);
        }
    }

    /// <summary> Applies the same function to every component of a vector. </summary>
    public Vector3Int Scale(Vector3 input, Func<float, int> scalingFunc)
    {
        return new Vector3Int(scalingFunc(input.x), scalingFunc(input.y), scalingFunc(input.z));
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
