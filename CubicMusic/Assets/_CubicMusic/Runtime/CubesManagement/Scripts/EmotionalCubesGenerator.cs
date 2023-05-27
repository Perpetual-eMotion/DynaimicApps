/*
 * Copyright (C) Antony Vitillo (aka Skarredghost), Perpetual eMotion 2023.
 * Distributed under the MIT License (license terms are at http://opensource.org/licenses/MIT).
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using vrroom.CubicMusic.Audio;
using vrroom.Dynaimic.Ai;
using vrroom.Dynaimic.GenerativeLogic;

namespace vrroom.CubicMusic.CubesMgmt
{
    /// <summary>
    /// Makes the AI generate cubes and their logic at runtime depending on the emotions of the user
    /// </summary>
    public class EmotionalCubesGenerator : MonoBehaviour, ICreatesLogicFromPrompt
    {
        /// <summary>
        /// The prompt template to generate Unity scripts that can be added to the cubes at runtime without requiring
        /// the setup of public properties. Scripts should work out of the bo
        /// </summary>
        static readonly AiPromptTemplate s_promptTemplateForUnityScripts = new AiPromptTemplate()
        {
            PrePrompt = @"
Create the initial setup for a mood representation system in Unity. The system consists of seven game objects, each representing the same mood but with variations in their behaviors. In the first step, I would like to obtain the coordinates for each game object, defining their initial positions in the scene. Please provide the coordinates in the format (x1, y1, z1), (x2, y2, z2), ..., (x7, y7, z7). The coordinates should be randomly generated within a specified range (x and z in [-4.5, 4.6] and y in [0.5, 3.6]) to ensure diversity among the objects.

The desired mood can be described as a spectrum ranging from happiness to sadness. Happiness is associated with bright colors, growing scale (limited to a minimum of 0.25 and a maximum value of 2), and graceful rotation. Sadness is characterized by muted colors, shrinking scale (with the same range described before), and melancholic rotation.

In the second step, for each game object, I would like prompts that define their behavior. The prompts should specify a subset of rotation, color transition, scale, and volume transition, where volume transition is dependent on other behaviors. The prompts should align with the desired mood and incorporate the variations mentioned above.

For each game object's behavior prompt, please include a combination of 1 to 3 behaviors from the following options:

    Rotation: [Specify the rotation behavior, e.g., slow and clockwise rotation]
    Color Transition: [Specify the color transition behavior, e.g., transition between warm and vibrant colors]
    Scale: [Specify the scale behavior, e.g., gradual growth with a maximum scale of 3]

Additionally, please specify how volume should be incorporated as a modality of the other behaviors. Volume is never assigned and never does a transition itself, but behaviours can be dependent on the current volume of the microphone or the volume of music. For example, you can specify that the color transition is dependent on the volume of the music, where low volume corresponds to green and high volume corresponds to red. Volume transition is optional and should not be part of all of the game object behaviours, but should be present in one of the game objects.

For colors, if they are needed, choose between the values: red, blue, black, grey, green, yellow, white, magenta, cyan

Please provide the behavior prompts for each game object, ensuring that they align with the desired mood and incorporate the variations mentioned above. Make sure to describe the behavior in a format that, when fed into OpenAI, will result in the desired outcome.

Remember to consider different combinations and variations for each game object to achieve a diverse and representative mood representation system.
Separate the various game objects descriptions with new line characters. Before the description, write the ""@"" symbol and then its set of coordinates in the format x,y,z with parenthesis;

The mood to consider is """,
            PostPrompt = @""" .Please generate the prompts only that are coherent with this mood. Write No examples, no explanations."
        };

        /// <summary>
        /// The element that performs the queries to the AI cloud
        /// </summary>
        private AiQueryPerformer m_aiQueryPerformer;

        /// <summary>
        /// Ai completion parameters
        /// </summary>
        private AiGenerationParameters m_aiParameters;

        /// <inheritdoc />
        public AiPromptTemplate PromptTemplate => s_promptTemplateForUnityScripts;

        /// <summary>
        /// Start
        /// </summary>
        private void Start()
        {
            m_aiQueryPerformer = CubesManager.Instance.AiQueryPerformer; //we use the same of the cubes manager, so also the status canvas can register to the events of only one
            m_aiParameters = new AiGenerationParameters()
            {
                CompletionModel = AiCompletionModel.Accurate,
                MaxTokens = 2048,
                Temperature = 1.0f, //notice that we are using a high temperature to generate more creative instructions
            };
        }

        /// <inheritdoc />
        public async Task GenerateLogicForGroupFromAudio(AudioClip audioPrompt, CancellationToken cancellationToken = default)
        {
            Debug.Log($"[Emotional Cubes Generator] Requested cubes from audio prompt");

            var transcription = await m_aiQueryPerformer.GetAudioTranscription(audioPrompt, "en", cancellationToken);
            Debug.Log($"[Emotional Cubes Generator] Audio prompt from user is: {transcription}");

            await GenerateLogicForGroupFromText(transcription);
        }

        /// <inheritdoc />
        public async Task GenerateLogicForGroupFromText(string prompt, CancellationToken cancellationToken = default)
        {
            Debug.Log($"[Emotional Cubes Generator] Requested cubes from text prompt");

            var instructions = await m_aiQueryPerformer.GetCompletion(PromptTemplate.GenerateFullPrompt(prompt), m_aiParameters.CompletionModel, 
                m_aiParameters.Temperature, m_aiParameters.MaxTokens, cancellationToken);
            Debug.Log($"[Emotional Cubes Generator] Instructions returned from AI are:\n {instructions}");

            await ExecuteInstructions(instructions);
            Debug.Log($"[Emotional Cubes Generator] Cubes generation completed");
        }

        /// <summary>
        /// Executes the instructions returned from the AI to generate the cubes depending on the emotions of the user.
        /// The format for every line is:
        /// cube position; prompt of the logic of the cube
        /// </summary>
        /// <param name="instructions">Instructions received by the AI</param>
        /// <returns></returns>
        private async Task ExecuteInstructions(string instructions)
        {
            var instructionLines = instructions.Split('\n');

            //for every line of the instructions, create a cube and generate the logic
            foreach (var line in instructionLines)
            {
                if(string.IsNullOrEmpty(line) || string.IsNullOrWhiteSpace(line))
                    continue;

                //separate the position from the logic
                int openParenthesisIndex = line.IndexOf('(');
                int closeParenthesisIndex = line.IndexOf(')');

                if (openParenthesisIndex == -1 || closeParenthesisIndex == -1) //may be some empty line or some example written by the AI
                    continue;

                var position = line.Substring(openParenthesisIndex + 1, closeParenthesisIndex - openParenthesisIndex - 1);
                var prompt = line.Substring(closeParenthesisIndex + 1);

                var positionParts = position.Split(',');

                //read the position and beware that ChatGPT returns a dot as decimal separator, while in some countries Unity expects a comma
                Vector3 readPosition = new Vector3(
                    float.Parse(positionParts[0],System.Globalization.CultureInfo.InvariantCulture),
                    float.Parse(positionParts[1],System.Globalization.CultureInfo.InvariantCulture),
                    float.Parse(positionParts[2],System.Globalization.CultureInfo.InvariantCulture));

                //generate a cube on the position
                CubesManager.Instance.AddCubeToCurrentGroup(readPosition, Quaternion.identity, Vector3.one);

                //generate the logic for the cube
                await CubesManager.Instance.GenerateLogicForGroupFromText(prompt);
            }
        }
    }

}