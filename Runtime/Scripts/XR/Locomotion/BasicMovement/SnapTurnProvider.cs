using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Inputs;
using Chroma.Rendering.ScreenEffects;

namespace Chroma.XR.Locomotion
{
    public class SnapTurnProvider : ActionBasedSnapTurnProvider
    {
        [SerializeField] bool leftHandInput = true;
        [SerializeField] bool rightHandInput = true;

        [SerializeField] bool useBlinkLeftRight = true;
        [SerializeField] bool useBlinkTurnAround = true;

        [SerializeField, Min(0f)] float fadeInTime = 0.2f;
        [SerializeField, Min(0f)] float fadeOutTime = 0.2f;
        [SerializeField] ScreenFade screenFade = null;

        [SerializeField] bool leftRightFadeInBlockMovement = false;
        [SerializeField] bool leftRightFadeOutBlockMovement = false;
        [SerializeField] bool turnAroundFadeInBlockMovement = false;
        [SerializeField] bool turnAroundFadeOutBlockMovement = false;


        bool _useBlink = false;
        bool _fadeInBlockMovement = false;
        bool _fadeOutBlockMovement = false;
        float _currentTurnAmount = 0f;
        float _timeStarted = 0f;


        private new void Awake()
        {
            base.Awake();
            leftHandSnapTurnAction.action.Enable();
            rightHandSnapTurnAction.action.Enable();
        }

        protected void Start()
        {
            if (!screenFade)
            {
                useBlinkLeftRight = false;
                useBlinkTurnAround = false;
            }
        }

        protected new void Update()
        {
            // Wait for a certain amount of time before allowing another turn.
            if (_timeStarted > 0f && (_timeStarted + debounceTime < Time.time))
            {
                _timeStarted = 0f;
                return;
            }

            var input = ReadInput();
            var amount = GetTurnAmount(input);
            if (Mathf.Abs(amount) > 0f)
                StartTurn(amount, input);
        }

        private IEnumerator TurnSequence()
        {
            if (_fadeInBlockMovement)
                while (!BeginLocomotion()) 
                    yield return null;

            // Fade to black
            if (_useBlink)
            {
                float fadeInDuration = fadeInTime > 0f ? screenFade.FadeIn(fadeInTime) : screenFade.FadeIn();
                yield return new WaitForSeconds(fadeInDuration);
            }

            if (!_fadeInBlockMovement)
                while (!BeginLocomotion()) 
                    yield return null;

            Turn();

            if (!_fadeOutBlockMovement) 
                EndLocomotion();

            // Fade to clear
            if (_useBlink)
            {
                float fadeOutDuration = fadeOutTime > 0f ? screenFade.FadeOut(fadeOutTime) : screenFade.FadeOut();
                yield return new WaitForSeconds(fadeOutDuration);
            }

            if (_fadeOutBlockMovement) 
                EndLocomotion();
        }

        private void Turn()
        {
            var xrRig = system.xrRig;
            if (xrRig == null) 
                return;

            xrRig.RotateAroundCameraUsingRigUp(_currentTurnAmount);

            _currentTurnAmount = 0f;
        }

        /// <summary>Determines whether screen should fade during turn for the given 
        /// <paramref name="input"/> vector basing on useBlink_ fields.</summary>
        /// <param name="input">Input vector, such as from a thumbstick.</param>
        /// <returns>Returns <see langword="true"/> if screen should fade during turn.</returns>
        protected bool ShouldBlink(Vector2 input)
        {
            if (input == Vector2.zero)
                return false;

            var cardinal = CardinalUtility.GetNearestCardinal(input);
            switch (cardinal)
            {
                case Cardinal.North:
                    break;
                case Cardinal.South:
                    if (turnAroundFadeInBlockMovement) _fadeInBlockMovement = true;
                    else _fadeInBlockMovement = false;
                    if (turnAroundFadeOutBlockMovement) _fadeOutBlockMovement = true;
                    else _fadeOutBlockMovement = false;
                    if (useBlinkTurnAround) return true;
                    break;
                case Cardinal.East:
                    if (leftRightFadeInBlockMovement) _fadeInBlockMovement = true;
                    else _fadeInBlockMovement = false;
                    if (leftRightFadeOutBlockMovement) _fadeOutBlockMovement = true;
                    else _fadeOutBlockMovement = false;
                    if (useBlinkLeftRight) return true;
                    break;
                case Cardinal.West:
                    if (leftRightFadeInBlockMovement) _fadeInBlockMovement = true;
                    else _fadeInBlockMovement = false;
                    if (leftRightFadeOutBlockMovement) _fadeOutBlockMovement = true;
                    else _fadeOutBlockMovement = false;
                    if (useBlinkLeftRight) return true;
                    break;
                default:
                    Assert.IsTrue(false, $"Unhandled {nameof(Cardinal)}={cardinal}");
                    break;
            }

            return false;
        }

        /// <summary>Begins turning locomotion.</summary>
        /// <param name="amount">Amount to turn.</param>
        protected void StartTurn(float amount, Vector2 input)
        {
            if (_timeStarted > 0f)
                return;

            if (!CanBeginLocomotion())
                return;

            _timeStarted = Time.time;
            _currentTurnAmount = amount;
            _useBlink = ShouldBlink(input);
            StartCoroutine(TurnSequence());
        }

        protected override Vector2 ReadInput()
        {
            var leftHandValue = leftHandInput ? leftHandSnapTurnAction.action?.ReadValue<Vector2>() ?? Vector2.zero : Vector2.zero;
            var rightHandValue = rightHandInput ? rightHandSnapTurnAction.action?.ReadValue<Vector2>() ?? Vector2.zero : Vector2.zero;

            return leftHandValue + rightHandValue;
        }

        public void EnableLeftHandInput(bool enable) => leftHandInput = enable;

        public void EnableRightHandInput(bool enable) => rightHandInput = enable;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (screenFade == null)
                screenFade = FindObjectOfType<ScreenFade>();

            if (_useBlink && screenFade != null)
            {
                float fadesTime = fadeInTime > 0f ? fadeInTime : screenFade.DefaultFadeTime
                    + fadeOutTime > 0f ? fadeOutTime : screenFade.DefaultFadeTime;
                debounceTime = Mathf.Max(debounceTime, fadesTime);
            }
        }
    }
#endif
}
