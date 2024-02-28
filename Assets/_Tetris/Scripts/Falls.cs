using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> How a tetromino will automatically fall. </summary>
[RequireComponent(typeof(Piece))]
[DefaultExecutionOrder(100)] // ensure lockTime is incremented before InputBinding's Update
public class Falls : MonoBehaviour
{
    public Piece piece
    {
        get
        {
            if (_piece == null)
            {
                _piece = this.GetComponent<Piece>();
            }
            return _piece;
        }
    }
    private Piece _piece;

    public float stepDelay = 1f;
    public float lockDelay = 0.5f;

    private float stepTime;
    private float lockTime;

    void Start()
    {
        piece.Initialized += Piece_Initialized;
        piece.Moved += Piece_Moved;
    }

    private void Piece_Moved()
    {
        this.lockTime = 0f;
    }

    private void Piece_Initialized()
    {
        this.stepTime = Time.time + this.stepDelay;
        this.lockTime = 0f;
    }

    void Update()
    {
        this.lockTime += Time.deltaTime;
        if (Time.time >= this.stepTime)
        {
            Step();
        }
    }

    public void Step()
    {
        this.stepTime = Time.time + this.stepDelay;
        piece.Move(Vector2Int.down);

        if (this.lockTime >= this.lockDelay)
        {
            piece.Lock();
        }
    }
}
