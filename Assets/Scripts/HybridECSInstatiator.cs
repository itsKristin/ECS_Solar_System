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
    [SerializeField] List<OrbitalElipse> sunOrbitalElipses;

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
        GameObject sun = Instantiate(sunPrefab,sunPosition,Quaternion.identity);
        GameObject sunParent = new GameObject();

        sunParent.transform.parent = this.transform;
        sunParent.name = "Sun";

        sun.transform.parent = sunParent.transform;

        foreach(OrbitalElipse elipse in sunOrbitalElipses)
        {
            GameObject currentElipse = Instantiate(orbitalElipsePrefab,sunPosition,Quaternion.identity);

            currentElipse.transform.parent = sun.transform;

            DrawOrbitalElipse(currentElipse.GetComponent<LineRenderer>(), elipse);
        }
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
    void DrawOrbitalElipse(LineRenderer _lineRenderer, OrbitalElipse _elipse)
    {
        Vector3[] drawPoints = new Vector3[elipseSegments + 1];

        for(int i = 0; i < elipseSegments; i++)
        {
            Vector2 position = _elipse.Evaluate((float)i / (float)elipseSegments);
            drawPoints[i] = new Vector3(position.x, position.y, 0.0f);
        }
        drawPoints[elipseSegments] = drawPoints[0];

        _lineRenderer.useWorldSpace = false;
        _lineRenderer.positionCount = elipseSegments + 1;
        _lineRenderer.startWidth = elipseWidth;
        _lineRenderer.SetPositions(drawPoints);

        activeOrbitalElipses.Add(_lineRenderer);
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
            LineRenderer assignedOrbitalElipse = activeOrbitalElipses[Random.Range(0, activeOrbitalElipses.Count)];

            GameObject currentPlanet = Instantiate(planets[i].planetPrefab);
            currentPlanet.transform.parent = planetParent.transform;

            currentPlanet.GetComponent<PlanetComponent>().rotationSpeed = planets[i].rotationSpeed;
            currentPlanet.GetComponent<PlanetComponent>().movementSpeed = planets[i].movementSpeed;
            currentPlanet.GetComponent<PlanetComponent>().orbitalElipseToFollow = assignedOrbitalElipse;

            int currentPathPosition = Random.Range(0, assignedOrbitalElipse.positionCount - 1);
            currentPlanet.GetComponent<PlanetComponent>().currentPathpoint = currentPathPosition;

            currentPlanet.transform.position = assignedOrbitalElipse.GetPosition(currentPathPosition);
            
        }
    }
    #endregion
}

[System.Serializable]
public class OrbitalElipse
{
    public float xExtent;
    public float yExtent;

    public OrbitalElipse(float _xExtent, float _yExtent)
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
    [Header("Prefab:")]
    public GameObject planetPrefab;

    [Header("Movement Settings:")]
    public float rotationSpeed;
    public float movementSpeed;

    [HideInInspector] public LineRenderer orbitalElipseToFollow;
}
