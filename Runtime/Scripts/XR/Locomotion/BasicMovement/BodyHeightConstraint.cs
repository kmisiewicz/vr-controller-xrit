using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Chroma.XR.Locomotion
{
    [RequireComponent(typeof(CapsuleCollider))]
    public class BodyHeightConstraint : MonoBehaviour
    {
        [SerializeField, Min(0f)] float minHeight = 1f;
        public float MinHeight => minHeight;

        [SerializeField, Min(0f)] float maxHeight = 2f;
        public float MaxHeight => maxHeight;


        CapsuleCollider _collider;
        XRRig _rig;


        private void Awake()
        {
            _collider = GetComponent<CapsuleCollider>();
            _rig = GetComponentInChildren<XRRig>();
        }

        private void Update()
        {
            UpdateColliderHeight();
        }

        /// <summary>Updates capsule collider's height and center according to <see cref="XRRig.cameraInRigSpaceHeight"/>.</summary>
        private void UpdateColliderHeight()
        {
            var center = _collider.center;
            _collider.height = Mathf.Clamp(_rig.transform.localPosition.y + _rig.cameraInRigSpaceHeight, minHeight, maxHeight);
            _collider.center = new Vector3(center.x, _collider.height / 2, center.z);
        }
    }
}
