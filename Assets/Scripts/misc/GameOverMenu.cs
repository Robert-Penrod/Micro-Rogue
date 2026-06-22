using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverMenu : SimpleMenu
{
    bool _isGameOver;

    protected override void Update()
    {
        base.Update();

        if (!_isGameOver && PlayerManager.I.AreAllPlayersDead())
        {
            _isGameOver = true;
            if (!IsOpen) SetOpen(true);
        }
    }
}
