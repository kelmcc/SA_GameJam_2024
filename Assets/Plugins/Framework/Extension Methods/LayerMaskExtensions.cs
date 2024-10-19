using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Framework
{
    /// <summary>
    /// Extension methods for LayerMasks.
    /// </summary>
    public static class LayerMaskExtensions
    {


        /// <summary>
        /// Returns an array of all the layers this mask contains.
        /// </summary>
        /// <returns>An array of all the layers this mask contains</returns>
        public static int[] GetLayers(this LayerMask layerMask)
        {
            List<int> layers = new List<int>();
            for (int i = 0; i < 32; i++)
            {
                if (layerMask == (layerMask | (1 << i))) layers.Add(i);
            }

            return layers.ToArray();
        }

        /// <summary>
        /// Returns the names of the layers in this mask.
        /// </summary>
        /// <returns>An array of layer names</returns>
        public static string[] GetLayerNames(this LayerMask layerMask)
        {
            List<string> layers = new List<string>();
            for (int i = 0; i < 32; i++)
            {
                if (layerMask == (layerMask | (1 << i))) layers.Add(LayerMask.LayerToName(i));
            }

            return layers.ToArray();
        }

        /// <summary>
        /// Returns the names of the layers in this mask in a single-line string.
        /// </summary>
        /// <returns>Comma separated list of layer names</returns>
        public static string LayersToString(this LayerMask layerMask)
        {
            StringBuilder builder = new StringBuilder();

            for (int mask = layerMask.value, layer = 0; mask != 0; mask = mask >> 1, layer++)
            {
                if ((mask & 1) != 0)
                {
                    if (builder.Length > 0)
                    {
                        builder.Append(", ");
                    }
                    builder.Append(LayerMask.LayerToName(layer));
                }
            }

            return builder.ToString();
        }

        /// <summary>
        /// Whether or not this mask contains a specific layer.
        /// </summary>
        /// <param name="layer">The layer to test (unshifted)</param>
        /// <returns>True if the mask contains the layer</returns>
        public static bool Contains(this LayerMask layerMask, int layer)
        {
            return layerMask == (layerMask | (1 << layer));
        }

        public static bool ContainsAny(this LayerMask layerMask, LayerMask mask)
        {
            return (layerMask & mask) > 0;
        }

        public static bool ContainsAll(this LayerMask layerMask, LayerMask mask)
        {

            for (int i = 0; i < 32; i++)
            {
                if (mask == (mask | (1 << i)) && layerMask != (layerMask | (1 << i))) return false;
            }

            return true;
        }


        /// <summary>
        /// Returns the mask with additional layers added.
        /// </summary>
        /// <param name="additionalLayers">The layers to add, note that these must be castable to int.</param>
        /// <returns>The combined layer mask</returns>
        public static LayerMask With(this LayerMask layerMask, params object[] additionalLayers)
        {
            for (int i = 0; i < additionalLayers.Length; i++)
            {
                layerMask |= (1 << (int)additionalLayers[i]);
            }

            return layerMask;
        }

        /// <summary>
        /// Returns the mask with additional layers added.
        /// </summary>
        /// <param name="additionalLayers">The layers to add.</param>
        /// <returns>The combined layer mask</returns>
        public static LayerMask With(this LayerMask layerMask, params int[] additionalLayers)
        {
            for (int i = 0; i < additionalLayers.Length; i++)
            {
                layerMask |= (1 << additionalLayers[i]);
            }

            return layerMask;
        }

        /// <summary>
        /// Returns the mask with an additional layer added.
        /// </summary>
        /// <param name="additionalLayer">The layer to add</param>
        /// <returns>The combined layer mask</returns>
        public static LayerMask With(this LayerMask layerMask, int additionalLayer)
        {
            return layerMask | (1 << additionalLayer);
        }


        public static LayerMask WithAll(this LayerMask layerMask)
        {
            return ~0;
        }

        public static LayerMask WithNone(this LayerMask layerMask)
        {
            return 0;
        }

        /// <summary>
        /// Returns the mask with some unwanted layers removed.
        /// </summary>
        /// <param name="unwantedLayers">The layers to remove, note that these must be castable to int.</param>
        /// <returns>The new layer mask</returns>
        public static LayerMask WithOut(this LayerMask layerMask, params object[] unwantedLayers)
        {
            for (int i = 0; i < unwantedLayers.Length; i++)
            {
                layerMask &= ~(1 << (int)unwantedLayers[i]);
            }

            return layerMask;
        }

        /// <summary>
        /// Returns the mask with some unwanted layers removed.
        /// </summary>
        /// <param name="unwantedLayers">The layers to remove</param>
        /// <returns>The new layer mask</returns>
        public static LayerMask WithOut(this LayerMask layerMask, params int[] unwantedLayers)
        {
            for (int i = 0; i < unwantedLayers.Length; i++)
            {
                layerMask &= ~(1 << unwantedLayers[i]);
            }

            return layerMask;
        }

        /// <summary>
        /// Returns the mask with an unwante layers removed.
        /// </summary>
        /// <param name="unwantedLayer">The layer to remove</param>
        /// <returns>The new layer mask</returns>
        public static LayerMask WithOut(this LayerMask layerMask, int unwantedLayer)
        {
            return layerMask & ~(1 << unwantedLayer);
        }

        public static LayerMask Inverse(this LayerMask layerMask)
        {
            return ~layerMask;
        }


    }
}
