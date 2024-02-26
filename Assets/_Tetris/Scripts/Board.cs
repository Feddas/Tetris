using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

public class Board : MonoBehaviour
{
    public Tilemap tilemap;
    public Piece activePiece { get; private set; }

    public TetrominoData[] tetrominoes = TetrominoData.All();

    public Vector3Int spawnPosition = new Vector3Int(-1, 8, 0);
    public int spawnSeed; // I=0, J=1, L=42
    public Vector2Int boardSize = new Vector2Int(10, 20);

    public RectInt Bounds
    {
        get
        {
            Vector2Int position = new Vector2Int(-this.boardSize.x / 2, -this.boardSize.y / 2);
            return new RectInt(position, this.boardSize);
        }
    }

    private void Start()
    {
        Random.InitState(spawnSeed);
        activePiece = GetComponentInChildren<Piece>();
        SpawnPiece();
    }

    public void SpawnPiece()
    {
        int blockIndex = Random.Range(0, this.tetrominoes.Length);
        TetrominoData data = this.tetrominoes[blockIndex];

        this.activePiece.Initialize(this, data, spawnPosition);
        Set(this.activePiece);
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

    public bool IsValidPosition(Piece piece, Vector3Int position)
    {
        foreach (var cell in piece.cells)
        {
            Vector3Int tilePosition = cell + position;

            // is contained in original position
            if (piece.cells.Any(c => piece.position + c == tilePosition))
            {
                continue;
            }

            // collides with edge of playable board
            if (Bounds.Contains((Vector2Int)tilePosition) == false)
            {
                return false;
            }

            // collides with already placed tetromino
            if (this.tilemap.HasTile(tilePosition))
            {
                return false;
            }
        }

        return true;
    }
}
