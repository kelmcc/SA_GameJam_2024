using UnityEngine;

namespace Framework
{
    /// <summary>
    /// Forces an int, float or range field to be higher than some minimum value. 
    /// </summary>
    public class MinValueAttribute : ClampAttribute
    {

        /// <summary>
        /// Forces an int, float or range field to be higher than some minimum value. 
        /// </summary>
        /// <param name="value">The minimum value</param>
        public MinValueAttribute(float value) : base(value, Mathf.Infinity)
        {

        }

    }
}
