using UnityEngine;

public static class Constants
{
    public static class DungeonStats
    {
        public static float PortalTime = 2f;
    }

    public static class ActorStats
    {
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
            public static float High => 3 * SkillStats.Damage.High;
            public static float Default => 3 * SkillStats.Damage.Default;
            public static float Low => 3 * SkillStats.Damage.Light;
        }

        public static class DodgeCooldown
        {
            public static float Default = 3f;
        }
    }

    public static class SkillStats
    {
        public static float BaseAlpha = 1f;
        public static float BaseTelegraphTime = 0.2f;
        public static float BaseFadeTime => BaseTelegraphTime / 2f;
        public static float SIE_ProjectileInheritVelocityMult = 0.5f;

        public static class HitboxDelay
        {
            public const float Default = 1f;
        }

        public static class Knockback
        {
            public const float Default = 1f;
        }

        public static class Lunge
        {
            public const float Default = 0.25f;
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
            public const float Melee = 0.25f;
            public const float Projectile = 1f;
        }

        public static class Speed
        {
            public const float Slow = Default / 2f;
            public const float Default = 8f;
            public const float Fast = Default * 2f;
        }

        public static class Damage
        {
            public const float Light = 3f;
            public const float Default = 5f;
            public const float High = 7f;
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
