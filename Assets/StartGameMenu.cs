using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StartGameMenu : MonoBehaviour
{
    [Header("State")]
    [field: SerializeField] public int SelectedBiomeIndex { get; private set; }
    public BiomeData GetSelectedBiome()
    { 
        return SelectedBiomeIndex < BiomeList.Count? BiomeList [SelectedBiomeIndex] : null;
    }
    [field: SerializeField] public int SelectedTier { get; private set; }

    [Header("Data")]
    
    public List<BiomeData> BiomeList = new();

    [Header("Reference")]
    [SerializeField] SimpleButton _biomeLeft;
    [SerializeField] SimpleButton _biomeRight;
    [SerializeField] SimpleButton _tierLeft;
    [SerializeField] SimpleButton _tierRight;
    [SerializeField] SimpleButton _startButton;
    [SerializeField] Image _biomeImage;
    [SerializeField] TextMeshProUGUI _tierText;
    [SerializeField] TextMeshProUGUI _infoText;
    [SerializeField] Image _checkImage;

    SimpleMenu _simpleMenu;

    private void Start()
    {
        _simpleMenu = GetComponent<SimpleMenu>();
        _simpleMenu.OnOpenChanged += (isOpen) =>
        {
            ScrollBiome(0);
            ScrollTier(0);
            this.DelayedInvoke(-2, () =>
            {
                if (!_simpleMenu.IsOpen) return;
                EventSystem.current.SetSelectedGameObject(_startButton.gameObject);
            });
        };

        SelectedTier = PlayerPrefs.GetInt("SelectedTier", 1);
        SelectedBiomeIndex = PlayerPrefs.GetInt("SelectedBiomeIndex", 0);
        //ScrollBiome(0);
        //ScrollTier(0);

        // BIOME CONTROLS
        _biomeLeft.OnSubmit += () =>
        {
            ScrollBiome(-1);
        };
        _biomeRight.OnSubmit += () =>
        {
            ScrollBiome(1);
        };

        // TIER CONTROLS
        _tierLeft.OnSubmit += () =>
        {
            ScrollTier(-1);
        };
        _tierRight.OnSubmit += () =>
        {
            ScrollTier(1);
        };
    }

    void ScrollBiome(int i)
    {
        var maxBiomeIndex = 0;// BiomeList.Count;
        var unlockStringList = Player.GetUnlockedStringsList();
        if (unlockStringList.Contains("Cave")) maxBiomeIndex = 2;
        if (unlockStringList.Contains("Dungeon")) maxBiomeIndex = 3;

        SelectedBiomeIndex = (SelectedBiomeIndex + i).ClampInt(0, maxBiomeIndex);

        bool leftActive = SelectedBiomeIndex > 0;
        bool rightActive = SelectedBiomeIndex < (maxBiomeIndex - 1);
        _biomeLeft.gameObject.SetActive(leftActive);
        _biomeRight.gameObject.SetActive(rightActive);
        if (_simpleMenu.IsOpen)
        {
            if (i >= 0 && !rightActive)
            {
                Debug.Log("Selecting Left!");
                this.DelayedInvoke(-1, () =>
                {
                    EventSystem.current.SetSelectedGameObject(_biomeLeft.gameObject);
                });
            }
            else if (i <= 0 && !leftActive)
            {
                Debug.Log("Selecting Right!");
                this.DelayedInvoke(-1, () =>
                {
                    EventSystem.current.SetSelectedGameObject(_biomeRight.gameObject);
                });
            }
        }

        var selectedBiome = BiomeList[SelectedBiomeIndex];
        _biomeImage.sprite = selectedBiome?.Icon;
        _biomeImage.color = selectedBiome?.Color ?? Color.white;

        ScrollTier(0);

        PlayerPrefs.SetInt("SelectedBiomeIndex", SelectedBiomeIndex);

        ScrollUpdate(); 
    }

    void ScrollTier(int i)
    {
        SelectedTier += i;
        if (SelectedTier < 0) SelectedTier = 0;
        var maxTier = DungeonManager.GetMaxTier(GetSelectedBiome().BiomeType);
        if (SelectedTier > maxTier) SelectedTier = maxTier;

        _tierLeft.gameObject.SetActive(SelectedTier > 1);
        _tierRight.gameObject.SetActive(SelectedTier < maxTier);

        if(i >= 0 && !_tierRight.gameObject.activeInHierarchy) EventSystem.current.SetSelectedGameObject(_tierLeft.gameObject);
        else if(i <= 0 && !_tierLeft.gameObject.activeInHierarchy) EventSystem.current.SetSelectedGameObject(_tierRight.gameObject);

        _tierText.text = Utils.ToRomanNumeral(SelectedTier);

        PlayerPrefs.SetInt("SelectedTier", SelectedTier);

        ScrollUpdate();
    }

    void ScrollUpdate()
    {
        DungeonManager.I.Data.Biome = GetSelectedBiome().BiomeType;
        DungeonManager.I.Data.RunTier = SelectedTier;
        DungeonManager.I.UpdateBiomeTextures();

        int lootBonus = (int)((DungeonManager.I.GetLootMult() - 1f) * 100f);
        Debug.Log(lootBonus);
        _infoText.text = lootBonus > 1? $"Loot: +{lootBonus}%" : string.Empty;

        _checkImage.enabled = DungeonManager.I.GetHasCollectedRelic();

        var selectedObject = EventSystem.current.currentSelectedGameObject;
        Debug.Log("Selected: " + selectedObject.name + " " + selectedObject.activeInHierarchy);
        if (selectedObject == null || !selectedObject.activeInHierarchy)
        {
            this.DelayedInvoke(-1, () =>
            {
                if (!_simpleMenu.IsOpen) return;
                EventSystem.current.SetSelectedGameObject(_startButton.gameObject);
            });
        }
    }
}
