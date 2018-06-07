﻿using HoloToolkit.Unity.SpatialMapping;
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
    private List<IceFloe> floeList;
    private List<Vector3> newPosList;
    private List<IceFloe> pathList;
    private List<int> usedFieldsList;
    private GameObject iceFloeParent;

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

        iceFloeParent = GameObject.Find("Ice Floes");

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
        startPosition = new Vector3(playerPosition.x, yPositionFloor, playerPosition.z);

        // create first floe exactly at player position (under his/her feet)
        floe = Instantiate(iceFloe, startPosition, Quaternion.identity, iceFloeParent.transform).GetComponent<IceFloe>();
        floe.SetPosition(startPosition);
        floeList.Add(floe);
        lastId += 1;

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
                    floe = Instantiate(iceFloe, newPosList[i], Quaternion.identity, iceFloeParent.transform).GetComponent<IceFloe>();
                    //floe = new IceFloe(newPosList[i], lastId, false);
                    //floe.SetID(lastId);
                    floe.SetPosition(newPosList[i]);
                    //print(newPosList[i] + " " + floe.GetID());
                    floeList.Add(floe);
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
        int lastID = 0;
        List<IceFloe> tempList = new List<IceFloe>();

        // if floe list is not empty
        if (floeList.Count != 0)
        {
            startPosition = floeList[0].GetPosition();

            // first floe in path is the first floe that has been created (nearest to player)
            floeList[0].SetIsGoodFloe(true);
            pathList.Add(floeList[0]);
            usedFieldsList.Add(0);

            for (int x = 0; x <= 2000; x++)
            {
                for (int j = 0; j < floeList.Count; j++)
                {
                    if (startPosition == floeList[0].GetPosition())
                    {
                        if (Vector3.Distance(startPosition, floeList[j].GetPosition()) <= 1f)
                        {
                            tempList.Add(floeList[j]);
                            usedFieldsList.Add(j);
                        }
                    }
                    else if (!usedFieldsList.Contains(j))
                    {
                        if (Vector3.Distance(startPosition, floeList[j].GetPosition()) <= 1f)
                        {
                            tempList.Add(floeList[j]);
                            usedFieldsList.Add(j);
                        }
                    }
                }

                // randomly choose which floe to add to the path
                nextPath = Random.Range(0, tempList.Count - 1);
                if (nextPath < tempList.Count)
                {
                    pathList.Add(tempList[nextPath]);
                    //pathList[nextPath].SetIsGoodFloe(true);
                    startPosition = tempList[nextPath].GetPosition();
                }
                tempList.Clear();
            }

            // set isGood tags and ID of all floes in pathList
            foreach (IceFloe iceFloe in pathList)
            {
                iceFloe.SetIsGoodFloe(true);
                iceFloe.SetID(lastID);
                lastID++;
            }
        }
        else
        {
            print("CreatePath(): floe list is empty");
        }
    }
}
