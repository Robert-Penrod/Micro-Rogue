using System.Collections.Generic;
using UnityEngine;

public class DebugTools : MonoBehaviour
{
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.K))
        {
            var actorList = new List<Actor>(FindObjectsByType<Actor>(FindObjectsSortMode.None));
            actorList.ForEach(x =>
            {
                if (x.Faction == Actor.FactionType.Player) return;
                x.Stats.Health = 0;
            });
        }
    }
}
