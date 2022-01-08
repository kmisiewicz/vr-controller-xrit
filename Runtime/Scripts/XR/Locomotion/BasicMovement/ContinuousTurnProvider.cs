using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.Events;
using Chroma.Rendering.ScreenEffects;

namespace Chroma.XR.Locomotion
{
    public class ContinuousTurnProvider : ActionBasedContinuousTurnProvider
    {
        [SerializeField] bool _UseVignette = true;
        [SerializeField] VignetteController _Vignette = null;

        [SerializeField] bool _LeftHandInput = true;
        [SerializeField] bool _RightHandInput = true;

        /// <summary>The startedTurning action will be called on continuous movement start (after player pushes controller stick).</summary>
        public UnityEvent StartedTurning;
        /// <summary>The finishedTurning action will be called on continuous movement stop (after player releases controller stick).</summary>
        public UnityEvent FinishedTurning;

        public bool IsTurning { get; private set; } = false;

        bool _wasTurning = false;


        protected new void Update()
        {
            // Use the input amount to scale the turn speed.
            var input = ReadInput();
            var turnAmount = GetTurnAmount(input);
            UpdateIsTurning(turnAmount);
            TurnRig(turnAmount);
        }

        protected void UpdateIsTurning(float turnAmount)
        {
            IsTurning = turnAmount != 0f;
            if (IsTurning != _wasTurning)
            {
                if (IsTurning)
                {
                    if (_UseVignette && _Vignette) 
                        _Vignette.FadeIn();
                    StartedTurning?.Invoke();
                }
                else
                {
                    if (_UseVignette && _Vignette) 
                        _Vignette.FadeOut();
                    FinishedTurning?.Invoke();
                }
            }
            _wasTurning = IsTurning;
        }

        public void EnableLeftHandInput(bool enable) => _LeftHandInput = enable;

        public void EnableRightHandInput(bool enable) => _RightHandInput = enable;

        protected override Vector2 ReadInput()
        {
            var leftHandValue = _LeftHandInput ? leftHandTurnAction.action?.ReadValue<Vector2>() ?? Vector2.zero : Vector2.zero;
            var rightHandValue = _RightHandInput ? rightHandTurnAction.action?.ReadValue<Vector2>() ?? Vector2.zero : Vector2.zero;

            return leftHandValue + rightHandValue;
        }
    }
}
