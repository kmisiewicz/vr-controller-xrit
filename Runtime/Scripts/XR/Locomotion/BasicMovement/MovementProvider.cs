using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Inputs;
using Chroma.Rendering.ScreenEffects;
using Chroma.Utility;
using Chroma.Utility.Attributes;

namespace Chroma.XR.Locomotion
{
    /// <summary>Substitute class for <see cref="ActionBasedContinuousMoveProvider"/> for a setup
    /// with <see cref="Rigidbody"/> + head collider. Uses a lot of code from mentioned class.</summary>
    public class MovementProvider : LocomotionProvider
    {
        /// <summary>Sets which orientation the forward direction of continuous movement is relative to.</summary>
        public enum MoveForwardSource
        {
            /// <summary>Use to continuously move in a direction based on the head orientation.</summary>
            Head,
            /// <summary>Use to continuously move in a direction based on the left hand orientation.</summary>
            LeftHand,
            /// <summary>Use to continuously move in a direction based on the right hand orientation.</summary>
            RightHand,
        }


        [SerializeField, Tooltip("The speed, in units per second, to move forward.")]
        float moveSpeed = 1f;
        /// <summary>The speed, in units per second, to move forward.</summary>
        public float MoveSpeed
        {
            get => moveSpeed;
            set => moveSpeed = value;
        }

        [SerializeField, Tooltip("Controls acceleration speed.")]
        float maxVelocityChange = 10.0f;
        /// <summary>Controls acceleration speed.</summary>
        public float MaxVelocityChange
        {
            get => maxVelocityChange;
            set => maxVelocityChange = value;
        }

        [SerializeField, Tooltip("Controls whether to enable strafing (sideways movement).")]
        bool enableStrafe = true;
        /// <summary>Controls whether to enable strafing (sideways movement).</summary>
        public bool EnableStrafe
        {
            get => enableStrafe;
            set => enableStrafe = value;
        }

        [SerializeField, Tooltip("Reference to Vignette Controller.")]
        VignetteController vignette = null;

        [SerializeField, Tooltip("Controls whether to dim screen edges while moving (may help with motion sickness).")]
        bool useVignette = true;
        /// <summary>Controls whether to dim screen edges while moving (may help with motion sickness).</summary>
        public bool UseVignette
        {
            get => useVignette;
            set => useVignette = value;
        }

        [SerializeField]
        MoveForwardSource forwardSource = MoveForwardSource.Head;
        /// <summary>Controls which transform to use as forward source.</summary>
        public MoveForwardSource ForwardSource
        {
            get => forwardSource;
            set => forwardSource = value;
        }

        [SerializeField]
        EnumNamedArray<Transform> forwardSources = new EnumNamedArray<Transform>(typeof(MoveForwardSource));

        [SerializeField, OnValueChanged("UpdateLeftHandInput")] 
        bool leftHandInput = true;

        [SerializeField, OnValueChanged("UpdateRightHandInput")] 
        bool rightHandInput = true;

        [SerializeField, Tooltip("The Input System Action that will be used to read Move data from the left controller." +
            " Must be a Value Vector2 Control.")]
        InputActionProperty leftHandMoveAction;
        /// <summary>The Input System Action that will be used to read Move data from the left controller.
        /// Must be a <see cref="InputActionType.Value"/> <see cref="Vector2Control"/> Control.</summary>
        public InputActionProperty LeftHandMoveAction
        {
            get => leftHandMoveAction;
            set => SetInputActionProperty(ref leftHandMoveAction, value);
        }

        [SerializeField, Tooltip("The Input System Action that will be used to read Move data from the right controller." +
            " Must be a Value Vector2 Control.")]
        InputActionProperty rightHandMoveAction;
        /// <summary>The Input System Action that will be used to read Move data from the right controller.
        /// Must be a <see cref="InputActionType.Value"/> <see cref="Vector2Control"/> Control.</summary>
        public InputActionProperty RightHandMoveAction
        {
            get => rightHandMoveAction;
            set => SetInputActionProperty(ref rightHandMoveAction, value);
        }

        [SerializeField, Tooltip("Player's main Rigidbody.")]
        Rigidbody body;


        /// <summary>This event will be called when player <see cref="Rigidbody"/> starts moving.</summary>
        public UnityEvent startedMoving;
        /// <summary>This event will be called when player <see cref="Rigidbody"/> stops moving.</summary>
        public UnityEvent finishedMoving;

        public bool IsMoving { get; private set; } = false;


        bool _wasMoving = false;
        Vector2 _input = Vector2.zero;


        #region Methods

        private new void Awake()
        {
            base.Awake();
            if (body == null)
                body = GetComponentInParent<Rigidbody>();
            body.freezeRotation = true;

            EnableLeftHandInput(leftHandInput);
            EnableRightHandInput(rightHandInput);
        }

        protected void OnEnable()
        {
            leftHandMoveAction.EnableDirectAction();
            rightHandMoveAction.EnableDirectAction();
        }

        protected void OnDisable()
        {
            leftHandMoveAction.DisableDirectAction();
            rightHandMoveAction.DisableDirectAction();
        }

