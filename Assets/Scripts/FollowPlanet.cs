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

using UnityEngine;

public class FollowPlanet : MonoBehaviour
{
    public bool player;
    public Transform planet; //Planet
    public Transform[] planetList;
    public Vector3 offset;
    public Vector3 defaultOffset;

    void Update()
    {
        if (player == true)
        {
            if (planet == planetList[1] || planet == planetList[3] || planet == planetList[5] || planet == planetList[7])
            {
                offset = new Vector3(-1, -1, 0);
                Debug.Log("small");
            }
            else if (planet == planetList[9] || planet == planetList[11])
            {
                offset = new Vector3(-3, -3, 0);
                Debug.Log("medium");
            }
            else if (planet == planetList[13] || planet == planetList[15])
            {
                offset = new Vector3(-2, -2, 0);
                Debug.Log("large");
            } else
            {
                offset = defaultOffset;
            }
        }
    }

    void LateUpdate()
    {
        transform.position = planet.position + offset;
    }
}