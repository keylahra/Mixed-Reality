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

    public GameObject iceFloe;
    List<IceFloe> floeList;
    List<Vector3> newPosList;
    List<IceFloe> pathList;
    List<int> usedFieldsList;

    private float spawnDistance = 0.8f;
    private float distanceBetweenFloes = 0.6f;

    public float secondToWaitForSpawn = 5f;

    private Vector3 newPosVec;

    public int whileLimit = 2000;

    void Start()
    {
        floeList = new List<IceFloe>();
        newPosList = new List<Vector3>();
        pathList = new List<IceFloe>();
        usedFieldsList = new List<int>();

        StartCoroutine(WaitAndCreateFloes());
    }

    void Update()
    {

    }

    public void Reset()
    {
        foreach(IceFloe floe in floeList)
        {
            floe.Reset();
        }
        floeList.Clear();
        newPosList.Clear();
        pathList.Clear();
        usedFieldsList.Clear();
        newPosVec = new Vector3(0.3f, yPositionFloor, 0.52f);
        CreateFloes();
    }


    private IEnumerator WaitAndCreateFloes()
    {
        yield return new WaitForSecondsRealtime(secondToWaitForSpawn);
        yPositionFloor = GameObject.Find("SpatialProcessing").GetComponent<SurfaceMeshesToPlanes>().FloorYPosition;
        newPosVec = new Vector3(0.4f, yPositionFloor, 0.69f);
        CreateFloes();
    }

    private void CreateFloes()
    {
        IceFloe floe;
        int whileInt = 0;
        int lastId = 0;
        int listPos = 0;

        playerPosition = Camera.main.transform.position;
        sourcePosition = playerPosition;
        startPosition = playerPosition;

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
                    floe.SetID(lastId);
                    floe.SetPosition(newPosList[i]);
                    //print(newPosList[i] + " " + floe.GetID());
                    floeList.Add((IceFloe)floe);
                    lastId += 1;
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
            if (Vector3.Distance(sourcePos, floeList[i].GetPosition()) < distanceBetweenFloes)
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
        int nextPath = 0;

        // if floe list is not empty
        if (floeList.Count != 0)
        {
            startPosition = floeList[0].GetPosition();

            // first floe in path is the first floe that has been created (nearest to player)
            pathList.Add(floeList[0]);

            for (int x = 0; x <= 2000; x++)
            {
                for (int j = 1; j < floeList.Count - 1; j++)
                {
                    if (startPosition == floeList[0].GetPosition())
                    {
                        if (Vector3.Distance(startPosition, floeList[j].GetPosition()) <= 1f)
                        {
                            pathList.Add(floeList[j]);
                            usedFieldsList.Add(j);
                        }
                    }
                    else if (!usedFieldsList.Contains(j))
                    {
                        if (Vector3.Distance(startPosition, floeList[j].GetPosition()) <= 1f)
                        {
                            pathList.Add(floeList[j]);
                            usedFieldsList.Add(j);
                        }
                    }
                }
                nextPath = Random.Range(0, pathList.Count - 1);
                if (nextPath < pathList.Count)
                {
                    pathList[nextPath].SetIsGoodFloe(true);
                    startPosition = pathList[nextPath].GetPosition();
                }
                pathList.Clear();
            }
        }
        else
        {
            print("CreatePath(): floe list is empty");
        }
    }
}
