using CommonCore;
using CommonCore.Config;
using CommonCore.LockPause;
using CommonCore.Scripting;
using CommonCore.State;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CommonCore.Bigscreen
{

    /// <summary>
    /// Controller for the Bigscreen ingame menu
    /// </summary>
    public class BigscreenIngameMenuController : MonoBehaviour
    {
        [SerializeField]
        private GameObject MainPanel = null;
        [SerializeField]
        private Button SaveButton = null;

        private void Start()
        {
            MainPanel.SetActive(false); //in case I forget to deactivate it

            //TODO set the save button
        }

        private void Update()
        {
            CheckMenuOpen();
        }

        private void CheckMenuOpen()
        {
            //bool menuToggled = UnityEngine.Input.GetKeyDown(KeyCode.Escape);
            bool menuToggled = UnityEngine.Input.GetKeyDown(KeyCode.JoystickButton7);

            if (menuToggled)
            {
                //if we're locked out, let the menu be closed but not opened
                if (!AllowMenu)
                {
                    if (MainPanel.activeSelf)
                    {
                        MainPanel.SetActive(false);

                        DoUnpause();

                    }
                }
                else
                {
                    //otherwise, flip state
                    bool newState = !MainPanel.activeSelf;
                    MainPanel.SetActive(newState);

                    //handle pause
                    if (newState)
                        DoPause();
                    else
                        DoUnpause();


                    if (newState)
                    {
                        //set state of save button on open
                        if(CoreParams.AllowSaveLoad && CoreParams.AllowManualSave && !GameState.Instance.SaveLocked)
                        {
                            SaveButton.interactable = true;
                        }
                        else
                        {
                            SaveButton.interactable = false;
                        }

                        //set default button
                        EventSystem.current.SetSelectedGameObject(null);
                        EventSystem.current.SetSelectedGameObject(MainPanel.transform.FindDeepChild("ButtonResume").gameObject);

                        ScriptingModule.CallHooked(ScriptHook.OnIGUIMenuOpen, this);
                    }

                }
            }

        }

        private void DoPause()
        {
            LockPauseModule.PauseGame(PauseLockType.AllowMenu, this);
            LockPauseModule.LockControls(InputLockType.GameOnly, this);
        }

        private void DoUnpause()
        {
            LockPauseModule.UnlockControls(this);
            LockPauseModule.UnpauseGame(this);
        }

        private bool AllowMenu
        {
            get
            {
                var lockState = LockPauseModule.GetInputLockState();
                return (lockState == null || lockState.Value >= InputLockType.GameOnly);
                //TODO allow temporary locking with a session flag or something
            }
        }

        public void HandleResumeButtonClicked()
        {
            //close the menu

            if (MainPanel.activeSelf)
            {
                MainPanel.SetActive(false);

                DoUnpause();

            }
        }

        public void HandleSaveButtonClicked()
        {
            //technically, it's a quicksave!
            SaveUtils.DoQuickSave();

            //TODO save indicator, probably will use the same as quicksave and autosave

            //close the menu

            if (MainPanel.activeSelf)
            {
                MainPanel.SetActive(false);

                DoUnpause();

            }
        }

        public void HandleExitButtonClicked()
        {
            //exit the game

            Time.timeScale = ConfigState.Instance.DefaultTimescale; //needed?
                                                                    //BaseSceneController.Current.("MainMenuScene");
            SharedUtils.EndGame();
        }       

    }
}