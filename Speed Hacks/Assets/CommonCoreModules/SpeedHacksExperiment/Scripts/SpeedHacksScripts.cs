using CommonCore.Config;
using CommonCore.Scripting;
using CommonCore.World;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

namespace CommonCore.Experimental.SpeedHacks
{

    internal class SpeedHacksOptions
    {
        [DisplayName("Disable Shadows")] //we will probably switch from DisplayName to a custom OptionDisplay attribute to support other things
        public bool DisableShadows { get; set; } = false;
        [DisplayName("Disable Lights")]
        public bool DisableLights { get; set; } = false;
        [DisplayName("Disable Detail Objects")]
        public bool DisableDetailObjects { get; set; } = false;
        [DisplayName("Disable Background Objects")]
        public bool DisableBackgroundObjects { get; set; } = false;
        [DisplayName("Reduce Terrain Detail")]
        public bool ReduceTerrainDetails { get; set; } = false;

    }

    internal static class SpeedHacksExtensions
    {
        public static SpeedHacksOptions GetSpeedHacksOptions(this ConfigState configState)
        {
            configState.AddCustomVarIfNotExists(nameof(SpeedHacksOptions), () => new SpeedHacksOptions());

            return configState.CustomConfigVars[nameof(SpeedHacksOptions)] as SpeedHacksOptions;
        }
    }

    public static class SpeedHacksScripts
    {
        [CCScript, CCScriptHook(Hook = ScriptHook.AfterModulesLoaded)]
        private static void RegisterConfigPanel()
        {
            ConfigModule.Instance.RegisterConfigPanel("SpeedHacksOptionsPanel", 1100, CoreUtils.LoadResource<GameObject>("Modules/SpeedHacksExperiment/SpeedHacksOptionsPanel"));
        }

        [CCScript, CCScriptHook(Hook = ScriptHook.AfterSceneLoad)]
        private static void ApplySpeedHacksOnSceneLoad()
        {
            Debug.Log("[SpeedHacks] Applying speed hacks...");

            var options = ConfigState.Instance.GetSpeedHacksOptions();

            if (options.DisableLights)
            {
                foreach(var light in CoreUtils.GetWorldRoot().GetComponentsInChildren<Light>())
                {
                    var el = light.GetComponent<SpeedHackExemptLight>();
                    if (el != null && el.AlwaysShowLight)
                        continue;

                    if (light.GetComponentInParent<BaseController>() != null)
                        continue;

                    light.enabled = false;
                }

                var lo = CoreUtils.GetWorldRoot().GetComponent<SpeedHackAmbientLightOverride>();
                if(lo != null)
                {                    
                    RenderSettings.ambientIntensity = lo.AmbientIntensity;
                    if(lo.OverrideColor)
                    {
                        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
                        RenderSettings.ambientLight = lo.AmbientColor;
                    }
                }
            }

            if (options.DisableShadows)
            {
                foreach (var light in CoreUtils.GetWorldRoot().GetComponentsInChildren<Light>())
                {
                    var el = light.GetComponent<SpeedHackExemptLight>();
                    if (el != null && el.AlwaysShowShadows)
                        continue;

                    if (light.GetComponentInParent<BaseController>() != null)
                        continue;

                    light.shadows = LightShadows.None;
                }
            }

            if (options.DisableBackgroundObjects)
            {
                foreach (var s in CoreUtils.GetWorldRoot().GetComponentsInChildren<SpeedHackBackgroundObject>(true))
                {
                    s.gameObject.SetActive(false);
                }
            }

            if (options.DisableDetailObjects)
            {
                foreach (var s in CoreUtils.GetWorldRoot().GetComponentsInChildren<SpeedHackDetailObject>(true))
                {
                    s.gameObject.SetActive(false);
                }
            }

            if(options.ReduceTerrainDetails)
            {
                foreach(var terrain in CoreUtils.GetWorldRoot().GetComponentsInChildren<Terrain>(true))
                {
                    terrain.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                    terrain.detailObjectDensity /= 2f;
                    terrain.detailObjectDistance /= 2f;
                    terrain.treeBillboardDistance /= 2f;
                    terrain.treeDistance /= 2f;
                    terrain.treeMaximumFullLODCount /= 2;
                }
            }

            Debug.Log("...done!");

        }

        //not available until we add "pipeline" hooks (OnEntitySpawned, OnEffectSpawned)
        /*
        [CCScript, CCScriptHook(Hook = ScriptHook.)]
        private static void ApplySpeedHacksOnEntitySpawn()
        {

        }
        */
    }
}