using UnityEngine;

public class NodeMenu : MonoBehaviour
{
    private void Update()
    {
        this.gameObject.SetActive(!DungeonManager.I.IsRunStarted);
    }
}
