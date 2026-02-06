using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FoucaultPendulum : MonoBehaviour
{
    public enum RodAxis { X, Y, Z }

    [Header("Anchor")]
    public Transform anchor;

    [Header("Pendulum Objects")]
    public Transform bob;
    public Transform rod;
    RodAxis rodAxis = RodAxis.Z;
    float rodThickness = 0.05f;

    [Range(0f, 45f)]
    float initialAngleDegrees = 10f;

    float length = 10f;
    float gravity = 9.81f;

    [Header("Earth / Location")]
    [Range(-90f, 90f)]
    public float latitude = 90f;
    public float precessionScale = 1000f;

    [Header("UI")]
    public TMP_Text latitudeLabel;
    public Slider latitudeSlider;

    float theta0;
    float wEarth;
    float w;
    float time;

    void Start()
    {
        if (anchor == null)
            anchor = transform;

        theta0 = initialAngleDegrees * Mathf.Deg2Rad;

        wEarth = 2f * Mathf.PI / (23f * 3600f + 56f * 60f + 4.0905f);

        RecalculateAngularVelocity();

        if (latitudeSlider != null)
        {
            latitudeSlider.minValue = -90f;
            latitudeSlider.maxValue = 90f;
            latitudeSlider.value = latitude;
        }

        UpdateLatitudeLabel();
    }

    void Update()
    {
        time += Time.deltaTime;

        float phi = w * time;
        float theta = theta0 * Mathf.Cos(Mathf.Sqrt(gravity / length) * time);

        float x = length * Mathf.Cos(phi) * Mathf.Sin(theta);
        float y = length * Mathf.Sin(phi) * Mathf.Sin(theta);
        float z = -length * Mathf.Cos(theta);

        Vector3 anchorPos = anchor.position;
        Vector3 bobPos = anchorPos + new Vector3(x, z, y);
        bob.position = bobPos;

        if (rod != null)
        {
            Vector3 direction = bobPos - anchorPos;
            float rodLength = direction.magnitude;

            rod.position = anchorPos + direction * 0.5f;

            Vector3 localAxis = Vector3.up;
            if (rodAxis == RodAxis.X) localAxis = Vector3.right;
            if (rodAxis == RodAxis.Z) localAxis = Vector3.forward;

            rod.rotation = Quaternion.FromToRotation(localAxis, direction.normalized);

            Vector3 scale = Vector3.one;
            switch (rodAxis)
            {
                case RodAxis.X:
                    scale = new Vector3(rodLength / 1f, rodThickness, rodThickness);
                    break;
                case RodAxis.Y:
                    scale = new Vector3(rodThickness, rodLength / 1f, rodThickness);
                    break;
                case RodAxis.Z:
                    scale = new Vector3(rodThickness, rodThickness, rodLength / 1f);
                    break;
            }

            rod.localScale = scale;
        }
    }

    void RecalculateAngularVelocity()
    {
        w = wEarth * Mathf.Sin(latitude * Mathf.Deg2Rad) * precessionScale;
    }

    public void ResetSimulation()
    {
        time = 0f;

        float theta = theta0;

        float x = length * Mathf.Sin(theta);
        float y = 0f;
        float z = -length * Mathf.Cos(theta);

        Vector3 anchorPos = anchor.position;
        Vector3 bobPos = anchorPos + new Vector3(x, z, y);
        bob.position = bobPos;
    }

    // UI

    public void UpdateLatitudeUI()
    {
        latitude = latitudeSlider.value;
        UpdateLatitudeLabel();
        RecalculateAngularVelocity();
        ResetSimulation();
    }

    void UpdateLatitudeLabel()
    {
        if (latitudeLabel != null)
            latitudeLabel.text = $"{latitude:F1}°";
    }
}
