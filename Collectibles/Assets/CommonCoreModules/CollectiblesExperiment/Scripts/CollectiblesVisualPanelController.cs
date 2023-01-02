using CommonCore.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CommonCore.Experimental.Collectibles
{
    /// <summary>
    /// Controller for the panel that actually displays the collectible visual
    /// </summary>
    public class CollectiblesVisualPanelController : MonoBehaviour
    {
        [SerializeField]
        private AspectRatioFitter Fitter;
        [SerializeField]
        private Image SpriteView;
        [SerializeField]
        private RawImage TextureView;
        [SerializeField]
        private RectTransform PrefabContainer;
        [SerializeField]
        private RectTransform TextContainer;
        [SerializeField]
        private Text TextView;
        

        public void LoadVisual(UnityEngine.Object visual)
        {
            if(visual is GameObject prefab)
            {
                Fitter.gameObject.SetActive(false);
                PrefabContainer.gameObject.SetActive(true);
                GameObject.Instantiate(prefab, PrefabContainer);
            }
            else if(visual is Sprite sprite)
            {
                TextureView.gameObject.SetActive(false);
                SpriteView.gameObject.SetActive(true);
                SpriteView.sprite = sprite;
                Fitter.aspectRatio = sprite.bounds.size.x / sprite.bounds.size.y;
            }
            else if(visual is Texture2D texture)
            {
                TextureView.gameObject.SetActive(true);
                SpriteView.gameObject.SetActive(false);
                TextureView.texture = texture;
                Fitter.aspectRatio = texture.width / texture.height;
            }
            else if(visual is TextAsset text)
            {
                Fitter.gameObject.SetActive(false);
                TextView.text = text.text;
                TextContainer.gameObject.SetActive(true);
                CCBase.GetModule<UIModule>().ApplyThemeRecurse(TextContainer);
            }
            else
            {
                Debug.LogWarning($"[CollectiblesVisualPanelController] can't handle visual \"{visual?.name}\" of type {visual?.GetType()?.Name}");
            }
        }
    }
}