using System.Text;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public static class StringOps
{
    public static string LoadingDots()
    {
        float time = 0;
        
        #if UNITY_EDITOR
        {
            time = (float)EditorApplication.timeSinceStartup;
        } 
        #else
        {
            time = Time.time;
        }
        #endif
        
        StringBuilder dots = new StringBuilder();
        int dotCount = Mathf.RoundToInt(time) % 3;

        for (int i = 0; i < dotCount; i++)
        {
            dots.Append('.');
        }
        return dots.ToString();
    }
}
