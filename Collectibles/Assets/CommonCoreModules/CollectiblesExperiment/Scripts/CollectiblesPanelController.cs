using CommonCore.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace CommonCore.Experimental.Collectibles
{

    /// <summary>
    /// Controller for the "view collectibles" panel in menus etc
    /// </summary>
    public class CollectiblesPanelController : PanelController
    {
        public bool ApplyTheme = true;

        public CollectiblesPanelShows Show = CollectiblesPanelShows.All;

        [Header("Elements"), SerializeField]
        private RectTransform Container = null;
        [SerializeField]
        private ScrollRect ScrollView = null;
        [SerializeField]
        private RectTransform ScrollViewContents = null;
        [SerializeField]
        private RectTransform ScrollViewTemplate = null;
        [SerializeField]
        private Text VisualText = null;
        [SerializeField]
        private RectTransform VisualContainer = null;

        private CollectiblesExperimentModule Module = null;
        private Color SelectedButtonColor;
        private string SelectedKey = null;

        public override void SignalInitialPaint()
        {
            base.SignalInitialPaint();

            Module = CCBase.GetModule<CollectiblesExperimentModule>();
            ResolveSelectedItemColor();
        }

        private void ResolveSelectedItemColor()
        {
            var theme = CCBase.GetModule<UIModule>().GetThemeByName(GetEffectiveTheme());
            if(theme != null)
            {
                SelectedButtonColor = theme.HighlightColor;
            }
            else
            {
                SelectedButtonColor = new Color(1,0,1,1);
            }
        }

        public override void SignalPaint()
        {
            base.SignalPaint();

            PaintList();

            if (ApplyTheme)
                ApplyThemeToElements(Container);

            CallPostRepaintHooks();
        }

        private void PaintList()
        {
            ClearList();
            VisualContainer.DestroyAllChildren();
            VisualText.text = "";
            SelectedKey = null;

            IEnumerable<CollectibleRecord> collectibles;
            switch (Show)
            {
                case CollectiblesPanelShows.OnlyInGame:
                    collectibles = Module.EnumerateAllCollectibles().Where(c => c.Type == CollectibleRecordType.InGame);
                    break;
                case CollectiblesPanelShows.OnlyPersistent:
                    collectibles = Module.EnumerateAllCollectibles().Where(c => c.Type == CollectibleRecordType.Persistent);
                    break;
                default:
                    collectibles = Module.EnumerateDistinctCollectibles();
                    break;
            }

            foreach(var collectible in collectibles)
            {
                string key = collectible.Key; //actually needed for scoping

                var go = GameObject.Instantiate(ScrollViewTemplate.gameObject, ScrollViewContents);
                var bfr = go.GetComponent<BackingFieldReference>();                
                bfr.Id = key;
                bfr.Value = collectible.Name;
                var button = go.GetComponent<Button>();
                button.onClick.AddListener(() => HandleItemSelected(go));
                var text = go.GetComponentInChildren<Text>();
                text.text = collectible.Name;

                go.name = $"ListItem_{key}";
                go.SetActive(true);
            }

        }

        private void ClearList()
        {
            foreach (Transform t in ScrollViewContents)
            {
                if (t != ScrollViewTemplate)
                    Destroy(t.gameObject);
            }
        }

        private void HandleItemSelected(GameObject item)
        {
            var bfr = item.GetComponent<BackingFieldReference>();
            string key = bfr.Id;
            //Debug.Log($"Selected {key}");

            var button = item.GetComponent<Button>();
            var defaultColors = ScrollViewTemplate.GetComponent<Button>().colors;

            foreach (var b in ScrollViewContents.GetComponentsInChildren<Button>())
            {
                var colors = b.colors;

                colors.normalColor = defaultColors.normalColor;
                colors.highlightedColor = defaultColors.highlightedColor;
                colors.pressedColor = defaultColors.pressedColor;
                colors.selectedColor = defaultColors.selectedColor;

                b.colors = colors;
            }

            {
                var colors = button.colors;

                colors.normalColor = SelectedButtonColor;
                colors.highlightedColor = SelectedButtonColor;
                colors.pressedColor = SelectedButtonColor;
                colors.selectedColor = SelectedButtonColor;

                button.colors = colors;
            }

            if(SelectedKey != key)
            {
                var visual = Module.GetVisualForCollectible(key);
                VisualContainer.DestroyAllChildren();

                VisualText.text = bfr.Value as string ?? key;

                var go = GameObject.Instantiate(CoreUtils.LoadResource<GameObject>("Modules/CollectiblesExperiment/CollectiblesVisualPanel"), VisualContainer);
                var controller = go.GetComponent<CollectiblesVisualPanelController>();
                controller.LoadVisual(visual);

                SelectedKey = key;
            }
        }

        [Serializable]
        public enum CollectiblesPanelShows
        {
            All, OnlyInGame, OnlyPersistent
        }
    }
}