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
        #region Editor fields
        [SerializeField] bool _LeftHandInput = true;
        [SerializeField] bool _RightHandInput = true;

        [SerializeField] bool _UseBlinkLeftRight = true;
        [SerializeField] bool _UseBlinkTurnAround = true;

        [SerializeField, Min(0f)] float _FadeInTime = 0.2f;
        [SerializeField, Min(0f)] float _FadeOutTime = 0.2f;
        [SerializeField] ScreenFade _ScreenFade = null;

        [SerializeField] bool _LeftRightFadeInBlockMovement = false;
        [SerializeField] bool _LeftRightFadeOutBlockMovement = false;
        [SerializeField] bool _TurnAroundFadeInBlockMovement = false;
        [SerializeField] bool _TurnAroundFadeOutBlockMovement = false;
        #endregion

        #region Private fields
        bool _useBlink = false;
        bool _fadeInBlockMovement = false;
        bool _fadeOutBlockMovement = false;
        float _currentTurnAmount = 0f;
        float _timeStarted = 0f;
        #endregion

        #region Unity methods
        // TODO: Should work without this
        //private new void Awake()
        //{
        //    base.Awake();
        //    leftHandSnapTurnAction.action.Enable();
        //    rightHandSnapTurnAction.action.Enable();
        //}

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
        #endregion

        #region Public methods
        public void EnableLeftHandInput(bool enable) => _LeftHandInput = enable;

        public void EnableRightHandInput(bool enable) => _RightHandInput = enable;
        #endregion

        #region Private and protected methods
        private IEnumerator TurnSequence()
        {
            if (_fadeInBlockMovement)
                while (!BeginLocomotion()) 
                    yield return null;

            // Fade to black
            if (_useBlink)
            {
                float fadeInDuration = _ScreenFade.FadeIn(_FadeInTime);
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
                float fadeOutDuration = _ScreenFade.FadeOut(_FadeOutTime);
                yield return new WaitForSeconds(fadeOutDuration);
            }

            if (_fadeOutBlockMovement) 
                EndLocomotion();
        }

        private void Turn()
        {
            var xrOrigin = system.xrOrigin;
            if (xrOrigin == null) 
                return;

            xrOrigin.RotateAroundCameraUsingOriginUp(_currentTurnAmount);

            _currentTurnAmount = 0f;
        }

        /// <summary>Determines whether screen should fade during turn for the given 
        /// <paramref name="input"/> vector basing on useBlink_ fields.</summary>
        /// <param name="input">Input vector, such as from a thumbstick.</param>
        /// <returns>Returns <see langword="true"/> if screen should fade during turn.</returns>
        protected bool ShouldBlink(Vector2 input)
        {
            if (input == Vector2.zero || _ScreenFade == null)
                return false;

            var cardinal = CardinalUtility.GetNearestCardinal(input);
            switch (cardinal)
            {
                case Cardinal.North:
                    break;
                case Cardinal.South:
                    if (_TurnAroundFadeInBlockMovement) _fadeInBlockMovement = true;
                    else _fadeInBlockMovement = false;
                    if (_TurnAroundFadeOutBlockMovement) _fadeOutBlockMovement = true;
                    else _fadeOutBlockMovement = false;
                    if (_UseBlinkTurnAround) return true;
                    break;
                case Cardinal.East:
                    if (_LeftRightFadeInBlockMovement) _fadeInBlockMovement = true;
                    else _fadeInBlockMovement = false;
                    if (_LeftRightFadeOutBlockMovement) _fadeOutBlockMovement = true;
                    else _fadeOutBlockMovement = false;
                    if (_UseBlinkLeftRight) return true;
                    break;
                case Cardinal.West:
                    if (_LeftRightFadeInBlockMovement) _fadeInBlockMovement = true;
                    else _fadeInBlockMovement = false;
                    if (_LeftRightFadeOutBlockMovement) _fadeOutBlockMovement = true;
                    else _fadeOutBlockMovement = false;
                    if (_UseBlinkLeftRight) return true;
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
            var leftHandValue = _LeftHandInput ? leftHandSnapTurnAction.action?.ReadValue<Vector2>() ?? Vector2.zero : Vector2.zero;
            var rightHandValue = _RightHandInput ? rightHandSnapTurnAction.action?.ReadValue<Vector2>() ?? Vector2.zero : Vector2.zero;
            return leftHandValue + rightHandValue;
        }
        #endregion

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_ScreenFade == null)
                _ScreenFade = FindObjectOfType<ScreenFade>();

            if (_useBlink && _ScreenFade != null)
            {
                float fadesTime = _FadeInTime > 0f ? _FadeInTime : _ScreenFade.DefaultFadeTime
                    + _FadeOutTime > 0f ? _FadeOutTime : _ScreenFade.DefaultFadeTime;
                debounceTime = Mathf.Max(debounceTime, fadesTime);
            }
        }
    }
#endif
}
