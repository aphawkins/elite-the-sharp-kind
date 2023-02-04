namespace Elite.Engine
{
    internal interface IView
    {
        void Reset();

        void UpdateUniverse();

        void Draw();

        void HandleInput();
    }
}
