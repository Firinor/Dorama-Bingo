using System;
using System.Collections.Generic;

public class WinConditions
{
    public bool IsBINGO(BingoCard card)
    {
        List<BingoCell> cells = card.GetActiveCells();

        if (cells.Count == 0)
            return false;

        int cardSide = card.Size.x;

        bool columnResult = ColumnsChecking(card, cells, cardSide);
        if (columnResult)
            return true;

        bool rowResult = RowsChecking(card, cells, cardSide);
        if (rowResult)
            return true;

        bool diagonalResult = DiagonalsChecking(card, cells, cardSide);
        if (diagonalResult)
            return true;

        return false;
    }

    private bool DiagonalsChecking(BingoCard card, List<BingoCell> activeCells, int cardSide)
    {
        // "\" -diagonal
        int diagonalHit = 0;
        List<BingoCell> diagonalCells = activeCells.FindAll(x => x.X == x.Y);
        if (diagonalCells.Count == cardSide)
        {
            for (int i = 0; i < diagonalCells.Count; i++)
            {
                if (DataBase.Doramas[card.Dorama][activeCells[i].Tag] != true)
                    break;

                diagonalHit++;
            }
            if (diagonalHit == cardSide)
                return true;
        }

        // "/" -diagonal
        diagonalHit = 0;
        diagonalCells = activeCells.FindAll(x => x.X + x.Y == cardSide - 1);// x0y4, x1y3, x2y2, x3y1, x4y0
        if (diagonalCells.Count == cardSide)
        {
            for (int i = 0; i < diagonalCells.Count; i++)
            {
                if (DataBase.Doramas[card.Dorama][activeCells[i].Tag] != true)
                    break;

                diagonalHit++;
            }

            if (diagonalHit == cardSide)
                return true;
        }

        return false;
    }

    private bool RowsChecking(BingoCard card, List<BingoCell> cells, int cardSide)
    {
        //Row check
        for (int i = 0; i < cardSide; i++)
        {
            BingoCell cell = cells[i];

            if (DataBase.Doramas[card.Dorama][cell.Tag] != true)
                break;//The user did not guess the tag

            if (cell.X != 0)
                break;//Not the first column

            List<BingoCell> rowCells = cells.FindAll(x => x.Y == cell.Y);

            if (rowCells.Count < cardSide)
                break;//An incomplete column has been assembled

            bool IsBingo = true;

            //Columns check
            for (int j = 1; j < rowCells.Count; j++)
            {
                if (DataBase.Doramas[card.Dorama][rowCells[j].Tag] != true)
                {
                    IsBingo = false;
                    break;//The user did not guess the tag
                }
            }

            if (IsBingo)
                return true;
        }

        return false;
    }
    private bool ColumnsChecking(BingoCard card, List<BingoCell> cells, int cardSide)
    {
        //Column check
        for (int i = 0; i < cardSide; i++)
        {
            BingoCell cell = cells[i];

            if (DataBase.Doramas[card.Dorama][cell.Tag] != true)
                break;//The user did not guess the tag

            if (cell.Y != 0)
                break;//Not the first line

            List<BingoCell> columnCells = cells.FindAll(x => x.X == cell.X);

            if (columnCells.Count < cardSide)
                break;//An incomplete column has been assembled

            bool IsBingo = true;

            //Rows check
            for (int j = 1; j < columnCells.Count; j++)
            {
                if (DataBase.Doramas[card.Dorama][columnCells[j].Tag] != true)
                {
                    IsBingo = false;
                    break;//The user did not guess the tag
                }
            }

            if (IsBingo)
                return true;
        }

        return false;
    }
}