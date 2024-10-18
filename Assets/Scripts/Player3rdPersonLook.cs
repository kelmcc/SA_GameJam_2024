using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player3rdPersonLook : MonoBehaviour
{
    public CinemachineVirtualCamera VCam;
    private CinemachinePOV pov;
    // Start is called before the first frame update
    void Start()
    {
        VCam.GetCinemachineComponent<CinemachinePOV>();
        //pov.
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
