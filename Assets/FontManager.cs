using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FontManager : Singleton<FontManager>
{
    public TMP_FontAsset Font;

    private void OnValidate()
    {
        new List<FontHandler>(FindObjectsByType<FontHandler>(FindObjectsSortMode.None)).ForEach(fontHandler =>
        {
            fontHandler.SetFont(Font);
        });
    }
}
