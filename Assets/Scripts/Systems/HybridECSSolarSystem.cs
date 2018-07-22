using UnityEngine;
using Unity.Entities;

public class HybridECSSolarSystem : ComponentSystem
{
    struct Stars
    {
        public StarComponent starComponent;
        public MeshRenderer renderer;
    }

    struct Planets
    {
        public PlanetComponent planetComponent;
        public Transform transform;
    }

    struct Moons
    {
        public Transform transform;
        public MoonComponent moonComponent;
    }

    protected override void OnUpdate()
    {
        foreach (var starEntity in GetEntities<Stars>())
        {
            int timeAsInt = (int)Time.time;
            if(Random.Range(1f, 100f) < starEntity.starComponent.twinkleFrequency)
            {
                starEntity.renderer.enabled = timeAsInt % 2 == 0;
            }
        }


        foreach (var planetEntity in GetEntities<Planets>())
        {
            
            planetEntity.transform.Rotate(Vector3.up * Time.deltaTime * planetEntity.planetComponent.rotationSpeed, Space.Self);
            
            planetEntity.transform.position = planetEntity.planetComponent.orbit.Evaluate(Time.time / planetEntity.planetComponent.orbitDuration);
        }

        foreach (var moonEntity in GetEntities<Moons>())
        {
            Vector3 parentPos = moonEntity.moonComponent.parentPlanet.transform.position;

            Vector3 desiredPos = (moonEntity.transform.position - parentPos).normalized * 5f + parentPos;

            moonEntity.transform.position = Vector3.MoveTowards(moonEntity.transform.position, desiredPos, moonEntity.moonComponent.movementSpeed);
            moonEntity.transform.RotateAround(moonEntity.moonComponent.parentPlanet.transform.position, Vector3.up, moonEntity.moonComponent.movementSpeed);

        }

    }
    
}
