using UnityEngine;

namespace Plugins.RossesEssentials.Extensions
{
    public static class WaitForSecondsExtension
    {
        public static float GetSeconds(this WaitForSeconds waitForSeconds)
        {
            var waitField = typeof(WaitForSeconds).GetField("m_Seconds", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            return (float)waitField.GetValue(waitForSeconds);
        }
    }
}