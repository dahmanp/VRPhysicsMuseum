using UnityEngine;

public class OrbitLoader : MonoBehaviour
{
    public OrbitData orbitData;
    public OrbitData defaultOrbitData;
    public LineRenderer lineRenderer;
    public Transform orbitAnchor;

    public Quaternion rotationVector = Quaternion.Euler(0f, 0f, 10f);

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
        Vector3 pivot = orbitAnchor.position;

        for (int j = 0; j < orbitData.points.Length; j++)
        {
            Vector3 offset = orbitData.points[j] - pivot;
            offset = rotationVector * offset;             
            orbitData.points[j] = pivot + offset;        
        }

        lineRenderer.SetPositions(orbitData.points);
    }
}
