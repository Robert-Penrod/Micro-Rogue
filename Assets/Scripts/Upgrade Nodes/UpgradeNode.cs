using UnityEngine;
using UnityEngine.UI;

public class UpgradeNode : MonoBehaviour
{
    [System.Serializable]
    public class UpgradeNodeStatUpgrade
    {
        public SkillStats.SkillStatTypes SkillStatType;
        public ActorStats.ActorStatTypes ActorStatType;
        public float FlatBonus;
    }


    [Header("Unlock")]
    [SerializeField] string UpgradeString;
    [SerializeField] UpgradeNodeStatUpgrade _unlockedStat;
    [SerializeField] Skill _unlockedSkill;
    [SerializeField] Actor _unlockedChar;

    [Header("Cost")]
    [SerializeField] int _gemCost;

    [Header("Params")]
    [SerializeField] string _name;
    [SerializeField] int _maxUpgrades = 1;
    [SerializeField] Sprite _sprite;
    [SerializeField] Color _color = Color.clear;

    [Header("References")]
    [SerializeField] Image _iconRef;
    [SerializeField] Image _outlineRef;

    private void OnValidate()
    {
        _iconRef.enabled = false;

        if (_sprite != null)
        {
            _iconRef.enabled = true;
            _iconRef.sprite = _sprite;
            if (_color != Color.clear) _iconRef.color = _color;
        }

        if(_unlockedSkill != null)
        {
            _iconRef.enabled = true;
            _iconRef.sprite = _unlockedSkill.Icon;
            _iconRef.color = _unlockedSkill.GetColor().SetValue(1f);
        }

        if(_unlockedChar != null)
        {
            _iconRef.enabled = true;
            _iconRef.sprite = _unlockedChar._spriteRend.sprite;
            _iconRef.color = _unlockedChar.Color;
        }

        _iconRef.gameObject.SetActive(_iconRef.enabled);
    }

    public string GetDescription()
    {
        return "(not implimented)";
    }
}
