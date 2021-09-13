using Newtonsoft.Json;
using System.Collections.Generic;

namespace CommonCore.State
{
    public partial class PersistState
    {
        /// <summary>
        /// [AchievementsExperiment] Unlocked achievements for interim achievements system
        /// </summary>
        [JsonProperty(PropertyName = "UnlockedAchievements")]
        public HashSet<string> UnlockedAchievements { get; private set; } = new HashSet<string>();
    }
}