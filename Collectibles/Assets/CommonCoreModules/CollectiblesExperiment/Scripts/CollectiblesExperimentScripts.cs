using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CommonCore.State;
using System;
using CommonCore.Scripting;

namespace CommonCore.Experimental.Collectibles
{

    public static class CollectiblesExperimentScripts
    {
        [CCScript(NeverPassExecutionContext = true)]
        public static bool IsCollectibleGranted(string key)
        {
            return CCBase.GetModule<CollectiblesExperimentModule>().IsCollectibleGranted(key);
        }

        [CCScript(NeverPassExecutionContext = true)]
        public static void GrantCollectible(string key)
        {
            CCBase.GetModule<CollectiblesExperimentModule>().GrantCollectible(key, false);
        }

        [CCScript(NeverPassExecutionContext = true)]
        public static void GrantCollectiblePersist(string key)
        {
            CCBase.GetModule<CollectiblesExperimentModule>().GrantCollectible(key, true);
        }

    }

}