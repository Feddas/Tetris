using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum Tetromino
{
    I,
    O,
    T,
    J,
    L,
    S,
    Z,
}

[System.Serializable]
public struct TetrominoData
{
    public Tetromino tetromino;
    public Tile tile;

    /// <summary> Cells that make up the shape of the tetromino. </summary>
    public Vector2Int[] shape { get; private set; }

    public TetrominoData(Tetromino tetromino)
    {
        this.tetromino = tetromino;
        this.tile = null;
        this.shape = Data.Cells[tetromino];
    }

    public static TetrominoData[] All()
    {
        var enumValues = System.Enum.GetValues(typeof(Tetromino)).Cast<Tetromino>();
        var asData = enumValues.Select(t => new TetrominoData(t)).ToArray();

        return asData;
    }
}
