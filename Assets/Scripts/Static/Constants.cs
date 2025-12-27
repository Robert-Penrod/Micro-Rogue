using UnityEngine;

public static class Constants
{
    public enum Rarity { Common, Uncommon, Rare, Epic, Legendary, Secret }
    public static float RarityToWeight(Rarity rarity)
    {
        switch (rarity)
        {
            case Rarity.Common:
                return 1f;

            case Rarity.Uncommon:
                return 1/2f;

            case Rarity.Rare:
                return 1/4f;

            case Rarity.Epic:
                return 1/8f;

            case Rarity.Legendary:
                return 1/16f;

            case Rarity.Secret:
                return 1/32f;

            default:
                return 1f;
        }
    }
    
    public static string ChangeValueString(float initValue, float newValue, float positiveDir = 1f, string unit = "")
    {
        Color initColor = new Color(0.75f, 0.75f, 0.75f);
        Color positiveColor = Color.green;
        Color negativeColor = Color.red;
        bool isPositive = Mathf.Sign(newValue - initValue) == Mathf.Sign(positiveDir);
        Color c = isPositive ? positiveColor : negativeColor;
        return ((initValue.ToStatNumString() + unit).Color(initColor) + " → " + (newValue.ToStatNumString() + unit).Color(c)).Bold();
    }

    public static string ToStatNumString(this float t)
    {
        return string.Format("{0:0.#}", t);
    }

    public static class DungeonStats
    {
        public static float PortalTime = 1.5f;
    }

    public static class ActorStats
    {
        public static float HealthGain = 2f;

        public enum FactionType
        {
            Player,
            Enemy
        }

        public static class MoveSpeed
        {
            public enum Types { Default, Fast, Slow}

            public static float? GetTypeValue(Types type)
            {
                switch(type)
                {
                    case Types.Fast:
                        return Fast;
                    case Types.Default:
                        return Default;
                    case Types.Slow:
                        return Slow;
                };
                return null;
            }

            public static float Fast => Default * 2f;
            public const float Default = 4.5f;
            public static float Slow => Default / 2f;
        }

        public static class Health
        {
            public static float High => 2.5f * SkillStats.Damage.High;
            public static float Default => 2.5f * SkillStats.Damage.Default;
            public static float Low => 2.5f * SkillStats.Damage.Light;
        }

        public static class DodgeCooldown
        {
            public static float Default = 2f; // 1.75f
        }
    }

    public static class SkillStats
    {
        public static float BaseAlpha = 1f;
        public static float BaseTelegraphTime = 0.25f;
        public static float BaseFadeTime => BaseTelegraphTime / 2f;
        public static float SIE_ProjectileInheritVelocityMult = 0.75f;
        public static float HitboxDelay = 1f;
        public static float SpawnDelay = 0.25f;

        public static class Knockback
        {
            public const float Default = 1f;
        }

        public static class Lunge
        {
            public const float Default = 0.25f;
        }

        public static class Homing
        {
            public enum Label { Default = 0, Moderate = 1, Strong = 2}
            public const float Default = 0f;
            public const float Moderate = 1f;
            public const float Strong = 2f;

            public static float LabelToStat(Label label)
            {
                switch (label)
                {
                    case Label.Default: return Default;
                    case Label.Moderate: return Moderate;
                    case Label.Strong: return Strong;
                }
                throw new System.Exception("No stat label " + label.ToString());
            }
        }

        public static class HitStun
        {
            public const float Default = 0.1f;
        }

        public static class Cooldown
        {
            public const float Short = Default / 2f;
            public const float Default = 3f;
            public const float Long = Default * 2f;
        }

        public static class Duration
        {
            public enum Label { Melee = 1, Projectile = 2}
            public const float Melee = 0.25f;
            public const float Projectile = 1f;

            public static float LabelToStat(Label label)
            {
                switch (label)
                {
                    case Label.Melee: return Melee;
                    case Label.Projectile: return Projectile;
                }
                throw new System.Exception("No stat label " + label.ToString());
            }
        }

        public static class Speed
        {
            public enum Label { Default = 0, Slow = -1, Fast = 1}
            public const float Slow = 8f;
            public const float Default = 10f;
            public const float Fast = 12f;
            public static float LabelToStat(Label label)
            {
                switch (label)
                {
                    case Label.Slow: return Slow;
                    case Label.Default: return Default;
                    case Label.Fast: return Fast;
                }
                throw new System.Exception("No stat label " + label.ToString());
            }
        }

        public static class Damage
        {
            public enum Label { Default = 0, Light = -1, High = 1 }
            public const float Light = 3f;
            public const float Default = 5f;
            public const float High = 7f;

            public static float LabelToStat(Label label)
            {
                switch (label)
                {
                    case Label.Light: return Light;
                    case Label.Default: return Default;
                    case Label.High: return High;
                }
                throw new System.Exception("No stat label " + label.ToString());
            }
        }

        public static class Pierce
        {
            public const float Default = 1;
        }

        public static class Size
        {
            public const float Small = Default / 2f;
            public const float Default = 1f;
            public const float Large = Default * 2f;
        }
    }
}
