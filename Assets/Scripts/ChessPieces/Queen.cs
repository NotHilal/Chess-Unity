using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Queen : ChessPiece
{
    public int points = 9;
    public override List<Vector2Int> GetAvailableMoves(ref ChessPiece[,] board, int TileCountX, int TileCountY)
    {
        List<Vector2Int> r = new List<Vector2Int>();
        int cpt = 1;
        bool NoStop = true;
        while (CurrentY + cpt < TileCountY && CurrentX + cpt < TileCountX && NoStop) // monter droite
        {
            if (board[CurrentX + cpt, CurrentY + cpt] == null)
            {
                r.Add(new Vector2Int(CurrentX + cpt, CurrentY + cpt));
            }
            if (board[CurrentX + cpt, CurrentY + cpt] != null)
            {
                if (board[CurrentX + cpt, CurrentY + cpt].color != color)
                {
                    r.Add(new Vector2Int(CurrentX + cpt, CurrentY + cpt));
                }
                NoStop = false;
            }
            cpt++;
        }
        NoStop = true;
        cpt = 1;
        while (CurrentY - cpt >= 0 && CurrentX - cpt >= 0 && NoStop) // Descendre gauche
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
        while (CurrentY + cpt < TileCountX && NoStop) // monter
        {
            if (board[CurrentX, CurrentY + cpt] == null && CurrentY + cpt < TileCountX)
            {
                r.Add(new Vector2Int(CurrentX, CurrentY + cpt));
            }
            if (board[CurrentX, CurrentY + cpt] != null)
            {
                if (board[CurrentX, CurrentY + cpt].color != color )
                {
                    r.Add(new Vector2Int(CurrentX, CurrentY + cpt));
                }
                NoStop = false;
            }
            cpt++;
        }
        NoStop = true;
        cpt = 1;
        while (CurrentY - cpt >= 0 && NoStop) // descendre
        {
            if (board[CurrentX, CurrentY - cpt] == null && CurrentY - cpt >= 0)
            {
                r.Add(new Vector2Int(CurrentX, CurrentY - cpt));
            }
            if (board[CurrentX, CurrentY - cpt] != null)
            {
                if (board[CurrentX, CurrentY - cpt].color != color )
                {
                    r.Add(new Vector2Int(CurrentX, CurrentY - cpt));
                }
                NoStop = false;
            }
            cpt++;
        }
        cpt = 1;
        NoStop = true;

        while (CurrentX + cpt < TileCountX && NoStop) // droite
        {
            if (board[CurrentX + cpt, CurrentY] == null && CurrentX + cpt < TileCountX)
            {
                r.Add(new Vector2Int(CurrentX + cpt, CurrentY));
            }
            if (board[CurrentX + cpt, CurrentY] != null)
            {
                if (board[CurrentX + cpt, CurrentY].color != color )
                {
                    r.Add(new Vector2Int(CurrentX + cpt, CurrentY));
                }
                NoStop = false;
            }
            cpt++;
        }
        cpt = 1;
        NoStop = true;
        while (CurrentX - cpt >= 0 && NoStop) // droite
        {
            if (board[CurrentX - cpt, CurrentY] == null && CurrentX - cpt >= 0)
            {
                r.Add(new Vector2Int(CurrentX - cpt, CurrentY));
            }
            if (board[CurrentX - cpt, CurrentY] != null)
            {
                if (board[CurrentX - cpt, CurrentY].color != color )
                {
                    r.Add(new Vector2Int(CurrentX - cpt, CurrentY));
                }
                NoStop = false;
            }
            cpt++;
        }
        return r;

    }
}
