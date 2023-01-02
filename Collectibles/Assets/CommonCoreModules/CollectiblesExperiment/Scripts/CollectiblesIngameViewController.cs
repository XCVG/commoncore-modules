using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CommonCore;
using CommonCore.UI;
using CommonCore.LockPause;
using UnityEngine.UI;
using CommonCore.Input;
using CommonCore.Audio;

namespace CommonCore.Experimental.Collectibles
{

    /// <summary>
    /// Controller for the ingame (ie shows up when you interact with things) view for collectibles
    /// </summary>
    public class CollectiblesIngameViewController : MonoBehaviour
    {
        [SerializeField]
        private Text HeaderText;
        [SerializeField]
        private RectTransform OuterContainer;
        [SerializeField]
        private RectTransform Container;

        private void Start()
        {
            CCBase.GetModule<UIModule>().ApplyThemeRecurse(transform);
            SetSize();
        }

        private void OnEnable()
        {
            LockPauseModule.PauseGame(PauseLockType.All, this);
        }

        private void OnDisable()
        {
            LockPauseModule.UnpauseGame(this);
        }

        private void Update()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.Escape) || UnityEngine.Input.GetKeyDown(KeyCode.Return) || MappedInput.GetButtonDown(Input.DefaultControls.OpenMenu) || MappedInput.GetButtonDown(Input.DefaultControls.Use))
                CloseView();
        }

        public void CloseView()
        {
            Destroy(this.gameObject);
        }

        private void SetSize()
        {
            var module = CCBase.GetModule<CollectiblesExperimentModule>();
            var parameters = module.Params;

            OuterContainer.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, parameters.IngameViewWidth);
            OuterContainer.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, parameters.IngameViewHeight);
        }

        public void LoadVisual(string key)
        {
            var module = CCBase.GetModule<CollectiblesExperimentModule>();

            string title = module.GetNameForCollectible(key);
            HeaderText.text = title;

            var visual = module.GetVisualForCollectible(key);
            if(visual == null)
            {
                return;
            }
            else if(module.Params.PlayAudioLogsImmediately && visual is AudioClip ac)
            {
                CCBase.GetModule<AudioModule>().AudioPlayer.PlaySound(ac, false);
                CloseView();
            }
            else
            {
                var go = GameObject.Instantiate(CoreUtils.LoadResource<GameObject>("Modules/CollectiblesExperiment/CollectiblesVisualPanel"), Container);
                var controller = go.GetComponent<CollectiblesVisualPanelController>();
                controller.LoadVisual(visual);
            }
        }
    }
}