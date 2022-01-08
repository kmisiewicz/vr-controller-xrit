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
    /// with <see cref="Rigidbody"/> + head collider.</summary>
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

        #region Editor fields
        [SerializeField, Tooltip("Maximal movement speed, in units per second.")]
        float _MoveSpeed = 1f;
        /// <summary>The speed, in units per second, to move forward.</summary>
        public float MoveSpeed
        {
            get => _MoveSpeed;
            set => _MoveSpeed = value;
        }

        [SerializeField, Tooltip("Controls acceleration speed.")]
        float _MaxVelocityChange = 10.0f;
        /// <summary>Controls acceleration speed.</summary>
        public float MaxVelocityChange
        {
            get => _MaxVelocityChange;
            set => _MaxVelocityChange = value;
        }

        [SerializeField, Tooltip("Controls whether to enable strafing (sideways movement).")]
        bool _EnableStrafe = true;
        /// <summary>Controls whether to enable strafing (sideways movement).</summary>
        public bool EnableStrafe
        {
            get => _EnableStrafe;
            set => _EnableStrafe = value;
        }

        [SerializeField, Tooltip("Reference to Vignette Controller.")]
        VignetteController _Vignette = null;

        [SerializeField, Tooltip("Controls whether to dim screen edges while moving (may help with motion sickness).")]
        bool _UseVignette = true;
        /// <summary>Controls whether to dim screen edges while moving (may help with motion sickness).</summary>
        public bool UseVignette
        {
            get => _UseVignette;
            set => _UseVignette = value;
        }

        [SerializeField, Tooltip("Controls which transform to use as forward source.")]
        MoveForwardSource _ForwardSource = MoveForwardSource.Head;
        /// <summary>Controls which transform to use as forward source.</summary>
        public MoveForwardSource ForwardSource
        {
            get => _ForwardSource;
            set => _ForwardSource = value;
        }

        [SerializeField]
        EnumNamedArray<Transform> _ForwardSources = new EnumNamedArray<Transform>(typeof(MoveForwardSource));

        [SerializeField, OnValueChanged("UpdateLeftHandInput")] 
        bool _LeftHandInput = true;

        [SerializeField, OnValueChanged("UpdateRightHandInput")] 
        bool _RightHandInput = true;

        [SerializeField, Tooltip("The Input System Action that will be used to read Move data from the left controller." +
            " Must be a Value Vector2 Control.")]
        InputActionProperty _LeftHandMoveAction;
        /// <summary>The Input System Action that will be used to read Move data from the left controller.
        /// Must be a <see cref="InputActionType.Value"/> <see cref="Vector2Control"/> Control.</summary>
        public InputActionProperty LeftHandMoveAction
        {
            get => _LeftHandMoveAction;
            set => SetInputActionProperty(ref _LeftHandMoveAction, value);
        }

        [SerializeField, Tooltip("The Input System Action that will be used to read Move data from the right controller." +
            " Must be a Value Vector2 Control.")]
        InputActionProperty _RightHandMoveAction;
        /// <summary>The Input System Action that will be used to read Move data from the right controller.
        /// Must be a <see cref="InputActionType.Value"/> <see cref="Vector2Control"/> Control.</summary>
        public InputActionProperty RightHandMoveAction
        {
            get => _RightHandMoveAction;
            set => SetInputActionProperty(ref _RightHandMoveAction, value);
        }

        [SerializeField, Tooltip("Player's main Rigidbody.")]
        Rigidbody _Body;

        [SerializeField, Tooltip("In air control multiplier")]
        float _InAirControlMultiplier = 1f;

        [SerializeField, Tooltip("Ground check script.")]
        GravityProvider _GravityProvider = null;


        /// <summary>This event will be called when player <see cref="Rigidbody"/> starts moving.</summary>
        public UnityEvent StartedMoving;
        /// <summary>This event will be called when player <see cref="Rigidbody"/> stops moving.</summary>
        public UnityEvent FinishedMoving;
        #endregion

        #region Script fields
        public bool IsMoving { get; private set; } = false;


        bool _wasMoving = false;
        Vector2 _input = Vector2.zero;
        #endregion

        #region Unity methods
        private new void Awake()
        {
            base.Awake();

            if (_Body == null)
                _Body = GetComponentInParent<Rigidbody>();
            _Body.freezeRotation = true;

            EnableLeftHandInput(_LeftHandInput);
            EnableRightHandInput(_RightHandInput);
        }

        protected void OnEnable()
        {
            _LeftHandMoveAction.EnableDirectAction();
            _RightHandMoveAction.EnableDirectAction();
        }

        protected void OnDisable()
        {
            _LeftHandMoveAction.DisableDirectAction();
            _RightHandMoveAction.DisableDirectAction();
        }

        private void Update()
        {
            _input = ReadInput();
        }

        private void FixedUpdate()
        {
            var xrOrigin = system.xrOrigin;
            if (xrOrigin == null)
                return;

            var velocityChange = ComputeVelocityChange(_input, out Vector3 targetVelocity);
            UpdateIsMoving(targetVelocity);
            MoveRig(velocityChange);
        }
        #endregion

        #region Public methods
        public void EnableLeftHandInput(bool enable)
        {
            if (_LeftHandMoveAction.action != null)
            {
                if (enable)
                    _LeftHandMoveAction.action.Enable();
                else
                    _LeftHandMoveAction.action.Disable();
                _LeftHandInput = enable;
            }
        }

        public void EnableRightHandInput(bool enable)
        {
            if (_RightHandMoveAction.action != null)
            {
                if (enable)
                    _RightHandMoveAction.action.Enable();
                else
                    _RightHandMoveAction.action.Disable();
                _RightHandInput = enable;
            }
        }
        #endregion

        #region Private and protected methods
        protected virtual void MoveRig(Vector3 velocityChange)
        {
            if (CanBeginLocomotion() && BeginLocomotion())
            {
                _Body.AddForce(velocityChange, ForceMode.VelocityChange);
                EndLocomotion();
            }
        }

        protected virtual Vector2 ReadInput()
        {
            var leftHandValue = _LeftHandInput ? _LeftHandMoveAction.action?.ReadValue<Vector2>() ?? Vector2.zero : Vector2.zero;
            var rightHandValue = _RightHandInput ? _RightHandMoveAction.action?.ReadValue<Vector2>() ?? Vector2.zero : Vector2.zero;
            return leftHandValue + rightHandValue;
        }

        /// <summary>Calculates velocity change for <see cref="Rigidbody.AddForce"/>.
        /// Similar to <see cref="ContinuousMoveProviderBase.ComputeDesiredMove(Vector2)"/>, 
        /// idea from <see href="http://wiki.unity3d.com/index.php/RigidbodyFPSWalker">here</see></summary>
        /// <param name="input">Input vector from controller</param>
        protected Vector3 ComputeVelocityChange(Vector2 input, out Vector3 targetVelocity)
        {
            targetVelocity = Vector3.zero;

            var xrOrigin = system.xrOrigin;
            if (xrOrigin == null)
                return Vector3.zero;

            // Clamps the magnitude of the input direction [-1, 1] to prevent faster speed when moving diagonally,
            // while still allowing for analog input to move slower (which would be lost if simply normalizing).
            var inputMove = Vector3.ClampMagnitude(new Vector3(_EnableStrafe ? input.x : 0f, 0f, input.y), 1f);

            var originTransform = xrOrigin.transform;
            var originUp = originTransform.up;

            // Determine frame of reference for what the input direction is relative to
            var forwardSourceTransform = _ForwardSources[_ForwardSource] == null ?
                xrOrigin.Camera.transform : _ForwardSources[_ForwardSource];
            var inputForwardInWorldSpace = forwardSourceTransform.forward;
            if (Mathf.Approximately(Mathf.Abs(Vector3.Dot(inputForwardInWorldSpace, originUp)), 1f))
            {
                // When the input forward direction is parallel with the rig normal,
                // it will probably feel better for the player to move along the same direction
                // as if they tilted forward or up some rather than moving in the rig forward direction.
                // It also will probably be a better experience to at least move in a direction
                // rather than stopping if the head/controller is oriented such that it is perpendicular with the rig.
                inputForwardInWorldSpace = -forwardSourceTransform.up;
            }

            var inputForwardProjectedInWorldSpace = Vector3.ProjectOnPlane(inputForwardInWorldSpace, originUp);
            var forwardRotation = Quaternion.FromToRotation(originTransform.forward, inputForwardProjectedInWorldSpace);

            var velocityInOriginSpace = forwardRotation * inputMove * _MoveSpeed;
            targetVelocity = originTransform.TransformDirection(velocityInOriginSpace);

            Vector3 velocityChange = (targetVelocity - _Body.velocity);
            velocityChange = Vector3.ProjectOnPlane(velocityChange, transform.up);

            if (_GravityProvider != null)
            {
                if (!_GravityProvider.IsGrounded)
                    velocityChange *= _InAirControlMultiplier;
            }

            return velocityChange;
        }

        private void UpdateIsMoving(Vector3 targetVelocity)
        {
            IsMoving = targetVelocity != Vector3.zero;
            if (IsMoving != _wasMoving)
            {
                if (IsMoving)
                {
                    if (_UseVignette && _Vignette)
                        _Vignette.FadeIn();
                    StartedMoving?.Invoke();
                }
                else
                {
                    if (_UseVignette && _Vignette)
                        _Vignette.FadeOut();
                    FinishedMoving?.Invoke();
                }
            }
            _wasMoving = IsMoving;
        }

        private void UpdateLeftHandInput() => EnableLeftHandInput(_LeftHandInput);
        
        private void UpdateRightHandInput() => EnableRightHandInput(_RightHandInput);
        
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