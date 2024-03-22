using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;
using System.Collections.Generic;

/// <summary> Size of the tetris game board and logic for pieces that are no longer interactabled (locked into place). </summary>
public class Board : MonoBehaviour
{
    public Tilemap tilemap;
    public Vector2Int boardSize = new Vector2Int(10, 20);

    [Tooltip("If true, clearing a line will cause each individual tile above to fall into empty cells")]
    [SerializeField]
    private bool LineclearsCascade = true;

    public RectInt Bounds
    {
        get
        {
            Vector2Int position = new Vector2Int(-this.boardSize.x / 2, -this.boardSize.y / 2);
            return new RectInt(position, this.boardSize);
        }
    }

    public void GameOver()
    {
        this.tilemap.ClearAllTiles();
    }

    public void Set(Piece piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            this.tilemap.SetTile(tilePosition, piece.shape.tile);
        }
    }

    public void Clear(Piece piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            this.tilemap.SetTile(tilePosition, null);
        }
    }

    /// <returns> true if all proposedCells of <paramref name="piece"/> cover empty cells at position of <paramref name="at"/> </returns>
    public bool IsValidToHave(Piece piece, Vector3Int at)
    {
        foreach (var cell in piece.proposedCells)
        {
            Vector3Int tilePosition = cell + at;

            // is contained in original tetromino position
            if (piece.cells != null && piece.cells.Any(c => piece.position + c == tilePosition))
            {
                continue;
            }

            // collides with edge of playable board
            if (Bounds.Contains((Vector2Int)tilePosition) == false)
            {
                return false;
            }

            // collides with previously placed and locked tetromino
            if (this.tilemap.HasTile(tilePosition))
            {
                return false;
            }
        }

        return true;
    }

    public void ClearLines()
    {
        int row = Bounds.yMin;
        while (row < Bounds.yMax)
        {
            if (IsLineFull(row))
            {
                LineClear(row);

                // clean up new lines from a cascade
                while (IsLineFull(Bounds.yMin) && LineclearsCascade)
                {
                    LineClear(Bounds.yMin);
                }
            }
            else
            {
                row++;
            }
        }
    }

    private bool IsLineFull(int row)
    {
        for (int col = Bounds.xMin; col < Bounds.xMax; col++)
        {
            bool hasTile = this.tilemap.HasTile(new Vector3Int(col, row, 0));
            if (false == hasTile)
            {
                return false;
            }
        }

        return true;
    }

    private void LineClear(int row)
    {
        // clear row
        for (int col = Bounds.xMin; col < Bounds.xMax; col++)
        {
            Vector3Int position = new Vector3Int(col, row, 0);
            this.tilemap.SetTile(position, null);

            if (LineclearsCascade)
            {
                CascadeColumn(col, Bounds.yMin);
            }
        }

        if (false == LineclearsCascade)
        {
            ShiftAboveDown1(row);
        }
    }

    /// <summary> sorts cells so occupied cells, that are <paramref name="aboveRow"/>, are below empty cells </summary>
    private void CascadeColumn(int col, int aboveRow)
    {
        // initialize variables with the line that was just cleared, it contains null.
        Vector3Int position = new Vector3Int(col, aboveRow, 0);
        Queue<Vector3Int> positionNulls = new Queue<Vector3Int>(new[] { position });

        // in this column, move all blocks above down. cascade by moving down nonnull tile to last null tile.
        TileBase tileCurrent;
        int rowCascade = aboveRow + 1;
        while (rowCascade < Bounds.yMax)
        {
            position.y = rowCascade++;
            tileCurrent = this.tilemap.GetTile(position);
            if (tileCurrent == null)
            {
                positionNulls.Enqueue(position);
            }
            else if (positionNulls.Count > 0 && tileCurrent != null) // move to first null cell
            {
                this.tilemap.SetTile(positionNulls.Dequeue(), tileCurrent);
                this.tilemap.SetTile(position, null);
                positionNulls.Enqueue(position);
            }
        }
    }

    /// <summary> shifts all cells, that are <paramref name="aboveRow"/>, down by 1 row </summary>
    private void ShiftAboveDown1(int aboveRow)
    {
        // move rows above down 1 row
        while (aboveRow < Bounds.yMax && false == LineclearsCascade)
        {
            for (int col = Bounds.xMin; col < Bounds.xMax; col++)
            {
                Vector3Int position = new Vector3Int(col, aboveRow + 1, 0);
                TileBase above = this.tilemap.GetTile(position);

                position.y = aboveRow;
                this.tilemap.SetTile(position, above);
            }
            aboveRow++;
        }
    }
}
