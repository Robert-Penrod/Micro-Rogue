using UnityEngine;

public class LevelSkip_IE : ItemEffect
{
    public int LevelsToSkip = 1;
    bool _hasApplied;

    private void OnEnable()
    {
        if (!_hasApplied)
        {
            _hasApplied = true;
            DungeonManager.I.Data.LevelSkip += LevelsToSkip;
        }
    }
}
