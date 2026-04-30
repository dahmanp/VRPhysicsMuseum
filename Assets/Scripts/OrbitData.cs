using UnityEngine;

[CreateAssetMenu(fileName = "OrbitData", menuName = "SolarSystem/OrbitData")]
public class OrbitData : ScriptableObject
{
    //-----------------NOTE TO FUTURE DEVS-----------------
    //This script creates an object that keeps all of the points the orbit line renderer will use to create the orbit line visuals.
    //This can be set through OrbitSaver script.

    //VARIABLES----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public Vector3[] points;
}
