/*
 * Copyright (C) Antony Vitillo (aka Skarredghost), Perpetual eMotion 2023.
 * Distributed under the MIT License (license terms are at http://opensource.org/licenses/MIT).
 */

using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace vrroom.Dynaimic.Ai
{
    /// <summary>
    /// Base class for elements that can perform queries to AI cloud solutions (e.g. OpenAI APIs)
    /// </summary>
    public abstract class AiQueryPerformer
    {
        /// <summary>
        /// Event that is triggered when a textual prompt query is sent to the AI cloud solution.
        /// The parameter is the prompt that was sent
        /// </summary>
        public Action<string> OnPromptSent;

        /// <summary>
        /// Event that is triggered when a response to a prompt query is received from the AI cloud solution.
        /// The parameter is the response that was received 
        /// </summary>
        public Action<string> OnPromptResponseReceived;

        /// <summary>
        /// Event that is triggered when an audio transcription query is sent to the AI cloud solution.
        /// </summary>
        public Action OnAudioTranscriptionSent;

        /// <summary>
        /// Event that is triggered when a response to an audio transcription request is received from the AI cloud solution.
        /// The parameter is the response that was received
        /// </summary>
        public Action<string> OnAudioTranscriptionResponseReceived;

        /// <summary>
        /// Constructs the AI query performer with the given initialization data
        /// </summary>
        /// <param name="initData">Initialization data</param>
        public AiQueryPerformer(AiQueryPerformerInitializationData initData)
        {

        }

        /// <summary>
        /// Sends a textual prompt to the AI cloud solution and returns the completion response
        /// </summary>
        /// <param name="prompt">Textual prompt</param>
        /// <param name="model">The AI model to use</param>
        /// <param name="temperature">Temperature to provide to the GPT system, from 0 to 1</param>
        /// <param name="maxTokens">Maximum tokens of the answer</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Response that completes the prompt</returns>
        public abstract Task<string> GetCompletion(string prompt, AiCompletionModel model, float temperature, int maxTokens, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sends an audio prompt to the AI cloud solution and returns the transcription
        /// </summary>
        /// <param name="audio">Audio of interest</param>
        /// <param name="language">Language of the spoken audio in the clip</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Audio transcription</returns>
        public abstract Task<string> GetAudioTranscription(AudioClip audio, string language, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Base class for initialization data for all classes inheriting from <see cref="AiQueryPerformer"/>
    /// </summary>
    public class AiQueryPerformerInitializationData
    {
    }

    /// <summary>
    /// The type of AI completion model to use
    /// </summary>
    public enum AiCompletionModel
    {
        Accurate,
        Cheap
    }

}