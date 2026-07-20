// UnityWebBrowser (UWB)
// Copyright (c) 2021-2022 Voltstro-Studios
// 
// This project is under the MIT license. See the LICENSE.md file for more details.

using System;
using System.Runtime.InteropServices;
using System.Threading;
using VoltstroStudios.UnityWebBrowser.Shared;
using VoltstroStudios.UnityWebBrowser.Shared.Events;

namespace UnityWebBrowser.Engine.Cef.Shared.Browser.Audio;

/// <summary>
///     Thread-safe ring buffer for PCM audio packets between CEF's audio callback thread
///     and the Unity polling thread.
/// </summary>
internal class AudioRingBuffer
{
    private readonly AudioDataEvent[] buffer;
    private readonly byte[][] dataBuffers;
    private readonly int capacity;
    private int head;
    private int tail;
    private readonly object lockObj = new();
    private int droppedPackets;

    public AudioRingBuffer(int capacity = 16)
    {
        this.capacity = capacity;
        buffer = new AudioDataEvent[capacity];
        dataBuffers = new byte[capacity][];
    }

    public int DroppedPackets => Volatile.Read(ref droppedPackets);

    /// <summary>
    ///     Writes an interleaved PCM packet into the ring buffer.
    ///     Called from CEF's audio stream thread.
    /// </summary>
    public void Write(ReadOnlySpan<float> interleavedData, int sampleRate, int channels,
        AudioChannelLayout channelLayout, int frames)
    {
        int byteCount = interleavedData.Length * sizeof(float);
        byte[] dataCopy = new byte[byteCount];
        System.Runtime.InteropServices.MemoryMarshal.AsBytes(interleavedData).CopyTo(dataCopy);

        lock (lockObj)
        {
            int next = (head + 1) % capacity;
            if (next == tail)
            {
                droppedPackets++;
                tail = (tail + 1) % capacity;
            }

            dataBuffers[head] = dataCopy;
            buffer[head] = new AudioDataEvent
            {
                SampleRate = sampleRate,
                Channels = channels,
                ChannelLayout = channelLayout,
                Frames = frames,
                Data = dataCopy
            };
            head = next;
        }
    }

    /// <summary>
    ///     Tries to read the oldest packet from the ring buffer.
    ///     Called from the Unity polling thread.
    /// </summary>
    public bool TryRead(out AudioDataEvent audioData)
    {
        lock (lockObj)
        {
            if (head == tail)
            {
                audioData = default;
                return false;
            }

            audioData = buffer[tail];
            buffer[tail] = default;
            dataBuffers[tail] = null;
            tail = (tail + 1) % capacity;
            return true;
        }
    }

    /// <summary>
    ///     Clears the ring buffer, resetting it to empty.
    /// </summary>
    public void Clear()
    {
        lock (lockObj)
        {
            for (int i = 0; i < capacity; i++)
            {
                buffer[i] = default;
                dataBuffers[i] = null;
            }

            head = 0;
            tail = 0;
        }
    }
}
