/*
 * Copyright (C) Antony Vitillo (aka Skarredghost), Perpetual eMotion 2023.
 * Distributed under the MIT License (license terms are at http://opensource.org/licenses/MIT).
 */

using UnityEngine;

namespace vrroom.CubicMusic.Audio
{
    /// <summary>
    /// Sets the current audio source as background music
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class BackgroundMusic : MonoBehaviour
    {
        /// <summary>
        /// Start
        /// </summary>
        void Start()
        {
            AudioManager.Instance.SetBackgroundMusic(GetComponent<AudioSource>());
        }

    }

}