/*
 * Copyright (C) Antony Vitillo (aka Skarredghost), Perpetual eMotion 2023.
 * Distributed under the MIT License (license terms are at http://opensource.org/licenses/MIT).
 */

using OpenAI;
using OpenAI.Audio;
using OpenAI.Models;
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace vrroom.Dynaimic.Ai
{
    /// <summary>
    /// Performs queries to the OpenAI API
    /// </summary>
    public class OpenAiQueryPerformer : AiQueryPerformer
    {
        /// <summary>
        /// Client that performs the queries to OpenAI 
        /// </summary>
        private OpenAIClient m_openAiClient;

        /// <summary>
        /// Constructs the AI query performer with the given initialization data
        /// </summary>
        public OpenAiQueryPerformer() :
            base(new AiQueryPerformerInitializationData())
        {
            m_openAiClient = new OpenAIClient();
        }

        /// <summary>
        /// Constructs the AI query performer with the given initialization data
        /// </summary>
        /// <param name="initData">Initialization data</param>
        public OpenAiQueryPerformer(AiQueryPerformerInitializationData initData) :
            base(initData)
        {
            m_openAiClient = new OpenAIClient();
        }

        /// <inheritdoc />
        public override async Task<string> GetCompletion(string prompt, AiCompletionModel model, float temperature, int maxTokens, CancellationToken cancellationToken = default)
        {
            OnPromptSent?.Invoke(prompt);
            var result = await m_openAiClient.CompletionsEndpoint.CreateCompletionAsync(prompt, model: GetOpenAiModel(model), temperature: temperature, maxTokens: maxTokens,
                cancellationToken: cancellationToken);
            OnPromptResponseReceived?.Invoke(result);

            return result;
        }

        /// <inheritdoc />
        public override async Task<string> GetAudioTranscription(AudioClip audio, string language, CancellationToken cancellationToken = default)
        {
            OnAudioTranscriptionSent?.Invoke();
            var request = new AudioTranscriptionRequest(audio, language: language);
            var result = await m_openAiClient.AudioEndpoint.CreateTranscriptionAsync(request, cancellationToken);
            OnAudioTranscriptionResponseReceived?.Invoke(result);

            return result;
        }

        /// <summary>
        /// Gets the OpenAI model that corresponds to the given AI model
        /// </summary>
        /// <param name="aiModel">AI model of interest</param>
        /// <returns>OpenAI model</returns>
        /// <exception cref="ArgumentOutOfRangeException">In case an unknown model is provided as parameter</exception>
        private static Model GetOpenAiModel(AiCompletionModel aiModel)
        {
            switch(aiModel)
            {
                case AiCompletionModel.Accurate:
                    return Model.Davinci;
                case AiCompletionModel.Cheap:
                    return Model.GPT3_5_Turbo;
                default:
                    throw new ArgumentOutOfRangeException(nameof(aiModel), aiModel, null);
            }
        }
    }

}