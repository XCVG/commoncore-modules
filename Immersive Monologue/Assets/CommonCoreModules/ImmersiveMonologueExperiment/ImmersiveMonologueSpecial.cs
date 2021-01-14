using CommonCore.ObjectActions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CommonCore.RpgGame.Dialogue;
using CommonCore.World;
using System;

namespace CommonCore.Experimental.ImmersiveMonologue
{

    /// <summary>
    /// Action Special that displays an Immersive Monologue
    /// </summary>
    public class ImmersiveMonologueSpecial : ActionSpecial
    {
        [SerializeField]
        private ImmersiveMonologue ImmersiveMonologue;
        [SerializeField]
        private bool Pause = true;
        [SerializeField]
        private TargetType Target = TargetType.None;
        [SerializeField]
        private GameObject TargetReference = null;
        [SerializeField]
        private string TargetId = null;

        private bool Triggered = false;

        public override void Execute(ActionInvokerData data)
        {
            if (!enabled && !AllowInvokeWhenDisabled)
                return;

            if (Triggered && !Repeatable)
                return;

            string target = null;
            switch (Target)
            {
                case TargetType.Activator:
                    target = data.Activator.Ref()?.gameObject.name;
                    break;
                case TargetType.NearestController:
                    target = GetComponentInParent<BaseController>().Ref()?.gameObject.name;
                    break;
                case TargetType.ByTID:
                    target = TargetId;
                    break;
                case TargetType.ByReference:
                    target = TargetReference.name;
                    break;
            }

            DialogueInitiator.SetDynamicDialogue(ImmersiveMonologue.BuildDialogueScene());
            DialogueInitiator.InitiateDialogue(DialogueModule.DynamicDialogueName, Pause, null, target);

            Triggered = true;
        }

        [Serializable]
        public enum TargetType
        {
            None,
            Activator,
            NearestController,
            ByTID,
            ByReference
        }
    }
}