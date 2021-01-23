using CommonCore.RpgGame.Dialogue;
using System;
using System.Collections.Generic;


/// <summary>
/// Immersive Monologue: like a Monologue but (mis)using the dialogue system
/// </summary>
namespace CommonCore.Experimental.ImmersiveMonologue
{

    /// <summary>
    /// Model class for an Immersive Monologue
    /// </summary>
    [Serializable]
    public class ImmersiveMonologue
    {
        public bool HideNameText = true;
        public string Music = null;
        public ChoicePanelHeight PanelHeight = ChoicePanelHeight.Half;

        public ImmersiveMonologueNode[] Nodes;

        /// <summary>
        /// Build a DialogueScene from this ImmersiveMonologue
        /// </summary>
        public DialogueScene BuildDialogueScene()
        {
            string defaultFrameName = "f_0";

            Dictionary<string, Frame> frames = new Dictionary<string, Frame>();
            for(int i = 0; i < Nodes.Length; i++)
            {
                var node = Nodes[i];

                string next = (i == (Nodes.Length - 1)) ? "meta.return" : $"f_{i+1}";
                FrameOptions options = new FrameOptions(new Dictionary<string, object>()
                {
                    { nameof(FrameOptions.HideNameText), HideNameText },
                    { nameof(FrameOptions.VoiceOverride), node.VoiceOverride },
                    { nameof(FrameOptions.PanelHeight), node.PanelHeight != ChoicePanelHeight.Default ? node.PanelHeight : PanelHeight }
                });

                TextFrame frame = new TextFrame(null, null, next, null, null, node.Text, node.NextText, null, default, node.AllowSkip, node.TimeToShow, node.TimeToShow > 0, null, null, options, null, null);
                frames.Add($"f_{i}", frame);
            }

            DialogueScene dialogueScene = new DialogueScene(frames, defaultFrameName, Music);
            return dialogueScene;
        }
    }

    /// <summary>
    /// Model class for an immersive monologue node
    /// </summary>
    [Serializable]
    public class ImmersiveMonologueNode
    {
        public string VoiceOverride = null;
        public string Text = null;
        public string NextText = null;
        public bool AllowSkip = true;
        public float TimeToShow = 0;
        public ChoicePanelHeight PanelHeight = ChoicePanelHeight.Default;
    }



}