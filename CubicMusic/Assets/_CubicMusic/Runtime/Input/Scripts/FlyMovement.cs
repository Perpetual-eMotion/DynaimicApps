/*
 * Copyright (C) Antony Vitillo (aka Skarredghost), Perpetual eMotion 2023.
 * Distributed under the MIT License (license terms are at http://opensource.org/licenses/MIT).
 */

using UnityEngine;
using UnityEngine.InputSystem;

namespace vrroom.CubicMusic.Input
{
    /// <summary>
    /// Makes a gameobject fly around the scene
    /// </summary>
    public class FlyMovement : MonoBehaviour
    {
        /// <summary>
        /// Action for translational movement (left/right, up/down)
        /// </summary>
        [SerializeField]
        private InputActionReference m_movementAction;

        /// <summary>
        /// Action for forward/backward movement
        /// </summary>
        [SerializeField]
        private InputActionReference m_upDownMovementAction;

        /// <summary>
        /// Action for activating/deactivating the current movement
        /// </summary>
        [SerializeField]
        private InputActionReference m_flyActivationAction;

        /// <summary>
        /// True if the movement is activated when the <see cref="m_flyActivationAction"/> is on, false otherwise
        /// </summary>
        [SerializeField]
        private bool m_requireModeOn = true;

        /// <summary>
        /// Movement speed
        /// </summary>
        [SerializeField]
        private float m_speed = 1.0f;

        /// <summary>
        /// If true, movement along the Y axis is mapped to up/down movement, otherwise it is mapped to forward/backward movement
        /// </summary>
        [SerializeField]
        private bool m_forwardSwitch = false;
        
        /// <summary>
        /// Update
        /// </summary>
        void Update()
        {
            //check if the movement is activated
            if (m_requireModeOn ^ m_flyActivationAction.action.ReadValue<float>() > 0.5f)
                return;

            //get the movement and apply it
            var newMovement = m_movementAction.action.ReadValue<Vector2>();
            var newUpDownMovement = m_upDownMovementAction.action.ReadValue<float>();

            var delta = newMovement;
            transform.position += 
                (transform.right * delta.x + 
                transform.up * (m_forwardSwitch ? delta.y : newUpDownMovement) + 
                transform.forward * (m_forwardSwitch ? newUpDownMovement : delta.y)).normalized 
                * m_speed * Time.deltaTime;
        }
    }

}