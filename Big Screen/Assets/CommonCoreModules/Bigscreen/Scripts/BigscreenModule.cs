using CommonCore;
using CommonCore.Config;
using CommonCore.Scripting;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CommonCore.Bigscreen
{

    /// <summary>
    /// Bigscreen module. Provides menus and other functionality for "bigscreen"/10-foot mode.
    /// </summary>
    public class BigscreenModule : CCModule
    {
        public BigscreenModule()
        {
            if (SystemInfo.deviceType == DeviceType.Console)
            {
                ConfigState.Instance.SetCustomFlag("UseBigScreenMode", true);
            }
        }

        [CCScript, CCScriptHook(AllowExplicitCalls = false, Hook = ScriptHook.AfterMainMenuCreate)]
        public static void EnterBigscreenMainMenuScene(ScriptExecutionContext context)
        {
            if (context.Caller is BigscreenMainMenuController)
                return;

            if (ConfigState.Instance.HasCustomFlag("UseBigScreenMode"))
            {
                SceneManager.LoadScene("BigscreenMainMenuScene");
            }
        }

        [CCScript, CCScriptHook(AllowExplicitCalls = false, Hook = ScriptHook.AfterIGUIMenuCreate)]
        public static void InjectBigscreenIngameMenuController()
        {
            if (!ConfigState.Instance.HasCustomFlag("UseBigScreenMode"))
                return;

            if (CoreUtils.GetUIRoot() != null && CoreUtils.GetUIRoot().GetComponentInChildren<BigscreenIngameMenuController>() != null)
                return;

            UnityEngine.Object.Instantiate(CoreUtils.LoadResource<GameObject>("Modules/Bigscreen/BigscreenIngameMenu"), CoreUtils.GetUIRoot());
        }

    }
}