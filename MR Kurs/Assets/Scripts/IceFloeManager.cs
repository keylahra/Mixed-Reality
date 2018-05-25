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

    private Vector3 startPosition;
    private Vector3 lastPosition;

    public GameObject iceFloe;
    List<IceFloe> floeList;
    List<Vector3> newPosList;
    List<IceFloe> pathList;

    private float spawnDistance = 0.6f;
    private float DistanceBetweenFloes = 0.6f;

    public float secondToWaitForSpawn = 5f;

    private Vector3 newPosVec;

    public int whileLimit = 2000;

    void Start()
    {
        floeList = new List<IceFloe>();
        newPosList = new List<Vector3>();
        pathList = new List<IceFloe>();
        playerPosition = Camera.main.transform.position;
        sourcePosition = playerPosition;
        startPosition = playerPosition;
        lastPosition = new Vector3(0,0,0);
        //playerPosition = new Vector3(0f,-0.67f,0f);
        StartCoroutine(WaitAndCreateFloes());
    }

    void Update()
    {

    }


    private IEnumerator WaitAndCreateFloes()
    {
        yield return new WaitForSecondsRealtime(secondToWaitForSpawn);
        yPositionFloor = GameObject.Find("SpatialProcessing").GetComponent<SurfaceMeshesToPlanes>().FloorYPosition;
        newPosVec = new Vector3(0.3f, yPositionFloor, 0.52f);
        CreateFloes();
    }

    private void CreateFloes()
    {
        IceFloe floe = new IceFloe();
        int whileInt = 0;
        int lastId = 0;
        int listPos = 0;

        while (whileInt < whileLimit)
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

            if (listPos < floeList.Count)
            {
                sourcePosition = floeList[listPos].GetPosition();
                listPos++;
            }


        }

        CreatePath();
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

    private void CreatePath()
    {
        List<IceFloe> nextPathFloe = new List<IceFloe>();

        int nextPath = 0;

        //if(startposition == playerPosition){
        startPosition = floeList[0].GetPosition();
        lastPosition = floeList[0].GetPosition();
        for (int j = 0; j < floeList.Count -1; j++)
        {  
            if (Vector3.Distance(lastPosition, floeList[j].GetPosition()) <= 2f)
            {
                pathList.Add(floeList[j]);
                nextPath = Random.Range(0, pathList.Count-1);
                pathList[nextPath].SetIsGoodFloe(true);
                lastPosition = pathList[nextPath].GetPosition();
                pathList.Clear();
            }
            else
            {
            }
        }
        //}else{}
    }

}
