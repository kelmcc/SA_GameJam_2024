using Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zoop : MonoBehaviour
{
    public float Force;
    public LayerMask ZoopMask;
    public void OnTriggerEnter(Collider other)
    {
        if (ZoopMask.Contains(other.gameObject.layer))
        {
            Player player = other.GetComponentInParent<Player>();
            player.Zoop(transform.forward * Force);
        }
    }
    
    public void OnTriggerStay(Collider other)
    {
        if (ZoopMask.Contains(other.gameObject.layer))
        {
            Player player = other.GetComponentInParent<Player>();
            player.Zoop(transform.forward * Force);
        }
    }
}
