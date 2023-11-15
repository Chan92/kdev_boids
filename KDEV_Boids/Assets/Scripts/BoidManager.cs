using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.EventSystems.EventTrigger;

public class BoidManager : MonoBehaviour {
    [SerializeField] 
    private Transform boidPrefab;
    [SerializeField]
    private int boidAmount = 20;

    [Space(10)]
    public float coheasionStrength = 100f;
    public float seperationStrength = 100f;
    public float alignmentStrength = 8f;
    public float moveSpeed = 10f;

    [Space(10)]
    public Vector3 fieldSize = new Vector3(25f, 15f, 5f);

    List<Boid> boids = new List<Boid>();
    Vector3 cohesion, seperation, alignment, boundPosition;


    private void Start() {
        CreateBoids();
    }

    private void Update() {
        UpdatePositions();
    }

    //Create boids and put them in a list
    private void CreateBoids() {
        for (int i = 0; i < boidAmount; i++) {
            Transform newBoid = Instantiate(boidPrefab);
            Boid b = newBoid.GetComponent<Boid>();

            float randomX = Random.Range(-fieldSize.x, fieldSize.x);
            float randomY = Random.Range(-fieldSize.y, fieldSize.y);
            float randomZ = Random.Range(0, fieldSize.z);
            //float randomZ = 0;
            newBoid.position = new Vector3(randomX, randomY, randomZ);

            newBoid.name = "Boid " + i;
            newBoid.parent = transform;
            boids.Add(b);
        }
    }

    //Calculate the movement
    private void UpdatePositions() {
        //Vector3 cohesion, seperation, alignment, boundPosition;

        foreach (Boid b in boids) {
            cohesion = Cohesion(b);
            seperation = Seperation(b);
            alignment = Alignment(b);

            boundPosition = BoundPosition(b);

            b.velocity = b.velocity + cohesion + seperation + alignment + boundPosition;
            b.velocity = b.velocity.normalized * moveSpeed;

            Vector3 newPos = b.transform.position + b.velocity * Time.deltaTime;
            b.transform.position = newPos;
        }
    }

    //rule 1: fly towards the center of neighbouring boids
    private Vector3 Cohesion(Boid currentBoid) {
        Vector3 perceivedCentre = Vector3.zero;

        foreach (Boid b in boids) {
            if (b != currentBoid) {
                perceivedCentre = perceivedCentre + b.transform.position;
            }
        }

        perceivedCentre = perceivedCentre / (boids.Count - 1);
        return (perceivedCentre - currentBoid.transform.position)/coheasionStrength;
    }

    //rule 2: keep small distance between other objects/boids
    private Vector3 Seperation(Boid currentBoid) {
        Vector3 collisionDistance = Vector3.zero;
        
        foreach (Boid b in boids) { 
            if (b != currentBoid) {               
                 if (Vector3.Distance(b.transform.position, currentBoid.transform.position) < seperationStrength) {
                    collisionDistance = collisionDistance - (b.transform.position - currentBoid.transform.position);
                }
            }
        }
        
        return collisionDistance;
    }

    //rule 3: match velocity with nearby boids
    private Vector3 Alignment(Boid currentBoid) {
        Vector3 perceivedVelocity = Vector3.zero;

        foreach (Boid b in boids) {
            if(b != currentBoid) {
                perceivedVelocity = perceivedVelocity + b.velocity;
            }
        }

        perceivedVelocity = perceivedVelocity / (boids.Count - 1);

        return (perceivedVelocity - currentBoid.velocity) / alignmentStrength;
    }

    private Vector3 BoundPosition(Boid b) {
        Vector3 v = Vector3.zero;

        if (b.transform.position.x < -fieldSize.x) {
            v.x = 10;
        } else if (b.transform.position.x > fieldSize.x) {
            v.x = -10;
        }

        if (b.transform.position.y < -fieldSize.y) {
            v.y = 10;
        } else if (b.transform.position.y > fieldSize.y) {
            v.y = -10;
        }

        if (b.transform.position.z < -fieldSize.z) {
            v.z = 10;
        } else if (b.transform.position.z > fieldSize.z) {
            v.z = -10;
        }

        return v;
    }
}
