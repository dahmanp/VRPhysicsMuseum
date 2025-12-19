/*using System.Collections.Generic;
using UnityEngine;

public class OrbitSimulator : MonoBehaviour
{
    [System.Serializable]
    public class Planet
    {
        public string name;
        public GameObject body;
        public double a;     // semi-major axis km
        public double e;     // eccentricity
        public double i;     // inclination rad
        public double w;     // argument of periapsis rad
        public double W;     // longitude of ascending node rad
        public double Mo;    // mean anomaly at epoch rad

        public List<Vector3> positions = new List<Vector3>();
    }

    public Planet[] planets;

    const double mu = 1.327e11;   // Sun GM constant

    double JulianDate(int y, int m, int d)
    {
        return (367 * y
                - (7 * (y + (m + 9) / 12)) / 4
                + (275 * m) / 9
                + d + 1721013.5);
    }

    private void Start()
    {
        double ta = JulianDate(2025, 9, 3);
        double tepoch = JulianDate(2000, 1, 1);

        double t = (ta - tepoch) * 86400.0;

        foreach (var planet in planets)
        {
            ComputeOrbit(planet, t);
        }
    }

    void ComputeOrbit(Planet p, double t0)
    {
        double period = 2.0 * Mathf.PI * Mathf.Sqrt((float)(p.a * p.a * p.a / mu));

        for (int j = 0; j < 1000; j++)
        {
            double dt = t0 + j * period / 100.0;

            double Mf = dt * System.Math.Sqrt(mu / (p.a * p.a * p.a));

            double E = SolveKepler(p.e, p.Mo + Mf);

            double theta = 2.0 * System.Math.Atan(
                System.Math.Sqrt((1 + p.e) / (1 - p.e)) * System.Math.Tan(E / 2));

            double r = p.a * (1 - p.e * p.e) / (1 + p.e * System.Math.Cos(theta));

            double x =
                r * (System.Math.Cos(p.W) * System.Math.Cos(p.w + theta)
                - System.Math.Sin(p.W) * System.Math.Sin(p.w + theta) * System.Math.Cos(p.i));

            double y =
                r * (System.Math.Sin(p.W) * System.Math.Cos(p.w + theta)
                + System.Math.Cos(p.W) * System.Math.Sin(p.w + theta) * System.Math.Cos(p.i));

            double z =
                r * (System.Math.Sin(p.w + theta) * System.Math.Sin(p.i));

            // Convert km → Unity units (scale down)
            float scale = 1f / 1e8f;

            p.positions.Add(new Vector3(
                (float)x * scale,
                (float)z * scale,
                (float)y * scale
            ));
        }
    }

    double SolveKepler(double e, double M)
    {
        // Newton-Raphson method
        double E = M;
        for (int k = 0; k < 30; k++)
        {
            double f = E - e * System.Math.Sin(E) - M;
            double fp = 1 - e * System.Math.Cos(E);
            E -= f / fp;
        }
        return E;
    }

    int index = 0;

    private void Update()
    {
        index++;
        if (index >= 1000) index = 0;

        foreach (var p in planets)
        {
            if (p.body != null)
                p.body.transform.position = p.positions[index];
        }
    }
}*/

using System.Collections.Generic;
using UnityEngine;

public class OrbitSimulator : MonoBehaviour
{
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

    [Header("Simulation Start Date")]
    public int startYear = 2025;
    public int startMonth = 9;
    public int startDay = 3;

    [Header("Playback")]
    public double timeScale = 86400.0; // 1 day per sec by default, this can be customizable
    public bool playSimulation = true;

    [Header("Unity Scaling")]
    public float distanceScale = 1f / 1e8f;

    const double mu = 1.32712440018e11; // Sun GM (km^3 / s^2)

    double simulationTimeSeconds;
    double epochJD;

    // UNITY

    void Start()
    {
        epochJD = JulianDate(2000, 1, 1);
        ResetSimulation();
    }

    void Update()
    {
        if (playSimulation)
            simulationTimeSeconds += Time.deltaTime * timeScale;

        /*foreach (var planet in planets)
        {
            if (planet.body != null)
            {
                planet.body.transform.position =
                    ComputeOrbitPosition(planet, simulationTimeSeconds);
            }
        }*/

        if (scaled)
        {
            scaledPlanetsParent.SetActive(true);
            planetsParent.SetActive(false);

            foreach (var planet in scaledPlanets)
            {
                if (planet.body != null)
                {
                    planet.body.transform.position =
                        ComputeOrbitPosition(planet, simulationTimeSeconds);
                }
            }
        }
        else
        {
            scaledPlanetsParent.SetActive(false);
            planetsParent.SetActive(true);

            foreach (var planet in planets)
            {
                if (planet.body != null)
                {
                    planet.body.transform.position =
                        ComputeOrbitPosition(planet, simulationTimeSeconds);
                }
            }
        }
    }

    // ORBIT CALCULATION

    Vector3 ComputeOrbitPosition(Planet p, double t)
    {
        double n = System.Math.Sqrt(mu / (p.a * p.a * p.a));
        double M = p.Mo + n * t;

        double E = SolveKepler(p.e, M);

        double theta = 2.0 * System.Math.Atan(
            System.Math.Sqrt((1 + p.e) / (1 - p.e)) *
            System.Math.Tan(E / 2.0)
        );

        double r = p.a * (1 - p.e * p.e) /
                   (1 + p.e * System.Math.Cos(theta));

        double x =
            r * (System.Math.Cos(p.W) * System.Math.Cos(p.w + theta)
            - System.Math.Sin(p.W) * System.Math.Sin(p.w + theta) * System.Math.Cos(p.i));

        double y =
            r * (System.Math.Sin(p.W) * System.Math.Cos(p.w + theta)
            + System.Math.Cos(p.W) * System.Math.Sin(p.w + theta) * System.Math.Cos(p.i));

        double z =
            r * (System.Math.Sin(p.w + theta) * System.Math.Sin(p.i));

        return new Vector3(
            (float)x * distanceScale,
            (float)z * distanceScale,
            (float)y * distanceScale
        );
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

