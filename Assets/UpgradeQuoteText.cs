using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UpgradeQuoteText : TMProSetter
{
    [TextArea]
    public string GeneralQuotes;

    public void RandomizeQuote()
    {
        TextMesh.text = GetQuote();
    }

    string GetQuote()
    {
        string quoteString = GeneralQuotes;



        return GetQuoteList(quoteString).GetRandomElement();
    }

    List<string> GetQuoteList(string quoteString)
    {
        List<string> quoteList = new(quoteString.Split(";"));
        return quoteList.Select(quote => quote.Trim()).Where(quote => quote != string.Empty).ToList();
    }
}
