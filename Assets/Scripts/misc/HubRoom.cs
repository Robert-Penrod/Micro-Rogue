using System;
using UnityEngine;

public class HubRoom : Singleton<HubRoom>
{
    public int Level;
    string _hubLevelKey = "hub_level";

    public Action OnUpgrade;

    protected override void Awake()
    {
        base.Awake();
        Level = PlayerPrefs.GetInt(_hubLevelKey, 0);
    }

    public void Upgrade()
    {
        Level++;
        PlayerPrefs.SetInt(_hubLevelKey, Level);
        OnUpgrade?.Invoke();
    }
}
