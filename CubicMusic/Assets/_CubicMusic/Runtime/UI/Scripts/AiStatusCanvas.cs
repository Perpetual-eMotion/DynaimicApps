/*
 * Copyright (C) Antony Vitillo (aka Skarredghost), Perpetual eMotion 2023.
 * Distributed under the MIT License (license terms are at http://opensource.org/licenses/MIT).
 */

using System;
using System.Collections;
using TMPro;
using UnityEngine;
using vrroom.CubicMusic.CubesMgmt;

namespace vrroom.CubicMusic.UI
{
    /// <summary>
    /// UI that displays the status of the AI operations
    /// </summary>
    public class AiStatusCanvas : MonoBehaviour
    {
        /// <summary>
        /// Text that shows the status of the AI operations
        /// </summary>
        [SerializeField]
        private TMP_Text m_statusText;

        /// <summary>
        /// Text field that displays the latest computed audio transcript
        /// </summary>
        [SerializeField]
        private TMP_Text m_audioTranscriptText;

        /// <summary>
        /// Text field that displays the latest prompt response
        /// </summary>
        [SerializeField]
        private TMP_Text m_promptResponseText;

        /// <summary>
        /// The coroutine that shows the progress of the AI operations
        /// </summary>
        private Coroutine m_progressCoroutine = default;

        /// <summary>
        /// On Enable
        /// </summary>
        private void OnEnable()
        {
            // Register to the events
            CubesManager.Instance.AiQueryPerformer.OnPromptSent += OnPromptSent;
            CubesManager.Instance.AiQueryPerformer.OnPromptResponseReceived += OnPromptResponseReceived;

            CubesManager.Instance.AiQueryPerformer.OnAudioTranscriptionSent += OnAudioTranscriptionSent;
            CubesManager.Instance.AiQueryPerformer.OnAudioTranscriptionResponseReceived += OnAudioTranscriptionResponseReceived;
        }

        /// <summary>
        /// On Disable
        /// </summary>
        private void OnDisable()
        {
            // Unregister from the events
            CubesManager.Instance.AiQueryPerformer.OnPromptSent -= OnPromptSent;
            CubesManager.Instance.AiQueryPerformer.OnPromptResponseReceived -= OnPromptResponseReceived;

            CubesManager.Instance.AiQueryPerformer.OnAudioTranscriptionSent -= OnAudioTranscriptionSent;
            CubesManager.Instance.AiQueryPerformer.OnAudioTranscriptionResponseReceived -= OnAudioTranscriptionResponseReceived;
        }

        /// <summary>
        /// Callback called when the prompt is sent to the AI cloud
        /// </summary>
        private void OnAudioTranscriptionSent()
        {
            m_statusText.text = "Audio sent. Transcribing";
            m_progressCoroutine = StartCoroutine(ShowProgress());
        }

        /// <summary>
        /// Callback called when the audio transcription is received from the AI cloud
        /// </summary>
        /// <param name="transcription">Audio transcription</param>
        private void OnAudioTranscriptionResponseReceived(string transcription)
        {
            StopCoroutine(m_progressCoroutine);
            m_statusText.text = "Audio transcription received!";
            m_audioTranscriptText.text = transcription;
        }

        /// <summary>
        /// Callback called when a textual prompt is sent to the AI cloud
        /// </summary>
        /// <param name="prompt">Textual prompt sent</param>
        private void OnPromptSent(string prompt)
        {
            m_statusText.text = "Prompt sent. Thinking";
            m_progressCoroutine = StartCoroutine(ShowProgress());
        }

        /// <summary>
        /// Callback called when a textual prompt got a response from the AI cloud
        /// </summary>
        /// <param name="response">Textual response from cloud</param>
        private void OnPromptResponseReceived(string response)
        {
            StopCoroutine(m_progressCoroutine);
            m_statusText.text = "Prompt response received!";
            m_promptResponseText.text = response;
        }

        /// <summary>
        /// Shows progress of the AI operations while we wait for the response
        /// </summary>
        /// <returns></returns>
        private IEnumerator ShowProgress()
        { 
            //keep adding a dot every 0.5 seconds
            while(true)
            {
                m_statusText.text += ".";
                yield return new WaitForSeconds(0.5f);
            }
        }
    }

}