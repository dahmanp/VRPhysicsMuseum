using UnityEngine;

[ExecuteInEditMode]
public class BakeOrbitLines : MonoBehaviour
{
    [Header("References")]
    public Transform centerBody;               // Usually the Sun
    public Transform[] orbitingBodies;         // Planets
    public LineRenderer[] lineRenderers;       // Must match planet order

    [Header("Orbit Settings")]
    public int segments = 360;


    void Start()
    {
        if (!Application.isPlaying)
        {
            BakeOrbits();
        }
    }


    public void BakeOrbits()
    {
        for (int i = 0; i < orbitingBodies.Length; i++)
        {
            if (orbitingBodies[i] == null || lineRenderers[i] == null)
                continue;

            float radius = Vector3.Distance(
                orbitingBodies[i].position,
                centerBody.position
            );

            LineRenderer lr = lineRenderers[i];
            lr.positionCount = segments + 1;

            for (int j = 0; j <= segments; j++)
            {
                float angle = (float)j / segments * Mathf.PI * 2f;

                Vector3 pos = new Vector3(
                    Mathf.Cos(angle) * radius,
                    0f,
                    Mathf.Sin(angle) * radius
                );

                lr.SetPosition(j, centerBody.position + pos);
            }
        }

        Debug.Log("Orbit lines baked.");
    }
}
