// 'Elite - The Sharp Kind' - Andy Hawkins 2023.
// 'Elite - The New Kind' - C.J.Pinder 1999-2001.
// Elite (C) I.Bell & D.Braben 1984.

using System.Diagnostics;
using MeltySynth;

namespace Useful.Audio;

public sealed class AudioController
{
    private readonly bool _effectsOn;
    private readonly bool _musicOn;
    private readonly IDictionary<int, SfxSample> _sfx;
    private readonly ISound _sound;

    public AudioController(ISound sound, IDictionary<int, SfxSample> sfx)
    {
        _sound = sound;
        _sfx = sfx;
#if DEBUG
        _musicOn = true;
        _effectsOn = true;
#else
        _musicOn = true;
        _effectsOn = true;
#endif
    }

    public void PlayEffect(int effectType)
    {
        if (!_effectsOn)
        {
            return;
        }

        if (_sfx[effectType].HasTimeRemaining)
        {
            return;
        }

        _sfx[effectType].ResetTime();
        _sound.Play(effectType);
    }

    public void PlayMusic(int musicType, bool loop)
    {
        if (!_musicOn)
        {
            return;
        }

        _sound.Play(musicType, loop);
    }

    public void StopMusic()
    {
        if (!_musicOn)
        {
            return;
        }

        _sound.StopMusic();
    }

    public void UpdateSound()
    {
        foreach (KeyValuePair<int, SfxSample> sfx in _sfx)
        {
            sfx.Value.ReduceTimeRemaining();
        }
    }

    internal static void GenerateWaveFromMidi(string midiFilename, string waveFilename)
    {
        const int sampleRate = 44100;
        Synthesizer synthesizer = new(Path.Combine("Assets", "Music", "TimGM6mb.sf2"), sampleRate);
        MidiFile midiFile = new(Path.Combine("Assets", "Music", midiFilename));
        MidiFileSequencer sequencer = new(synthesizer);
        sequencer.Play(midiFile, false);

        // The output buffer.
        float[] left = new float[(int)(sampleRate * midiFile.Length.TotalSeconds)];
        float[] right = new float[(int)(sampleRate * midiFile.Length.TotalSeconds)];

        Debug.Assert(left.Length == right.Length, "Left and right buffers must be the same length.");

        sequencer.Render(left, right);

        WriteStereoWav(Path.Combine("Assets", "Music", waveFilename), left, right, 44100);
    }

    internal static void WriteStereoWav(string filePath, float[] left, float[] right, int sampleRate)
    {
        int numSamples = Math.Min(left.Length, right.Length);
        const int bytesPerSample = 2; // 16-bit PCM
        const int numChannels = 2;
        int byteRate = sampleRate * numChannels * bytesPerSample;
        int dataSize = numSamples * numChannels * bytesPerSample;

        using FileStream fs = new(filePath, FileMode.Create, FileAccess.Write);
        using BinaryWriter bw = new(fs);

        // RIFF header
        bw.Write(System.Text.Encoding.ASCII.GetBytes("RIFF"));
        bw.Write(36 + dataSize);
        bw.Write(System.Text.Encoding.ASCII.GetBytes("WAVE"));

        // fmt subchunk
        bw.Write(System.Text.Encoding.ASCII.GetBytes("fmt "));
        bw.Write(16); // Subchunk1Size for PCM
        bw.Write((short)1); // AudioFormat PCM
        bw.Write((short)numChannels);
        bw.Write(sampleRate);
        bw.Write(byteRate);
        bw.Write((short)(numChannels * bytesPerSample)); // BlockAlign
        bw.Write((short)(bytesPerSample * 8)); // BitsPerSample

        // data subchunk
        bw.Write(System.Text.Encoding.ASCII.GetBytes("data"));
        bw.Write(dataSize);

        // Write samples
        for (int i = 0; i < numSamples; i++)
        {
            short l = (short)Math.Clamp(left[i] * 32767f, -32768, 32767);
            short r = (short)Math.Clamp(right[i] * 32767f, -32768, 32767);
            bw.Write(l);
            bw.Write(r);
        }

        bw.Close();
        fs.Close();
    }
}
