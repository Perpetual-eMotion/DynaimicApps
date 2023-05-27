/*
 * Copyright (C) Antony Vitillo (aka Skarredghost), Perpetual eMotion 2023.
 * Distributed under the MIT License (license terms are at http://opensource.org/licenses/MIT).
 */

using System;
using UnityEngine;

namespace vrroom.CubicMusic.Audio
{
    /// <summary>
    /// Act as a centralized manager for a certain microphone of the application
    /// </summary>
    public class MicrophoneManager
    {
        /// <summary>
        /// The audio clip of the microphone. It may be still recording, or the
        /// clip from the previous recording
        /// </summary>
        public AudioClip MicAudioClip { get; private set; }

        /// <summary>
        /// The name of the device to use for recording.
        /// null to use the default microphone
        /// </summary>
        private string m_deviceName;

        /// <summary>
        /// Get the position in samples of the recording
        /// </summary>
        public int Position => Microphone.GetPosition(m_deviceName);

        /// <summary>
        /// True if the microphone is recording, false otherwise
        /// </summary>
        public bool IsRecording => Microphone.IsRecording(m_deviceName);

        /// <summary>
        /// Event called when the recording starts
        /// </summary>
        public Action<string, AudioClip> OnRecordingStarted;

        /// <summary>        
        /// Event called when the recording ends
        /// </summary>
        public Action<string, AudioClip> OnRecordingEnded;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="deviceName">The name of the device to use for recording. null to use the default microphone</param>
        public MicrophoneManager(string deviceName = null)
        {
            m_deviceName = deviceName;
        }

        /// <summary>
        /// Start recording from the microphone. If a recording is already in progress, it will be stopped
        /// </summary>
        /// <param name="loop">True to loop the detection after lengthSec is reached</param>
        /// <param name="lengthSec">How many seconds maximum long should be the recorded Audioclip</param>
        /// <param name="frequency">Frequency of the recording</param>
        /// <returns>AudioClip used by the recording microphone. Beware that this will be overwritten by the next recording</returns>
        public AudioClip StartRecording(bool loop = true, int lengthSec = 20, int frequency = 44100)
        {
            if (Microphone.devices.Length == 0)
            {
                Debug.LogWarning("No microphone detected. No recording will start");
                return null;
            }
            if (Microphone.IsRecording(m_deviceName))
            {
                Debug.LogWarning("Microphone is already recording. Stopping it...");
                Microphone.End(m_deviceName);
            }

            MicAudioClip = Microphone.Start(m_deviceName, loop, lengthSec, frequency);
            OnRecordingStarted?.Invoke(m_deviceName, MicAudioClip);

            return MicAudioClip;
        }

        /// <summary>
        /// Finishes recording from the microphone
        /// </summary>
        /// <returns>The recorded audioclip. It will be created by this method, and should be destroyed by the caller</returns>
        public AudioClip EndRecording()
        {
            if (Microphone.devices.Length == 0)
            {
                Debug.LogWarning("No microphone detected");
                return null;
            }
            if (!Microphone.IsRecording(null))
            {
                Debug.LogWarning("Microphone is not recording");
                return null;
            }

            //saves the recorded audio clip up to now
            var recordedAudioClip = CreateAudioClipWithRecordedData();

            //stop the recording
            Microphone.End(m_deviceName);
            OnRecordingEnded?.Invoke(m_deviceName, MicAudioClip);

            //returns the recorded audioclip
            return recordedAudioClip;
        }

        /// <summary>
        /// Create a new audioclip from the current recording
        /// </summary>
        /// <remarks>
        /// We need this method because the AudioClip used by the Microphone is always the same,
        /// so if we don't want the data to be overwritten, we need to create a new one with
        /// a deep copy of the recorded data
        /// </remarks>
        /// <returns>Audio clip with recorded data</returns>
        private AudioClip CreateAudioClipWithRecordedData()
        {
            //create a new audioclip that stores the data of the Microphone clip up to the current position
            //and return it
            float[] samples = new float[MicAudioClip.samples];
            MicAudioClip.GetData(samples, 0);
            var newAudioClip = AudioClip.Create(Guid.NewGuid().ToString(), this.Position, MicAudioClip.channels, MicAudioClip.frequency, false);
            newAudioClip.SetData(samples, 0);

            return newAudioClip;
        }
    }

}