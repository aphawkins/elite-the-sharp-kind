namespace Elite.Enums
{
    [Flags]
    internal enum FLG
    {
        FLG_DEAD = 1,
        FLG_REMOVE = 2,
        FLG_EXPLOSION = 4,
        FLG_ANGRY = 8,
        FLG_FIRING = 16,
        FLG_HAS_ECM = 32,
        FLG_HOSTILE = 64,
        FLG_CLOAKED = 128,
        FLG_FLY_TO_PLANET = 256,
        FLG_FLY_TO_STATION = 512,
        FLG_INACTIVE = 1024,
        FLG_SLOW = 2048,
        FLG_BOLD = 4096,
        FLG_POLICE = 8192,
    }
}