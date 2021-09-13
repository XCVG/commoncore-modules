using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonCore.Experimental.SpeedHacks
{

    /// <summary>
    /// Script specifying ambient light override to use after "no lights" speed hack is applied
    /// </summary>
    public class SpeedHackAmbientLightOverride : MonoBehaviour
    {
        public float AmbientIntensity = 1.0f;
        public bool OverrideColor = false;
        [ColorUsage(true, true)]
        public Color AmbientColor = Color.white;
    }
}

