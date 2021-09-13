using CommonCore;
using CommonCore.Config;
using CommonCore.Scripting;
using CommonCore.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace CommonCore.Experimental.SpeedHacks
{

    /// <summary>
    /// Panel controller for the Speed Hacks options subpanel
    /// </summary>
    /// <remarks>
    /// <para>Doing some early/experimental shitty-MVC stuff here</para>
    /// </remarks>
    public class SpeedHacksOptionsPanelController : ConfigSubpanelController
    {
        [SerializeField]
        private RectTransform Container = null;
        [SerializeField]
        private RectTransform Template = null;

        private bool Initialized = false;

        private void Initialize()
        {
            if (Initialized)
                return;

            var props = typeof(SpeedHacksOptions).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.PropertyType == typeof(bool))
                .ToList();

            for(int i = 0; i < props.Count; i++)
            {
                var prop = props[i];

                var titleAttr = prop.GetCustomAttribute<DisplayNameAttribute>();
                var titleString = titleAttr == null ? prop.Name : titleAttr.DisplayName;

                var item = Instantiate(Template.gameObject, Container);
                item.name = prop.Name;

                var itemBF = item.GetComponent<BackingFieldReference>();
                itemBF.Id = prop.Name;

                var itemRT = item.transform as RectTransform;
                //itemRT.SetParent(Container);
                itemRT.anchoredPosition = new Vector2(Template.anchoredPosition.x + (i % 2 == 1 ? Template.anchoredPosition.x + 580f : 0), Template.anchoredPosition.y - (40f * (i / 2)));

                var itemToggle = item.GetComponentInChildren<Toggle>();
                itemToggle.onValueChanged.AddListener(b => SignalPendingChanges(PendingChangesFlags.RequiresRestart));

                var title = itemRT.Find("Title").GetComponent<Text>();
                title.text = titleString;

            }

            Container.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Container.sizeDelta.y + (40f * (Math.Max(0, props.Count - 1 ) / 2f))); //correct?

            Template.gameObject.SetActive(false);

            Initialized = true;
        }

        public override void PaintValues()
        {
            Initialize();

            var options = ConfigState.Instance.GetSpeedHacksOptions();

            var props = typeof(SpeedHacksOptions).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.PropertyType == typeof(bool));

            foreach(var prop in props)
            {
                var go = GetFieldForProp(prop.Name);

                var value = TypeUtils.CoerceValue<bool>(prop.GetValue(options));

                go.GetComponentInChildren<Toggle>().isOn = value;
            }
            
        }

        public override void UpdateValues()
        {
            var options = ConfigState.Instance.GetSpeedHacksOptions();

            var props = typeof(SpeedHacksOptions).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.PropertyType == typeof(bool));

            foreach (var prop in props)
            {
                var go = GetFieldForProp(prop.Name);

                var value = go.GetComponentInChildren<Toggle>().isOn;

                prop.SetValue(options, value);
            }
        }

        private GameObject GetFieldForProp(string fieldName)
        {
            foreach(Transform t in Container)
            {
                if (!t.gameObject.activeSelf)
                    continue;

                var bf = t.GetComponent<BackingFieldReference>();

                if (bf == null)
                    continue;

                if (bf.Id == fieldName)
                    return t.gameObject;

            }

            throw new KeyNotFoundException();
        }
    }
}