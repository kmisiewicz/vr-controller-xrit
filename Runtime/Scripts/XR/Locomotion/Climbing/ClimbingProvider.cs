using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using DG.Tweening;

namespace Chroma.XR.Locomotion
{
    public class ClimbingProvider : LocomotionProvider
    {
        [SerializeField]
        [Tooltip("Multiplier of force applied after letting go off ClimbingInteractable.")]
        float _PullForce = 100f;

        [SerializeField]
        [Tooltip("Minimal magnitude of controller's velocity while letting go off ClimbInteractable to apply force.")]
        float _PullThreshold = 1f;

        [SerializeField]
        [Tooltip("Gravity damp factor applied after pulling from Climbing Interactable for gravityDampTime seconds.")]
        float _GravityDampFactor = 0.9f;

        [SerializeField]
        [Tooltip("Duration (in seconds) of gravity damping after pulling from Climbing Interactable.")]
        float _GravityDampTime = 1.5f;

        [SerializeField]
        [Tooltip("Layers to check for new ground while climbing on ledge.")]
        LayerMask _LedgeLayerMask;

        [SerializeField, Min(0f)]
        [Tooltip("Duration (in seconds) of climb animation.")]
        float _ClimbOnLedgeDuration = 0.2f;

        [SerializeField, Min(0f)]
        [Tooltip("Minimal height player's head needs to be above ledge to consider climbing on it.")]
        float _MinHeightOverLedge = 0.2f;

        [SerializeField]
        LocomotionSystemExtender _LocomotionSystemExtender;


        ControllerInputManager _inputs = null;
        Rigidbody _rb;


        protected override void Awake()
        {
            base.Awake();
            _rb = GetComponent<Rigidbody>();
            if (_LocomotionSystemExtender == null)
                _LocomotionSystemExtender = FindObjectOfType<LocomotionSystemExtender>();
        }

        private void FixedUpdate()
        {
            if (_inputs && BeginLocomotion())
            {
                Climb();
                EndLocomotion();
            }
        }

        private void Climb()
        {
            // Move in direction opposite to climbing hand's velocity
            var motion = transform.rotation * -_inputs.Velocity;
            _rb.MovePosition(_rb.position + motion);
        }

        public void BeginClimbing(SelectEnterEventArgs args)
        {
            // Stop climbing on ledge if player grabs another climb interactable
            StopAllCoroutines();
            DOTween.Kill("ledgeClimb");

            // Get component to poll controller's velocity and request exclusive operation, return if can't do both
            if (!args.interactorObject.transform.TryGetComponent(out _inputs) || !_LocomotionSystemExtender.RequestExclusivity(this))
            {
                _inputs = null;
                return;
            }

            // Disable body collision and remove any forces
            _rb.velocity = Vector3.zero;
            _rb.isKinematic = true;
        }

        public void EndClimbing(SelectExitEventArgs args)
        {
            // If player is climbing with another hand (controller), don't exit climbing mode (return)
            if (args.interactorObject.transform.GetComponent<ControllerInputManager>() != _inputs)
                return;

            // If player leans behind ledge, climb on it, 
            // else restore default movement and check if player pulled himself
            if (ShouldClimbOnLedge(out Vector3 newPosition))
                StartCoroutine(ClimbOnLedge(newPosition));
            else
            {
                _rb.isKinematic = false;
                _LocomotionSystemExtender.FinishExclusivity(this);
                StartCoroutine(Pull(_inputs));
            }

            _inputs = null;
        }

        // TODO: Make gravity damp dependent on gravity provider
        private IEnumerator Pull(ControllerInputManager inputs)
        {
            bool pulled = false;

            // Add force if controller's velocity square magnitude is bigger than pullThreshold
            var pullVelocity = -inputs.Velocity;
            if (pullVelocity.magnitude > _PullThreshold)
            {
                _rb.AddForce(transform.rotation * pullVelocity.normalized * _PullForce);
                pulled = true;
            }

            if (pulled)
            {
                // If player pulled strong enough, wait till he begins falling and damp gravity
                while (_rb.velocity.y > 0)
                    yield return new WaitForFixedUpdate();

                float t = 0f;
                while (t < _GravityDampTime)
                {
                    _rb.AddForce(Physics.gravity * -_GravityDampFactor * Time.fixedDeltaTime * _rb.mass);
                    t += Time.fixedDeltaTime;
                    yield return new WaitForFixedUpdate();
                }
            }
        }

        private bool ShouldClimbOnLedge(out Vector3 newPosition)
        {
            newPosition = Vector3.zero;
            Vector3 cameraPosition = system.xrOrigin.Camera.transform.position;

            // Check if player's head is at least 'minHeightOverLedge' over new ground (max is player height)
            if (Physics.Raycast(cameraPosition, Vector3.down, out RaycastHit hit,
                system.xrOrigin.CameraInOriginSpaceHeight, _LedgeLayerMask))
            {
                if (hit.distance > _MinHeightOverLedge)
                {
                    // Shoot raycast up to check if player will fit on new ground
                    float upDistance = system.xrOrigin.CameraInOriginSpaceHeight - hit.distance + 0.1f;
                    if (!Physics.Raycast(system.xrOrigin.Camera.transform.position, Vector3.up, out _, upDistance, _LedgeLayerMask))
                    {
                        newPosition = hit.point;
                        return true;
                    }
                }
            }
            return false;
        }

        private IEnumerator ClimbOnLedge(Vector3 newPosition)
        {
            // Tween XRRig to new ground (shortcuts don't work without assembly definition setup)
            //_rb.DOMove(newPosition, climbOnLedgeDuration, false).SetId("ledgeClimb");
            DOTween.To(() => _rb.position, x => _rb.position = x, newPosition, _ClimbOnLedgeDuration)
                .SetUpdate(UpdateType.Fixed)
                .SetId("ledgeClimb");
            yield return new WaitForSeconds(_ClimbOnLedgeDuration);

            // Make sure player object is in correct posiiton
            var cameraPosition = new Vector3(newPosition.x, newPosition.y + system.xrOrigin.CameraInOriginSpaceHeight, newPosition.z);
            system.xrOrigin.MoveCameraToWorldLocation(cameraPosition);

            // Restore default movemnt
            _rb.isKinematic = false;
            _LocomotionSystemExtender.FinishExclusivity(this);
        }
    }
}
