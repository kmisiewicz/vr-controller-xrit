using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Chroma.XR.Locomotion
{
    public class ClimbingInteractable : XRBaseInteractable
    {
        [SerializeField]
        [Tooltip("The climb provider that this climb interactable will communicate climb requests to." +
                " If no climb provider is configured, will attempt to find a climb provider during Awake.")]
        ClimbingProvider _ClimbingProvider;

        /// <summary>
        /// The climb provider that this climb interactable will communicate teleport requests to.
        /// If no climb provider is configured, will attempt to find a climb provider during Awake.
        /// </summary>
        public ClimbingProvider ClimbingProvider
        {
            get => _ClimbingProvider;
            set => _ClimbingProvider = value;
        }

        protected override void Awake()
        {
            base.Awake();
            if (_ClimbingProvider == null)
                _ClimbingProvider = FindObjectOfType<ClimbingProvider>();
        }

        protected override void OnSelectEntered(SelectEnterEventArgs args)
        {
            base.OnSelectEntered(args);
            _ClimbingProvider.BeginClimbing(args);
        }

        protected override void OnSelectExited(SelectExitEventArgs args)
        {
            _ClimbingProvider.EndClimbing(args);
            base.OnSelectExited(args);
        }
    }
}
