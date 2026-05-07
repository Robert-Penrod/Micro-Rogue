using UnityEngine;

public class HubLevelUnlockObject : MonoBehaviour
{
    public int UnlockLevel = 1;
    bool _unlocked;

    private void Start()
    {
        transform.GetChild(0).gameObject.SetActive(false);
        transform.localScale = Vector3.zero;
        HubRoom.I.OnUpgrade += () =>
        {
            UpdateState();
        };
        UpdateState();
    }

    void UpdateState()
    {
        int hubLevel = HubRoom.I.Level;
        if(hubLevel >= UnlockLevel)
        {
            _unlocked = true;
            transform.GetChild(0).gameObject.SetActive(true);
        }
    }

    private void Update()
    {
        float lerpS = transform.localScale.x.Lerp(_unlocked ? 1f : 0f, 6f * Time.deltaTime);
        transform.localScale = Vector3.one * lerpS;
    }
}
