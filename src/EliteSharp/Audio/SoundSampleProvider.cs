// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Diagnostics;
using MeltySynth;
using NAudio.Vorbis;
using NAudio.Wave;

namespace EliteSharp.Audio;

public class SoundSampleProvider : ISampleProvider
{
    private float[] _audioData = [];
    private long _position;

    public required bool Repeat { get; set; }

    public required WaveFormat WaveFormat { get; init; }

    public static SoundSampleProvider Create(string fileName)
    {
        using WaveStream audioReader = Path.GetExtension(fileName) switch
        {
            ".mid" => GetMidiStream(fileName),
            ".ogg" => new VorbisWaveReader(fileName),
            _ => new AudioFileReader(fileName),
        };

        return new()
        {
            Repeat = false,
            WaveFormat = audioReader.WaveFormat,
            _audioData = ReadAudioData(audioReader),
        };
    }

    public int Read(float[] buffer, int offset, int count)
    {
        long availableSamples = _audioData.LongLength - _position;
        int samplesToCopy = (int)Math.Min(availableSamples, count);
        Array.Copy(_audioData, _position, buffer, offset, samplesToCopy);
        _position += samplesToCopy;
        return samplesToCopy;
    }

    public void Reset() => _position = 0;

    private static float[] ReadAudioData(WaveStream audioReader)
    {
        Debug.Assert(audioReader.WaveFormat.Channels == 2, "Audio should have correct number of channels.");
        Debug.Assert(audioReader.WaveFormat.SampleRate == 44100, "Audio should have correct sample rate.");
        Debug.Assert(audioReader.WaveFormat.BitsPerSample == 32, "Audio should have correct bits per sample.");

        List<float> audioData = new((int)(audioReader.Length / 4));
        float[] readBuffer = new float[audioReader.WaveFormat.SampleRate * audioReader.WaveFormat.Channels];
        int samplesRead;
        ISampleProvider sampleProvider = audioReader.ToSampleProvider();

        while ((samplesRead = sampleProvider.Read(readBuffer, 0, readBuffer.Length)) > 0)
        {
            audioData.AddRange(readBuffer.AsSpan(0, samplesRead).ToArray());
        }

        return [.. audioData];
    }

    private static WaveFileReader GetMidiStream(string filename)
    {
        try
        {
            const int sampleRate = 44100;
            Synthesizer synthesizer = new(
                "C:/code/github/aphawkins/elite-the-sharp-kind/src/EliteSharp.SDL/bin/Debug/net9.0/Assets/Music/TimGM6mb.sf2",
                sampleRate);
            MidiFile midiFile = new(filename);
            MidiFileSequencer sequencer = new(synthesizer);
            sequencer.Play(midiFile, false);

            // The output buffer.
            float[] leftChannel = new float[(int)(sampleRate * midiFile.Length.TotalSeconds)];
            float[] rightChannel = new float[(int)(sampleRate * midiFile.Length.TotalSeconds)];

            Debug.Assert(leftChannel.Length == rightChannel.Length, "Left and right buffers must be the same length.");

            sequencer.Render(leftChannel, rightChannel);

            int sampleCount = Math.Min(leftChannel.Length, rightChannel.Length);
            MemoryStream memoryStream = new();

            WaveFormat waveFormat = WaveFormat.CreateIeeeFloatWaveFormat(sampleRate, 2); // stereo

            using WaveFileWriter writer = new(memoryStream, waveFormat);

            for (int i = 0; i < sampleCount; i++)
            {
                writer.WriteSample(leftChannel[i]);
                writer.WriteSample(rightChannel[i]);
            }

            writer.Flush();
            memoryStream.Flush();
            memoryStream.Position = 0;

            return new WaveFileReader(new MemoryStream(memoryStream.ToArray()));
        }
        catch (Exception ex)
        {
            throw new EliteException("Ooops", ex);
        }
    }
}
