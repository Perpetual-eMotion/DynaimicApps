/*
 * Copyright (C) Antony Vitillo (aka Skarredghost), Perpetual eMotion 2023.
 * Distributed under the MIT License (license terms are at http://opensource.org/licenses/MIT).
 */

using RoslynCSharp;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using vrroom.Dynaimic.Ai;
using vrroom.Dynaimic.GenerativeLogic;

namespace vrroom.CubicMusic.CubesMgmt
{
    /// <summary>
    /// Main class of the CubicMusic system. It manages the creation and destruction of the cubes and the logic attached to them
    /// </summary>
    [DefaultExecutionOrder(-1)]
    public class CubesManager : MonoBehaviour, ICreatesLogicFromPrompt
    {
        /// <summary>
        /// The prompt template to generate Unity scripts that can be added to the cubes at runtime without requiring
        /// the setup of public properties. Scripts should work out of the bo
        /// </summary>
        static readonly AiPromptTemplate s_promptTemplateForUnityScripts = new AiPromptTemplate()
        {
            PrePrompt = @"Generate a Unity C# script with internally initialized properties that does the following to the gameobject: ",
            PostPrompt = @"The script should work out of the box without requiring any external configuration. Here are the requirements:
                        - The script can NOT include public properties.
                        - The properties should be initialized internally within the script, in the start method.
                        - If the property is a prefab, initialize it with a primitive, in the start method.
                        - The properties should not be modifiable from external sources.
                        - The script should include any necessary logic or code that utilizes these properties.
                        - If the gameobject has to interact the hand, the hand can be found as a trigger collider on the Hand layer.
                        - IF and only if the query is about the microphone, you can use vrroom.CubicMusic.Audio.AudioManager.Instance.MicrophoneAnalyzer.CurrentVolume property, range from 0 to 1.
                        - IF and only if the query is about the music, you can use vrroom.CubicMusic.Audio.AudioManager.Instance.BackgroundMusicAnalyzer.CurrentVolume, range from 0 to 1.        
                        Please generate the Unity script meeting these specifications."
        };

        /// <summary>
        /// The prefab to use for the cubes to generate. If null, a default cube will be used
        /// </summary>
        [SerializeField]
        public GameObject CubePrefab;

        /// <summary>
        /// The assemblies that the generated scripts will reference
        /// </summary>
        [SerializeField]
        private AssemblyReferenceAsset[] m_referenceAssemblies;

        /// <summary>
        /// The element that performs the queries to the AI cloud
        /// </summary>
        private AiQueryPerformer m_aiQueryPerformer;

        /// <summary>
        /// The element that creates the logic from the AI prompts
        /// </summary>
        private GenerativeLogicManager m_generativeLogicManager;

        /// <summary>
        /// The list of cube groups managed by this object.
        /// Every group contains a list of cubes to which logic can be added at runtime
        /// </summary>
        private List<ObjectsGroupLogicHandler> m_managedCubeGroups;

        /// <inheritdoc />
        public AiPromptTemplate PromptTemplate => s_promptTemplateForUnityScripts;

        /// <summary>
        /// Get the element that performs the queries to the AI cloud
        /// </summary>
        public AiQueryPerformer AiQueryPerformer => m_aiQueryPerformer;

        /// <summary>
        /// Singleton instance
        /// </summary>
        public static CubesManager Instance;

        /// <summary>
        /// Awake
        /// </summary>
        private void Awake()
        {
            //destroy this object if another instance already exists
            if(Instance != null && Instance != this)
            {
                Destroy(this);
                return;
            }
            //else we are the singleton instance
            else
            {
                Instance = this;

                //initialize a few things
                m_managedCubeGroups = new List<ObjectsGroupLogicHandler>(1);
                m_managedCubeGroups.Add(new ObjectsGroupLogicHandler()); //creates the first group

                m_aiQueryPerformer = new OpenAiQueryPerformer();
                m_generativeLogicManager = new GenerativeLogicManager(m_aiQueryPerformer, new AiGenerationParameters(), m_referenceAssemblies);

                Debug.Log("[Cubes Manager] Initialized");
            }
            
        }

        /// <summary>
        /// Adds a cube at the specified position, rotation and scale to the current managed group
        /// </summary>
        /// <param name="position">Position</param>
        /// <param name="rotation">Rotation</param>
        /// <param name="scale">Local scale</param>
        public void AddCubeToCurrentGroup(Vector3 position, Quaternion rotation, Vector3 scale)
        {
            GameObject cube = GenerateCube();
            cube.transform.position = position;
            cube.transform.rotation = rotation;
            cube.transform.localScale = scale;
            m_managedCubeGroups[0].AddObjectToCurrentGroup(cube);

            Debug.Log($"[Cubes Manager] New cube added to the group. Number of cubes is now {m_managedCubeGroups[0].Count}");
        }

        /// <inheritdoc />
        public async Task GenerateLogicForGroupFromAudio(AudioClip audioPrompt, CancellationToken cancellationToken = default)
        {
            Debug.Log($"[Cubes Manager] Requested logic from audio prompt");

            var script = await m_generativeLogicManager.GenerateLogicFromAudio(audioPrompt, s_promptTemplateForUnityScripts, cancellationToken);
            Debug.Log($"[Cubes Manager] Script generated from audio is called {script.FullName}");

            AttachScriptToGroup(script);
        }

        /// <inheritdoc />
        public async Task GenerateLogicForGroupFromText(string prompt, CancellationToken cancellationToken = default)
        {
            Debug.Log($"[Cubes Manager] Requested logic from text prompt");

            ScriptType script = null;
            int tries = 0;
            do
            {
                script = await m_generativeLogicManager.GenerateLogicFromText(prompt, s_promptTemplateForUnityScripts, cancellationToken);

                if (script != null) //in case of error, the script is null
                {
                    Debug.Log($"[Cubes Manager] Script generated from text is called {script.FullName}");
                    AttachScriptToGroup(script);
                }
            } while (script == null && ++tries < 3); //if a script fails, try again a few times
        }

        /// <summary>
        /// Generates a cube
        /// </summary>
        /// <returns>Generated cube</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private GameObject GenerateCube()
        {
            if (CubePrefab == null)
            {
                return GameObject.CreatePrimitive(PrimitiveType.Cube);
            }
            else
            {
                return Object.Instantiate(CubePrefab);
            }
        }

        /// <summary>
        /// Attaches the specified script to the current group.
        /// After this, a new group is created and becomes the current group
        /// </summary>
        /// <param name="script">Script that has been generated</param>
        private void AttachScriptToGroup(ScriptType script)
        {
            m_managedCubeGroups[0].AttachLogicToGroupElements(script);
            m_managedCubeGroups.Insert(0, new ObjectsGroupLogicHandler());
        }
    }

}