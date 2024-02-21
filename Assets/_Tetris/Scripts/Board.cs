using UnityEngine;
using UnityEngine.Tilemaps;

public class Board : MonoBehaviour
{
    public Tilemap tilemap;
    public Piece activePiece { get; private set; }

    public TetrominoData[] tetrominoes = TetrominoData.All();

    public Vector3Int spawnPosition = new Vector3Int(-1, 8, 0);

    private void Start()
    {
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
}
