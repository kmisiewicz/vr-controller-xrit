using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.Events;
using Chroma.Rendering.ScreenEffects;

namespace Chroma.XR.Locomotion
{
    public class ContinuousTurnProvider : ActionBasedContinuousTurnProvider
    {
        [SerializeField] bool useVignette = true;
        [SerializeField] VignetteController vignette = null;

        [SerializeField] bool leftHandInput = true;
        [SerializeField] bool rightHandInput = true;

        /// <summary>The startedTurning action will be called on continuous movement start (after player pushes controller stick).</summary>
        public UnityEvent startedTurning;
        /// <summary>The finishedTurning action will be called on continuous movement stop (after player releases controller stick).</summary>
        public UnityEvent finishedTurning;

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
                    if (useVignette && vignette) 
                        vignette.FadeIn();
                    startedTurning?.Invoke();
                }
                else
                {
                    if (useVignette && vignette) 
                        vignette.FadeOut();
                    finishedTurning?.Invoke();
                }
            }
            _wasTurning = IsTurning;
        }

        public void EnableLeftHandInput(bool enable) => leftHandInput = enable;

        public void EnableRightHandInput(bool enable) => rightHandInput = enable;

        protected override Vector2 ReadInput()
        {
            var leftHandValue = leftHandInput ? leftHandTurnAction.action?.ReadValue<Vector2>() ?? Vector2.zero : Vector2.zero;
            var rightHandValue = rightHandInput ? rightHandTurnAction.action?.ReadValue<Vector2>() ?? Vector2.zero : Vector2.zero;

            return leftHandValue + rightHandValue;
        }
    }
}
