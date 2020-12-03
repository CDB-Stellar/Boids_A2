using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidController : MonoBehaviour
{
    // Public Variables
    public GameObject creator; //reference the creator for position info
    public float maxForce = 2f; //so that the boids don't go out of control
    public float maxVelocity = 5f; //so that the boids don't go out of control

    // Private Variables
    private Vector2 location; //for boid movement
    private Vector2 velocity; //for boid movement
    private Vector2 mousePosition; //goal target
    private Vector2 totalForce; //made up of all 3 flocking principles

    Vector2 MoveDirection(Vector2 target) //vector pointing from where the boid is to where it needs to go (mouse location)
    {
        return (target - location);
    }

    public void ApplyForce(Vector2 f) //apply force to the boid (totalForce)
    {
        Vector3 force = new Vector3(f.x, f.y, 0);

        if (force.magnitude > maxForce)
        {
            // If the force is greater than the max allowed, Cap it
            force = force.normalized; //first set to a length of 1
            force *= maxForce; //then make it the maxForce
        }

        this.GetComponent<Rigidbody2D>().AddForce(force);

        if (this.GetComponent<Rigidbody2D>().velocity.magnitude > maxVelocity)
        {
            // If the velocity is greater than the max allowed, Cap it
            this.GetComponent<Rigidbody2D>().velocity = this.GetComponent<Rigidbody2D>().velocity.normalized;
            this.GetComponent<Rigidbody2D>().velocity *= maxVelocity;
        }
    }

    public Vector2 Alignment()
    {
        float relevantBoidsRange = creator.GetComponent<BoidCreator>().relevantBoidsRange;
        // Add vectors then divide by count to get the average direction of surrounding boids
        Vector2 sum = Vector2.zero; //for average direction 
        int count = 0;
        
        foreach (GameObject otherBoid in creator.GetComponent<BoidCreator>().boids) //go through all 30 boids
        {
            if (otherBoid == this.gameObject)
                continue; //ignore self
            
            float distanceFromOtherBoid = Vector2.Distance(location, otherBoid.GetComponent<BoidController>().location);

            if (distanceFromOtherBoid < relevantBoidsRange) //if within relevant distance
            {
                sum += otherBoid.GetComponent<BoidController>().velocity;
                count++;
            }
        }

        if (count > 0) //if there were other boids nearby find the average direction
        {
            sum /= count; //sum divided by count = average
            Vector2 moveTo = sum - velocity;
            return moveTo;
        }

        return Vector2.zero; //if there were no nearby boids return nothing
    }

    public Vector2 Cohesion()
    {
        float relevantBoidsRange = creator.GetComponent<BoidCreator>().relevantBoidsRange;
        // Add vectors then divide by count to get the average position of surrounding boids
        Vector2 sum = Vector2.zero;
        int count = 0;

        foreach (GameObject otherBoid in creator.GetComponent<BoidCreator>().boids) //go through all 30 boids
        {
            if (otherBoid == this.gameObject)
                continue; //ignore self
            
            float distanceFromOtherBoid = Vector2.Distance(location, otherBoid.GetComponent<BoidController>().location);

            if (distanceFromOtherBoid < relevantBoidsRange) //if within relevant distance
            {
                sum += otherBoid.GetComponent<BoidController>().location;
                count++;
            }
        }

        if (count > 0) //if there were other boids nearby find the average position
        {
            sum /= count; //sum divided by count = average
            return MoveDirection(sum); //vector towards the average position of the nearby group
        }

        return Vector2.zero; //if there were no nearby boids return nothing
    }

    public void Flocking()
    {
        // Use the 3 Principles to Guide the Boids
        location = this.transform.position;
        velocity = this.GetComponent<Rigidbody2D>().velocity;

        totalForce = (MoveDirection(mousePosition) + Alignment() + Cohesion()).normalized;
        //normalize it so that all boids have the same velocity

        ApplyForce(totalForce);
    }

    private void FixedUpdate()
    {
        Flocking();
        //check mouse position (converted to world point instead of pixel location)
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }
}