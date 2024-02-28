using System;
using System.Linq;
using UnityEngine;

/// <summary> Where a tetromino piece is placed. </summary>
public class Piece : MonoBehaviour
{
    // https://stackoverflow.com/questions/1431359/event-action-vs-event-eventhandler
    public event Action Initialized;

    [SerializeField]
    private Vector3Int spawnPosition = new Vector3Int(-1, 8, 0); // preview uses 8, 7, 0

    public Board board { get; set; }
    public TetrominoData shape { get; set; }
    public Vector3Int position { get; set; }

    public Vector3Int[] cells { get; set; }
    public Vector3Int[] proposedCells { get; set; }

    public void Initialize(Board board, TetrominoData shape)
    {
        this.board = board;
        this.shape = shape;
        this.position = spawnPosition;

        // use proposedCells until the piece is determined to be a valid spawn, then set this.cells.
        this.cells = null;
        this.proposedCells = shape.cells
            .Select(v => (Vector3Int)v)
            .ToArray();

        Initialized?.Invoke();
    }
}
