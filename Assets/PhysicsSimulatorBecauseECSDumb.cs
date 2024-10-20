using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsSimulatorBecauseECSDumb : MonoBehaviour
{
    private void Start()
    {
        Physics.simulationMode = SimulationMode.FixedUpdate;
    }
}
