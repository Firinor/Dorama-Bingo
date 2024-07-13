using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BingoCard
{
    public string Dorama;
    public Vector2Int Size;
    public BingoCell[] Cells;

    public List<BingoCell> GetActiveCells()
    {
        List<BingoCell> result = new();

        foreach(BingoCell cell in Cells)
        {
            if (cell.IsPressed)
                result.Add(cell);
        }

        return result;
    }
}
