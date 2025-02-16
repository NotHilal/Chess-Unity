using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class King : ChessPiece
{
    public int points = 20;
    public override List<Vector2Int> GetAvailableMoves(ref ChessPiece[,] board, int TileCountX, int TileCountY)
    {
        List<Vector2Int> r = new List<Vector2Int>();

        if(CurrentX+1< TileCountX && CurrentY + 1 < TileCountY)
        {
            if (board[CurrentX + 1, CurrentY + 1]==null|| board[CurrentX + 1, CurrentY + 1].color !=color)
            {
                r.Add(new Vector2Int(CurrentX+1, CurrentY+1));
            }
        }
        if (CurrentX - 1 >=0 && CurrentY - 1 >= 0)
        {
            if (board[CurrentX - 1, CurrentY - 1] == null || board[CurrentX - 1, CurrentY - 1].color != color)
            {
                r.Add(new Vector2Int(CurrentX - 1, CurrentY - 1));
            }
        }
        if (CurrentX + 1 < TileCountX && CurrentY - 1 >= 0)
        {
            if (board[CurrentX + 1, CurrentY - 1] == null || board[CurrentX + 1, CurrentY - 1].color != color)
            {
                r.Add(new Vector2Int(CurrentX + 1, CurrentY - 1));
            }
        }
        if (CurrentX - 1 >= 0 && CurrentY + 1 < TileCountY)
        {
            if (board[CurrentX - 1, CurrentY + 1] == null || board[CurrentX - 1, CurrentY + 1].color != color)
            {
                r.Add(new Vector2Int(CurrentX - 1, CurrentY + 1));
            }
        }

        if (CurrentX + 1 < TileCountX && CurrentY < TileCountY)
        {
            if (board[CurrentX + 1, CurrentY ] == null || board[CurrentX + 1, CurrentY ].color != color)
            {
                r.Add(new Vector2Int(CurrentX + 1, CurrentY ));
            }
        }
        if (CurrentX - 1 >= 0 && CurrentY  >= 0)
        {
            if (board[CurrentX - 1, CurrentY ] == null || board[CurrentX - 1, CurrentY ].color != color)
            {
                r.Add(new Vector2Int(CurrentX - 1, CurrentY ));
            }
        }
        if (CurrentX  < TileCountX && CurrentY - 1 >= 0)
        {
            if (board[CurrentX , CurrentY - 1] == null || board[CurrentX , CurrentY - 1].color != color)
            {
                r.Add(new Vector2Int(CurrentX , CurrentY - 1));
            }
        }
        if (CurrentX  >= 0 && CurrentY + 1 < TileCountY)
        {
            if (board[CurrentX, CurrentY + 1] == null || board[CurrentX , CurrentY + 1].color != color)
            {
                r.Add(new Vector2Int(CurrentX , CurrentY + 1));
            }
        }


        return r;
    }

    public override SpecialMove GetSpecialMoves(ref ChessPiece[,] board, ref List<Vector2Int[]> movelist, ref List<Vector2Int> availableMoves, int TileCountX, int TileCountY)
    {
        List<Vector2Int> opponentmoves = new List<Vector2Int>();
        bool possible = true;
        for (int i = 0; i < TileCountX; i++)
        {
            for (int j = 0; j < TileCountY; j++)
            {
                if(board[i, j] != null)
                {
                    if (board[i, j].color != color)
                    {
                        List<Vector2Int> b = board[i, j].GetAvailableMoves(ref board, TileCountX, TileCountY);
                        for (int s = 0; s < b.Count; s++)
                        {
                            opponentmoves.Add(b[s]);
                        }
                    }
                }
            }
        }
        SpecialMove r = SpecialMove.None;
        int a = -1;
        if(color==0)
        {
            a = 0; //position en Y du roi blanc
        }
        else
        {
            a = 7; //position en Y du roi noir
        }
        var kingmove = movelist.Find(m => m[0].x == 4 && m[0].y == a);  // On cherche si les roi on deja bouge (si ils ya deja eu un move dans movelist de la case [4,0] ou [4,7]
        var leftrook = movelist.Find(m => m[0].x == 0 && m[0].y == a);  // de meme pour la tour de gauche
        var rightrook = movelist.Find(m => m[0].x == 7 && m[0].y == a); // de meme pour la tour de droite
        if(kingmove ==null && CurrentX==4)
        {
            // white 
            if(color==0)
            {
                if (leftrook == null )
                {
                    if(board[3,0]==null && board[2, 0] == null && board[1, 0] == null && board[0, 0] != null && board[0, 0].type ==ChessPieceType.Rook) //verifier les places vides et que tout a gauche == tour
                    {
                        if (board[0, 0].color == 0) // verifier si la tour est de meme couleur
                        {
                            
                            for (int i = 0; i < opponentmoves.Count; i++)
                            {
                                if (opponentmoves[i].x==3 && opponentmoves[i].y == 0)
                                {
                                     possible = false;
                                }
                                if (opponentmoves[i].x == 2 && opponentmoves[i].y == 0)
                                {
                                    possible = false;
                                }
                                if (opponentmoves[i].x == 1 && opponentmoves[i].y == 0)
                                {
                                    possible = false;
                                }
                                if (opponentmoves[i].x == 0 && opponentmoves[i].y == 0)
                                {
                                    possible = false;
                                }

                            }
                            if(possible)
                            {
                                availableMoves.Add(new Vector2Int(2, 0));
                                r = SpecialMove.Casteling;
                            }
                            
                        }
                    }

                }
                possible = true;
                if (rightrook == null)
                {
                    if (board[5, 0] == null && board[6, 0] == null && board[7, 0] != null && board[7, 0].type == ChessPieceType.Rook)
                    {
                        if (board[7, 0].color == 0)
                        {
                            for (int i = 0; i < opponentmoves.Count; i++)
                            {
                                if (opponentmoves[i].x == 5 && opponentmoves[i].y == 0)
                                {
                                    possible = false;
                                }
                                if (opponentmoves[i].x == 6 && opponentmoves[i].y == 0)
                                {
                                    possible = false;
                                }
                                if (opponentmoves[i].x == 7 && opponentmoves[i].y == 0)
                                {
                                    possible = false;
                                }

                            }
                            if (possible)
                            {
                                availableMoves.Add(new Vector2Int(6, 0));
                                r = SpecialMove.Casteling;
                            }
                        }
                    }
                }
                possible = true;
            }
            else
            {
                if (leftrook == null)
                {
                    if (board[3, 7] == null && board[2, 7] == null && board[1, 7] == null && board[0, 7] != null && board[0, 7].type == ChessPieceType.Rook)
                    {
                        if(board[0, 7].color==1)
                        {
                            for (int i = 0; i < opponentmoves.Count; i++)
                            {
                                if (opponentmoves[i].x == 2 && opponentmoves[i].y == 7)
                                {
                                    possible = false;
                                }
                                if (opponentmoves[i].x == 3 && opponentmoves[i].y == 7)
                                {
                                    possible = false;
                                }
                                if (opponentmoves[i].x == 1 && opponentmoves[i].y == 7)
                                {
                                    possible = false;
                                }
                                if (opponentmoves[i].x == 0 && opponentmoves[i].y == 7)
                                {
                                    possible = false;
                                }
                            }
                            if (possible)
                            {
                                availableMoves.Add(new Vector2Int(2, 7));
                                r = SpecialMove.Casteling;
                            }
                        }
                    }
                }
                possible = true;
                if (rightrook == null)
                {
                    if (board[5, 7] == null && board[6, 7] == null && board[7, 7] != null && board[7, 7].type == ChessPieceType.Rook)
                    {
                        if (board[7, 7].color == 1)
                        {
                            for (int i = 0; i < opponentmoves.Count; i++)
                            {
                                if (opponentmoves[i].x == 5 && opponentmoves[i].y == 7)
                                {
                                    possible = false;
                                }
                                if (opponentmoves[i].x == 6 && opponentmoves[i].y == 7)
                                {
                                    possible = false;
                                }
                                if (opponentmoves[i].x == 7 && opponentmoves[i].y == 7)
                                {
                                    possible = false;
                                }

                            }
                            if (possible)
                            {
                                availableMoves.Add(new Vector2Int(6, 7));
                                r = SpecialMove.Casteling;
                            }
                        }
                    }
                }
                
            }
            
        }
        possible = true;
        return r;
    }
}
