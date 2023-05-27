/*
 * Copyright (C) Antony Vitillo (aka Skarredghost), Perpetual eMotion 2023.
 * Distributed under the MIT License (license terms are at http://opensource.org/licenses/MIT).
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace vrroom.CubicMusic.Input
{
    /// <summary>
    /// Disable specific input actions when the mouse is over a UI element,
    /// to avoid triggering actions when interacting with the UI.
    /// </summary>
    public class DisableActionsOnUi : MonoBehaviour
    {
        /// <summary>
        /// Reference to the actions to disable
        /// </summary>
        [SerializeField]
        private InputActionReference[] m_actions;

        /// <summary>
        /// True if currently actions are enabled, false otherwise
        /// </summary>
        private bool m_currentStatus = true;

        /// <summary>
        /// Update
        /// </summary>
        void Update()
        {
            //if we are interacting with the UI and the actions are enabled, or viceversa,
            //we need to change the status
            if (!(EventSystem.current.IsPointerOverGameObject() ^ m_currentStatus))
            {
                //change the status
                m_currentStatus = !m_currentStatus;

                //enable or disable the actions
                foreach (var action in m_actions)
                {
                    if (m_currentStatus)
                        action.action.Enable();
                    else
                        action.action.Disable();
                }

            }
        }
    }

}