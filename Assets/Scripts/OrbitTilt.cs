using UnityEngine;

public class OrbitTilt : MonoBehaviour
{
    //1- Get the scriptable object- or an array of them depending on if it is curr. scaled or not
    //2- Read all of the data points of each object.
    //3- make a loop that goes through every data point and multiply it by rotation matrix

    //4- connect to slider somehow?
    //Mathf.Sin()
    // And
    //Mathf.Cos()
    public float tilt = 5;
    Quaternion rotationVector = Quaternion.Euler(0f, 0f, 0f);

    void Start()
    {
        OrbitData[] scaled = Resources.LoadAll<OrbitData>("ScaledOrbits");
        Debug.Log(scaled[1]);
        OrbitData[] unscaled = Resources.LoadAll<OrbitData>("Orbits");

        foreach (OrbitData obj in scaled)
        {
            for (int i = 0; i < obj.points.Length; i++)
            {
                obj.points[i] = rotationVector * obj.points[i];
            }
        }

        foreach (OrbitData obj in unscaled)
        {
            for (int i = 0; i < obj.points.Length; i++)
            {
                obj.points[i] = rotationVector * obj.points[i];
            }
        }
    }

    void Update()
    {
        rotationVector = Quaternion.Euler(tilt, 0f, 0f);
    }
}