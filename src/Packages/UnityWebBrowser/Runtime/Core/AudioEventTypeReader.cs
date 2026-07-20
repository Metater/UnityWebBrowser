using System;
using VoltRpc.IO;
using VoltRpc.Types;
using VoltstroStudios.UnityWebBrowser.Shared;
using VoltstroStudios.UnityWebBrowser.Shared.Events;

namespace VoltstroStudios.UnityWebBrowser.Core
{
    /// <summary>
    ///     VoltRpc type reader for <see cref="AudioDataEvent"/> on the Unity side.
    ///     Reads PCM audio packets from the engine and stores metadata
    ///     for consumption on the Unity main thread.
    /// </summary>
    internal class AudioEventTypeReader : TypeReadWriter<AudioDataEvent>
    {
        public override void Write(BufferedWriter writer, AudioDataEvent value)
        {
            throw new NotImplementedException();
        }

        public override AudioDataEvent Read(BufferedReader reader)
        {
            int sampleRate = reader.ReadInt();
            if (sampleRate <= 0)
                return default;

            int channels = reader.ReadInt();
            AudioChannelLayout channelLayout = (AudioChannelLayout)reader.ReadInt();
            int frames = reader.ReadInt();
            int dataLength = reader.ReadInt();

            if (dataLength <= 0)
                return default;

            byte[] data = new byte[dataLength];
            ReadOnlySpan<byte> span = reader.ReadBytesSpanSlice(dataLength);
            span.CopyTo(data);

            return new AudioDataEvent
            {
                SampleRate = sampleRate,
                Channels = channels,
                ChannelLayout = channelLayout,
                Frames = frames,
                Data = data
            };
        }
    }
}
