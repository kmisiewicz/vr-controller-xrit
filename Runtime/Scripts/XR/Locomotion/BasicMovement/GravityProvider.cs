using Chroma.Utility;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Chroma.XR.Locomotion
{
    [RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider)), DefaultExecutionOrder(-200)]
    /// <summary>Custom gravity provider for Rigidbody player controller.</summary>
    public class GravityProvider : LocomotionProvider
    {
        [SerializeField, Tooltip("Enable gravity.")]
        bool useGravity = true;
        /// <summary>Enable gravity.</summary>
        public bool UseGravity
        {
            get => useGravity;
            set => useGravity = value;
        }

        [SerializeField, Tooltip("Gravity acceleration in world_units/s^2.")]
        float gravity = 9.81f;
        /// <summary>Gravity acceleration in world_units/s^2 (local).</summary>
        public float Gravity
        {
            get => gravity;
            set => gravity = value;
        }

        [SerializeField, Tooltip("How steep can the slope be until gravity pulls player down.")]
        float slopeLimit = 50f;
        public float SlopeLimit => slopeLimit;

        [SerializeField, Tooltip("Gravity applied on slopes that exceed 'slopeLimit'.")]
        float slopeGravity = 5f;
        /// <summary>Gravity applied on slopes that exceed <see cref="slopeLimit"/>.</summary>
        public float SlopeGravity
        {
            get => slopeGravity;
            set => slopeGravity = value;
        }


        public bool IsGrounded { get; private set; } = false;

        public bool IsSliding { get; private set; } = false;

        public Vector3 GroundPoint { get; private set; } = Vector3.positiveInfinity;

        public float GroundDistance { get; private set; } = Mathf.Infinity;

        public Vector3 GroundNormal { get; private set; } = Vector3.positiveInfinity;


        CapsuleCollider _collider;
        Rigidbody _rb;
        int _currentLayer;
        LayerMask _groundLayerMask;
        Vector3 _rayOrigin = Vector3.zero;
        bool _wasGrounded = false;


        protected override void Awake()
        {
            base.Awake();

            _collider = GetComponent<CapsuleCollider>();
            _rb = GetComponent<Rigidbody>();

            _currentLayer = gameObject.layer;
            _groundLayerMask = gameObject.GetLayerCollisionMask();
        }

        private void FixedUpdate()
        {
            CheckIfGrounded();
            HandleGravity();
        }

        /// <summary>Checks whether the character is on the ground and updates <see cref="IsGrounded"/>.</summary>
        private void CheckIfGrounded()
        {
            // Reset grounded variable
            _wasGrounded = IsGrounded;
            IsGrounded = IsSliding = false;

            // Calculate raycast length - longer if player was previously grounded
            float floatHeight = _collider.center.y - _collider.height * 0.5f;
            float rayLength = _collider.radius + floatHeight * (_wasGrounded ? 2 : 1) + 0.001f;

            // Calculate raycast origin
            Vector3 rayOrigin = _collider.center;
            rayOrigin.y = floatHeight + _collider.radius;
            _rayOrigin = transform.TransformPoint(rayOrigin);

            // If layer changed, recalculate collision layer mask
            if (_currentLayer != gameObject.layer)
                _groundLayerMask = gameObject.GetLayerCollisionMask();

            // If raycast hits, player is grounded, setup variables
            if (Physics.Raycast(_rayOrigin, -transform.up, out RaycastHit hit, rayLength, _groundLayerMask, QueryTriggerInteraction.Ignore))
            {
                IsGrounded = true;
                GroundPoint = hit.point;
                GroundDistance = hit.distance;
                GroundNormal = hit.normal;
                IsSliding = Vector3.Angle(GroundNormal, transform.up) > slopeLimit;
            }
        }

        private void HandleGravity()
        {
            if (!useGravity)
                return;

            if (IsGrounded)
            {
                if (!_wasGrounded)
                {
                    ZeroOutGravity();
                }
                AdjustPositionToGround();
            }
            else
            {
                if (BeginLocomotion())
                {
                    _rb.AddForce(-transform.up * gravity, ForceMode.Acceleration);
                    EndLocomotion();
                }
            }
        }

        private void ZeroOutGravity()
        {
            _rb.velocity = _rb.velocity.RemoveDotVector(transform.up, out _);
        }

        private void AdjustPositionToGround()
        {
            Vector3 targetPosition = Vector3.Lerp(_rb.position, GroundPoint, 20 * Time.deltaTime);
            _rb.MovePosition(targetPosition);
        }

        private void OnDrawGizmos()
        {
            if (IsGrounded)
            {
                Debug.DrawRay(GroundPoint, GroundNormal, Color.red, Time.deltaTime);
                Debug.DrawLine(_rayOrigin, GroundPoint, Color.cyan, Time.deltaTime);
                float _markerSize = 0.2f;
                Debug.DrawLine(GroundPoint + Vector3.up * _markerSize, GroundPoint - Vector3.up * _markerSize, Color.green, Time.deltaTime);
                Debug.DrawLine(GroundPoint + Vector3.right * _markerSize, GroundPoint - Vector3.right * _markerSize, Color.green, Time.deltaTime);
                Debug.DrawLine(GroundPoint + Vector3.forward * _markerSize, GroundPoint - Vector3.forward * _markerSize, Color.green, Time.deltaTime);
            }
        }
    }
}
