using System;
using UnityEngine;

[Serializable]
public class LoadBingoCardData
{
    public string DoramaName;
    //Application.persistentDataPath = "C:\Users\<userprofile>\AppData\LocalLow\<companyname>\<productname>"
    //path + "/scrn-1.jpg";
    public string ScreenPath;
    public DateTime Date;
    public BingoCard BingoCard;
}
