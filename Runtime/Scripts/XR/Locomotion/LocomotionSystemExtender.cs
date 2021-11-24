using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Chroma.XR.Locomotion
{
    [RequireComponent(typeof(LocomotionProvider))]
    public class LocomotionSystemExtender : MonoBehaviour
    {
        [SerializeField] MovementProvider movementProvider = null;
        [SerializeField] ContinuousTurnProvider continuousTurnProvider = null;
        [SerializeField] SnapTurnProvider snapTurnProvider = null;
        [SerializeField] TeleportationProvider teleportationProvider = null;
        [SerializeField] GravityProvider gravityProvider = null;


        private LocomotionSystem _system;
        private LocomotionProvider _exclusiveProvider = null;


        private void Awake()
        {
            _system = GetComponent<LocomotionSystem>();
        }

        private void SetDefaultMovementActive(bool enabled)
        {
            movementProvider.enabled = continuousTurnProvider.enabled =
                snapTurnProvider.enabled = teleportationProvider.enabled = enabled;
            gravityProvider.UseGravity = enabled;
        }

        public bool RequestExclusivity(LocomotionProvider provider)
        {
            if (_exclusiveProvider != null && _exclusiveProvider != provider)
                return false;

            // Disable default movement methods
            SetDefaultMovementActive(false);
            _exclusiveProvider = provider;
            return true;
        }

        public bool FinishExclusivity(LocomotionProvider provider)
        {
            if (_exclusiveProvider != provider)
                return false;

            // Restore default movement methods
            SetDefaultMovementActive(true);
            _exclusiveProvider = null;
            return true;
        }
    }
}
