using System;
using UnityEngine;

public class floking : MonoBehaviour
{
    public Vector3 randomRotation;

    public float maxSpeed;

    public float minSpeed;

    public float CheckRadius;

    public float separationMultiplayer;

    public float coesionMultiplayer;

    public float alliasingMultiplayer;

    public Vector3 velocity;

    public Vector3 acceleration;

    // Start is called before the first frame update
    void Start()
    {
        float angle = UnityEngine.Random.Range(0, 2f * Mathf.PI);
        transform.rotation = Quaternion.Euler(new Vector3(0, 0,angle)+ randomRotation);
        velocity = new Vector3(Mathf.Cos(angle),Mathf.Sin(angle));
    }

    // Update is called once per frame
    void Update()
    {
        Collider2D[] collider = Physics2D.OverlapCircleAll(transform.position, CheckRadius);
        //floking[] boids =   collider.
    }
}
