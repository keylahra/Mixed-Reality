using HoloToolkit.Unity.SpatialMapping;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class IceFloeManager : MonoBehaviour
{
    /// <summary>
    /// iceFloe prefab
    /// </summary>
    public GameObject iceFloe;

    /// <summary>
    /// parent object for new ice floes
    /// </summary>
    private GameObject iceFloeParent;

    // distances
    public float spawnDistance = 0.8f;
    public float secondToWaitForSpawn = 5f;

    private float distanceBetweenFloes = 0.6f;
    private float a; // = 0.4f if spawnDistance = 0.8f
    private float b; // = 0.69f if spawnDistance = 0.8f

    // positions
    private Vector3 sourcePosition;
    private Vector3 playerPosition;
    private Vector3 startPosition;
    private Vector3 newPosVec;
    private static float yPositionFloor = -0.67f;

    // lists
    private List<IceFloe> floeList;
    private List<Vector3> newPosList;
    private List<IceFloe> pathList;
    private List<int> usedFieldsList;

    public int whileLimit = 2000;

    [HideInInspector]
    public int finalFloeID = 9999;

    public bool paintPath;


    void Start()
    {
        floeList = new List<IceFloe>();
        newPosList = new List<Vector3>();
        pathList = new List<IceFloe>();
        usedFieldsList = new List<int>();

        iceFloeParent = GameObject.Find("Ice Floes");

        // calculate values for newPosVec, based on the Sinus Rule 
        float alpha = 0.5f;         // Mathf.Sin(30 * Mathf.PI / 180);  // alpha = 30°
        float beta = 0.8660254f;    // Mathf.Sin(60 * Mathf.PI / 180);  // beta = 60°
        float gamma = 1f;           // Mathf.Sin(90 * Mathf.PI / 180);  // gamma = 90°
        a = (spawnDistance / gamma) * alpha;
        b = (spawnDistance / gamma) * beta;

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
        newPosVec = new Vector3(a, yPositionFloor, b);
        CreateFloes();
    }


    private IEnumerator WaitAndCreateFloes()
    {
        yield return new WaitForSecondsRealtime(secondToWaitForSpawn);
        yPositionFloor = GameObject.Find("SpatialProcessing").GetComponent<SurfaceMeshesToPlanes>().FloorYPosition;
        newPosVec = new Vector3(a, yPositionFloor, b);
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
                    floe.SetPosition(newPosList[i]);
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
                        if (Vector3.Distance(startPosition, floeList[j].GetPosition()) <= (spawnDistance*1.25f))    // compare distance with a value that is relativ to the spawnDistance (sp.d.= 0.8 -> value = 1)
                        {
                            tempList.Add(floeList[j]);
                            usedFieldsList.Add(j);
                        }
                    }
                    else if (!usedFieldsList.Contains(j))
                    {
                        if (Vector3.Distance(startPosition, floeList[j].GetPosition()) <= (spawnDistance*1.25f))
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
                    startPosition = tempList[nextPath].GetPosition();
                }
                tempList.Clear();
            }

            // set isGood tags and ID of all floes in pathList
            foreach (IceFloe iceFloe in pathList)
            {
                iceFloe.SetIsGoodFloe(true);
                iceFloe.SetID(lastID);
                if(paintPath)
                    iceFloe.ChangeColor(true);
                lastID++;
            }

            finalFloeID = pathList.Count - 1;
        }
        else
        {
            print("CreatePath(): floe list is empty");
        }
    }


    public List<IceFloe> GetPathList()
    {
        return pathList;
    }
}
