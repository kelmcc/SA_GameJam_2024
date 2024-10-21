using Framework;
using UnityEngine;

public class FacePlayer : MonoBehaviour
{
   
    // Update is called once per frame
    void Update()
    {
        if (Camera.main)
        {
            transform.forward = (Camera.main.transform.position - transform.position).WithY(0).normalized;
        }
    }
}
