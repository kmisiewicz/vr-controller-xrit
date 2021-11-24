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
        [SerializeField, OnValueChanged("UpdateLeftHandInput")] bool leftHandInput = true;
        [SerializeField, OnValueChanged("UpdateRightHandInput")] bool rightHandInput = true;

        [SerializeField] bool useBlink = true;
        [SerializeField] bool fadeInBlockMovement = false;
        [SerializeField] bool fadeOutBlockMovement = false;

        [SerializeField, Min(0f)]
        [Tooltip("Duration of the fade in (to black). If set to 0, the default value from ScreenFader will be used.")]
        float fadeInTime = 0.2f;

        [SerializeField, Min(0f)]
        [Tooltip("Duration of the fade out (to clear). If set to 0, the default value from ScreenFader will be used.")]
        float fadeOutTime = 0.2f;

        [SerializeField] ScreenFade screenFade = null;

        [SerializeField] Transform bodyRoot = null;

        [SerializeField] TeleportationRayToggler leftRayToggler = null;
        [SerializeField] TeleportationRayToggler rightRayToggler = null;


        XRRayInteractor currentRay = null;
        Rigidbody _playerRb;


        protected override void Awake()
        {
            base.Awake();
            _playerRb = bodyRoot.GetComponent<Rigidbody>();
        }

        private void Start()
        {
            EnableLeftHandInput(leftHandInput);
            EnableRightHandInput(rightHandInput);
        }

        public override bool QueueTeleportRequest(TeleportRequest teleportRequest)
        {
            if (validRequest || system.xrRig == null || !this.enabled)
                return false;

            base.QueueTeleportRequest(teleportRequest);
            StartCoroutine(TeleportSequence(currentRequest));
            return true;
        }

        protected override void Update() { }

        private IEnumerator TeleportSequence(TeleportRequest request)
        {
            bool shouldBlink = useBlink;

            if (fadeInBlockMovement)
            {
                while (!BeginLocomotion())
                    yield return null;
                _playerRb.velocity = Vector3.zero;
            }

            // Fade to black
            if (shouldBlink)
            {
                float fadeInDuration = screenFade.FadeIn(fadeInTime);
                yield return new WaitForSeconds(fadeInDuration);
            }

            if (!fadeInBlockMovement)
                while (!BeginLocomotion()) 
                    yield return null;
            _playerRb.velocity = Vector3.zero;

            Teleport(request);

            if (!fadeOutBlockMovement)
            {
                EndLocomotion();
                validRequest = false;
            }

            // Fade to clear
            if (shouldBlink)
            {
                float fadeOutDuration = screenFade.FadeOut(fadeOutTime);
                yield return new WaitForSeconds(fadeOutDuration);
            }

            if (fadeOutBlockMovement)
            {
                EndLocomotion();
                validRequest = false;
            }
        }

        private void Teleport(TeleportRequest request)
        {
            var xrRig = system.xrRig;
            if (xrRig == null)
                return;

            switch (currentRequest.matchOrientation)
            {
                case MatchOrientation.WorldSpaceUp:
                    xrRig.MatchRigUp(Vector3.up);
                    break;
                case MatchOrientation.TargetUp:
                    xrRig.MatchRigUp(currentRequest.destinationRotation * Vector3.up);
                    break;
                case MatchOrientation.TargetUpAndForward:
                    xrRig.MatchRigUpCameraForward(currentRequest.destinationRotation * Vector3.up, currentRequest.destinationRotation * Vector3.forward);
                    break;
                case MatchOrientation.None:
                    // Change nothing. Maintain current rig rotation.
                    break;
                default:
                    Assert.IsTrue(false, $"Unhandled {nameof(MatchOrientation)}={currentRequest.matchOrientation}.");
                    break;
            }

            var heightAdjustment = xrRig.rig.transform.up * xrRig.cameraInRigSpaceHeight;
            var cameraDestination = currentRequest.destinationPosition + heightAdjustment;

            bodyRoot.position = currentRequest.destinationPosition;
            xrRig.MoveCameraToWorldLocation(cameraDestination);
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

        private void UpdateLeftHandInput() => EnableLeftHandInput(leftHandInput);

        public void EnableLeftHandInput(bool enable) => leftRayToggler.enabled = leftHandInput = enable;

        private void UpdateRightHandInput() => EnableRightHandInput(rightHandInput);

        public void EnableRightHandInput(bool enable) => rightRayToggler.enabled = rightHandInput = enable;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (screenFade == null)
                screenFade = FindObjectOfType<ScreenFade>();
        }
#endif
    }
}