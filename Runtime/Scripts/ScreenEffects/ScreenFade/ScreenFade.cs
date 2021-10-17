using UnityEngine;
using UnityEngine.Rendering.Universal;
using DG.Tweening;

namespace Chroma.Rendering.ScreenEffects
{
    public class ScreenFade : MonoBehaviour
    {
        public ForwardRendererData rendererData = null;

        [SerializeField] Ease fadeInEase = Ease.Linear;
        [SerializeField] Ease fadeOutEase = Ease.Linear;

        [SerializeField, Min(0f)] float defaultFadeTime = 0.5f;
        public float DefaultFadeTime => defaultFadeTime;

        Material _fadeMaterial = null;


        private void Start()
        {
            // Find and set the feature's material
            SetupFadeFeature();
        }

        private void SetupFadeFeature()
        {
            // Look for the screen fade feature
            ScriptableRendererFeature feature = rendererData.rendererFeatures.Find(item => item is ScreenFadeFeature);

            // Ensure it's the correct feature
            if (feature is ScreenFadeFeature screenFade)
            {
                // Duplicate material so we don't change the renderer's asset
                _fadeMaterial = Instantiate(screenFade.settings.material);
                _fadeMaterial.SetFloat("_Alpha", 0);
                screenFade.settings.runTimeMaterial = _fadeMaterial;
            }
        }

        public float FadeIn(float fadeTime = 0f)
        {
            // Fade to black
            float duration = fadeTime > 0f ? fadeTime : defaultFadeTime;
            _fadeMaterial.DOFloat(1f, "_Alpha", duration).SetEase(fadeInEase);
            return duration;
        }

        public float FadeOut(float fadeTime = 0f)
        {
            // Fade to clear
            float duration = fadeTime > 0f ? fadeTime : defaultFadeTime;
            _fadeMaterial.DOFloat(0f, "_Alpha", duration).SetEase(fadeOutEase);
            return duration;
        }
    }
}