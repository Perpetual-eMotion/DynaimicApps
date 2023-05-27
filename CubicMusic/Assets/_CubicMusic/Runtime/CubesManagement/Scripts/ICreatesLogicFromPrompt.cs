/*
 * Copyright (C) Antony Vitillo (aka Skarredghost), Perpetual eMotion 2023.
 * Distributed under the MIT License (license terms are at http://opensource.org/licenses/MIT).
 */

using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using vrroom.Dynaimic.GenerativeLogic;

namespace vrroom.CubicMusic.CubesMgmt
{

    /// <summary>
    /// Interface for objects that generate logic from an AI prompt
    /// </summary>
    public interface ICreatesLogicFromPrompt
    {
        /// <summary>
        /// Template used to give more context to every prompt to make the instructions clearer to the AI
        /// </summary>
        AiPromptTemplate PromptTemplate { get; }

        /// <summary>
        /// Generate logic for a group of objects from a text prompt
        /// </summary>
        /// <param name="prompt">The text prompt of the behaviour to implement</param>
        /// <param name="cancellationToken">Cancellation token</param>
        Task GenerateLogicForGroupFromText(string prompt, CancellationToken cancellationToken = default);

        /// <summary>
        /// Generate logic for a group of objects from a text prompt
        /// </summary>
        /// <param name="audioPrompt">The audio prompt of the behaviour to implement</param>
        /// <param name="cancellationToken">Cancellation token</param>
        Task GenerateLogicForGroupFromAudio(AudioClip audioPrompt, CancellationToken cancellationToken = default);
    }

}