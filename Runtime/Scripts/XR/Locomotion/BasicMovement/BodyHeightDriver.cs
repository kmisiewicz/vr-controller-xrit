using Unity.XR.CoreUtils;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]
public class BodyHeightDriver : MonoBehaviour
{
    [SerializeField, Min(0f)] float _MinHeight = 1f;
    public float MinHeight => _MinHeight;

    [SerializeField, Min(0f)] float _MaxHeight = 2f;
    public float MaxHeight => _MaxHeight;

    [SerializeField, Range(0f, 1f), Tooltip("Determines how high above ground is the collider floating. Allows to climb stairs.")]
    float _FloatHeight = 0.2f;
    public float FloatHeight => _FloatHeight;


    CapsuleCollider _collider;
    XROrigin _origin;


    private void Awake()
    {
        _collider = GetComponent<CapsuleCollider>();
        _origin = GetComponentInChildren<XROrigin>();
    }

    private void FixedUpdate()
    {
        UpdateCollider();
    }

    /// <summary>Updates capsule collider's height and center according to <see cref="XROrigin.CameraInOriginSpaceHeight"/>.</summary>
    private void UpdateCollider()
    {
        var center = _collider.center;
        _collider.height = Mathf.Clamp(_origin.CameraInOriginSpaceHeight, _MinHeight, _MaxHeight) - _FloatHeight;
        _collider.center = new Vector3(center.x, _collider.height / 2f + _FloatHeight, center.z);
    }
}
