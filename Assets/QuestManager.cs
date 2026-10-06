using System;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : Singleton<QuestManager>
{
    [Header("References")]
    [SerializeField] List<Actor> _enemyList;
    [SerializeField] List<Skill> _skillList;

    public class QuestData
    {
        public string Description;
        public float value;
        public float targetValue;
        public float PercentComplete => (value / targetValue).Clamp01();

        public QuestData(string description, float value, float targetValue)
        {
            this.Description = description;
            this.value = value;
            this.targetValue = targetValue;
        }
    }

    public Action OnQuestUpdate;

    private void Start()
    {
        PlayerManager.I.OnPlayerJoin += (Player joiningPlayer) =>
        {
            this.DelayedInvoke(-1, () =>
            {
                if(joiningPlayer != null)
                {
                    joiningPlayer.Actor.OnKill += HandlePlayerKill;
                }
            });
        };
        PlayerManager.I.OnPlayerLeave += (Player leavingPlayer) =>
        {
            leavingPlayer.Actor.OnKill -= HandlePlayerKill;
        };
    }

    void HandlePlayerKill(Actor killedActor, Skill killingSkill)
    {
        // Actor kill quest
        string actorKillQuestKey = $"ActorKillQuest_{killedActor.GetName()}";
        float actorKillQuestValue = PlayerPrefs.GetFloat(actorKillQuestKey, 0);
        PlayerPrefs.SetFloat(actorKillQuestKey, actorKillQuestValue + 1);

        // Skill kill quest
        string skillKillQuestKey = $"SkillKillQuest_{killingSkill.Name}";
        float skillKillQuestValue = PlayerPrefs.GetFloat(skillKillQuestKey, 0);
        PlayerPrefs.SetFloat(skillKillQuestKey, skillKillQuestValue + 1);

        OnQuestUpdate?.Invoke();
    }

    public List<QuestData> GetKillQuests()
    {
        List<QuestData> questList = new();
        questList.AddRange(GetSkillKillQuests());
        questList.AddRange(GetEnemyKillQuests());
        questList.Sort((x, y) => (int)(y.PercentComplete - x.PercentComplete).Sign());
        return questList;
    }

    public List<QuestData> GetEnemyKillQuests()
    {
        List<QuestData> questList = new();

        GetEnemiesList().ForEach(enemy =>
        {
            string actorKillQuestKey = $"ActorKillQuest_{enemy.GetName()}";
            float currentValue = PlayerPrefs.GetFloat(actorKillQuestKey, 0);
            float targetValue = 100;
            string desc = $"Defeat {targetValue} {enemy.GetName()}";
            questList.Add(new(desc, currentValue, targetValue));
        });

        return questList;
    }
    public List<QuestData> GetSkillKillQuests()
    {
        List<QuestData> questList = new();

        GetSkillsList().ForEach(skill =>
        {
            if (skill.Stats.Damage.BaseValue <= 0) return;
            string skillKillQuestKey = $"SkillKillQuest_{skill.Name}";
            float currentValue = PlayerPrefs.GetFloat(skillKillQuestKey, 0);
            float targetValue = 100;
            string desc = $"Defeat {targetValue} enemies with the {skill.Name}";
            questList.Add(new(desc, currentValue, targetValue));
        });

        return questList;
    }

    List<Actor> GetEnemiesList()
    {
        return _enemyList;
    }

    List<Skill> GetSkillsList()
    {
        return _skillList;
    }
}
