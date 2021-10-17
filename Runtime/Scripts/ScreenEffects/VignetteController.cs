using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using DG.Tweening;

namespace Chroma.Rendering.ScreenEffects
{
    public class VignetteController : MonoBehaviour
    {
        #region Editor variables
        [SerializeField]
        bool useVignette = false;

        [SerializeField, Range(0f, 1f), Tooltip("Target amount of vignetting on screen.")]
        float vignetteIntensity = 0.75f;

        [SerializeField, Range(0f, 1f), Tooltip("Smoothness of vignette borders.")]
        float vignetteSmoothness = 0.7f;

        [SerializeField, Min(0f)]
        float duration = 0.5f;

        [SerializeField]
        Volume volume = null;

        [SerializeField]
        Ease fadeInEase = Ease.Linear;

        [SerializeField]
        Ease fadeOutEase = Ease.Linear;
        #endregion

        Vignette _vignette = null;


        private void Awake()
        {
            if (volume.profile.TryGet(out Vignette vignette))
                this._vignette = vignette;
        }

        public void FadeIn()
        {
            if (!useVignette || _vignette == null) return;

            // Kill previous fade and fade to vignetteIntensity in time relative to current vignette intensity
            DOTween.Kill("moveVignette");
            float dur = duration * ((vignetteIntensity - _vignette.intensity.value) / vignetteIntensity);
            DOTween.To(() => _vignette.intensity.value, SetVignetteIntensity, vignetteIntensity, dur)
                .SetId("moveVignette")
                .SetEase(fadeInEase);
        }

        public void FadeOut()
        {
            if (!useVignette || _vignette == null) return;

            // Kill previous fade and fade to clear in time relative to current vignette intensity
            DOTween.Kill("moveVignette");
            float dur = duration * (_vignette.intensity.value / vignetteIntensity);
            DOTween.To(() => _vignette.intensity.value, SetVignetteIntensity, 0f, dur)
                .SetId("moveVignette")
                .SetEase(fadeOutEase);
        }

        private void SetVignetteIntensity(float value) => _vignette.intensity.Override(value);

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_vignette)
                _vignette.smoothness.Override(vignetteSmoothness);
        }
#endif
    }
}
