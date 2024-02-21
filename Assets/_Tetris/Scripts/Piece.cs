using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Piece : MonoBehaviour
{
    public Board board { get; set; }
    public TetrominoData data { get; set; }
    public Vector3Int position { get; set; }

    public Vector3Int[] cells { get; set; }

    public void Initialize(Board board, TetrominoData data, Vector3Int position)
    {
        this.board = board;
        this.data = data;
        this.position = position;

        this.cells = data.shape
            .Select(v => (Vector3Int)v)
            .ToArray();
    }
}
