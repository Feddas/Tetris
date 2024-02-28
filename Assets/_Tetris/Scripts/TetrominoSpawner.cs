using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetrominoSpawner : MonoBehaviour
{
    public Board board;
    public Piece nextPiece;
    public Piece activePiece;

    public TetrominoData[] tetrominoes = TetrominoData.All();

    public int spawnSeed; // I=0, J=1, L=42

    private void Start()
    {
        Random.InitState(spawnSeed);

        SetNextPiece();
        SpawnPiece();
    }

    private void SetNextPiece()
    {
        // Clear the existing piece from the board
        if (nextPiece.cells != null)
        {
            board.Clear(nextPiece);
        }

        // Pick a random tetromino to use
        int blockIndex = Random.Range(0, this.tetrominoes.Length);
        TetrominoData piece = this.tetrominoes[blockIndex];

        // Initialize the next piece with the random data
        // Draw it at the "preview" position on the board
        nextPiece.Initialize(board, piece);
        nextPiece.cells = nextPiece.proposedCells; // preview is always valid
        board.Set(nextPiece);
    }

    public void SpawnPiece()
    {
        // Initialize the active piece with the next piece data
        this.activePiece.Initialize(board, nextPiece.data);

        // If invalid position call GameOver before spawning the piece for the next round
        if (false == board.IsValidToHave(this.activePiece, at: this.activePiece.position))
        {
            board.GameOver();
        }

        this.activePiece.cells = this.activePiece.proposedCells;
        board.Set(this.activePiece);

        // Set the next random piece
        SetNextPiece();
    }
}
