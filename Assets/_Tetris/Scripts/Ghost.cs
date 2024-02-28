using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary> Visualizer to see where trackingPiece will land after a hard drop. </summary>
[DefaultExecutionOrder(300)] // ensure Piece.cs is placed before creating this ghost
public class Ghost : MonoBehaviour
{
    public Tile tileGhost;
    public Board board;
    public Piece trackingPiece;
    public Tilemap tilemapGhost;

    public Vector3Int position { get; set; }

    public Vector3Int[] cells { get; set; }

    private void Awake()
    {
        this.cells = new Vector3Int[4];
    }

    private void Update()
    {
        Clear();
        if (trackingPiece.cells != null)
        {
            Copy();
            Drop();
            Set();
        }
    }

    private void Clear()
    {
        for (int i = 0; i < this.cells.Length; i++)
        {
            Vector3Int tilePosition = this.cells[i] + this.position;
            this.tilemapGhost.SetTile(tilePosition, null);
        }
    }

    private void Copy()
    {
        for (int i = 0; i < this.cells.Length; i++)
        {
            this.cells[i] = this.trackingPiece.cells[i];
        }
    }

    private void Drop()
    {
        Vector3Int position = this.trackingPiece.position;

        int current = position.y;
        int bottom = -this.board.boardSize.y / 2 - 1;

        this.board.Clear(this.trackingPiece);

        for (int row = current; row >= bottom; row--)
        {
            position.y = row;
            if (this.board.IsValidToHave(trackingPiece, at: position))
            {
                this.position = position;
            }
            else
            {
                break;
            }
        }
        this.board.Set(this.trackingPiece);
    }

    private void Set()
    {
        for (int i = 0; i < this.cells.Length; i++)
        {
            Vector3Int tilePosition = this.cells[i] + this.position;
            this.tilemapGhost.SetTile(tilePosition, this.tileGhost);
        }
    }
}
