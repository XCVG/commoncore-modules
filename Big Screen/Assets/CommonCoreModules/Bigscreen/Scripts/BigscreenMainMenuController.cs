using CommonCore;
using CommonCore.Config;
using CommonCore.Scripting;
using CommonCore.State;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace CommonCore.Bigscreen
{
    /// <summary>
    /// Controller for the Bigscreen Main Menu
    /// </summary>
    public class BigscreenMainMenuController : MonoBehaviour
    {
        [Header("Buttons"), SerializeField]
        private Button ContinueButton = null;

        [Header("Panel"), SerializeField]
        private GameObject CurrentPanel = null;
        [SerializeField]
        private GameObject MessagePanel = null;
        [SerializeField]
        private GameObject HelpPanel = null;

        [Header("Special"), SerializeField]
        private Text SystemText = null;

        [Header("Options"), SerializeField]
        private bool ShowMessageModal = true;


        private void Start()
        {
            //setup system text
            if (CoreParams.IsDebug)
            {
                SystemText.text = CoreParams.GetShortSystemText();
            }
            else
            {
                SystemText.gameObject.SetActive(false);
            }

            //setup continue button
            if(CoreParams.AllowSaveLoad && SaveUtils.GetLastSave() != null)
            {
                ContinueButton.interactable = true;
                EventSystem.current.SetSelectedGameObject(ContinueButton.gameObject);
            }
            else
            {
                ContinueButton.interactable = false;
            }

            //call hooked scripts
            ScriptingModule.CallHooked(ScriptHook.AfterMainMenuCreate, this);
        }

        public void OnClickContinue()
        {
            SharedUtils.LoadGame(SaveUtils.GetLastSave());
        }

        public void OnClickNew()
        {
            if(!ShowMessageModal)
            {
                StartGame();
                return;
            }

            if (CurrentPanel != null)
                CurrentPanel.SetActive(false);

            CurrentPanel = MessagePanel;

            CurrentPanel.SetActive(true);

            //select the continue/cancel buttons
            EventSystem.current.SetSelectedGameObject(CurrentPanel.transform.FindDeepChild("Button (1)").gameObject);
        }

        public void OnClickModalContinue()
        {
            StartGame();
        }

        public void OnClickModalCancel()
        {
            if (CurrentPanel == MessagePanel)
            {
                CurrentPanel.SetActive(false);
                CurrentPanel = null;
            }

            EventSystem.current.SetSelectedGameObject(GameObject.Find("ButtonNew"));
        }

        public void StartGame()
        {
            //start a new game the normal way
            SharedUtils.StartGame();
        }

        public void OnClickHelp()
        {
            if (CurrentPanel != null)
                CurrentPanel.SetActive(false);

            CurrentPanel = HelpPanel;

            CurrentPanel.SetActive(true);

            EventSystem.current.SetSelectedGameObject(CurrentPanel.transform.FindDeepChild("Button").gameObject);
        }

        public void OnClickHelpClose()
        {
            if (CurrentPanel == HelpPanel)
            {
                CurrentPanel.SetActive(false);
                CurrentPanel = null;
            }

            EventSystem.current.SetSelectedGameObject(GameObject.Find("ButtonHelp"));
        }

        public void OnClickExit()
        {
            Application.Quit();
        }

        public void OnClickExitBigscreenMode()
        {
            ConfigState.Instance.SetCustomFlag("UseBigScreenMode", false);
            ConfigState.Save();
            SceneManager.LoadScene("MainMenuScene");
        }

    }
}