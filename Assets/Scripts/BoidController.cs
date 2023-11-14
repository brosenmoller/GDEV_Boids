using System.Collections.Generic;
using UnityEngine;

public class BoidController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject boidPrefab;
    [SerializeField] private Transform boundingCenter;

    [Header("Settings")]
    [SerializeField] private int boidSpawnSize = 5;
    [SerializeField] private float boidSpeed = 5.0f;
    [SerializeField] private float boidRange = 1f;
    [SerializeField] private int flockAmount = 1;
    [SerializeField] private float cohesionStrength = 1f;
    [SerializeField] private float seperationStrength = 1f;
    [SerializeField] private float aligningStrength = 1f;
    [SerializeField] private float boundingRuleStart = 15f;
    [SerializeField] private float boundingRadius = 20f;
    [SerializeField] private float boundingStrength = 1f;

    private Flock[] flocks;

    private Boid[] SpawnBoids(Vector3 spawnOffset, Transform flockParent)
    {
        List<Boid> newBoids = new();

        for (int x = 0; x < boidSpawnSize; x++)
        {
            for (int y = 0; y < boidSpawnSize; y++)
            {
                for (int z = 0; z < boidSpawnSize; z++)
                {
                    Boid newBoid = new(Instantiate(boidPrefab, new Vector3(x, y, z) + spawnOffset, Quaternion.identity, flockParent));
                    newBoids.Add(newBoid);
                }
            }
        }

        return newBoids.ToArray();
    }

    private void SpawnFlocks()
    {
        List<Flock> newFlocks = new();

        for (int i = 0; i < flockAmount; i++)
        {
            GameObject flockParent = new GameObject($"FlockParent: {i}");
            newFlocks.Add(new Flock(SpawnBoids(Vector3.one * i, flockParent.transform)));
        }

        flocks = newFlocks.ToArray();
    }

    private void Awake()
    {
        SpawnFlocks();
    }

    private void Update()
    {
        UpdateFlocks();
    }

    private void UpdateFlocks()
    {
        for (int i = 0; i < flocks.Length; i++)
        {
            Flock flock = flocks[i];

            flock.OnUpdate();
            for (int j = 0; j < flock.boids.Length; j++)
            {
                Boid boid = flock.boids[j];

                ApplyCohesionRule(boid, flock.averageBoidPosition);
                ApplySeperationRule(boid);
                ApplyAligningRule(boid, flock.averageBoidDirection);
                ApplyBoudingRule(boid);

                boid.gameObject.transform.Translate(boidSpeed * Time.deltaTime * boid.direction);
                boid.gameObject.transform.forward = boid.direction;
            }
        }
    }

    private void ApplyCohesionRule(Boid thisBoid, Vector3 averageBoidPosition)
    {
        Vector3 cohesionDirection = (averageBoidPosition - thisBoid.position).normalized;

        thisBoid.direction += cohesionDirection * cohesionStrength / (cohesionStrength + 1);
        thisBoid.direction.Normalize();
    }

    private void ApplySeperationRule(Boid thisBoid)
    {
        Vector3 seperationDirection = Vector3.zero;

        for (int i = 0; i < flockAmount; i++)
        {
            Flock flock = flocks[i];

            for (int j = 0; j < flock.boids.Length; j++)
            {
                Boid otherBoid = flock.boids[j];

                if (otherBoid.gameObject == thisBoid.gameObject) { continue; }

                if ((otherBoid.position - thisBoid.position).sqrMagnitude < boidRange)
                {
                    seperationDirection += thisBoid.position - otherBoid.position;
                }
            }
        }

        thisBoid.direction += seperationStrength * seperationDirection / (seperationStrength + 1);
        thisBoid.direction.Normalize();
    }

    private void ApplyAligningRule(Boid thisBoid, Vector3 averageBoidDirection)
    {
        thisBoid.direction += aligningStrength * averageBoidDirection / (aligningStrength + 1);
        thisBoid.direction.Normalize();
    }

    private void ApplyBoudingRule(Boid thisBoid)
    {
        Vector3 centerDirection = (boundingCenter.position - thisBoid.position).normalized;
        float distance = Vector3.Distance(boundingCenter.position, thisBoid.position);
        if (distance < boundingRuleStart) { return; }

        float distance01 = Mathf.Lerp(boundingRuleStart, boundingRadius, distance);

        thisBoid.direction += boundingStrength * distance01 * centerDirection / (boundingStrength + 1);
        thisBoid.direction.Normalize();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(boundingCenter.position, boundingRadius);
    }
}
