// UnityWebBrowser (UWB)
// Copyright (c) 2021-2022 Voltstro-Studios
// 
// This project is under the MIT license. See the LICENSE.md file for more details.

using VoltstroStudios.UnityWebBrowser.Shared;
using VoltstroStudios.UnityWebBrowser.Shared.Events;

namespace VoltstroStudios.UnityWebBrowser.Events
{
    /// <summary>
    ///     Invoked when the browser audio stream starts.
    /// </summary>
    public delegate void OnAudioStreamStarted(int sampleRate, int channels, AudioChannelLayout channelLayout, int framesPerBuffer);

    /// <summary>
    ///     Invoked when the browser audio stream stops.
    /// </summary>
    public delegate void OnAudioStreamStopped();

    /// <summary>
    ///     Invoked when an error occurs in the browser audio stream.
    /// </summary>
    public delegate void OnAudioStreamError(string message);

    /// <summary>
    ///     Invoked when PCM audio data is received from the browser.
    ///     The <see cref="AudioDataEvent"/> contains interleaved float32 PCM samples
    ///     and format metadata (sample rate, channels, channel layout, frame count).
    /// </summary>
    public delegate void OnAudioDataReceived(AudioDataEvent audioData);
}
