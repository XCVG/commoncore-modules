

using Newtonsoft.Json;
using System.Collections.Generic;

namespace CommonCore.Experimental.Achievements
{
    public class AchievementDefinitions
    {
        [JsonIgnore]
        public IReadOnlyDictionary<string, AchievementDefinitionNode> Achievements => _Achievements;

        [JsonProperty(PropertyName = "achievements")]
        private Dictionary<string, AchievementDefinitionNode> _Achievements;
    }

    public class AchievementDefinitionNode
    {
        [JsonProperty]
        public string Title { get; private set; }
        [JsonProperty]
        public string Icon { get; private set; }
        [JsonProperty]
        public string Description { get; private set; }
        [JsonProperty]
        public string Hint { get; private set; }
    }
}