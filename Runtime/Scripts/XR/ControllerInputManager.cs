using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

[AddComponentMenu("XR/Action Based Controller Input Manager")]
public class ControllerInputManager : MonoBehaviour
{
    [SerializeField, Tooltip("Return manually calculated controller velocity.")]
    bool useCalculatedVelocity = false;

    [SerializeField] InputActionProperty velocityAction;
    public Vector3 Velocity => useCalculatedVelocity ? calculatedVelocity : velocityAction.action.ReadValue<Vector3>();


    ActionBasedController _controller;
    XRDirectInteractor _directInteractor;
    Vector3 previousPosition = Vector3.zero;
    Vector3 currentPosition = Vector3.zero;
    Vector3 calculatedVelocity = Vector3.zero;


    private void Awake()
    {
        _controller = GetComponent<ActionBasedController>();
        _directInteractor = GetComponent<XRDirectInteractor>();
    }

    private void FixedUpdate()
    {
        CalculateVelocity(); // May delete if controller velocity input gets fixed
    }

    private void CalculateVelocity()
    {
        previousPosition = currentPosition;
        currentPosition = transform.localPosition;
        calculatedVelocity = previousPosition - currentPosition;
        calculatedVelocity.y = -calculatedVelocity.y;
    }
}
