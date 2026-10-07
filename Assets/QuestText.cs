using UnityEngine;

public class QuestText : TMProSetter
{
    [SerializeField] SimpleMenu _upgradeGridMenu;
    float _questUpdateTick = 0f;

    private void Start()
    {
        DungeonManager.I.OnDungeonDataChanged += () =>
        {
            QuestUpdate();
            _questUpdateTick = 1f;
        };
        DungeonManager.I.OnGenerateLevel += () =>
        {
            QuestUpdate();
        };
        QuestManager.I.OnQuestUpdate += () =>
        {
            QuestUpdate();
        };

        QuestUpdate();

        _questUpdateTick = 0f;
        TextMesh.color = TextMesh.color.Alpha(0f);
    }

    private void Update()
    {
        float targetAlpha = 0f;
        float minAlpha = 0f;

        if (DungeonManager.I.IsRunStarted) minAlpha = 0.25f;

        //if (_upgradeGridMenu.IsOpen) targetAlpha = 1f;
        if (UpgradeMenu.I.IsOpen) targetAlpha = 1f;
        else if(_questUpdateTick > 0)
        {
            targetAlpha = _questUpdateTick;
            _questUpdateTick -= 0.15f * Time.deltaTime;
        }

        targetAlpha = targetAlpha.ClampMin(minAlpha);

        float lerpAlpha = TextMesh.color.a.Lerp(targetAlpha, 12f * Time.deltaTime);
        TextMesh.color = TextMesh.color.Alpha(lerpAlpha);
    }

    void QuestUpdate()
    {
        var newQuestText = GetQuestText();
        if (newQuestText != TextMesh.text)
        {
            _questUpdateTick = 1f;

            this.DelayedInvoke(0.5f, () =>
            {
                TextMesh.text = GetQuestText();
            });
        }
    }

    string GetQuestText()
    {
        var questList = QuestManager.I.GetKillQuests().FindAll(x =>
        {
            return x.PercentComplete > 0f && x.PercentComplete < 1f;
        });

        string questText = string.Empty;
        for (int i = 0; i < 3 && i < questList.Count; i++)
        {
            var quest = questList[i];
            if (i != 0) questText += "\n";
            questText += $"{quest.Description} ({(int)(quest.PercentComplete * 100f)}%)";
        }
        return questText;
    }

}
