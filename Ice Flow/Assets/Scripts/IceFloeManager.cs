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
    public int maxPathLength = 6;

    private Vector3 roomMaxX;
    private Vector3 roomMinX;
    private Vector3 roomMaxZ;
    private Vector3 roomMinZ;


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
        SetBoundingValues();

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
            // check if the position is still in the room (not behind a wall)
            if (hit.position.x > roomMaxX.x || hit.position.x < roomMinX.x || hit.position.z > roomMaxZ.z || hit.position.z < roomMinZ.z)
                return false;
            else
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

            int counter = 1;

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
                    if (!pathList.Contains(tempList[nextPath]))
                    {
                        pathList.Add(tempList[nextPath]);
                        counter++;
                    }
                    startPosition = tempList[nextPath].GetPosition();
                }
                tempList.Clear();

                if (counter >= maxPathLength)
                    break;
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

    private void SetBoundingValues()
    {
        List<GameObject> vertical = new List<GameObject>();
        vertical = SurfaceMeshesToPlanes.Instance.GetActivePlanes(PlaneTypes.Wall);

        float maxX = Camera.main.transform.position.x;
        float minX = maxX;
        float maxZ = Camera.main.transform.position.z;
        float minZ = maxZ;

        foreach (GameObject plane in vertical)
        {
            Vector3 negX = plane.transform.position;
            Vector3 posX = plane.transform.position;
            Vector3 negZ = plane.transform.position;
            Vector3 posZ = plane.transform.position;
            Vector3[] points = getColliderPoints(plane);

            // get the min/max values within this plane for x and z 
            foreach (Vector3 point in points)
            {
                if (point.x > posX.x)
                    posX = point;
                else if (point.x < negX.x)
                    negX = point;

                if (point.z > posZ.z)
                    posZ = point;
                else if (point.z < negZ.z)
                    negZ = point;
            }

            //negX = plane.transform.position.x - plane.transform.localScale.x / 2f;
            //posX = plane.transform.position.x + plane.transform.localScale.x / 2f;
            //negZ = plane.transform.position.z - plane.transform.localScale.z / 2f;
            //posZ = plane.transform.position.z + plane.transform.localScale.z / 2f;

            // compare the plane max/min values to the game max/min values
            if (posX.x > maxX)
            {
                roomMaxX = posX;
            }
            else if (negX.x < minX)
            {
                roomMinX = negX;
            }

            if (posZ.z > maxZ)
            {
                roomMaxZ = posZ;
            }
            else if (negZ.z < minZ)
            {
                roomMinZ = negZ;
            }
        }
        float adjust = 0.3f;
        roomMaxX.x -= adjust;
        roomMaxZ.z -= adjust;
        roomMinX.x += adjust;
        roomMinZ.z += adjust;
        print("roomMinX: " + roomMinX + "roomMaxX: " + roomMaxX + "roomMinZ: " + roomMinZ + "roomMaxZ: " + roomMaxZ);
        //GameObject cube1 = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //cube1.transform.position = roomMinX;
        //GameObject cube2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //cube2.transform.position = roomMinZ;
        //GameObject cube3 = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //cube3.transform.position = roomMaxX;
        //GameObject cube4 = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //cube4.transform.position = roomMaxZ;

    }

    private Vector3[] getColliderPoints(GameObject go)
    {
        Vector3[] verts = new Vector3[4];        // Array that will contain the BOX Collider Vertices
        BoxCollider b = go.GetComponent<BoxCollider>();

        verts[0] = go.transform.position + new Vector3(go.transform.localScale.x, -go.transform.localScale.y, go.transform.localScale.z) * 0.5f;
        verts[1] = go.transform.position + new Vector3(-go.transform.localScale.x, -go.transform.localScale.y, go.transform.localScale.z) * 0.5f;
        verts[2] = go.transform.position + new Vector3(-go.transform.localScale.x, -go.transform.localScale.y, -go.transform.localScale.z) * 0.5f;
        verts[3] = go.transform.position + new Vector3(go.transform.localScale.x, -go.transform.localScale.y, -go.transform.localScale.z) * 0.5f;

        return verts;
    }

    public List<IceFloe> GetPathList()
    {
        return pathList;
    }
}
