using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidCreator : MonoBehaviour
{
    // Public Variables
    public GameObject[] boids; //array holds all boids
    [SerializeField] private GameObject boidPrefab; //what to duplicate
    [SerializeField] private int numberOfBoids = 30;
    [SerializeField] private Vector3 range = new Vector3(5, 5, 5); //creation range where the boids will be instantiated
    public int relevantBoidsRange = 50; //stay close to surrounding boids

    void Start()
    {
        boids = new GameObject[numberOfBoids];
        for (int i = 0; i < numberOfBoids; i++) //create 30 boids
        {
            Vector3 position = new Vector3(
                Random.Range(-range.x, range.x), Random.Range(-range.y, range.y), Random.Range(0, 0));
            //creation position will be random within the creation range
            boids[i] = Instantiate(boidPrefab, this.transform.position + position, Quaternion.identity) as GameObject;
            //this.transform.position = center of the creator object
            
            boids[i].GetComponent<BoidController>().creator = this.gameObject; //the boid knows that the creator created it
        }
    }
}