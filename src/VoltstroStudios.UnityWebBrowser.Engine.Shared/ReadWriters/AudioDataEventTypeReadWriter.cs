// UnityWebBrowser (UWB)
// Copyright (c) 2021-2022 Voltstro-Studios
// 
// This project is under the MIT license. See the LICENSE.md file for more details.

using System;
using VoltRpc.IO;
using VoltRpc.Types;
using VoltstroStudios.UnityWebBrowser.Shared.Events;

namespace VoltstroStudios.UnityWebBrowser.Engine.Shared.ReadWriters;

/// <summary>
///     VoltRpc type reader/writer for <see cref="AudioDataEvent"/>.
///     Engine only writes; read is not implemented.
/// </summary>
public class AudioDataEventTypeReadWriter : TypeReadWriter<AudioDataEvent>
{
    public override void Write(BufferedWriter writer, AudioDataEvent audioEvent)
    {
        if (!audioEvent.HasData)
        {
            writer.WriteInt(-1);
            return;
        }

        writer.WriteInt(audioEvent.SampleRate);
        writer.WriteInt(audioEvent.Channels);
        writer.WriteInt((int)audioEvent.ChannelLayout);
        writer.WriteInt(audioEvent.Frames);
        writer.WriteInt(audioEvent.Data.Length);

        foreach (byte b in audioEvent.Data.Span)
            writer.WriteByte(b);
    }

    public override AudioDataEvent Read(BufferedReader reader)
    {
        throw new NotImplementedException();
    }
}
