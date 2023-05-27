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
    /// Interface for elements that analyze some audio source
    /// </summary>
    public abstract class IAudioAnalyzer
    {
        /// <summary>
        /// The sensitivity of the volume detection. 
        /// The higher this value, the higher the <see cref="CurrentVolume"/>
        /// </summary>
        public abstract float VolumeSensitivity { get; set; }

        /// <summary>
        /// The current volume of the audio, in the range [0, 1]
        /// </summary>
        public abstract float CurrentVolume { get; }
    }

    /// <summary>
    /// Analyzes the audio output of an audio source that is playing
    /// </summary>
    public class AudioAnalyzer : IAudioAnalyzer
    {
        /// <summary>
        /// The element providing the audio data (e.g. the microphone)
        /// </summary>
        private IAudioDataSource m_audioDataSource;

        /// <summary>
        /// Array that contains the values we read from the audio source
        /// </summary>
        private float[] m_audioReadValue;

        /// <summary>
        /// Number of samples we read from the audio source
        /// </summary>
        private int m_samplesCount;

        /// <summary>
        /// Alpha value for the running average, used to provide smoothing of the volume.
        /// Every frame the volume is computed as alpha * currentVolume + (1 - alpha) * newVolume
        /// </summary>
        private float m_runningAvgAlpha;

        /// <summary>
        /// The sensitivity of the volume detection
        /// </summary>
        private float m_volumeSensitivity;

        /// <summary>
        /// Current volume of the audio
        /// </summary>
        private float m_currentVolumeValue = 0;        

        /// <inheritdoc/>
        public override float VolumeSensitivity { get => m_volumeSensitivity; set => m_volumeSensitivity = value; }

        /// <inheritdoc/>
        public override float CurrentVolume
        {
            get
            {
                ComputeVolume();
                return Mathf.Clamp01(m_currentVolumeValue * VolumeSensitivity);
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="volumeSensitivity">Sensitivity of the detection. Higher values means there will be higher values in return for the same audio level</param>
        /// <param name="samplesCount">Number of samples to use to compute the volume</param>
        /// <param name="runningAvgAlpha">Alpha constant for running average, used for smoothing. Higher values produce more smoothed results</param>
        public AudioAnalyzer(IAudioDataSource audioDataSource, float volumeSensitivity = 10, int samplesCount = 128, float runningAvgAlpha = 0.25f)
        {
            m_audioDataSource = audioDataSource;
            m_samplesCount = samplesCount;
            m_runningAvgAlpha = runningAvgAlpha;
            m_audioReadValue = new float[samplesCount];
            m_volumeSensitivity = volumeSensitivity;
        }

        /// <summary>
        /// Computes the volume of the audio source in this moment
        /// </summary>
        private void ComputeVolume()
        {
            if (m_audioDataSource == null || !m_audioDataSource.IsPlaying)
                return;

            //read audio source data and compute the sum of the absolute values
            float sum = 0;

            for (int c = 0; c < m_audioDataSource.AudioChannels; c++)
            {
                m_audioDataSource.GetAudioData(m_audioReadValue, c);

                for (int i = 0; i < m_audioReadValue.Length; i++)
                    sum += Mathf.Abs(m_audioReadValue[i]);
            }

            //compute the running average: alpha * currentVolume + (1 - alpha) * newVolume
            m_currentVolumeValue = m_currentVolumeValue * m_runningAvgAlpha + (sum / (m_samplesCount * m_audioDataSource.AudioChannels)) * (1 - m_runningAvgAlpha);
        }

    }

}