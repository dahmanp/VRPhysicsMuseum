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
    public Transform planet; //Planet
    public Vector3 offset;

    void LateUpdate()
    {
        transform.position = planet.position + offset;
    }
}