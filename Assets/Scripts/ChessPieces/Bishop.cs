using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bishop : ChessPiece
{
    public int points=3;
    public override List<Vector2Int> GetAvailableMoves(ref ChessPiece[,] board, int TileCountX, int TileCountY)
    {
        List<Vector2Int> r = new List<Vector2Int>();
        int cpt = 1;
        bool NoStop = true;
        while (CurrentY + cpt < TileCountY && CurrentX + cpt < TileCountX &&  NoStop) // monter droite
        {
            if (board[CurrentX+cpt, CurrentY + cpt] == null)
            {
                r.Add(new Vector2Int(CurrentX+cpt, CurrentY + cpt));
            }
            if (board[CurrentX+cpt, CurrentY + cpt] != null)
            {
                if (board[CurrentX+cpt, CurrentY + cpt].color != color )
                {
                    r.Add(new Vector2Int(CurrentX+cpt, CurrentY + cpt));
                }
                NoStop = false;
            }
            cpt++;
        }
        NoStop = true;
        cpt = 1;
        while (CurrentY - cpt >=0 && CurrentX - cpt >= 0 && NoStop) // Descendre gauche
        {
            if (board[CurrentX - cpt, CurrentY - cpt] == null)
            {
                r.Add(new Vector2Int(CurrentX - cpt, CurrentY - cpt));
            }
            if (board[CurrentX - cpt, CurrentY - cpt] != null)
            {
                if (board[CurrentX - cpt, CurrentY - cpt].color != color)
                {
                    r.Add(new Vector2Int(CurrentX - cpt, CurrentY - cpt));
                }
                NoStop = false;
            }
            cpt++;
        }
        NoStop = true;
        cpt = 1;
        while (CurrentY + cpt < TileCountX && CurrentX - cpt >= 0 && NoStop) // monter gauche
        {
            if (board[CurrentX - cpt, CurrentY + cpt] == null)
            {
                r.Add(new Vector2Int(CurrentX - cpt, CurrentY + cpt));
            }
            if (board[CurrentX - cpt, CurrentY + cpt] != null)
            {
                if (board[CurrentX - cpt, CurrentY + cpt].color != color)
                {
                    r.Add(new Vector2Int(CurrentX - cpt, CurrentY + cpt));
                }
                NoStop = false;
            }
            cpt++;
        }
        NoStop = true;
        cpt = 1;
        while (CurrentX + cpt < TileCountX && CurrentY - cpt >= 0 && NoStop) // descendre droite
        {
            if (board[CurrentX + cpt, CurrentY - cpt] == null)
            {
                r.Add(new Vector2Int(CurrentX + cpt, CurrentY - cpt));
            }
            if (board[CurrentX + cpt, CurrentY - cpt] != null)
            {
                if (board[CurrentX + cpt, CurrentY - cpt].color != color)
                {
                    r.Add(new Vector2Int(CurrentX + cpt, CurrentY - cpt));
                }
                NoStop = false;
            }
            cpt++;
        }
        NoStop = true;
        cpt = 1;

        return r;

    }
}
