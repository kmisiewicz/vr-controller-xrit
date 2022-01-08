using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Inputs;

namespace Chroma.XR.Locomotion
{
    [RequireComponent(typeof(XRRayInteractor))]
    public class TeleportationRayToggler : MonoBehaviour
    {
        [SerializeField, Tooltip("The Input System Action that will be used to read Teleportation Activate data from the controller.")]
        InputActionProperty activateAction;
        /// <summary>The Input System Action that will be used to read Teleportation Activate data from the controller.</summary>
        public InputActionProperty ActivateAction
        {
            get => activateAction;
            set => SetInputActionProperty(ref activateAction, value);
        }

        [SerializeField]
        InputActionProperty teleportDirectionAction;
        public InputActionProperty TeleportDirectionAction
        {
            get => teleportDirectionAction;
            set => SetInputActionProperty(ref teleportDirectionAction, value);
        }

        [Space]

        [SerializeField] GameObject areaReticle = null;
        public GameObject AreaReticle => areaReticle;

        [SerializeField] Transform reticleRotating = null;
        public Quaternion ReticleRotation => reticleRotating.rotation;

        [SerializeField] TeleportationProvider teleportationProvider = null;


        XRRayInteractor _rayInteractor = null;
        XRInteractorLineVisual _lineVisual = null;
        bool _isEnabled = false;
        bool _reticleEnabled = false;
        bool _rotateDestination = true;

        private void Awake()
        {
            _rayInteractor = GetComponent<XRRayInteractor>();
            _lineVisual = GetComponent<XRInteractorLineVisual>();
            if (teleportationProvider == null)
                teleportationProvider = FindObjectOfType<TeleportationProvider>();
        }

        private void Start()
        {
            _rayInteractor.enabled = false;
        }

        private void OnEnable()
        {
            activateAction.action.started += EnableRay;
            activateAction.action.canceled += DisableRay;
        }

        private void OnDisable()
        {
            activateAction.action.started -= EnableRay;
            activateAction.action.canceled -= DisableRay;
            _rayInteractor.enabled = _isEnabled = false;
        }

        private void EnableRay(InputAction.CallbackContext ctx)
        {
            if (teleportationProvider.RequestRayExclusivity(_rayInteractor))
                _isEnabled = true;
        }

        private void DisableRay(InputAction.CallbackContext ctx)
        {
            if (teleportationProvider.FinishRayExclusivity(_rayInteractor))
                _isEnabled = false;
        }

        private void LateUpdate()
        {
            ApplyStatus();
            UpdateReticle();
        }

        private void ApplyStatus()
        {
            if (_rayInteractor.enabled != _isEnabled)
                _rayInteractor.enabled = _isEnabled;
        }

        private void UpdateReticle()
        {
            if (_isEnabled && _reticleEnabled)
            {
                Vector3 controllerRotation = transform.eulerAngles;
                Quaternion reticleRotation = Quaternion.Euler(0, controllerRotation.y, 0);
                if (_rotateDestination)
                {
                    Vector2 teleportDirection = teleportDirectionAction.action.ReadValue<Vector2>();
                    Quaternion stickRotation = Quaternion.LookRotation(new Vector3(teleportDirection.x, 0, teleportDirection.y));
                    reticleRotation = stickRotation * reticleRotation;
                }
                reticleRotating.rotation = reticleRotation;
            }
        }

        public void HoverEntered(HoverEnterEventArgs args)
        {
            var teleportationArea = args.interactableObject as TeleportationArea;
            if (teleportationArea != null && areaReticle != null)
            {
                _lineVisual.AttachCustomReticle(areaReticle);
                _reticleEnabled = true;
            }
        }

        public void HoverExited(HoverExitEventArgs args)
        {
            var teleportationArea = args.interactableObject as TeleportationArea;
            if (teleportationArea != null && areaReticle != null)
            {
                _reticleEnabled = false;
                _lineVisual.RemoveCustomReticle();
            }
        }

        private void SetInputActionProperty(ref InputActionProperty property, InputActionProperty value)
        {
            if (Application.isPlaying)
                property.DisableDirectAction();

            property = value;

            if (Application.isPlaying && isActiveAndEnabled)
                property.EnableDirectAction();
        }
    }
}
