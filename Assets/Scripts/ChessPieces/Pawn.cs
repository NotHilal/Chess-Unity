using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : ChessPiece
{
    public int points = 1;
    public override List<Vector2Int> GetAvailableMoves(ref ChessPiece[,] board, int TileCountX, int TileCountY)
    {
        List<Vector2Int> r = new List<Vector2Int>();

        int direction = (color==0) ? 1 : -1; //go upwards if u are white, go downwords if you are black

        if(Promoted==false)
        {
            // One in front
            if (board[CurrentX, CurrentY + direction] == null)
            {
                r.Add(new Vector2Int(CurrentX, CurrentY + direction));
            }
            //two in front
            if (direction > 0 && CurrentY == 1 && board[CurrentX, CurrentY + direction] == null && board[CurrentX, CurrentY + 2 * direction] == null)
            {
                r.Add(new Vector2Int(CurrentX, CurrentY + 2 * direction));
            }
            if (direction < 0 && CurrentY == 6 && board[CurrentX, CurrentY + direction] == null && board[CurrentX, CurrentY + 2 * direction] == null)
            {
                r.Add(new Vector2Int(CurrentX, CurrentY + 2 * direction));
            }
            //eat diag
            if (CurrentX + 1 < TileCountX)
            {
                if (board[CurrentX + 1, CurrentY + direction] != null && board[CurrentX + 1, CurrentY + direction].color != color)
                {
                    r.Add(new Vector2Int(CurrentX + 1, CurrentY + direction));
                }

            }
            if (CurrentX - 1 >= 0)
            {
                if (board[CurrentX - 1, CurrentY + direction] != null && board[CurrentX - 1, CurrentY + direction].color != color)
                {
                    r.Add(new Vector2Int(CurrentX - 1, CurrentY + direction));
                }

            }
        }
        

        return r;
    }

    public override SpecialMove GetSpecialMoves(ref ChessPiece[,] board, ref List<Vector2Int[]> movelist, ref List<Vector2Int> availableMoves, int TileCountX, int TileCountY)
    {
        int direction = (color ==0) ? 1 : -1;
        //EnPassant
        int Line =-1;
        int Column = -1;
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (board[i, j] != null)
                {
                    if (board[i, j].color == color)
                    {
                        if (board[i, j].type == ChessPieceType.King)
                        {
                            if(i== CurrentX)
                            {
                                Column = i;
                                
                            }
                            if (j == CurrentY)
                            {
                                Line = j;
                                
                            }
                        }
                    }
                }
            }
        }
        if(Line!=-1)
        {
            for (int i = 0; i < 8; i++)
            {
                if (board[i, Line] != null)
                {
                    if (board[i, Line].color != color)
                    {
                        if (board[i, Line].type == ChessPieceType.Queen || board[i, Line].type == ChessPieceType.Rook)
                        {
                            return SpecialMove.None;
                        }
                    }
                }
            }
        }


        if (movelist.Count>0)
        {
            Vector2Int[] lastmove = movelist[movelist.Count-1];
            if (board[lastmove[1].x, lastmove[1].y].type==ChessPieceType.Pawn) // Check if the last move did was a pawn move for a possible EnPassant
            {
                if(Mathf.Abs(lastmove[0].y - lastmove[1].y) ==2)  // Check if the difference between the position before (lastmove[0].y) and after (lastmove[1].y) is equal to 2 tiles
                {
                    if (board[lastmove[1].x,lastmove[1].y].color!=color ) // if the last move was from the other player (not useful)
                    {
                        if (lastmove[1].y==CurrentY) //If both pawns are on the same line
                        {
                            if (lastmove[1].x == CurrentX-1) // He is on my left
                            {
                                availableMoves.Add(new Vector2Int(CurrentX-1,CurrentY+direction));
                                return SpecialMove.EnPassant;
                            }
                            if(lastmove[1].x == CurrentX + 1)  // He is on my right
                            {
                                availableMoves.Add(new Vector2Int(CurrentX + 1, CurrentY + direction));
                                return SpecialMove.EnPassant;
                            }
                        }
                    }
                }
            }
        }


        return SpecialMove.None;
    }
}
