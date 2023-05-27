/*
 * Copyright (C) Antony Vitillo (aka Skarredghost), Perpetual eMotion 2023.
 * Distributed under the MIT License (license terms are at http://opensource.org/licenses/MIT).
 */

using System;
using UnityEngine;
using vrroom.Dynaimic.Common;

namespace vrroom.CubicMusic.Audio
{
    /// <summary>
    /// Manages the audio in the application
    /// </summary>
    public class AudioManager : Singleton<AudioManager>
    {
        /// <summary>
        /// The microphone manager to be used by the application
        /// </summary>
        public MicrophoneManager MicrophoneManager { get; private set; }

        /// <summary>
        /// Analyzer of data of the background music. Can be null if there is no background music set.
        /// Use <see cref="SetBackgroundMusic(AudioSource)"/> to set the background music.
        /// </summary>
        public IAudioAnalyzer BackgroundMusicAnalyzer { get; private set; }

        /// <summary>
        /// Analyzer of data of the microphone
        /// </summary>
        public IAudioAnalyzer MicrophoneAnalyzer { get; private set; }

        /// <summary>
        /// Constructor with initialization
        /// </summary>
        public AudioManager()
        {
            MicrophoneManager = new MicrophoneManager();
            MicrophoneManager.StartRecording();
            MicrophoneManager.OnRecordingEnded += OnMicrophoneRecordingEnded;   
            MicrophoneAnalyzer = new AudioAnalyzer(new MicrophoneAudioDataSource(MicrophoneManager), 15);
        }

        /// <summary>
        /// Called when the microphone recording ends. It restarts the recording automatically
        /// to keep the microphone analysis going on always
        /// </summary>
        /// <param name="deviceName">Ignored</param>
        /// <param name="recordedAudioClip">Ignored</param>
        private void OnMicrophoneRecordingEnded(string deviceName, AudioClip recordedAudioClip)
        {
            MicrophoneManager.StartRecording();
        }

        /// <summary>
        /// Set the background music to be analyzed
        /// </summary>
        /// <param name="audioSource">Audiosource of the background music</param>
        public void SetBackgroundMusic(AudioSource audioSource) 
        {
            BackgroundMusicAnalyzer = new AudioAnalyzer(new AudioSourceDataSource(audioSource));
        }
    }

}