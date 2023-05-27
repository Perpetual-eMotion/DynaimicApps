/*
 * Copyright (C) Antony Vitillo (aka Skarredghost), Perpetual eMotion 2023.
 * Distributed under the MIT License (license terms are at http://opensource.org/licenses/MIT).
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace vrroom.CubicMusic.Audio
{
    /// <summary>
    /// Interface for elements that provide audio data
    /// </summary>
    public interface IAudioDataSource
    {
        /// <summary>
        /// True if the audio source is playing (so data is available), false otherwise
        /// </summary>
        abstract bool IsPlaying { get; }

        /// <summary>
        /// The number of channels of the audio source
        /// </summary>
        abstract int AudioChannels { get; }

        /// <summary>
        /// Gets the audio data from a specific channel of the audio source
        /// </summary>
        /// <param name="data">Array of data that will be filled by the function</param>
        /// <param name="channel">Channel of interest</param>
        abstract void GetAudioData(float[] data, int channel);
    }

    /// <summary>
    /// Audio data source that uses an <see cref="AudioSource"/> as data source
    /// </summary>
    public class AudioSourceDataSource : IAudioDataSource
    {
        /// <summary>
        /// Audio Source of interest
        /// </summary>
        private AudioSource m_audioSource;

        /// <inheritdoc/>
        public bool IsPlaying => m_audioSource != null && m_audioSource.isPlaying;

        /// <inheritdoc/>
        public int AudioChannels => (m_audioSource != null && m_audioSource.clip != null) ? m_audioSource.clip.channels : 0;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="audioSource">The audio source to use as audio source :)</param>
        public AudioSourceDataSource(AudioSource audioSource) 
        { 
            m_audioSource = audioSource;
        }

        /// <inheritdoc/>
        public void GetAudioData(float[] data, int channel)
        {
            m_audioSource.GetOutputData(data, channel);
        }
    }

    /// <summary>
    /// Audio data source that uses a <see cref="MicrophoneManager"/> as data source
    /// </summary>
    public class MicrophoneAudioDataSource : IAudioDataSource
    {
        /// <summary>
        /// The manager of the microphone to use
        /// </summary>
        private MicrophoneManager m_microphoneManager;

        /// <inheritdoc/>
        public bool IsPlaying => m_microphoneManager.IsRecording;

        /// <inheritdoc/>
        public int AudioChannels => 1;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="deviceName">Name of the microphone device to use</param>
        public MicrophoneAudioDataSource(MicrophoneManager microphoneManager)
        {
            m_microphoneManager = microphoneManager;
        }

        /// <inheritdoc/>
        public void GetAudioData(float[] data, int channel)
        {
            int micPosition = m_microphoneManager.Position - (data.Length + 1);

            if (micPosition < 0)
                return;

            m_microphoneManager.MicAudioClip.GetData(data, micPosition);
        }
    }

}