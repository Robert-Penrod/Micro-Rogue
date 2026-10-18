using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EndRunQuoteText : TMProSetter
{
    [TextArea]
    public string GeneralQuotes;

    [TextArea]
    public string SoCloseQuotes;

    [TextArea]
    public string BeatBossQuotes;

    public void RandomizeQuote()
    {
        TextMesh.text = GetQuote();
    }
    
    string GetQuote()
    {
        string quoteString = GeneralQuotes;

        int runFloorCount = DungeonManager.I.GetLoopRoomCount();
        int floor = DungeonManager.I.Data.Coordinate.y;
        float runPercent = (float)floor / runFloorCount;
        if (runPercent >= 1f)
        {
            quoteString = BeatBossQuotes;
        }
        else if(runPercent >= 0.6f)
        {
            quoteString = SoCloseQuotes;
        }

        return GetQuoteList(quoteString).GetRandomElement();
    }

    List<string> GetQuoteList(string quoteString)
    {
        List<string> quoteList = new(quoteString.Split(";"));
        return quoteList.Select(quote => quote.Trim() + ".").ToList();
    }
}
