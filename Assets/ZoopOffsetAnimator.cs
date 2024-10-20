using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class ZoopOffsetAnimator : MonoBehaviour
{
    public Material ZoopMat;

    public float Speed = 1;

    void Update()
    {
        ZoopMat.mainTextureOffset = new Vector2(Time.time * Speed, 0);
    }
}
