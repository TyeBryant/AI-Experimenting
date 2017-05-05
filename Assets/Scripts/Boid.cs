using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TKBAPI;

public class Boid : MonoBehaviour {

    private Rigidbody boidRB;

    private  Vector3 steeringVector;
    private  Vector3 avoidVector;

    private  List<GameObject> otherBoids = new List<GameObject>();

    public int boidScanRadius;

    public float authority;
    public float boidAversion = 5;

    IEnumerator FlockMe ()
    {
        yield return new WaitForSeconds(0.1f);
        BoidScan();
    }

	// Use this for initialization
	void Start () {
        boidRB = GetComponent<Rigidbody>();
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        boidRB.MovePosition(transform.position + (transform.forward * authority + steeringVector + avoidVector * boidAversion) * 0.1f);
        BoidSafeFlock();
        StartCoroutine(FlockMe());
	}

    void BoidScan ()
    {
        steeringVector = Vector3.zero;
        otherBoids.Clear();

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, boidScanRadius);
        foreach (Collider col in hitColliders)
        {
            if (col.GetComponent<Boid>())
            {
                otherBoids.Add(col.gameObject);
                steeringVector += col.transform.position;
            }
        }

        steeringVector /= hitColliders.Length;

        if (hitColliders.Length > 1)
            authority = 1 - 1 / (Vector3.Distance(steeringVector * 2, transform.position));
        else
            authority = 1;

        steeringVector -= transform.position;
    }

    void BoidSafeFlock ()
    {
        foreach (GameObject boidObj in otherBoids)
        {
            Vector3 myPos = transform.position;
            Vector3 otherPos = boidObj.transform.position;

            VectorAPI.constrain_3d_min_dist(ref myPos, ref otherPos, 1f, 0.5f, 0.3f);

            avoidVector = myPos - transform.position;
        }
}
}
