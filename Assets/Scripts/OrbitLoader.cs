using UnityEngine;

public class OrbitLoader : MonoBehaviour
{
    public OrbitData orbitData;
    public OrbitData defaultOrbitData;
    public LineRenderer lineRenderer;
    Quaternion rotationVector = Quaternion.Euler(5f, 0f, 0f);
    //public int rotationVector = 10;

    void Start()
    {
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


        lineRenderer.positionCount = orbitData.points.Length;

        for (int j = 0; j < orbitData.points.Length; j++)
        {
            orbitData.points[j] = defaultOrbitData.points[j];
        }

        lineRenderer.SetPositions(orbitData.points);

    }

    public void changeAngle()
    {

        for (int j = 0; j < orbitData.points.Length; j++)
        {
            orbitData.points[j] = rotationVector * orbitData.points[j];
        }
        lineRenderer.SetPositions(orbitData.points);

        //rotationVector = Quaternion.Euler(tilt, 0f, 0f);
    }
}
