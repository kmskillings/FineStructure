using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exploder : MonoBehaviour
{
    public GameObject explosionParticles;

    public GameObject[] debrisChunks;
    public Vector3[] debrisOffsets;

    public float explosionForce;

    public Transform transformSource;
    public Rigidbody velocitySource;

    private void Awake()
    {
        if(debrisChunks.Length != debrisOffsets.Length)
        {
            throw new System.Exception("In the Exploder attached to " + gameObject.name + " the lists debrisChunks and debrisOffsets are of different lengths.");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Explode()
    {
        //Instantiates all the debris chunks
        for (int i = 0; i < debrisChunks.Length; i++)
        {
            GameObject chunk = Instantiate(debrisChunks[i]);
            chunk.transform.position = transformSource.TransformPoint(debrisOffsets[i]);
            chunk.transform.rotation = transformSource.rotation;
            Rigidbody chunkRigidBody = chunk.GetComponent<Rigidbody>();
            if(chunkRigidBody != null)
            {
                if (velocitySource != null)
                {
                    chunkRigidBody.velocity = velocitySource.velocity;
                    chunkRigidBody.angularVelocity = velocitySource.angularVelocity;
                }

                chunkRigidBody.velocity += (chunk.transform.position - transformSource.position).normalized * explosionForce;
                chunkRigidBody.angularVelocity += Random.insideUnitSphere;
                //chunkRigidBody.AddExplosionForce(explosionForce * 100, transformSource.position, 0);
            }
        }

        //Instantiates the particles
        Instantiate(explosionParticles, transformSource.position, transformSource.rotation);
    }
}
