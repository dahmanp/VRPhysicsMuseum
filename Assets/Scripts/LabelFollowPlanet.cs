using UnityEngine;

public class LabelFollowPlanet : MonoBehaviour
{
    //VARIABLES----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    public Transform planet;
    public float distanceFromPlanet_x = 2f;
    public float distanceFromPlanet_y = 2f;
    public float distanceFromPlanet_z = 2f;
    public float directionSmoothSpeed = 2f;

    private Vector3 smoothedDirection;

    // SIMULATION--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    void Start()
    {
        //-----------------NOTE TO FUTURE DEVS-----------------
        //Smooths the direction / movement of the object that has the script on it based on the planet's position
        smoothedDirection = (planet.position).normalized;
    }

    void Update()
    {
        //-----------------NOTE TO FUTURE DEVS-----------------
        //This contstantly updates the position of the object with the script (planet labels in this case), and adjusts them according to the planet location and the offset you input in (which change depending on the planet size)
        float targetX = planet.position.x + distanceFromPlanet_x;
        float targetY = planet.position.y + distanceFromPlanet_y;
        float targetZ = planet.position.z + distanceFromPlanet_z;

        float smoothedX = Mathf.Lerp(transform.position.x, targetX, Time.deltaTime * directionSmoothSpeed);
        float smoothedY = Mathf.Lerp(transform.position.y, targetY, Time.deltaTime * directionSmoothSpeed);
        float smoothedZ = Mathf.Lerp(transform.position.z, targetZ, Time.deltaTime * directionSmoothSpeed);

        transform.position = new Vector3(smoothedX, smoothedY, smoothedZ);

    }
}
