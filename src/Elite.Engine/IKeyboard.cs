﻿namespace Elite.Engine
{
    using Elite.Engine.Enums;

    public interface IKeyboard
    {
        void KeyDown(int keyValue);

        void KeyUp(int keyValue);

        bool IsKeyPressed(CommandKey key);

        int GetKeyPressed();

        void ClearKeyPressed();
    }
}