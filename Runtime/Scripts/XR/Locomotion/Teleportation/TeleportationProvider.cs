using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.XR.Interaction.Toolkit;
using Chroma.Rendering.ScreenEffects;
using Chroma.Utility.Attributes;

namespace Chroma.XR.Locomotion
{
    public class TeleportationProvider : UnityEngine.XR.Interaction.Toolkit.TeleportationProvider
    {
        [SerializeField, OnValueChanged("UpdateLeftHandInput")] bool _LeftHandInput = true;
        [SerializeField, OnValueChanged("UpdateRightHandInput")] bool _RightHandInput = true;

        [SerializeField] bool _UseBlink = true;
        [SerializeField] bool _FadeInBlockMovement = false;
        [SerializeField] bool _FadeOutBlockMovement = false;

        [SerializeField, Min(0f)]
        [Tooltip("Duration of the fade in (to black). If set to 0, the default value from ScreenFader will be used.")]
        float _FadeInTime = 0.2f;

        [SerializeField, Min(0f)]
        [Tooltip("Duration of the fade out (to clear). If set to 0, the default value from ScreenFader will be used.")]
        float _FadeOutTime = 0.2f;

        [SerializeField] ScreenFade _ScreenFade = null;

        [SerializeField] Transform _BodyRoot = null;

        [SerializeField] TeleportationRayToggler _LeftRayToggler = null;
        [SerializeField] TeleportationRayToggler _RightRayToggler = null;


        XRRayInteractor currentRay = null;
        Rigidbody _playerRb;


        protected override void Awake()
        {
            base.Awake();
            _playerRb = _BodyRoot.GetComponent<Rigidbody>();
        }

        private void Start()
        {
            EnableLeftHandInput(_LeftHandInput);
            EnableRightHandInput(_RightHandInput);
        }

        public override bool QueueTeleportRequest(TeleportRequest teleportRequest)
        {
            if (validRequest || system.xrOrigin == null || !this.enabled)
                return false;

            base.QueueTeleportRequest(teleportRequest);
            StartCoroutine(TeleportSequence(currentRequest));
            return true;
        }

        protected override void Update() { }

        private IEnumerator TeleportSequence(TeleportRequest request)
        {
            bool shouldBlink = _UseBlink;

            if (_FadeInBlockMovement)
            {
                while (!BeginLocomotion())
                    yield return null;
                _playerRb.velocity = Vector3.zero;
            }

            // Fade to black
            if (shouldBlink)
            {
                float fadeInDuration = _ScreenFade.FadeIn(_FadeInTime);
                yield return new WaitForSeconds(fadeInDuration);
            }

            if (!_FadeInBlockMovement)
                while (!BeginLocomotion()) 
                    yield return null;
            _playerRb.velocity = Vector3.zero;

            Teleport(request);

            if (!_FadeOutBlockMovement)
            {
                EndLocomotion();
                validRequest = false;
            }

            // Fade to clear
            if (shouldBlink)
            {
                float fadeOutDuration = _ScreenFade.FadeOut(_FadeOutTime);
                yield return new WaitForSeconds(fadeOutDuration);
            }

            if (_FadeOutBlockMovement)
            {
                EndLocomotion();
                validRequest = false;
            }
        }

        private void Teleport(TeleportRequest request)
        {
            var xrOrigin = system.xrOrigin;
            if (xrOrigin == null)
                return;

            switch (currentRequest.matchOrientation)
            {
                case MatchOrientation.WorldSpaceUp:
                    xrOrigin.MatchOriginUp(Vector3.up);
                    break;
                case MatchOrientation.TargetUp:
                    xrOrigin.MatchOriginUp(currentRequest.destinationRotation * Vector3.up);
                    break;
                case MatchOrientation.TargetUpAndForward:
                    xrOrigin.MatchOriginUpCameraForward(currentRequest.destinationRotation * Vector3.up, currentRequest.destinationRotation * Vector3.forward);
                    break;
                case MatchOrientation.None:
                    // Change nothing. Maintain current rig rotation.
                    break;
                default:
                    Assert.IsTrue(false, $"Unhandled {nameof(MatchOrientation)}={currentRequest.matchOrientation}.");
                    break;
            }

            var heightAdjustment = xrOrigin.transform.up * xrOrigin.CameraInOriginSpaceHeight;
            var cameraDestination = currentRequest.destinationPosition + heightAdjustment;

            _BodyRoot.position = currentRequest.destinationPosition;
            xrOrigin.MoveCameraToWorldLocation(cameraDestination);
        }

        public bool RequestRayExclusivity(XRRayInteractor ray)
        {
            if (currentRay != null && ray != currentRay && !this.enabled)
                return false;

            currentRay = ray;
            return true;
        }

        public bool FinishRayExclusivity(XRRayInteractor ray)
        {
            if (ray != currentRay)
                return false;

            currentRay = null;
            return true;
        }

        private void UpdateLeftHandInput() => EnableLeftHandInput(_LeftHandInput);

        public void EnableLeftHandInput(bool enable) => _LeftRayToggler.enabled = _LeftHandInput = enable;

        private void UpdateRightHandInput() => EnableRightHandInput(_RightHandInput);

        public void EnableRightHandInput(bool enable) => _RightRayToggler.enabled = _RightHandInput = enable;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_ScreenFade == null)
                _ScreenFade = FindObjectOfType<ScreenFade>();
        }
#endif
    }
}