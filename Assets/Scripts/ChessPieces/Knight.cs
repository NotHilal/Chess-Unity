using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : ChessPiece
{
    public int points = 3;
    public override List<Vector2Int> GetAvailableMoves(ref ChessPiece[,] board, int TileCountX, int TileCountY)
    {
        List<Vector2Int> r = new List<Vector2Int>();

        if (CurrentX+2< TileCountX&& CurrentY+1< TileCountY)
        {
            if (board[CurrentX + 2, CurrentY + 1]==null|| board[CurrentX + 2, CurrentY + 1].color!=color)
            {
                r.Add(new Vector2Int(CurrentX+2, CurrentY+1));
            }
        }
        if (CurrentX + 1 < TileCountX && CurrentY + 2 < TileCountY)
        {
            if (board[CurrentX + 1, CurrentY +2] == null || board[CurrentX + 1, CurrentY + 2].color != color)
            {
                r.Add(new Vector2Int(CurrentX + 1, CurrentY + 2));
            }
        }
        if (CurrentX - 2 >=0 && CurrentY - 1 >=0)
        {
            if (board[CurrentX - 2, CurrentY - 1] == null || board[CurrentX - 2, CurrentY - 1].color != color)
            {
                r.Add(new Vector2Int(CurrentX - 2, CurrentY - 1));
            }
        }
        if (CurrentX - 1 >=0 && CurrentY - 2 >=0)
        {
            if (board[CurrentX - 1, CurrentY - 2] == null || board[CurrentX - 1, CurrentY - 2].color != color)
            {
                r.Add(new Vector2Int(CurrentX -1, CurrentY - 2));
            }
        }
        if (CurrentX + 2 < TileCountX && CurrentY - 1 >= 0)
        {
            if (board[CurrentX + 2, CurrentY - 1] == null || board[CurrentX + 2, CurrentY - 1].color != color)
            {
                r.Add(new Vector2Int(CurrentX + 2, CurrentY - 1));
            }
        }
        if (CurrentX + 1 < TileCountX && CurrentY - 2 >= 0)
        {
            if (board[CurrentX + 1, CurrentY - 2] == null || board[CurrentX + 1, CurrentY - 2].color != color)
            {
                r.Add(new Vector2Int(CurrentX + 1, CurrentY - 2));
            }
        }
        if (CurrentY + 2 < TileCountY && CurrentX - 1 >= 0)
        {
            if (board[CurrentX -1, CurrentY +2] == null || board[CurrentX -1, CurrentY+2].color != color)
            {
                r.Add(new Vector2Int(CurrentX -1, CurrentY +2));
            }
        }
        if (CurrentY + 1 < TileCountY && CurrentX - 2 >= 0)
        {
            if (board[CurrentX - 2, CurrentY + 1] == null || board[CurrentX - 2, CurrentY + 1].color != color)
            {
                r.Add(new Vector2Int(CurrentX - 2, CurrentY + 1));
            }
        }

        return r;
    }
}
