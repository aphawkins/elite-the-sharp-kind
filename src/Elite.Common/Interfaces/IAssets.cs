using Elite.Common.Enums;

namespace Elite.Common.Interfaces
{
    public interface IAssets
    {
        public Stream? Load(Image image);

        public Stream? Load(SoundEffect effect);

        public Stream? Load(Music music);
    }
}
