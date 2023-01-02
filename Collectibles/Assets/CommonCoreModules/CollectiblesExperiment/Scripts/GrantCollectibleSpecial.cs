using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CommonCore;
using CommonCore.ObjectActions;
using CommonCore.Messaging;

namespace CommonCore.Experimental.Collectibles
{

    /// <summary>
    /// Grants a collectible and optionally shows its visual
    /// </summary>
    public class GrantCollectibleSpecial : ActionSpecial
    {
        [Header("Grant Collectible Special")]
        public string Collectible;
        public bool ShowVisual = true;

        public bool GrantInGame = true;
        public bool GrantPersistent = false;

        private bool Locked = false;

        public override void Execute(ActionInvokerData data)
        {
            if (Locked || (!AllowInvokeWhenDisabled && !isActiveAndEnabled))
                return;

            var module = CCBase.GetModule<CollectiblesExperimentModule>();

            if (GrantInGame)
                module.GrantCollectible(Collectible, false);

            if (GrantPersistent)
                module.GrantCollectible(Collectible, true);

            if (GrantInGame || GrantPersistent)
            {
                QdmsMessageBus.Instance.PushBroadcast(new QdmsKeyValueMessage("CollectibleGranted",
                new Dictionary<string, object> {
                    { "CollectibleKey", Collectible },
                    { "GrantInGame", GrantInGame },
                    { "GrantPersistent", GrantPersistent },
                }));
            }

            if (ShowVisual)
                OpenVisualView();

            if (!Repeatable)
                Locked = true;
        }

        private void OpenVisualView()
        {
            var go = GameObject.Instantiate(CoreUtils.LoadResource<GameObject>("Modules/CollectiblesExperiment/CollectiblesIngameView"), CoreUtils.GetUIRoot());
            var controller = go.GetComponent<CollectiblesIngameViewController>();
            controller.LoadVisual(Collectible);
        }
    }
}