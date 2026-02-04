using UnityEngine;
public class FollowPlanet : MonoBehaviour
{
    public Transform planet;
    public Transform sun;
    public float distanceFromPlanet = 2f;
    public float directionSmoothSpeed = 2f;

    private Vector3 smoothedDirection;
    private Quaternion initialRotation; //Caches what the rotation of platform is before simulation

    void Start()
    {
        //Gets position between sun and planet so platform stays in the middle ish
        smoothedDirection = (sun.position - planet.position).normalized;

        initialRotation = transform.rotation;
    }

    //void LateUpdate()
    void Update()
    {

        Vector3 desiredDirection = (sun.position - planet.position).normalized; //Direction from Planet to Sun

        smoothedDirection = Vector3.Slerp( //Smooths the movent out, kinda weird
            smoothedDirection,
            desiredDirection,
            Time.deltaTime * directionSmoothSpeed
        ).normalized;

        transform.position = planet.position + smoothedDirection * distanceFromPlanet;

        //Always face the sun, but stays flat
        Vector3 flatLookDir = sun.position - transform.position; //Only y rotation, desired direction var is all 3
        flatLookDir.y = 0f; //keep level 

        if (flatLookDir.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(flatLookDir.normalized, Vector3.up);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                Time.deltaTime * 2
            );
        }
    }
}
