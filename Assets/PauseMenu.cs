using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenu : SimpleMenu
{
    public static bool IsPaused { get; private set; }
    InputAction _pauseAction;

    protected override void Awake()
    {
        base.Awake();
        _pauseAction = InputSystem.actions.FindAction("Pause");
    }

    protected override void Update()
    {
        base.Update();

        if (DungeonManager.I != null && !DungeonManager.I.IsRunActive) return;

        if (_pauseAction.WasPressedThisFrame())
        {
            TogglePause();
        }
    }

    void TogglePause()
    {
        if (IsPaused) Resume();
        else Pause();
    }

    public void Pause()
    {
        IsPaused = true;
        Time.timeScale = 0.00001f;
        SetOpen(true);
    }

    public void Resume()
    {
        IsPaused = false;
        Time.timeScale = 1f;
        SetOpen(false);
    }

    public void EndRun()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
