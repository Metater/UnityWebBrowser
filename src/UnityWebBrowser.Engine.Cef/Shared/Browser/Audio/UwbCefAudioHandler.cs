// UnityWebBrowser (UWB)
// Copyright (c) 2021-2022 Voltstro-Studios
// 
// This project is under the MIT license. See the LICENSE.md file for more details.

using System;
using System.Runtime.CompilerServices;
using VoltstroStudios.UnityWebBrowser.Engine.Shared.Core;
using VoltstroStudios.UnityWebBrowser.Shared;
using Xilium.CefGlue;

namespace UnityWebBrowser.Engine.Cef.Shared.Browser.Audio;

/// <summary>
///     Implements <see cref="CefAudioHandler"/> to capture browser PCM audio,
///     interleave planar float data, and push it into a ring buffer for Unity polling.
/// </summary>
internal class UwbCefAudioHandler : CefAudioHandler
{
    private readonly ClientControlsActions clientControls;
    private readonly AudioRingBuffer ringBuffer;

    private int currentSampleRate;
    private int currentChannels;
    private AudioChannelLayout currentChannelLayout;
    private bool streamActive;

    public UwbCefAudioHandler(ClientControlsActions clientControls, AudioRingBuffer ringBuffer)
    {
        this.clientControls = clientControls ?? throw new ArgumentNullException(nameof(clientControls));
        this.ringBuffer = ringBuffer ?? throw new ArgumentNullException(nameof(ringBuffer));
    }

    public bool IsStreamActive => streamActive;

    public AudioRingBuffer RingBuffer => ringBuffer;

    public int CurrentSampleRate => currentSampleRate;

    public int CurrentChannels => currentChannels;

    public AudioChannelLayout CurrentChannelLayout => currentChannelLayout;

    protected override bool GetAudioParameters(CefBrowser browser, CefAudioParameters parameters)
    {
        return true;
    }

    protected override void OnAudioStreamStarted(CefBrowser browser, in CefAudioParameters parameters, int channels)
    {
        streamActive = true;
        currentSampleRate = parameters.SampleRate;
        currentChannels = channels;
        currentChannelLayout = (AudioChannelLayout)(int)parameters.ChannelLayout;

        clientControls.AudioStreamStarted(
            currentSampleRate,
            currentChannels,
            currentChannelLayout,
            parameters.FramesPerBuffer);
    }

    protected override unsafe void OnAudioStreamPacket(CefBrowser browser, IntPtr data, int frames, long pts)
    {
        if (!streamActive || frames <= 0 || data == IntPtr.Zero)
            return;

        // data is float** — array of pointers to planar float arrays (one per channel)
        float** channelPointers = (float**)data;

        int totalSamples = frames * currentChannels;
        float[] interleaved = new float[totalSamples];

        // Interleave planar→interleaved: plane 0[0], plane 1[0], plane 0[1], plane 1[1]...
        for (int f = 0; f < frames; f++)
        {
            for (int c = 0; c < currentChannels; c++)
            {
                interleaved[f * currentChannels + c] = channelPointers[c][f];
            }
        }

        ringBuffer.Write(interleaved, currentSampleRate, currentChannels, currentChannelLayout, frames);
    }

    protected override void OnAudioStreamStopped(CefBrowser browser)
    {
        streamActive = false;
        ringBuffer.Clear();
        clientControls.AudioStreamStopped();
    }

    protected override void OnAudioStreamError(CefBrowser browser, string message)
    {
        streamActive = false;
        ringBuffer.Clear();
        clientControls.AudioStreamError(message ?? string.Empty);
    }
}
