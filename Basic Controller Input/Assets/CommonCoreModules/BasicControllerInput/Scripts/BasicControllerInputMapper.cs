using System;
using CommonCore.Input;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using CommonCore.Config;

namespace CommonCore.BasicControllerInput
{
    public class BasicControllerInputMapper : InputMapper
    {
        private BasicControllerInputMap InputMap = BasicControllerInputMap.GetDefault();

        public BasicControllerInputMapper()
        {
            Debug.Log("[BasicControllerInputMapper] initialized");

            if(ConfigState.Instance.CustomConfigVars.TryGetValue("BasicControllerInputMap", out var oSavedMap))
            {
                Debug.Log("[BasicControllerInputMapper] found existing map");

                try
                {
                    InputMap = (BasicControllerInputMap)oSavedMap;

                    Debug.Log("[BasicControllerInputMapper] loaded existing map");
                }
                catch(Exception ex)
                {
                    Debug.LogError($"[BasicControllerInputMapper] failed to load map with {ex.GetType().Name}\n{ex.Message}");
                    Debug.LogException(ex);
                }
                
            }
            /*
            else
            {
                ConfigState.Instance.CustomConfigVars.Add("BasicControllerInputMap", InputMap);
                ConfigState.Save();
            }
            */
        }

        public override float GetAxis(string axis)
        {
            return MathUtils.Clamp(GetAxisRaw(axis) * InputMap.StickPremultiply, -1, 1);
        }

        public override float GetAxisRaw(string axis)
        {
            float value = 0;

            //Debug.Log($"GetAxisRaw {axis}");

            if (InputMap.AxisMappings.TryGetValue(axis, out var mappedAxis))
            {
                if (mappedAxis.Equals("leftStick_x", StringComparison.OrdinalIgnoreCase))
                {
                    value = Gamepad.current.leftStick.x.ReadValue();
                    if(InputMap.MapDpadTo.Equals("leftStick", StringComparison.OrdinalIgnoreCase))
                    {
                        if (Gamepad.current.dpad.left.isPressed)
                            value -= 1;
                        if (Gamepad.current.dpad.right.isPressed)
                            value += 1;
                    }
                }
                else if (mappedAxis.Equals("leftStick_y", StringComparison.OrdinalIgnoreCase))
                {
                    value = Gamepad.current.leftStick.y.ReadValue();
                    if (InputMap.MapDpadTo.Equals("leftStick", StringComparison.OrdinalIgnoreCase))
                    {
                        if (Gamepad.current.dpad.up.isPressed)
                            value += 1;
                        if (Gamepad.current.dpad.down.isPressed)
                            value -= 1;
                    }
                }
                else if (mappedAxis.Equals("rightStick_x", StringComparison.OrdinalIgnoreCase))
                {
                    value = Gamepad.current.rightStick.x.ReadValue();
                    if (InputMap.MapDpadTo.Equals("rightStick", StringComparison.OrdinalIgnoreCase))
                    {
                        if (Gamepad.current.dpad.left.isPressed)
                            value -= 1;
                        if (Gamepad.current.dpad.right.isPressed)
                            value += 1;
                    }
                }
                else if (mappedAxis.Equals("rightStick_y", StringComparison.OrdinalIgnoreCase))
                {
                    value = Gamepad.current.rightStick.y.ReadValue();
                    if (InputMap.MapDpadTo.Equals("rightStick", StringComparison.OrdinalIgnoreCase))
                    {
                        if (Gamepad.current.dpad.up.isPressed)
                            value += 1;
                        if (Gamepad.current.dpad.down.isPressed)
                            value -= 1;
                    }
                }

               // Debug.Log($"get {axis} map to {mappedAxis} value {value:F2}");

            }

            return value;
        }

        public override bool GetButton(string button)
        {
            if(InputMap.ButtonMappings.TryGetValue(button, out var mapping))
            {
                return ((ButtonControl)Gamepad.current[mapping]).isPressed;
            }
            return false;            
        }

        public override bool GetButtonDown(string button)
        {
            if (InputMap.ButtonMappings.TryGetValue(button, out var mapping))
            {
                return ((ButtonControl)Gamepad.current[mapping]).wasPressedThisFrame;
            }
            return false;
        }

        public override bool GetButtonUp(string button)
        {
            if (InputMap.ButtonMappings.TryGetValue(button, out var mapping))
            {
                return ((ButtonControl)Gamepad.current[mapping]).wasReleasedThisFrame;
            }
            return false;
        }

        public override MappingDescriptor GetDescriptorForAxis(string axis, AxisDirection direction)
        {
            return new MappingDescriptor();
        }

        public override MappingDescriptor GetDescriptorForButton(string button)
        {
            return new MappingDescriptor();
        }
    }
}


