using TMPro;
using UnityEngine;

public class CampUpgrade_IBC : IBC
{
    [SerializeField] TextMeshPro _textMesh;

    private void Start()
    {
        UpdateCostText();
    }

    private void Update()
    {
        _interactionBubble.IsInteractable = Player.PlayerData.Gem >= CalculateCampUpgradeCost();
    }

    int CalculateCampUpgradeCost()
    {
        return (int)Mathf.Pow(2, (PlayerPrefs.GetInt("hub_level", 0) + 1));
    }

    void UpdateCostText()
    {
        _textMesh.text = "-" + CalculateCampUpgradeCost().ToString();
    }

    public void Upgrade()
    {
        Player.PlayerData.Gem -= CalculateCampUpgradeCost();
        HubRoom.I.Upgrade();
        UpdateCostText();
    }
}
