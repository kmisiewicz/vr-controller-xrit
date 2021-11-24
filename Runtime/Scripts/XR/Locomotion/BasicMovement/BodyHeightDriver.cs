using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(CapsuleCollider))]
public class BodyHeightDriver : MonoBehaviour
{
    [SerializeField, Min(0f)]
    float minHeight = 1f;
    public float MinHeight => minHeight;

    [SerializeField, Min(0f)]
    float maxHeight = 2f;
    public float MaxHeight => maxHeight;

    [SerializeField, Range(0f, 1f), Tooltip("Determines how high above ground is the collider floating. Allows to climb stairs.")]
    float floatHeight = 0.2f;
    public float FloatHeight => floatHeight;


    CapsuleCollider _collider;
    XRRig _rig;


    private void Awake()
    {
        _collider = GetComponent<CapsuleCollider>();
        _rig = GetComponentInChildren<XRRig>();
    }

    private void FixedUpdate()
    {
        UpdateCollider();
    }

    /// <summary>Updates capsule collider's height and center according to <see cref="XRRig.cameraInRigSpaceHeight"/>.</summary>
    private void UpdateCollider()
    {
        var center = _collider.center;
        _collider.height = Mathf.Clamp(_rig.cameraInRigSpaceHeight, minHeight, maxHeight) - floatHeight;
        _collider.center = new Vector3(center.x, _collider.height / 2 + floatHeight, center.z);
    }
}
