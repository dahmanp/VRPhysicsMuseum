using UnityEngine;

public class OrbitLoader : MonoBehaviour
{
    //VARIABLES----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    public OrbitData orbitData;
    public OrbitData defaultOrbitData;
    public LineRenderer lineRenderer;
    public Transform orbitAnchor;
    public Transform orbitAnchorDefault;

    // SIMULATION--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    void Start()
    {
        //-----------------NOTE TO FUTURE DEVS-----------------
        //Simple checks to see if orbitData, lineRenderer, or orbitData.points are null. Script doesn't work without 'em!
        if (orbitData == null || lineRenderer == null)
        {
            Debug.LogError("OrbitLoader: Missing references.");
            return;
        }

        if (orbitData.points == null || orbitData.points.Length == 0)
        {
            Debug.LogError("OrbitLoader: OrbitData has no points.");
            return;
        }
        setDefault();
    }

    public void setDefault()
    {
        //-----------------NOTE TO FUTURE DEVS-----------------
        //Sets the position of each of the data points to its default position. This is necessary with the way we did the orbit lines.
        lineRenderer.positionCount = orbitData.points.Length;

        for (int j = 0; j < orbitData.points.Length; j++)
        {
            orbitData.points[j] = defaultOrbitData.points[j];
        }

        lineRenderer.SetPositions(orbitData.points);
    }

    //-----------------NOTE TO FUTURE DEVS-----------------
    //Changes the angle of the orbit lines based on a given multiplier. This is used in a slider to allow players to simply move the handle to tilt the orbit lines.
    public void changeAngle(float multiplier)
    {
        setDefault();
        Vector3 pivot = orbitAnchor.position;
        Quaternion mult = Quaternion.Euler(0f, 0f, (multiplier));

        for (int j = 0; j < orbitData.points.Length; j++)
        {
            Vector3 offset = orbitData.points[j] - pivot;
            offset = mult * offset;             
            orbitData.points[j] = pivot + offset;
        }

        lineRenderer.SetPositions(orbitData.points);
    }
}
