using Elite.Engine.Enums;

namespace Elite.Engine
{
    public interface IKeyboard
    {
        void KeyDown(CommandKey keyValue);

        void KeyUp(CommandKey keyValue);

        bool IsKeyPressed(params CommandKey[] key);

        CommandKey GetKeyPressed();

        void ClearKeyPressed();
    }
}