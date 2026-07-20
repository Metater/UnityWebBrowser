// UnityWebBrowser (UWB)
// Copyright (c) 2021-2022 Voltstro-Studios
// 
// This project is under the MIT license. See the LICENSE.md file for more details.

using System;

namespace VoltstroStudios.UnityWebBrowser.Shared.Events;

/// <summary>
///     PCM audio data event transmitted from the engine to Unity.
///     Carries raw interleaved 32-bit float PCM samples and the format metadata
///     needed to interpret them.
/// </summary>
public struct AudioDataEvent
{
    /// <summary>
    ///     Sample rate in Hz (e.g. 44100, 48000).
    /// </summary>
    public int SampleRate;

    /// <summary>
    ///     Number of audio channels.
    /// </summary>
    public int Channels;

    /// <summary>
    ///     Channel layout describing the speaker arrangement.
    /// </summary>
    public AudioChannelLayout ChannelLayout;

    /// <summary>
    ///     Number of audio frames (samples per channel) in this packet.
    /// </summary>
    public int Frames;

    /// <summary>
    ///     Raw interleaved float32 PCM data as bytes.
    ///     Size in bytes = Frames * Channels * sizeof(float).
    /// </summary>
    public ReadOnlyMemory<byte> Data;

    /// <summary>
    ///     Whether this event contains valid data or represents an empty poll.
    /// </summary>
    public readonly bool HasData => Frames > 0 && Data.Length > 0;
}
