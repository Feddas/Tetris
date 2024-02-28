using System.Collections.Generic;
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

/// <summary>
/// static - Available Tetromino shapes
/// instance - single tetromino shape and tile visual
/// </summary>
[System.Serializable]
public struct TetrominoData
{
    // Copy / paste from https://github.com/zigurous/unity-tetris-tutorial/blob/main/Assets/Scripts/Data.cs
    // Which copied it from https://tetris.fandom.com/wiki/SRS
    public static readonly Dictionary<Tetromino, Vector2Int[]> Shapes = new Dictionary<Tetromino, Vector2Int[]>()
    {
        { Tetromino.I, new Vector2Int[] { new Vector2Int(-1, 1), new Vector2Int( 0, 1), new Vector2Int( 1, 1), new Vector2Int( 2, 1) } },
        { Tetromino.J, new Vector2Int[] { new Vector2Int(-1, 1), new Vector2Int(-1, 0), new Vector2Int( 0, 0), new Vector2Int( 1, 0) } },
        { Tetromino.L, new Vector2Int[] { new Vector2Int( 1, 1), new Vector2Int(-1, 0), new Vector2Int( 0, 0), new Vector2Int( 1, 0) } },
        { Tetromino.O, new Vector2Int[] { new Vector2Int( 0, 1), new Vector2Int( 1, 1), new Vector2Int( 0, 0), new Vector2Int( 1, 0) } },
        { Tetromino.S, new Vector2Int[] { new Vector2Int( 0, 1), new Vector2Int( 1, 1), new Vector2Int(-1, 0), new Vector2Int( 0, 0) } },
        { Tetromino.T, new Vector2Int[] { new Vector2Int( 0, 1), new Vector2Int(-1, 0), new Vector2Int( 0, 0), new Vector2Int( 1, 0) } },
        { Tetromino.Z, new Vector2Int[] { new Vector2Int(-1, 1), new Vector2Int( 0, 1), new Vector2Int( 0, 0), new Vector2Int( 1, 0) } },
    };

    public static TetrominoData[] All()
    {
        var enumValues = System.Enum.GetValues(typeof(Tetromino)).Cast<Tetromino>();
        var asData = enumValues.Select(t => new TetrominoData(t)).ToArray();

        return asData;
    }

    [Tooltip("Which tetromino shape this instance represents")]
    public Tetromino tetromino;

    [Tooltip("Tilemap visual used for each cell of this tetromino shape instance")]
    public Tile tile;

    /// <summary> Cells that make up the shape of this tetromino instance </summary>
    public Vector2Int[] cells { get; private set; }

    public TetrominoData(Tetromino tetromino)
    {
        this.tetromino = tetromino;
        this.tile = null;
        this.cells = Shapes[tetromino];
    }
}
