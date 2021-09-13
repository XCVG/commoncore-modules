using CommonCore;
using CommonCore.GameData;
using CommonCore.State;
using CommonCore.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CommonCore.Experimental.Achievements
{

    /// <summary>
    /// Panel controller for interim achievements system
    /// </summary>
    public class AchievementsPanelController : PanelController
    {
        [SerializeField]
        private RectTransform ContentTransform = null;
        [SerializeField]
        private GameObject TemplateObject = null;

        [SerializeField]
        private bool ApplyTheme = true;

        public override void SignalPaint()
        {
            base.SignalPaint();

            ClearContent();
            PaintAchievements();
            if (ApplyTheme)
                ApplyThemeToElements(transform.GetChild(0));
        }

        private void ClearContent()
        {
            for(int i = ContentTransform.childCount - 1; i >= 0; i--)
            {
                var child = ContentTransform.GetChild(i);
                if (child.gameObject != TemplateObject)
                    Destroy(child.gameObject);
            }
        }

        private void PaintAchievements()
        {
            //var achievementData = CCBase.GetModule<GameDataModule>().Get<AchievementDefinitions>();
            var achievementData = CoreUtils.LoadJson<AchievementDefinitions>(CoreUtils.LoadResource<TextAsset>("Modules/AchievementsExperiment/AchievementDefinitions").text);

            foreach (var achievementKvp in achievementData.Achievements)
            {
                var panel = Instantiate(TemplateObject, ContentTransform);

                var nameText = panel.transform.Find("NameText").GetComponent<Text>();
                var descriptionText = panel.transform.Find("DescriptionText").GetComponent<Text>();

                nameText.text = achievementKvp.Value.Title;

                var image = panel.transform.Find("Icon").GetComponent<RawImage>();
                
                var tex = CoreUtils.LoadResource<Texture2D>("Modules/AchievementsExperiment/Icons/" + achievementKvp.Value.Icon);
                if(tex != null)
                {
                    image.texture = tex;
                }
                else
                {
                    Debug.LogWarning($"Couldn't find icon \"{achievementKvp.Value.Icon}\" for achievement \"{achievementKvp.Key}\"");
                }

                bool haveAchievement = PersistState.Instance.UnlockedAchievements.Contains(achievementKvp.Key);

                if(haveAchievement)
                {
                    descriptionText.text = achievementKvp.Value.Description;
                    //image.color = Color.white;
                    
                }
                else
                {
                    descriptionText.text = string.IsNullOrEmpty(achievementKvp.Value.Hint) ? achievementKvp.Value.Description : achievementKvp.Value.Hint;
                    image.color = new Color(0.33f, 0.33f, 0.33f, 1f);
                    nameText.color = new Color(0.75f, 0.75f, 0.75f, 1f);
                    descriptionText.color = new Color(0.75f, 0.75f, 0.75f, 1f);
                }

                panel.gameObject.SetActive(true);
            }

            TemplateObject.gameObject.SetActive(false);
        }
    }
}