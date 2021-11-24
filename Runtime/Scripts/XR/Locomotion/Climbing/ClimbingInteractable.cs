using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Chroma.XR.Locomotion
{
    public class ClimbingInteractable : XRBaseInteractable
    {
        [SerializeField]
        [Tooltip("The climb provider that this climb interactable will communicate climb requests to." +
                " If no climb provider is configured, will attempt to find a climb provider during Awake.")]
        ClimbingProvider climbingProvider;

        /// <summary>
        /// The climb provider that this climb interactable will communicate teleport requests to.
        /// If no climb provider is configured, will attempt to find a climb provider during Awake.
        /// </summary>
        public ClimbingProvider ClimbingProvider
        {
            get => climbingProvider;
            set => climbingProvider = value;
        }

        protected override void Awake()
        {
            base.Awake();
            if (climbingProvider == null)
                climbingProvider = FindObjectOfType<ClimbingProvider>();
        }

        protected override void OnSelectEntered(SelectEnterEventArgs args)
        {
            base.OnSelectEntered(args);
            climbingProvider.BeginClimbing(args);
        }

        protected override void OnSelectExited(SelectExitEventArgs args)
        {
            climbingProvider.EndClimbing(args);
            base.OnSelectExited(args);
        }
    }
}
