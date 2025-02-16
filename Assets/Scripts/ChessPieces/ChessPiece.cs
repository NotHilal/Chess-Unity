using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ChessPieceType
{
    None = 0,
    Pawn = 1,
    Rook = 2,
    Knight = 3,
    Bishop = 4, 
    Queen = 5, 
    King = 6

}
public class ChessPiece : MonoBehaviour
{
    public int color;
    public int CurrentX;
    public int CurrentY;
    public ChessPieceType type;
    public bool Promoted = false;
    

    private Vector3 desiredPosition;
    private Vector3 desiredScale= Vector3.one;

    private void Update()
    {
        transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * 10);
        transform.localScale = Vector3.Lerp(transform.localScale, desiredScale, Time.deltaTime * 10);
    }
    private void Start()
    {
        if(color==0) // no rotation for white pieces
        {
            transform.rotation = Quaternion.Euler(Vector3.zero);
        }
        if (color == 1) // rotation 180 for black pieces so that they face he good way
        {
            transform.rotation = Quaternion.Euler(new Vector3(0,180,0));
        }
    }

    public virtual void SetPosition(Vector3 position, bool force = false)
    {
        desiredPosition = position;
        if(force)
        {
            transform.position = desiredPosition;
        }
    }

    public virtual void SetScale(Vector3 scale, bool force = false)
    {
        desiredScale = scale;
        if (force)
        {
            transform.localScale= desiredScale;
        }
    }
    public virtual List<Vector2Int> GetAvailableMoves(ref ChessPiece[,]board, int tileCountX, int tileCountY)
    {
        List<Vector2Int> r = new List<Vector2Int>();
        r.Add(new Vector2Int(3, 3));
        r.Add(new Vector2Int(2, 5));
        r.Add(new Vector2Int(1, 5));
        r.Add(new Vector2Int(2, 4));

        return r;
    }
    public virtual SpecialMove GetSpecialMoves(ref ChessPiece[,] board,ref List<Vector2Int[]> movelist, ref List<Vector2Int> availableMoves, int TileCountX, int TileCountY)
    {
        return SpecialMove.None;
    }
}
