using UnityEngine;

public class Flock
{
    public Boid[] boids;
    public Vector3 averageBoidPosition;
    public Vector3 averageBoidDirection;

    public Flock(Boid[] boids)
    {
        this.boids = boids;
    }

    public void CalculateAverageBoidPosition()
    {
        Vector3 totalPosition = Vector3.zero;
        for (int i = 0; i < boids.Length; i++)
        {
            Boid boid = boids[i];
            totalPosition += boid.position;
        }

        averageBoidPosition = (totalPosition / boids.Length).normalized;
    }

    public void CalculateAverageBoidDirection()
    {
        Vector3 totalVelocity = Vector3.zero;
        for (int i = 0; i < boids.Length; i++)
        {
            Boid boid = boids[i];
            totalVelocity += boid.direction.normalized;
        }

        averageBoidDirection = (totalVelocity / boids.Length).normalized;
    }

    public void OnUpdate()
    {
        CalculateAverageBoidPosition();
        CalculateAverageBoidDirection();
    }
}
