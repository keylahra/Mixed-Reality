using HoloToolkit.Unity.SpatialMapping;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class IceFloeManager : MonoBehaviour
{


    private Vector3 sourcePosition;
    private Vector3 playerPosition;
    private static float yPositionFloor = -0.67f;

    public GameObject iceFloe;
    List<IceFloe> floeList;
    List<Vector3> newPosList;

    private float spawnDistance = 0.6f;
    private float DistanceBetweenFloes = 0.6f;

    private Vector3 newPosVec = new Vector3(0.3f, yPositionFloor, 0.52f);

    public float secondToWaitForSpawn = 5f;



    void Start()
    {

        floeList = new List<IceFloe>();
        newPosList = new List<Vector3>();
        playerPosition = Camera.main.transform.position;
        sourcePosition = playerPosition;
        //playerPosition = new Vector3(0f,-0.67f,0f);
        StartCoroutine(WaitAndCreateFloes());
    }

    void Update()
    {

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

        IceFloe floe = new IceFloe();
        int whileInt = 0;
        int lastId = 0;
        int listPos = 0;

        while (whileInt < 100)
        {

        newPosList.Add(new Vector3(sourcePosition.x + spawnDistance, yPositionFloor, sourcePosition.z));
        newPosList.Add(new Vector3(sourcePosition.x + newPosVec.x, yPositionFloor, sourcePosition.z + newPosVec.z));
        newPosList.Add(new Vector3(sourcePosition.x - newPosVec.x, yPositionFloor, sourcePosition.z + newPosVec.z));
        newPosList.Add(new Vector3(sourcePosition.x - spawnDistance, yPositionFloor, sourcePosition.z));
        newPosList.Add(new Vector3(sourcePosition.x - newPosVec.x, yPositionFloor, sourcePosition.z - newPosVec.z));
        newPosList.Add(new Vector3(sourcePosition.x + newPosVec.x, yPositionFloor, sourcePosition.z - newPosVec.z));

            for (int i = 0; i < newPosList.Count; i++)
            {
                if (CheckNewPosition(newPosList[i]) && CompareToFloes(newPosList[i]))
                {
                    floe = Instantiate(iceFloe, newPosList[i], Quaternion.identity).GetComponent<IceFloe>();
                    floe.SetID(lastId + 1);
                    floe.SetPosition(newPosList[i]);
                    print(newPosList[i] + " " + floe.GetID());
                    floeList.Add((IceFloe)floe);
                }
                else
                {
                    whileInt++;
                }
            }
            newPosList.Clear();
            sourcePosition = floeList[listPos].GetPosition();
            listPos++;


        }
        
    }


    private bool CheckNewPosition(Vector3 sourcePos)
    {
        NavMeshHit hit;
        if (NavMesh.SamplePosition(sourcePos, out hit, 0.05f, 1))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool CompareToFloes(Vector3 sourcePos)
    {
        for (int i = 0; i < floeList.Count; i++)
        {
            if (Vector3.Distance(sourcePos, floeList[i].GetPosition()) < DistanceBetweenFloes)
            {
                return false;
            }
            else
            {
            }
        }
        return true;
    }

}
