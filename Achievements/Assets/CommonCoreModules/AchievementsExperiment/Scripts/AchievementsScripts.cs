using CommonCore.Messaging;
using CommonCore.Scripting;
using CommonCore.State;
using CommonCore.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonCore.Experimental.Achievements
{

    public static class AchievementsScripts
    {

        [CCScript, CCScriptHook(AllowExplicitCalls = false, Hook = ScriptHook.AfterAddonsLoaded)]
        private static void Initialize()
        {
            CCBase.GetModule<UIModule>().RegisterIGUIPanel("AchievementsPanel", 100, "Achievements", CoreUtils.LoadResource<GameObject>("Modules/AchievementsExperiment/AchievementsPanel"));
        }

        [CCScript(NeverPassExecutionContext = true)]
        public static void UnlockAchievement(string achievementKey)
        {
            if (PersistState.Instance.UnlockedAchievements.Contains(achievementKey)) //don't grant duplicates
                return;

            PersistState.Instance.UnlockedAchievements.Add(achievementKey);
            PersistState.Save(); //overly aggressive?

            //at least broadcast a message notifying an achievement has been granted
            QdmsMessageBus.Instance.PushBroadcast(new QdmsKeyValueMessage("AchievementUnlocked", new Dictionary<string, object>()
            {
                { "AchievementKey", achievementKey}
            }));
        }
    }
}