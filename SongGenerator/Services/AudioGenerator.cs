using System.Text;

namespace SongGenerator.Services;

public static class AudioGenerator
{
    private static readonly double[] Scale = { 261.63, 293.66, 329.63, 349.23, 392.00, 440.00, 493.88, 523.25 };

    public static byte[] GenerateSong(long seed, double durationSeconds = 5.0)
    {
        int internalSeed = (int)(seed ^ (seed >> 32));
        var rnd = new Random(internalSeed);

        int sampleRate = 44100;
        int numSamples = (int)(sampleRate * durationSeconds);
        int dataChunkSize = numSamples * 2;

        using (var stream = new MemoryStream())
        using (var writer = new BinaryWriter(stream))
        {
            writer.Write(Encoding.ASCII.GetBytes("RIFF"));
            writer.Write(36 + dataChunkSize);
            writer.Write(Encoding.ASCII.GetBytes("WAVE"));
            writer.Write(Encoding.ASCII.GetBytes("fmt "));
            writer.Write(16); writer.Write((short)1); writer.Write((short)1);
            writer.Write(sampleRate); writer.Write(sampleRate * 2);
            writer.Write((short)2); writer.Write((short)16);
            writer.Write(Encoding.ASCII.GetBytes("data"));
            writer.Write(dataChunkSize);

            int samplesPerBeat = sampleRate / 2;
            double currentFreq = Scale[rnd.Next(Scale.Length)];

            for (int i = 0; i < numSamples; i++)
            {
                if (i % samplesPerBeat == 0)
                    currentFreq = Scale[rnd.Next(Scale.Length)];

                double t = (double)i / sampleRate;

                double phase = (double)(i % samplesPerBeat) / samplesPerBeat;
                double envelope = Math.Sin(Math.PI * phase);

                short sample = (short)(10000 * Math.Sin(2 * Math.PI * currentFreq * t) * envelope);
                writer.Write(sample);
            }
            return stream.ToArray();
        }
    }
}
