// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Diagnostics;
using NAudio.Vorbis;
using NAudio.Wave;

namespace EliteSharp.Audio
{
    public class SoundSampleProvider : ISampleProvider
    {
        private readonly float[] _audioData;
        private long _position;

        public SoundSampleProvider(string fileName)
        {
            using WaveStream audioReader = string.Equals(
                Path.GetExtension(fileName),
                ".ogg",
                StringComparison.OrdinalIgnoreCase)
                ? new VorbisWaveReader(fileName)
                : new AudioFileReader(fileName);

            Debug.Assert(audioReader.WaveFormat.Channels == 2, "Audio should have correct number of channels.");
            Debug.Assert(audioReader.WaveFormat.SampleRate == 44100, "Audio should have correct sample rate.");
            Debug.Assert(audioReader.WaveFormat.BitsPerSample == 32, "Audio should have correct bits per sample.");

            WaveFormat = audioReader.WaveFormat;
            List<float> wholeFile = new((int)(audioReader.Length / 4));
            float[] readBuffer = new float[audioReader.WaveFormat.SampleRate * audioReader.WaveFormat.Channels];
            int samplesRead;
            while ((samplesRead = ((ISampleProvider)audioReader).Read(readBuffer, 0, readBuffer.Length)) > 0)
            {
                wholeFile.AddRange(readBuffer.Take(samplesRead));
            }

            _audioData = [.. wholeFile];
        }

        public bool Repeat { get; set; }

        public WaveFormat WaveFormat { get; }

        public int Read(float[] buffer, int offset, int count)
        {
            long availableSamples = _audioData.LongLength - _position;
            int samplesToCopy = (int)Math.Min(availableSamples, count);
            Array.Copy(_audioData, _position, buffer, offset, samplesToCopy);
            _position += samplesToCopy;
            return samplesToCopy;
        }

        public void Reset() => _position = 0;
    }
}
