using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Chroma.XR.Locomotion
{
    public class TeleportationAreaCustom : TeleportationArea
    {
        // Could try to make it not depend on TeleportationRayToggler
        protected override bool GenerateTeleportRequest(XRBaseInteractor interactor, RaycastHit raycastHit, ref TeleportRequest teleportRequest)
        {
            teleportRequest.destinationPosition = raycastHit.point;
            if (interactor.TryGetComponent(out TeleportationRayToggler rayToggler) && rayToggler.AreaReticle != null)
                teleportRequest.destinationRotation = rayToggler.ReticleRotation;
            else teleportRequest.destinationRotation = transform.rotation;
            return true;
        }
    }
}