using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FoucaultPendulum : MonoBehaviour
{
    [Header("Anchor")]
    public Transform anchor;

    [Header("Pendulum Objects")]
    public Transform bob;
    public Transform rod;
    float rodThickness = 0.05f;

    [Range(0f, 45f)]
    float initialAngleDegrees = 10f;

    float length = 10f;
    float gravity = 9.81f;

    [Header("Earth Variables")]
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

        Vector3 direction = bobPos - anchorPos;
        float rodLength = direction.magnitude;
        rod.position = anchorPos + direction * 0.5f;
        Vector3 localAxis = Vector3.up;
        localAxis = Vector3.forward;
        rod.rotation = Quaternion.FromToRotation(localAxis, direction.normalized);
        Vector3 scale = Vector3.one;
        scale = new Vector3(rodThickness, rodThickness, rodLength / 1f);
        rod.localScale = scale;
    }

    void RecalculateAngularVelocity()
    {
        w = wEarth * Mathf.Sin(latitude * Mathf.Deg2Rad) * precessionScale;
        Debug.Log(w);
    }

    public void ResetSimulation()
    {
        time = 0f;
        float x = length * Mathf.Sin(theta0);
        float y = 0f;
        float z = -length * Mathf.Cos(theta0);
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
        latitudeLabel.text = $"{latitude:F1}°";
    }
}
