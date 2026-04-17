using UnityEngine;

public class OrbitLoader : MonoBehaviour
{
    public OrbitData orbitData;
    public LineRenderer lineRenderer;

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
        lineRenderer.SetPositions(orbitData.points);
        // try rotation matrix
    }
}
