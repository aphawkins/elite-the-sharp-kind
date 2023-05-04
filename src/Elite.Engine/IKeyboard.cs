namespace Elite.Engine
{
    using Elite.Engine.Enums;

    public interface IKeyboard
    {
        void KeyDown(CommandKey keyValue);

        void KeyUp(CommandKey keyValue);

        bool IsKeyPressed(params CommandKey[] key);

        CommandKey GetKeyPressed();

        void ClearKeyPressed();
    }
}