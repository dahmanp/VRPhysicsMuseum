using UnityEngine;

[CreateAssetMenu(fileName = "OrbitData", menuName = "SolarSystem/OrbitData")]
public class OrbitData : ScriptableObject
{
    public Vector3[] points;
}
