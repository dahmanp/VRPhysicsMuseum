/*****************
A couple notes:
I couldn't just parent the platform to the planet because it would
also follow the rotation and would make it super buggy because of how 
often it was being updated.

Here are two options:
Simple- Just put in the planet as a target and the platform will 
follow the planet at a specified offset. The issue is that sometimes 
the planetvwould block the view of the sun.

Advanved- Tries to always have the platform in between the planet
and the sun, but is a lot more complicated and might get weird
with multiple platforms.

Can see the difference best when time scale 1 day = 1 week
*****************/

/*
using UnityEngine;

public class FollowPlanet : MonoBehaviour
{
    public Transform target; //Planet
    public Vector3 offset;

    void LateUpdate()
    {
        transform.position = target.position + offset;
    }
}
*/

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

        transform.rotation = initialRotation; //keeps level
    }
}
