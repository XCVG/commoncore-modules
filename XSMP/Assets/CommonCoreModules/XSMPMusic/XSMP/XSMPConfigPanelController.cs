using CommonCore.Config;
using CommonCore.UI;
using UnityEngine;
using UnityEngine.UI;

namespace CommonCore.XSMP
{

    public class XSMPConfigPanelController : ConfigSubpanelController
    {
        private const float UpdateInterval = 1.5f;

        [SerializeField]
        private Toggle XSMPEnableToggle = null;
        [SerializeField]
        private Toggle XSMPIngameToggle = null;
        [SerializeField]
        private Text XSMPStatusText = null;

        private float Elapsed = 0;

        public override void PaintValues()
        {
            XSMPEnableToggle.SetIsOnWithoutNotify(ConfigState.Instance.CustomConfigFlags.Contains("XSMPEnabled"));
            XSMPIngameToggle.SetIsOnWithoutNotify(ConfigState.Instance.CustomConfigFlags.Contains("XSMPUseIngame"));
        }

        public override void UpdateValues()
        {
            if (XSMPEnableToggle.isOn && !ConfigState.Instance.CustomConfigFlags.Contains("XSMPEnabled"))
                ConfigState.Instance.CustomConfigFlags.Add("XSMPEnabled");
            else if (!XSMPEnableToggle.isOn && ConfigState.Instance.CustomConfigFlags.Contains("XSMPEnabled"))
                ConfigState.Instance.CustomConfigFlags.Remove("XSMPEnabled");

            if (XSMPIngameToggle.isOn && !ConfigState.Instance.CustomConfigFlags.Contains("XSMPUseIngame"))
                ConfigState.Instance.CustomConfigFlags.Add("XSMPUseIngame");
            else if (!XSMPIngameToggle.isOn && ConfigState.Instance.CustomConfigFlags.Contains("XSMPUseIngame"))
                ConfigState.Instance.CustomConfigFlags.Remove("XSMPUseIngame");

        }

        public void HandleEnableToggleChanged()
        {
            SignalPendingChanges(PendingChangesFlags.RequiresRestart);
        }

        private void Update()
        {
            Elapsed += Time.unscaledDeltaTime;
            if (Elapsed >= UpdateInterval)
            {
                UpdateStatus();
                Elapsed = 0;
            }
        }

        private void UpdateStatus()
        {
            if(XSMPModule.Instance == null || !XSMPModule.Instance.Enabled)
            {
                XSMPStatusText.text = "Module Inactive";
            }
            else
            {
                XSMPStatusText.text = XSMPModule.Instance.Status.ToString();
            }
        }
    }
}