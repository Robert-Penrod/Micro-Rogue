using UnityEngine;

public static class Constants
{
    public static class DungeonStats
    {
        public static float PortalTime = 1.5f;
    }

    public static class ActorStats
    {
        public static class MoveSpeed
        {
            public static float Fast => Default * 2f;
            public const float Default = 4f;
            public static float Slow => Default / 2f;
        }

        public static class Health
        {
            public static float High => 3 * SkillStats.Damage.High;
            public static float Default => 3 * SkillStats.Damage.Default;
            public static float Low => 3 * SkillStats.Damage.Light;
        }
    }

    public static class SkillStats
    {
        public static class Damage
        {
            public const float High = 7f;
            public const float Default = 5f;
            public const float Light = 3f;

        }
    }
}
