using UnityEngine;

[RequireComponent(typeof(SimpleMenu))]
public class MainMenu : MonoBehaviour
{
    [SerializeField] SimpleMenu _shopMenu;
    SimpleMenu _thisMenu;

    private void Awake()
    {
        _thisMenu = GetComponent<SimpleMenu>();
    }

    private void Start()
    {
        bool isRestarting = PlayerPrefs.GetInt("Restarting", 0) > 0;

        if (isRestarting)
        {
            this.DelayedInvoke(-2, () =>
            {
                DungeonManager.I.StartGame();
            });
            _thisMenu.SetOpen(false);
            this.DelayedInvoke(-1, () =>
            {
                PlayerPrefs.SetInt("Restarting", 0);
            });
        }
        else
        {
            if(PlayerPrefs.GetInt("IsInvDirty", 1) > 0)
            {
                PlayerPrefs.SetInt("IsInvDirty", 0);
                ActorSkillSystem.ClearAllSavedSkills();
            }

            if (PlayerManager.I.PlayerList.Count > 0)
            {
                _thisMenu.SetOpen(false);
                _shopMenu.SetOpen(true);
            }
        }
    }
}
