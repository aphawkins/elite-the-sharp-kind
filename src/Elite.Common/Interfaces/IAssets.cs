namespace Elite.Common.Interfaces
{
    using Elite.Common.Enums;

    public interface IAssets
    {
        public Stream? Load(Image image);

        public Stream? Load(SoundEffect effect);

        public Stream? Load(Music music);
    }
}