/*
 * Copyright (C) Antony Vitillo (aka Skarredghost), Perpetual eMotion 2023.
 * Distributed under the MIT License (license terms are at http://opensource.org/licenses/MIT).
 */

using OpenAI.Audio;
using RoslynCSharp;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using vrroom.Dynaimic.Ai;

namespace vrroom.Dynaimic.GenerativeLogic
{
    /// <summary>
    /// Generates runtime logic (compiled C# scripts) starting from some prompts to the AI
    /// </summary>
    public class GenerativeLogicManager
    {
        /// <summary>
        /// The element that performs the queries to the AI cloud
        /// </summary>
        private AiQueryPerformer m_aiQueryPerformer;

        /// <summary>
        /// Parameters for the completion queries. We use always the same parameters for all the queries
        /// </summary>
        private AiGenerationParameters m_aiParameters;

        /// <summary>
        /// Runtime domain where the generated scripts will be loaded
        /// </summary>
        private ScriptDomain m_scriptsDomain;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="aiQueryPerformer">Element that performs the queries to the AI backend</param>
        /// <param name="aiParameters">Parameters for the completion queries. We use the same for all queries for simplicity</param>
        /// <param name="referenceAssets">The assemblies that are the references of the scripts being generated</param>
        public GenerativeLogicManager(AiQueryPerformer aiQueryPerformer, AiGenerationParameters aiParameters, AssemblyReferenceAsset[] referenceAssets)
        {
            //create the runtime domain where the scripts will be loaded and add the references
            m_scriptsDomain = ScriptDomain.CreateDomain(nameof(vrroom.Dynaimic));

            foreach (var reference in referenceAssets)
            {
                m_scriptsDomain.RoslynCompilerService.ReferenceAssemblies.Add(reference);
            }

            //initialize the AI query engine
            m_aiQueryPerformer = aiQueryPerformer;
            m_aiParameters = aiParameters;
        }

        /// <summary>
        /// Asks the AI to generate a script at runtime starting from a prompt
        /// </summary>
        /// <param name="prompt">The prompt with the behaviour desired from the script</param>
        /// <param name="template">Template to use to explain better the meaning of the prompt</param>
        /// <param name="cancellationToken">Cancelation token</param>
        /// <returns>Runtime script</returns>
        public async Task<ScriptType> GenerateLogicFromText(string prompt, AiPromptTemplate template, CancellationToken cancellationToken = default)
        {
            //perform the query to the AI
            var generatedCode = await m_aiQueryPerformer.GetCompletion(template.GenerateFullPrompt(prompt), 
                m_aiParameters.CompletionModel, m_aiParameters.Temperature, m_aiParameters.MaxTokens, cancellationToken);

            //compile the generated code and load it in the runtime domain
            try
            {

                var scriptAssembly =
#if ROSLYNCSHARP
                //backend is IL2CPP and we are using dotnow to load the script as interpreted in a sort of VM.
                //Notice that this must be called on the main thread, so it is going to block the application for a few seconds 
                m_scriptsDomain.CompileAndLoadSourceInterpreted(generatedCode);
#else
                await Task.Run(() =>
                    //no need to build for IL2CPP, we can load the script as it is on a separate thraed
                    m_scriptsDomain.CompileAndLoadSource(generatedCode),
                    cancellationToken
                );
#endif
                return scriptAssembly.MainType;   
            }
            catch (System.Exception e)
            {
                Debug.LogError(e);
                return null;
            }
        }

        /// <summary>
        /// Asks the AI to generate a script at runtime starting from an audio prompt in English
        /// </summary>
        /// <param name="audioPrompt">Audioclip containing the prompt, in English language</param>
        /// <param name="template">>Template to use to explain better the meaning of the prompt</param>
        /// <param name="cancellationToken">Cancelation token</param>
        /// <returns>Runtime script</returns>
        public async Task<ScriptType> GenerateLogicFromAudio(AudioClip audioPrompt, AiPromptTemplate template, CancellationToken cancellationToken = default)
        {
            var transcription = await m_aiQueryPerformer.GetAudioTranscription(audioPrompt, "en", cancellationToken);
            
            return await GenerateLogicFromText(transcription, template, cancellationToken);
        }
    }

    /// <summary>
    /// Parameters related to AI completions
    /// </summary>
    public class AiGenerationParameters
    {
        /// <summary>
        /// Type of completion model to use
        /// </summary>
        public AiCompletionModel CompletionModel { get; set; } = AiCompletionModel.Accurate;

        /// <summary>
        /// Temperature to use for the completion. Higher values will make the AI more creative
        /// </summary>
        public float Temperature { get; set; } = 0.33f;

        /// <summary>
        /// Maximum number of tokens to use for the completion
        /// </summary>
        public int MaxTokens { get; set; } = 2048;
    }

    /// <summary>
    /// Represents a template for a prompt to the AI.
    /// It lets specify some conditions to be applied around the
    /// prompt that has been specified by the user, so that to
    /// add some context that the AI system should use.
    /// (E.g. the pre-prompt may say "Generate a Unity script that does this:"
    /// so that the prompt asked by the user can only include the logic that 
    /// the user wants)
    /// </summary>
    public class AiPromptTemplate
    {
        /// <summary>
        /// Sentence to be added before the prompt
        /// </summary>
        public string PrePrompt { get; set; } = string.Empty;

        /// <summary>
        /// Sentence to be added after the prompt
        /// </summary>
        public string PostPrompt { get; set; } = string.Empty;

        /// <summary>
        /// Generates the full prompt to be sent to the AI cloud solution
        /// </summary>
        /// <param name="prompt">Prompt specified by the user</param>
        /// <returns>Compound prompt, ready to be read by the AI</returns>
        public string GenerateFullPrompt(string prompt)
        {
            return $"{PrePrompt} {prompt} {PostPrompt}";
        }
    }
 
}