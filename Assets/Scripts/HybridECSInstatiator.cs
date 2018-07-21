using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HybridECSInstatiator : MonoBehaviour
{
    [Header("General Settings:")]
    [SerializeField] float universeRadius;

    [Header("Sun:")]
    [SerializeField] GameObject sunPrefab;
    [SerializeField] Vector3 sunPosition;

    [Header("Stars:")]
    [SerializeField] GameObject starPrefab;
    [SerializeField] float minStarsize;
    [SerializeField] float maxStarsize;
    [SerializeField] int starsAmount;
    [SerializeField] [Range(0,100)] float minTwinkleFrequency;
    [SerializeField] [Range(0, 100)] float maxTwinkleFrequency;

    [Header("Orbital Elipses:")]
    [SerializeField] int elipseSegments;
    [SerializeField] float elipseWidth;
    [SerializeField] GameObject orbitalElipsePrefab;

    [Header("Planets:")]
    [SerializeField] List<Planet> planets = new List<Planet>();

    static HybridECSInstatiator instance;
    public static HybridECSInstatiator Instance { get { return instance; } }

    #region Cached Variables
    List<LineRenderer> activeOrbitalElipses = new List<LineRenderer>();
    #endregion

    GameObject sun;

    void Awake()
    {
        instance = this;
        PlaceSun();
        PlaceStars();
        PlacePlanets();
    }

    #region Sun
    void PlaceSun()
    {
        sun = Instantiate(sunPrefab,sunPosition,Quaternion.identity);
        GameObject sunParent = new GameObject();

        sunParent.transform.parent = this.transform;
        sunParent.name = "Sun";

        sun.transform.parent = sunParent.transform;
    }
    #endregion

    #region Stars
    void PlaceStars()
    {
        GameObject starParent = new GameObject();
        starParent.name = "Stars";
        starParent.transform.parent = this.transform;

        for(int i = 0; i < starsAmount; i++)
        {
            GameObject currentStar = Instantiate(starPrefab);
            currentStar.transform.parent = starParent.transform;

            currentStar.GetComponent<StarComponent>().twinkleFrequency = Random.Range(minTwinkleFrequency,maxTwinkleFrequency);

            float randomStarScale = Random.Range(minStarsize, maxStarsize);
            currentStar.transform.localScale = new Vector3(randomStarScale, randomStarScale, randomStarScale);
            currentStar.transform.position = Random.onUnitSphere * universeRadius;
            currentStar.SetActive(true);
        }
    }
    #endregion

    #region OrbitalElipses
    void DrawOrbitalElipse(LineRenderer line, OrbitalEllipse ellipse)
    {
        Vector3[] drawPoints = new Vector3[elipseSegments + 1];

        for(int i = 0; i < elipseSegments; i++)
        {
            Vector2 position = ellipse.Evaluate((float)i / (float)elipseSegments);
            drawPoints[i] = new Vector3(position.x, position.y, 0.0f);
        }
        drawPoints[elipseSegments] = drawPoints[0];

        line.useWorldSpace = false;
        line.positionCount = elipseSegments + 1;
        line.startWidth = elipseWidth;
        line.SetPositions(drawPoints);

        activeOrbitalElipses.Add(line);
    }

    #endregion

    #region Planets
    void PlacePlanets()
    {
        GameObject planetParent = new GameObject();
        planetParent.name = "Planets";
        planetParent.transform.parent = this.transform;

        for (int i = 0; i < planets.Count; i++)
        {
            GameObject currentPlanet = Instantiate(planets[i].planetPrefab);
            currentPlanet.transform.parent = planetParent.transform;

            currentPlanet.GetComponent<PlanetComponent>().rotationSpeed = planets[i].rotationSpeed;
            currentPlanet.GetComponent<PlanetComponent>().orbitDuration = planets[i].orbitDuration;
            currentPlanet.GetComponent<PlanetComponent>().orbit = planets[i].orbit;

            // Draw orbit
            GameObject currentElipse = Instantiate(orbitalElipsePrefab, sunPosition, Quaternion.identity);
            currentElipse.transform.parent = sun.transform;
            DrawOrbitalElipse(currentElipse.GetComponent<LineRenderer>(), planets[i].orbit);
        }
    }
    #endregion
}

[System.Serializable]
public class OrbitalEllipse
{
    public float xExtent;
    public float yExtent;

    public OrbitalEllipse(float _xExtent, float _yExtent)
    {
        xExtent = _xExtent;
        yExtent = _yExtent;
    }

    public Vector2 Evaluate(float _t)
    {
        float angle = Mathf.Deg2Rad * 360f * _t;

        float x = Mathf.Sin(angle) * xExtent;
        float y = Mathf.Cos(angle) * yExtent;

        return new Vector2(x, y);
    }
}
[System.Serializable]
public class Planet
{
    public GameObject planetPrefab;
    public OrbitalEllipse orbit;

    [Header("Movement Settings:")]
    public float rotationSpeed;
    public float orbitDuration;
}
