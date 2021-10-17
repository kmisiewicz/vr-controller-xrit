using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Chroma.XR.Locomotion
{
    [RequireComponent(typeof(Rigidbody))]
    public class BodyHeadFollowProvider : LocomotionProvider
    {
        public enum HeadFollowMethod
        {
            /// <summary>Body collider will always follow head position.</summary>
            Constant,
            /// <summary>Allows the head to move from body to <see cref="neckLength"/>.</summary>
            AllowLeaning,
        }
        [SerializeField, Tooltip("Controls how the body collider will follow player's head.")]
        HeadFollowMethod colliderPositionUpdateMethod;

        [SerializeField] float neckLength = 0.3f;


        Rigidbody _body;
        Transform _head;


        private new void Awake()
        {
            base.Awake();
            _body = GetComponent<Rigidbody>();
            _head = system.xrRig.cameraGameObject.transform;
        }

        private void FixedUpdate()
        {
            AdjustCollider();
        }

        private void AdjustCollider()
        {
            // Get head to body position difference
            Vector3 headPosition = _head.position;
            Vector3 bodyPosition = transform.position;
            headPosition.y = bodyPosition.y = 0;
            Vector3 motion = headPosition - bodyPosition;

            // If player can lean, then only move body collider when head exceeds neckLength, otherwise move always
            if (colliderPositionUpdateMethod == HeadFollowMethod.AllowLeaning)
            {
                float distance = motion.magnitude;
                if (distance > neckLength)
                {
                    float distanceToMove = distance - neckLength;
                    Vector3 direction = motion.normalized;
                    motion = distanceToMove * direction;
                }
                else motion = Vector3.zero;
            }

            // Move body but also rig in oposite direction
            // Explanation: https://www.reddit.com/r/learnVRdev/comments/pauaja/unity_but_i_think_it_would_work_the_same/hab1vfw
            if (motion != Vector3.zero)
            {
                if (CanBeginLocomotion() && BeginLocomotion())
                {
                    _body.MovePosition(_body.position + motion);
                    system.xrRig.transform.position -= motion;
                    EndLocomotion();
                }
            }
        }
    }
}