        private void Update()
        {
            _input = ReadInput();
        }

        private void FixedUpdate()
        {
            var xrRig = system.xrRig;
            if (xrRig == null)
                return;

            var velocityChange = ComputeVelocityChange(_input, out Vector3 targetVelocity);
            UpdateIsMoving(targetVelocity);
            MoveRig(velocityChange);
        }

        private void MoveRig(Vector3 velocityChange)
        {
            if (CanBeginLocomotion() && BeginLocomotion())
            {
                body.AddForce(velocityChange, ForceMode.VelocityChange);
                EndLocomotion();
            }
        }

        protected virtual Vector2 ReadInput()
        {
            var leftHandValue = leftHandMoveAction.action?.ReadValue<Vector2>() ?? Vector2.zero;
            var rightHandValue = rightHandMoveAction.action?.ReadValue<Vector2>() ?? Vector2.zero;
            return leftHandValue + rightHandValue;
        }

        /// <summary>Calculates velocity change for <see cref="Rigidbody.AddForce"/>.
        /// Similar to <see cref="ContinuousMoveProviderBase.ComputeDesiredMove(Vector2)"/>, 
        /// idea from <see href="http://wiki.unity3d.com/index.php/RigidbodyFPSWalker">here</see></summary>
        /// <param name="input">Input vector from controller</param>
        protected Vector3 ComputeVelocityChange(Vector2 input, out Vector3 targetVelocity)
        {
            targetVelocity = Vector3.zero;

            if (input == Vector2.zero)
                return Vector3.zero;

            var xrRig = system.xrRig;
            if (xrRig == null)
                return Vector3.zero;

            // Clamps the magnitude of the input direction [-1, 1] to prevent faster speed when moving diagonally,
            // while still allowing for analog input to move slower (which would be lost if simply normalizing).
            var inputMove = Vector3.ClampMagnitude(new Vector3(enableStrafe ? input.x : 0f, 0f, input.y), 1f);

            var rigTransform = xrRig.rig.transform;
            var rigUp = rigTransform.up;

            // Determine frame of reference for what the input direction is relative to
            var forwardSourceTransform = forwardSources[forwardSource] == null ?
                xrRig.cameraGameObject.transform : forwardSources[forwardSource];
            var inputForwardInWorldSpace = forwardSourceTransform.forward;
            if (Mathf.Approximately(Mathf.Abs(Vector3.Dot(inputForwardInWorldSpace, rigUp)), 1f))
            {
                // When the input forward direction is parallel with the rig normal,
                // it will probably feel better for the player to move along the same direction
                // as if they tilted forward or up some rather than moving in the rig forward direction.
                // It also will probably be a better experience to at least move in a direction
                // rather than stopping if the head/controller is oriented such that it is perpendicular with the rig.
                inputForwardInWorldSpace = -forwardSourceTransform.up;
            }

            var inputForwardProjectedInWorldSpace = Vector3.ProjectOnPlane(inputForwardInWorldSpace, rigUp);
            var forwardRotation = Quaternion.FromToRotation(rigTransform.forward, inputForwardProjectedInWorldSpace);

            var velocityInRigSpace = forwardRotation * inputMove * moveSpeed;
            targetVelocity = rigTransform.TransformDirection(velocityInRigSpace);

            Vector3 velocity = body.velocity;
            Vector3 velocityChange = (targetVelocity - velocity);
            velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
            velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
            velocityChange.y = 0;

            return velocityChange;
        }

        private void UpdateIsMoving(Vector3 targetVelocity)
        {
            IsMoving = targetVelocity != Vector3.zero;
            if (IsMoving != _wasMoving)
            {
                if (IsMoving)
                {
                    if (useVignette && vignette)
                        vignette.FadeIn();
                    startedMoving?.Invoke();
                }
                else
                {
                    if (useVignette && vignette)
                        vignette.FadeOut();
                    finishedMoving?.Invoke();
                }
            }
            _wasMoving = IsMoving;
        }

        private void UpdateLeftHandInput() => EnableLeftHandInput(leftHandInput);

        public void EnableLeftHandInput(bool enable)
        {
            if (leftHandMoveAction.action != null)
            {
                if (enable)
                    leftHandMoveAction.action.Enable();
                else
                    leftHandMoveAction.action.Disable();

                leftHandInput = enable;
            }
        }

        private void UpdateRightHandInput() => EnableRightHandInput(rightHandInput);

        public void EnableRightHandInput(bool enable)
        {
            if (rightHandMoveAction.action != null)
            {
                if (enable)
                    rightHandMoveAction.action.Enable();
                else
                    rightHandMoveAction.action.Disable();

                rightHandInput = enable;
            }
        }

        protected void SetInputActionProperty(ref InputActionProperty property, InputActionProperty value)
        {
            if (Application.isPlaying)
                property.DisableDirectAction();

            property = value;

            if (Application.isPlaying && isActiveAndEnabled)
                property.EnableDirectAction();
        }

        #endregion
    }
}