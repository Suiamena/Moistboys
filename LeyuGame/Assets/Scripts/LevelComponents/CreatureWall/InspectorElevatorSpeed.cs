using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InspectorElevatorSpeed : MonoBehaviour {

    public int setElevatorSpeed = 15;
    public GameObject elevatorObject;
    ElevatorPlatform elevatorScript;

    private void Awake()
    {
        elevatorScript = elevatorObject.GetComponent<ElevatorPlatform>();
    }

    private void Update()
    {
        elevatorScript.elevatorSpeed = setElevatorSpeed;
    }

}
