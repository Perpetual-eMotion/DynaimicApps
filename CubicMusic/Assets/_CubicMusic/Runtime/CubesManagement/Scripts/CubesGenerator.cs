/*
 * Copyright (C) Antony Vitillo (aka Skarredghost), Perpetual eMotion 2023.
 * Distributed under the MIT License (license terms are at http://opensource.org/licenses/MIT).
 */

using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace vrroom.CubicMusic.CubesMgmt
{
    /// <summary>
    /// Generates the cubes in the scene to which the AI logic can be added at runtime
    /// </summary>
    public class CubesGenerator : MonoBehaviour
    {
        /// <summary>
        /// Action to be used to add a cube to the scene
        /// </summary>
        [SerializeField]
        private InputActionReference m_addCubeAction;

        /// <summary>
        /// On Enable
        /// </summary>
        private void OnEnable()
        {
            m_addCubeAction.action.performed += AddCubeActionPerformed;
        }

        /// <summary>
        /// On Enable
        /// </summary>
        private void OnDisable()
        {
            m_addCubeAction.action.performed -= AddCubeActionPerformed;
        }

        /// <summary>
        /// Callback called when the action to add a cube is performed
        /// </summary>
        /// <param name="obj"></param>
        private void AddCubeActionPerformed(InputAction.CallbackContext obj)
        {
            CubesManager.Instance.AddCubeToCurrentGroup(transform.position, transform.rotation, transform.lossyScale);
        }
    }

}