using UnityEngine;

public class QuestText : TMProSetter
{
    private void Start()
    {
        DungeonManager.I.OnDungeonDataChanged += () =>
        {
            UpdateQuestText();
        };
        DungeonManager.I.OnGenerateLevel += () =>
        {
            UpdateQuestText();
        };
        QuestManager.I.OnQuestUpdate += () =>
        {
            UpdateQuestText();
        };

        UpdateQuestText();
    }

    void UpdateQuestText()
    {
        var questList = QuestManager.I.GetKillQuests();
        string questText = string.Empty;
        for (int i = 0; i < 3; i++)
        {
            var quest = questList[i];
            if (i != 0) questText += "\n";
            questText += $"- {quest.Description} ({(int)(quest.PercentComplete * 100)}%)";
        }
        TextMesh.text = questText;
    }
}
