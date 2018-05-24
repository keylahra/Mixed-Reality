using HoloToolkit.Unity.SpatialMapping;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class IceFloeManager : MonoBehaviour {


    private Vector3 sourcePosition;
    private Vector3 playerPosition;
    private float yPositionFloor = -0.67f;

    public float minDistanceBetweenFloes = 0.7f;
    public float maxDistanceBetweenFloes = 1.3f;

    public GameObject iceFloe;

    public int floatsToSpawn = 10;
    public float secondToWaitForSpawn = 5f;


    void Start () {

        playerPosition = Camera.main.transform.position;
        //playerPosition = new Vector3(0f,-0.67f,0f);
        StartCoroutine(WaitAndCreateFloes());
    }
	
	void Update () {

	}


    private IEnumerator WaitAndCreateFloes()
    {
        yield return new WaitForSecondsRealtime(secondToWaitForSpawn);
        CreateFloes();
        yPositionFloor = GameObject.Find("SpatialProcessing").GetComponent<SurfaceMeshesToPlanes>().FloorYPosition;
        print("floor pos:" + yPositionFloor);
    }

    private void CreateFloes()
    {
        //int switcher = 1; // switches from 1 to -1 to alternate x-positioning of the floes.
        sourcePosition = playerPosition;
        Vector3 newPosition = new Vector3();

        IceFloe floe = new IceFloe();

        if(SetNewPosition(sourcePosition, out newPosition))
        {
            floe = Instantiate(iceFloe, newPosition, Quaternion.identity).GetComponent<IceFloe>();
            floe.SetID(0);
            floe.SetPosition(newPosition);

            //print(newPosition + " " + floe.GetID());
        }

        for (int i = 1; i <= floatsToSpawn; i++)
        {
            float x = Random.Range(-maxDistanceBetweenFloes, maxDistanceBetweenFloes);
            float z = Random.Range(-maxDistanceBetweenFloes, maxDistanceBetweenFloes);
            sourcePosition = new Vector3(floe.GetPosition().x + x, yPositionFloor, floe.GetPosition().z + z);
            //print(i+" sourcePos" + sourcePosition);

            // if we could not find a new position, or if the distance between the old and new position is too small, pick new values and try again
            while (Vector3.Distance(sourcePosition, newPosition) < minDistanceBetweenFloes || !SetNewPosition(sourcePosition, out newPosition))
            {
                x = Random.Range(-maxDistanceBetweenFloes, maxDistanceBetweenFloes);
                z = Random.Range(-maxDistanceBetweenFloes, maxDistanceBetweenFloes);
                sourcePosition = new Vector3(floe.GetPosition().x + x, yPositionFloor, floe.GetPosition().z + z);
                //print(i + " distance: " + Vector3.Distance(sourcePosition, newPosition));
            }

            floe = Instantiate(iceFloe, newPosition, Quaternion.identity).GetComponent<IceFloe>();
            floe.SetID(i);
            floe.SetPosition(newPosition);

            print(newPosition + " " + floe.GetID());
        }
    }

    private bool SetNewPosition(Vector3 sourcePos, out Vector3 newPos)
    {
        NavMeshHit hit;
        if (NavMesh.SamplePosition(sourcePos, out hit, 1f, 1))
        {
            newPos = hit.position;
            return true;
        }
        else
        {
            newPos = sourcePos;
            return false;
        }
    }
}
