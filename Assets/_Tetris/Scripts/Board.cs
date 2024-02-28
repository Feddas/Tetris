using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

/// <summary> Size of the tetris game board and logic for pieces that are no longer interactabled (locked into place). </summary>
public class Board : MonoBehaviour
{
    public Tilemap tilemap;
    public Vector2Int boardSize = new Vector2Int(10, 20);

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
            this.tilemap.SetTile(tilePosition, piece.data.tile);
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

            // TODO: in this column, move all blocks above down. cascade by moving down nonnull tile to last null tile.
            //int rowCascade = row;
            //while (rowCascade < Bounds.yMax)
            //{

            //}
        }

        // move rows above down 1 row
        while (row < Bounds.yMax)
        {
            for (int col = Bounds.xMin; col < Bounds.xMax; col++)
            {
                Vector3Int position = new Vector3Int(col, row + 1, 0);
                TileBase above = this.tilemap.GetTile(position);

                position.y = row;
                this.tilemap.SetTile(position, above);
            }
            row++;
        }
    }
}
