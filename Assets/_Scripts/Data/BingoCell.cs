using System;

[Serializable]
public class BingoCell
{
    public string Tag;
    public int X;
    public int Y;
    public bool IsNeutral = true;
    public bool IsCorrect;

    public BingoCell() { }

    public BingoCell(BingoCell cell)
    {
        this.Tag = cell.Tag;
        this.X = cell.X;
        this.Y = cell.Y;
        this.IsNeutral = cell.IsNeutral;
        this.IsCorrect = cell.IsCorrect;
    }
}
