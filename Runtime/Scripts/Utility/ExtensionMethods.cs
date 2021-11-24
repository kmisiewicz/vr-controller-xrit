using UnityEngine;

namespace Chroma.Utility
{
    public static class ExtensionMethods
    {
        #region GameObject

        /// <summary>Set GameObject's layer by name.</summary>
        public static void SetLayer(this GameObject go, string layerName)
        {
            go.layer = LayerMask.NameToLayer(layerName);
        }

        /// <summary>Gets a LayerMask based on collison matrix from Project Settings -> Physics.</summary>
        public static LayerMask GetLayerCollisionMask(this GameObject go)
        {
            int layer = go.layer;
            LayerMask layerMask = 0;

            for (int i = 0; i < 32; i++)
            {
                if (!Physics.GetIgnoreLayerCollision(layer, i))
                    layerMask |= 1 << i;
            }

            // Make sure that the calculated layermask does not include the 'Ignore Raycast' layer
            if (layerMask == (layerMask | (1 << LayerMask.NameToLayer("Ignore Raycast"))))
                layerMask ^= (1 << LayerMask.NameToLayer("Ignore Raycast"));

            return layerMask;
        }

        #endregion

        #region Vector3

        /// <summary>Clamps all components of <paramref name="vector"/> between
        /// <paramref name="minValue"/> and <paramref name="maxValue"/>.</summary>
        public static void Clamp(this Vector3 vector, float minValue, float maxValue)
        {
            vector.x = Mathf.Clamp(vector.x, minValue, maxValue);
            vector.y = Mathf.Clamp(vector.y, minValue, maxValue);
            vector.z = Mathf.Clamp(vector.z, minValue, maxValue);
        }

        /// <summary>Extract and return parts from a <paramref name="vector"/>
        /// that are pointing in the same direction as <paramref name="direction"/>.
        /// Return the length of this component as <paramref name="amount"/>.</summary>
        public static Vector3 ExtractDotVector(this Vector3 vector, Vector3 direction, out float amount)
        {
            // Normalize vector if necessary
            if (direction.sqrMagnitude != 1)
                direction.Normalize();

            amount = Vector3.Dot(vector, direction);

            return direction * amount;
        }

        /// <summary>Remove all parts from a <paramref name="vector"/> 
        /// that are pointing in the same <paramref name="direction"/>.
        /// Return the length of this component as <paramref name="amount"/>.</summary>
        public static Vector3 RemoveDotVector(this Vector3 vector, Vector3 direction, out float amount)
        {
            // Extract vector pointing in set 'direction' and subtract it from 'vector' to remove it
            return vector - vector.ExtractDotVector(direction, out amount);
        }

        #endregion
    }
}
