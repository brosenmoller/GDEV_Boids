using UnityEngine;

public class Boid
{
    public GameObject gameObject;
    public Vector3 direction;
    public Vector3 position { get { return gameObject.transform.position; } }

    public Boid(GameObject gameObject)
    {
        this.gameObject = gameObject;
        direction = Vector3.zero;
    }
}
