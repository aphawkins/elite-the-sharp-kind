namespace Elite.Common.Interfaces
{
    using Elite.Common.Enums;

    public interface IAssets
    {
        public Stream? Load(IMG image);

        public Stream? Load(Sfx effect);

        public Stream? Load(Music music);
    }
}