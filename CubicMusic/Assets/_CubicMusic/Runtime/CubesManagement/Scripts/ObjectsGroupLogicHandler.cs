/*
 * Copyright (C) Antony Vitillo (aka Skarredghost), Perpetual eMotion 2023.
 * Distributed under the MIT License (license terms are at http://opensource.org/licenses/MIT).
 */

using RoslynCSharp;
using System.Collections.Generic;
using UnityEngine;

namespace vrroom.CubicMusic.CubesMgmt
{
    /// <summary>
    /// Handles the runtime addition-removal of logic from a group of objects
    /// </summary>
    public class ObjectsGroupLogicHandler
    {
        /// <summary>
        /// The objects that are part of the group
        /// </summary>
        private List<GameObject> m_groupObjects;

        /// <summary>
        /// Maps every gameobject to the list of instantiated scripts on that object
        /// </summary>
        private Dictionary<GameObject, List<ScriptProxy>> m_instantiatedScriptsByGo;

        /// <summary>
        /// Saves all the script types that have been added to the group
        /// </summary>
        private HashSet<ScriptType> m_addedScriptTypes;

        /// <summary>
        /// Get the number of objects in the group
        /// </summary>
        public int Count => m_groupObjects.Count;

        /// <summary>
        /// Constructor
        /// </summary>
        public ObjectsGroupLogicHandler() 
        {
            m_groupObjects = new List<GameObject>();
            m_instantiatedScriptsByGo = new Dictionary<GameObject, List<ScriptProxy>>();
            m_addedScriptTypes = new HashSet<ScriptType>();
        }
        
        /// <summary>
        /// Add an object to the group
        /// </summary>
        /// <param name="go">Gameobject to add</param>
        /// <param name="addExistingLogic">True to add to the object the scripts already added to the existing ones, false otherwise</param>
        public void AddObjectToCurrentGroup(GameObject go, bool addExistingLogic = false)
        {
            m_groupObjects.Add(go);
            m_instantiatedScriptsByGo.Add(go, new List<ScriptProxy>());

            if(addExistingLogic)
            {
                // Add the existing logic to the object
                foreach (var scriptType in m_addedScriptTypes)
                {
                    AttachLogicToGameObject(scriptType, go);
                }
            }
        }

        /// <summary>
        /// Attach a runtime script to all the objects in the group.
        /// Only script types that have not been attached previously will be added (no duplicates admitted)
        /// </summary>
        /// <param name="script">Runtime script to add</param>
        public void AttachLogicToGroupElements(ScriptType script)
        {
            // Check if the script has been already added to the group. If so, do nothing
            if (m_addedScriptTypes.Contains(script))
                return;

            // Save the script type as added
            m_addedScriptTypes.Add(script);

            // Attach the logic to all the objects in the group
            foreach (GameObject go in m_groupObjects)
            {
                AttachLogicToGameObject(script, go);
            }
        }

        /// <summary>
        /// Removes a runtime script from all the objects in the group
        /// </summary>
        /// <param name="script">Runtime script to add</param>
        public void RemoveLogicFromGroupElements(ScriptType script)
        {
            // Check if the script has been already added to the group. If not, do nothing
            if (!m_addedScriptTypes.Contains(script))
                return;

            // Remove the script type from the added ones
            m_addedScriptTypes.Remove(script);

            // Attach the logic to all the objects in the group
            foreach (GameObject go in m_groupObjects)
            {
                RemovesLogicFromGameObject(script, go);
            }
        }

        /// <summary>
        /// Attaches a runtime script to a gameobject, but only if the script has not been attached to the object yet
        /// </summary>
        /// <param name="script">Runtime script</param>
        /// <param name="go">Gameobject to attach this script to</param>
        /// <returns>True if the script was attached, false if it was not because it was duplicated</returns>
        private bool AttachLogicToGameObject(ScriptType script, GameObject go)
        {
            // Check if the script has been already attached to the object
            var existingProxy = m_instantiatedScriptsByGo[go].Find(p => p.ScriptType == script);

            //if not
            if (existingProxy == null) {
                //let's add it to the gameobject and to the list of instantiated scripts
                var proxy = script.CreateInstance(go);
                m_instantiatedScriptsByGo[go].Add(proxy);

                return true;
            }
            //if it is, just return false
            else
                return false;
        }

        /// <summary>
        /// Removes a runtime script from a gameobject, but only if the script was attached to the object
        /// </summary>
        /// <param name="script">Runtime script</param>
        /// <param name="go">Gameobject to remove this script from</param>
        /// <returns>True if the script was removed, false if it was not because it was not present</returns>
        private bool RemovesLogicFromGameObject(ScriptType script, GameObject go)
        {
            // Check if the script has been already attached to the object
            var existingProxy = m_instantiatedScriptsByGo[go].Find(p => p.ScriptType == script);

            //if not, don't anything
            if (existingProxy == null)
                return false;
            //if it is, remove it from the list of instantiated scripts and then destroy it
            else
            {
                m_instantiatedScriptsByGo[go].Remove(existingProxy);
                existingProxy.Dispose();
                return true;
            }

        }
    }

}