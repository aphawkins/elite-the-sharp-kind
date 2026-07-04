// 'Useful Libraries' - Andy Hawkins 2025.

using NAudio.Wave;

namespace Useful.Audio;

// Endlessly loops a stereo sample buffer, resampled by a variable pitch
// using linear interpolation (used for engine-style sounds).
internal sealed class PitchedLoopSampleProvider(ReadOnlyMemory<float> sampleData) : ISampleProvider
{
    private readonly ReadOnlyMemory<float> _sampleData = sampleData;
    private double _position;

    public WaveFormat WaveFormat { get; } = WaveFormat.CreateIeeeFloatWaveFormat(44100, 2);

    // 1.0 plays at the recorded rate.
    public double Pitch { get; set; } = 1.0;

    public int Read(float[] buffer, int offset, int count)
    {
        Guard.ArgumentNull(buffer);

        ReadOnlySpan<float> data = _sampleData.Span;
        int frames = data.Length / 2;

        if (frames < 2)
        {
            Array.Clear(buffer, offset, count);
            return count;
        }

        double pitch = Pitch;
        for (int i = 0; i < count; i += 2)
        {
            int frame0 = (int)_position;
            int frame1 = frame0 + 1 >= frames ? 0 : frame0 + 1;
            float fraction = (float)(_position - frame0);

            buffer[offset + i] = data[frame0 * 2] + ((data[frame1 * 2] - data[frame0 * 2]) * fraction);
            buffer[offset + i + 1] =
                data[(frame0 * 2) + 1] + ((data[(frame1 * 2) + 1] - data[(frame0 * 2) + 1]) * fraction);

            _position += pitch;
            while (_position >= frames)
            {
                _position -= frames;
            }
        }

        return count;
    }
}
