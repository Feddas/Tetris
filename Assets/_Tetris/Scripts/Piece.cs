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
        rotateCells(direction);
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
