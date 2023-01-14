namespace Elite.Enums
{
    public enum CommandKey
    {
        F1 = 112,
        F2 = 113,
        F3 = 114,
        F4 = 115,
        F5 = 116,
        F6 = 117,
        F7 = 118,
        F8 = 119,
        F9 = 120,
        F10 = 121,
        F11 = 122,
        F12 = 123,

        //TODO: fix these for arrow keys
        Up = 83, // S (or UP)
        Down = 88, // X (|| DOWN)
        Left = 188, // < (|| LEFT)
        Right = 190, // > (|| RIGHT)

        Y = 89,
        N = 78,

        Fire = 65, // A
        ECM = 69, // E
                                          // TODO: Fix unhandled TAB
        EnergyBomb = 9, // TAB
        Hyperspace = 72, // H
        CTRL = 17, // CTRL
        Jump = 74, // J
        Escape = 27, // ESC

        Dock = 67, // C
        D = 68, // D
        Origin = 79, // O
        Find = 70, // F

        FireMissile = 77, // M
        TargetMissile = 84, // T
        UnarmMissile = 85, // U

        Pause = 80, // P
        Resume = 82, // R

        IncSpeed = 32, // SPACE
        DecSpeed = 191, // FRONTSLASH

        Enter = 13, // ENTER
        Backspace = 8, // BACKSPACE
        Space = 32, // SPACE
    }
}