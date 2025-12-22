using System.Collections.Generic;
using UnityEngine;

public class OrbitSimulator : MonoBehaviour
{
    /* NOTES TO FIX:
     * Scale the prefabs so that they are close to the origin - that should fix the clipping problem
     * Do we want axial rotation?
     * 
     */

    [System.Serializable]
    public class Planet
    {
        public string name;
        public GameObject body;

        [Header("Orbital Elements (J2000)")]
        public double a;   // semi-major axis (km)
        public double e;   // eccentricity
        public double i;   // inclination (rad)
        public double w;   // argument of periapsis (rad)
        public double W;   // longitude of ascending node (rad)
        public double Mo;  // mean anomaly at epoch (rad)
    }

    [Header("Planets")]
    public Planet[] planets;
    public Planet[] scaledPlanets;
    public GameObject planetsParent;
    public GameObject scaledPlanetsParent;
    public bool scaled;

    [Header("Orbit Anchor")]
    public Transform orbitAnchor;

    [Header("Simulation Start Date")]
    public int startYear = 2004;
    public int startMonth = 4;
    public int startDay = 7;

    public enum TimeSpeed
    {
        OneSecondPerSecond,
        OneMinutePerSecond,
        OneHourPerSecond,
        OneDayPerSecond,
        OneWeekPerSecond,
        OneMonthPerSecond,
        OneYearPerSecond,
        OneDecadePerSecond
    }

    [Header("Playback")]
    public bool playSimulation = true;
    public TimeSpeed timeSpeed = TimeSpeed.OneDayPerSecond;

    [Header("Unity Scaling")]
    public float distanceScale = 1f / 1e8f;
    const double mu = 1.32712440018e11; // Sun GM (km^3 / s^2)
    double simulationTimeSeconds;
    double epochJD;

    // UNITY

    void Start()
    {
        epochJD = JulianDate(2004, 4, 7);
        ResetSimulation();
    }

    void Update()
    {
        if (playSimulation)
            simulationTimeSeconds += Time.deltaTime * GetTimeScaleSeconds();

        if (scaled)
        {
            scaledPlanetsParent.SetActive(true);
            planetsParent.SetActive(false);

            foreach (var planet in scaledPlanets)
            {
                if (planet.body != null)
                    planet.body.transform.position =
                        ComputeOrbitPosition(planet, simulationTimeSeconds);
            }
        }
        else
        {
            scaledPlanetsParent.SetActive(false);
            planetsParent.SetActive(true);

            foreach (var planet in planets)
            {
                if (planet.body != null)
                    planet.body.transform.position =
                        ComputeOrbitPosition(planet, simulationTimeSeconds);
            }
        }
    }

    double GetTimeScaleSeconds()
    {
        const double second = 1.0;
        const double minute = 60.0 * second;
        const double hour = 60.0 * minute;
        const double day = 24.0 * hour;

        switch (timeSpeed)
        {
            case TimeSpeed.OneSecondPerSecond:
                return second;

            case TimeSpeed.OneMinutePerSecond:
                return minute;

            case TimeSpeed.OneHourPerSecond:
                return hour;

            case TimeSpeed.OneDayPerSecond:
                return day;

            case TimeSpeed.OneWeekPerSecond:
                return 7.0 * day;

            case TimeSpeed.OneMonthPerSecond:
                return 30.0 * day;

            case TimeSpeed.OneYearPerSecond:
                return 365.25 * day;

            case TimeSpeed.OneDecadePerSecond:
                return 10.0 * 365.25 * day;

            default:
                return day;
        }
    }

    // ORBIT CALCULATION

    Vector3 ComputeOrbitPosition(Planet p, double t)
    {
        // Mean motion
        double n = System.Math.Sqrt(mu / (p.a * p.a * p.a));
        double M = p.Mo + n * t;

        // Solve Kepler
        double E = SolveKepler(p.e, M);

        // True anomaly
        double theta = 2.0 * System.Math.Atan(
            System.Math.Sqrt((1 + p.e) / (1 - p.e)) *
            System.Math.Tan(E / 2.0)
        );

        // Radius
        double r = p.a * (1 - p.e * p.e) /
                   (1 + p.e * System.Math.Cos(theta));

        // Heliocentric position (km)
        double x =
            r * (System.Math.Cos(p.W) * System.Math.Cos(p.w + theta)
            - System.Math.Sin(p.W) * System.Math.Sin(p.w + theta) * System.Math.Cos(p.i));

        double y =
            r * (System.Math.Sin(p.W) * System.Math.Cos(p.w + theta)
            + System.Math.Cos(p.W) * System.Math.Sin(p.w + theta) * System.Math.Cos(p.i));

        double z =
            r * (System.Math.Sin(p.w + theta) * System.Math.Sin(p.i));

        // Unity local position
        Vector3 localPos = new Vector3(
            (float)x * distanceScale,
            (float)z * distanceScale,
            (float)y * distanceScale
        );

        if (orbitAnchor != null)
            return orbitAnchor.TransformPoint(localPos);

        return localPos;
    }

    double SolveKepler(double e, double M)
    {
        M %= 2.0 * System.Math.PI;
        if (M < 0) M += 2.0 * System.Math.PI;

        double E = M;

        for (int i = 0; i < 30; i++)
        {
            double f = E - e * System.Math.Sin(E) - M;
            double fp = 1 - e * System.Math.Cos(E);
            E -= f / fp;
        }

        return E;
    }

    // DATE UTIL

    double JulianDate(int y, int m, int d)
    {
        return 367 * y
             - (7 * (y + (m + 9) / 12)) / 4
             + (275 * m) / 9
             + d + 1721013.5;
    }

    // RESET

    public void ResetSimulation()
    {
        double startJD = JulianDate(startYear, startMonth, startDay);
        simulationTimeSeconds = (startJD - epochJD) * 86400.0;
    }
}
