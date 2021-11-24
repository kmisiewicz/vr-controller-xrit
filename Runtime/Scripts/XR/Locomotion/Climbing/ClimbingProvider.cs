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
        float pullForce = 100f;

        [SerializeField]
        [Tooltip("Minimal magnitude of controller's velocity while letting go off ClimbInteractable to apply force.")]
        float pullThreshold = 1f;

        [SerializeField]
        [Tooltip("Gravity damp factor applied after pulling from Climbing Interactable for gravityDampTime seconds.")]
        float gravityDampFactor = 0.9f;

        [SerializeField]
        [Tooltip("Duration (in seconds) of gravity damping after pulling from Climbing Interactable.")]
        float gravityDampTime = 1.5f;

        [SerializeField]
        [Tooltip("Layers to check for new ground while climbing on ledge.")]
        LayerMask ledgeLayerMask;

        [SerializeField, Min(0f)]
        [Tooltip("Duration (in seconds) of climb animation.")]
        float climbOnLedgeDuration = 0.2f;

        [SerializeField, Min(0f)]
        [Tooltip("Minimal height player's head needs to be above ledge to consider climbing on it.")]
        float minHeightOverLedge = 0.2f;

        [SerializeField]
        LocomotionSystemExtender locomotionSystemExtender;


        ControllerInputManager _inputs = null;
        Rigidbody _rb;


        protected override void Awake()
        {
            base.Awake();
            _rb = GetComponent<Rigidbody>();
            if (locomotionSystemExtender == null)
                locomotionSystemExtender = FindObjectOfType<LocomotionSystemExtender>();
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
            if (!args.interactor.TryGetComponent(out _inputs) || !locomotionSystemExtender.RequestExclusivity(this))
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
            if (args.interactor.GetComponent<ControllerInputManager>() != _inputs)
                return;

            // If player leans behind ledge, climb on it, 
            // else restore default movement and check if player pulled himself
            if (ShouldClimbOnLedge(out Vector3 newPosition))
                StartCoroutine(ClimbOnLedge(newPosition));
            else
            {
                _rb.isKinematic = false;
                locomotionSystemExtender.FinishExclusivity(this);
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
            if (pullVelocity.magnitude > pullThreshold)
            {
                _rb.AddForce(transform.rotation * pullVelocity.normalized * pullForce);
                pulled = true;
            }

            if (pulled)
            {
                // If player pulled strong enough, wait till he begins falling and damp gravity
                while (_rb.velocity.y > 0)
                    yield return new WaitForFixedUpdate();

                float t = 0f;
                while (t < gravityDampTime)
                {
                    _rb.AddForce(Physics.gravity * -gravityDampFactor * Time.fixedDeltaTime * _rb.mass);
                    t += Time.fixedDeltaTime;
                    yield return new WaitForFixedUpdate();
                }
            }
        }

        private bool ShouldClimbOnLedge(out Vector3 newPosition)
        {
            newPosition = Vector3.zero;
            Vector3 cameraPosition = system.xrRig.cameraGameObject.transform.position;

            // Check if player's head is at least 'minHeightOverLedge' over new ground (max is player height)
            if (Physics.Raycast(cameraPosition, Vector3.down, out RaycastHit hit,
                system.xrRig.cameraInRigSpaceHeight, ledgeLayerMask))
            {
                if (hit.distance > minHeightOverLedge)
                {
                    // Shoot raycast up to check if player will fit on new ground
                    float upDistance = system.xrRig.cameraInRigSpaceHeight - hit.distance + 0.1f;
                    if (!Physics.Raycast(system.xrRig.cameraGameObject.transform.position, Vector3.up, out _,
                        upDistance, ledgeLayerMask))
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
            DOTween.To(() => _rb.position, x => _rb.position = x, newPosition, climbOnLedgeDuration)
                .SetUpdate(UpdateType.Fixed)
                .SetId("ledgeClimb");
            yield return new WaitForSeconds(climbOnLedgeDuration);

            // Make sure player object is in correct posiiton
            var cameraPosition = new Vector3(newPosition.x, newPosition.y + system.xrRig.cameraInRigSpaceHeight, newPosition.z);
            system.xrRig.MoveCameraToWorldLocation(cameraPosition);

            // Restore default movemnt
            _rb.isKinematic = false;
            locomotionSystemExtender.FinishExclusivity(this);
        }
    }
}
