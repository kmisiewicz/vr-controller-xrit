using UnityEngine;
using UnityEngine.Rendering.Universal;
using DG.Tweening;

namespace Chroma.Rendering.ScreenEffects
{
    public class ScreenFade : MonoBehaviour
    {
        public ForwardRendererData rendererData = null;

        [SerializeField] Ease _FadeInEase = Ease.Linear;
        [SerializeField] Ease _FadeOutEase = Ease.Linear;

        [SerializeField, Min(0f)] float _DefaultFadeTime = 0.5f;
        public float DefaultFadeTime => _DefaultFadeTime;

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
            float duration = fadeTime > 0f ? fadeTime : _DefaultFadeTime;
            _fadeMaterial.DOFloat(1f, "_Alpha", duration).SetEase(_FadeInEase);
            return duration;
        }

        public float FadeOut(float fadeTime = 0f)
        {
            // Fade to clear
            float duration = fadeTime > 0f ? fadeTime : _DefaultFadeTime;
            _fadeMaterial.DOFloat(0f, "_Alpha", duration).SetEase(_FadeOutEase);
            return duration;
        }
    }
}