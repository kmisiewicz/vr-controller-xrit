using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Chroma.XR.Locomotion
{
    public class TeleportationAreaCustom : TeleportationArea
    {
        // TODO: Could try to make it not depend on TeleportationRayToggler
        [Obsolete]
        protected override bool GenerateTeleportRequest(XRBaseInteractor interactor, RaycastHit raycastHit, ref TeleportRequest teleportRequest)
        {
            teleportRequest.destinationPosition = raycastHit.point;
            if (interactor.TryGetComponent(out TeleportationRayToggler rayToggler) && rayToggler.AreaReticle != null)
                teleportRequest.destinationRotation = rayToggler.ReticleRotation;
            else teleportRequest.destinationRotation = transform.rotation;
            return true;
        }

        protected override bool GenerateTeleportRequest(IXRInteractor interactor, RaycastHit raycastHit, ref TeleportRequest teleportRequest)
        {
            teleportRequest.destinationPosition = raycastHit.point;
            if (interactor.transform.TryGetComponent(out TeleportationRayToggler rayToggler) && rayToggler.AreaReticle != null)
                teleportRequest.destinationRotation = rayToggler.ReticleRotation;
            else teleportRequest.destinationRotation = transform.rotation;
            return true;
        }
    }
}