using UnityEngine;
using UnityEngine.InputSystem;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class OrbitSaver : MonoBehaviour
{
    //VARIABLES----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    
    public TrailRenderer trail;
    public string assetName = "OrbitData";

    // SIMULATION--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    //Add this script to any TRAIL RENDERER (make sure NOT line render component)
    //assign itself in the inspector, name it something, then when the render is 
    //where you want it, press O key, this will stop the simulation. It will save the positions in a scriptable
    //object (this is what OrbitData script is). Then, to display, make a game
    //object with a LINE RENDERER as a child and put orbit loader on the game object
    //and populate the orbit data object and new child line renderer in the inspector. 

    //Another note: I messed with the width of the renderers in the inspector to make them appear the same width even if they were far away.

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

        Mesh mesh = new Mesh();
        trail.BakeMesh(mesh, true);

        Vector3[] verts = mesh.vertices;
        int vertCount = verts.Length;

        if (vertCount < 2)
        {
            Debug.LogError("TrailOrbitSaver: Trail mesh has too few vertices.");
            return;
        }

        int pointCount = vertCount / 2;
        Vector3[] centerline = new Vector3[pointCount];

        for (int i = 0; i < pointCount; i++)
        {
            Vector3 a = verts[i * 2];
            Vector3 b = verts[i * 2 + 1];
            centerline[i] = (a + b) * 0.5f;
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
