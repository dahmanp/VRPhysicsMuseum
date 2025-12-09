using System.Collections.Generic;
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
}
