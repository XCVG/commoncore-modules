using CommonCore.Input;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonCore.BasicControllerInput
{
    public class BasicControllerInputMap
    {
        public Dictionary<string, string> ButtonMappings = new Dictionary<string, string>();
        public Dictionary<string, string> AxisMappings = new Dictionary<string, string>();

        public float StickPremultiply = 1;
        public string MapDpadTo = "";

        public static BasicControllerInputMap GetDefault()
        {
            var map = new BasicControllerInputMap();

            map.AxisMappings.Add(DefaultControls.MoveX, "leftStick_X");
            map.AxisMappings.Add(DefaultControls.MoveY, "leftStick_Y");

            map.ButtonMappings.Add(DefaultControls.Fire, "A");
            map.ButtonMappings.Add(DefaultControls.Use, "X");
            map.ButtonMappings.Add(DefaultControls.Jump, "B");

            map.MapDpadTo = "leftStick";

            return map;
        }
    }
}

