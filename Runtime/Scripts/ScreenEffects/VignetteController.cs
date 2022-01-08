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
        bool _UseVignette = false;

        [SerializeField, Range(0f, 1f), Tooltip("Target amount of vignetting on screen.")]
        float _VignetteIntensity = 0.75f;

        [SerializeField, Range(0f, 1f), Tooltip("Smoothness of vignette borders.")]
        float _VignetteSmoothness = 0.7f;

        [SerializeField, Min(0f)]
        float _Duration = 0.5f;

        [SerializeField]
        Volume _Volume = null;

        [SerializeField]
        Ease _FadeInEase = Ease.Linear;

        [SerializeField]
        Ease _FadeOutEase = Ease.Linear;
        #endregion

        Vignette _vignette = null;
        const string DOTWEEN_ID = "moveVignette";


        private void Awake()
        {
            if (_Volume.profile.TryGet(out Vignette vignette))
                this._vignette = vignette;
        }

        public void FadeIn()
        {
            if (!_UseVignette || _vignette == null) return;

            // Kill previous fade and fade to vignetteIntensity in time relative to current vignette intensity
            DOTween.Kill(DOTWEEN_ID);
            float dur = _Duration * ((_VignetteIntensity - _vignette.intensity.value) / _VignetteIntensity);
            DOTween.To(() => _vignette.intensity.value, SetVignetteIntensity, _VignetteIntensity, dur)
                .SetId(DOTWEEN_ID)
                .SetEase(_FadeInEase);
        }

        public void FadeOut()
        {
            if (!_UseVignette || _vignette == null) return;

            // Kill previous fade and fade to clear in time relative to current vignette intensity
            DOTween.Kill(DOTWEEN_ID);
            float dur = _Duration * (_vignette.intensity.value / _VignetteIntensity);
            DOTween.To(() => _vignette.intensity.value, SetVignetteIntensity, 0f, dur)
                .SetId(DOTWEEN_ID)
                .SetEase(_FadeOutEase);
        }

        private void SetVignetteIntensity(float value) => _vignette.intensity.Override(value);

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_vignette)
                _vignette.smoothness.Override(_VignetteSmoothness);
        }
#endif
    }
}
