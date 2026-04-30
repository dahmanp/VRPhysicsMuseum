using UnityEngine;

public class FollowPlanet : MonoBehaviour
{
    //VARIABLES----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    public bool player;
    public Transform planet;
    public Transform[] planetList;
    public Vector3 offset;
    public Vector3 defaultOffset;
    public OrbitSimulator simReference;

    // SIMULATION--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    void Start()
    {
        simReference.timeSpeed = OrbitSimulator.TimeSpeed.OneDayPerSecond;
        simReference.SetTimeSpeedFromDropdown(true);
    }

    void Update()
    {
        if (player == true)
        {
            //-----------------NOTE TO FUTURE DEVS-----------------
            //Sets the offset of the planet based on the size. If it is not scaled, it just goes to the default offset, as they would all be the same.
            if (planet == planetList[1] || planet == planetList[3] || planet == planetList[5] || planet == planetList[7])
            {
                offset = new Vector3(-1, -1, 0);
                //Small Planets
            }
            else if (planet == planetList[9] || planet == planetList[11])
            {
                offset = new Vector3(-3, -3, 0);
                //Medium Planets
            }
            else if (planet == planetList[13] || planet == planetList[15])
            {
                offset = new Vector3(-2, -2, 0);
                //Large Planets
            } else
            {
                offset = defaultOffset;
            }
        }
    }

    //-----------------NOTE TO FUTURE DEVS-----------------
    //Every frame moves the platform/player to a to the position of a planet with an offset.
    //We want the platform and player to be underneath the planet.Since some planets are significantly larger than others when scaled, we have positions hardcoded in to be a certain distance away so that the player is a good distance from the planet.
    void LateUpdate()
    {
        transform.position = planet.position + offset;
    }
}

//-----------------NOTE TO FUTURE DEVS-----------------
//This might be helpful, it takes the position of the planet and the sun and moves the [platform/player/whatever it is attached to] to a position of a vector in between the two, always facing the sun. We decided to have the platform right underneath of the planet instead, but the original script might still be useful, feel free to delete if not.

/*
using UnityEngine;

public class FollowPlanet : MonoBehaviour
{
    public Transform planet;
    public Transform sun;
    public float distanceFromPlanet = 2f;
    public float directionSmoothSpeed = 2f;

    private Vector3 smoothedDirection;

    void Start()
    {
        smoothedDirection = (sun.position - planet.position).normalized;
    }

    void Update()
    {
        // Smoothly follow planet-sun direction
        Vector3 desiredDirection = (sun.position - planet.position).normalized;
        smoothedDirection = Vector3.Slerp(smoothedDirection, desiredDirection, Time.deltaTime * directionSmoothSpeed).normalized;

        // Update platform position
        transform.position = planet.position + smoothedDirection * distanceFromPlanet;

        // Keep platform rotation locked to face sun but flat
        Vector3 flatLookDir = sun.position - transform.position;
        flatLookDir.y = 0f;

        if (flatLookDir.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(flatLookDir.normalized, Vector3.up);
            transform.rotation = targetRotation; // Locked rotation
        }
    }
}
*/