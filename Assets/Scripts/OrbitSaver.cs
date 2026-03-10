/* Will clean up and explain according to guidelines later...

Add this script to any TRAIL RENDERER (make sure NOT line render component)
assign itself in the inspector, name it something, then when the render is 
where you want it, press O key. It will save the positions in a scriptable
object (this is what OrbitData script is). Then, to display, make a game
object with a LINE renderer attached and put orbit loader on the game object
and populate the orbit data object and LINE renderer in the inspector. You
may have to mess with material and width.

*/

using UnityEngine;
using UnityEngine.InputSystem;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class OrbitSaver : MonoBehaviour
{
    public TrailRenderer trail;
    public string assetName = "OrbitData";

    void Update()
    {
#if UNITY_EDITOR
        if (Keyboard.current != null && Keyboard.current.oKey.wasPressedThisFrame)
        {
            SaveTrail();
        }
#endif
    }

    void SaveTrail()
    {
        if (trail == null)
        {
            Debug.LogError("TrailOrbitSaver: No TrailRenderer assigned.");
            return;
        }

        // Bake the trail into a mesh
        Mesh mesh = new Mesh();
        trail.BakeMesh(mesh, true);

        // Extract centerline points from the mesh
        Vector3[] verts = mesh.vertices;
        int vertCount = verts.Length;

        if (vertCount < 2)
        {
            Debug.LogError("TrailOrbitSaver: Trail mesh has too few vertices.");
            return;
        }

        // The mesh is a ribbon: pairs of vertices form each segment
        int pointCount = vertCount / 2;
        Vector3[] centerline = new Vector3[pointCount];

        for (int i = 0; i < pointCount; i++)
        {
            Vector3 a = verts[i * 2];
            Vector3 b = verts[i * 2 + 1];
            centerline[i] = (a + b) * 0.5f; // midpoint of the ribbon
        }

#if UNITY_EDITOR
        OrbitData data = ScriptableObject.CreateInstance<OrbitData>();
        data.points = centerline;

        string path = $"Assets/{assetName}.asset";
        AssetDatabase.CreateAsset(data, path);
        AssetDatabase.SaveAssets();

        Debug.Log($"Saved orbit with {centerline.Length} points to {path}");
#endif
    }
}
