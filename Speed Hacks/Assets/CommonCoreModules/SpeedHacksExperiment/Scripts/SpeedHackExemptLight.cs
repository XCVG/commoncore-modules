using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonCore.Experimental.SpeedHacks
{

    /// <summary>
    /// Script that denotes a light as "speedhack-exempt"
    /// </summary>
    public class SpeedHackExemptLight : MonoBehaviour
    {
        [Tooltip("If set, this light will always remain enabled")]
        public bool AlwaysShowLight = true;
        [Tooltip("If set, shadows on this light will always remain enabled")]
        public bool AlwaysShowShadows = true;
    }
}