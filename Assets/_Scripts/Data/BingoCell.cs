using System;

[Serializable]
public class BingoCell
{
    public string Tag;
    public int X;
    public int Y;
    public bool IsNeutral = true;
    public bool IsCorrect;
}
