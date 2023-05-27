/*
 * Copyright (C) Antony Vitillo (aka Skarredghost), Perpetual eMotion 2023.
 * Distributed under the MIT License (license terms are at http://opensource.org/licenses/MIT).
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using vrroom.CubicMusic.Audio;
using vrroom.CubicMusic.CubesMgmt;

namespace vrroom.CubicMusic.UI
{
    /// <summary>
    /// Managest the UI for the user to create an AI query and receive the responses
    /// </summary>
    public class UserQueryCreationCanvas : MonoBehaviour
    {
        /// <summary>
        /// Toggle to enable/disable the recording of the microphone
        /// </summary>
        [SerializeField]
        private Toggle m_recordingToggle;

        /// <summary>
        /// Button to send the prompt to the AI cloud
        /// </summary>
        [SerializeField]
        private Button m_sendPromptButton;

        /// <summary>
        /// Input field with the textual query
        /// </summary>
        [SerializeField]
        private TMP_InputField m_textQueryInputField;

        /// <summary>
        /// The object responsible to generate the logic from the prompts.
        /// Must implement <see cref="ICreatesLogicFromPrompt"/>
        /// If it is null, defaults to <see cref="CubesManager"/>
        /// </summary>
        [SerializeField]
        private MonoBehaviour m_logicFromQueriesGeneratorBehaviour;

        /// <summary>
        /// Element to be notified of the queries so that can generate logic
        /// </summary>
        private ICreatesLogicFromPrompt m_logicFromPromptCreator;

        /// <summary>
        /// Cancellation token
        /// </summary>
        private CancellationTokenSource m_cancellationTokenSource;
        
        /// <summary>
        /// Awake
        /// </summary>
        private void Awake()
        {
            m_cancellationTokenSource = new CancellationTokenSource();

            if(m_logicFromQueriesGeneratorBehaviour == null)
                m_logicFromPromptCreator = CubesManager.Instance;
            else
                m_logicFromPromptCreator = m_logicFromQueriesGeneratorBehaviour as ICreatesLogicFromPrompt;
        }

        /// <summary>
        /// On Enable
        /// </summary>
        private void OnEnable()
        {
            m_recordingToggle.onValueChanged.AddListener(OnRecordingToggleValueChanged);
            m_sendPromptButton.onClick.AddListener(OnSendPromptButtonClicked);
        }

        /// <summary>
        /// On Application Quit
        /// </summary>
        private void OnApplicationQuit()
        {
            //cancel all pending tasks
            m_cancellationTokenSource.Cancel(); 
        }

        /// <summary>
        /// On Disable
        /// </summary>
        private void OnDisable()
        {
            m_recordingToggle.onValueChanged.RemoveListener(OnRecordingToggleValueChanged);
            m_sendPromptButton.onClick.RemoveListener(OnSendPromptButtonClicked);
        }

        /// <summary>
        /// Callback called when the recording toggle value changes
        /// </summary>
        /// <param name="value">The new value of the toggle</param>
        private async void OnRecordingToggleValueChanged(bool value)
        {
            //if the toggle is on, start recording
            if (value)
                AudioManager.Instance.MicrophoneManager.StartRecording(false, 30);
            //if the toggle is off, stop recording and generate the logic
            else
            {
                var userAudioClip = AudioManager.Instance.MicrophoneManager.EndRecording();
                await m_logicFromPromptCreator.GenerateLogicForGroupFromAudio(userAudioClip);
            }
        }

        /// <summary>
        /// Callback called when the send prompt button is clicked
        /// </summary>
        private async void OnSendPromptButtonClicked()
        {
            await m_logicFromPromptCreator.GenerateLogicForGroupFromText(m_textQueryInputField.text);
        }

#if UNITY_EDITOR
        /// <summary>
        /// On Validate
        /// </summary>
        private void OnValidate()
        {
            //check that the assignment of the logic from queries generator is correct
            if (m_logicFromQueriesGeneratorBehaviour != null && m_logicFromQueriesGeneratorBehaviour.GetComponent<ICreatesLogicFromPrompt>() == null)
            {
                Debug.LogError("[User Queries UI] The logic from queries generator must implement ICreatesLogicFromPrompt");
                m_logicFromQueriesGeneratorBehaviour = null;
            }
        }
#endif
    }

}