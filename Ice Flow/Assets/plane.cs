using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class plane : MonoBehaviour {

    GameObject kreis;
    GameObject cube;
    Rect rect;
    Collider cubeCol;

	// Use this for initialization
	void Start () {

        kreis = GameObject.Find("Sphere");
        //rect = new Rect(transform.position.x - transform.localScale.x/2f, transform.position.z - transform.localScale.z / 2f, transform.localScale.x, transform.localScale.z);


        Vector3 a = GameObject.Find("A").transform.position;
        Vector3 b = GameObject.Find("B").transform.position;
        Vector3 c = GameObject.Find("C").transform.position;
        Vector3 d = GameObject.Find("D").transform.position;

        CreateCube(a, b, c, d);
        cube = GameObject.Find("Cube");
        cubeCol = cube.GetComponent<Collider>();
        Vector3[] verts = getColliderPoints(cube);

        foreach (Vector3 point in verts)
        {
            GameObject s = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            s.transform.position = point;
        }
    }
	
	// Update is called once per frame
	void Update () {

        Vector3 kreisPos = kreis.transform.position;
        //print(cubeCol.bounds.Contains(kreisPos));
        print(ColliderContainsPoint(cubeCol.transform, kreisPos, true));
	}

    //void SetTarget(Vector3 source, Vector3 target)
    //{
    //    Vector3 direction = target - source;
    //    cube.transform.rotation = Quaternion.LookRotation(direction);
    //    cube.transform.localScale = new Vector3(1, 1, direction.magnitude);
    //    cube.transform.localPosition = new Vector3((source.x + target.x)/2f, (source.y + target.y) / 2f, (source.z + target.z) / 2f);
    //}

     // a ist links unten, beschriftung im uhrzeigersinn
    void CreateCube(Vector3 a, Vector3 b, Vector3 c, Vector3 d)
    {
        cube = GameObject.CreatePrimitive(PrimitiveType.Cube);

        Vector3 source = new Vector3((a.x + d.x) / 2f, (a.y + d.y) / 2f, (a.z + d.z) / 2f); // middle between a und d
        Vector3 target = new Vector3((b.x + c.x) / 2f, (b.y + c.y) / 2f, (b.z + c.z) / 2f); // middle between b und c
        Vector3 width = c - b;
        Vector3 direction = target - source; // also works as height

        cube.transform.rotation = Quaternion.LookRotation(direction);
        cube.transform.localScale = new Vector3(width.magnitude, 1, direction.magnitude);
        cube.transform.localPosition = new Vector3((source.x + target.x) / 2f, (source.y + target.y) / 2f, (source.z + target.z) / 2f);
    }

    private bool ColliderContainsPoint(Transform ColliderTransform, Vector3 Point, bool Enabled)
    {
        Vector3 localPos = ColliderTransform.InverseTransformPoint(Point);
        if (Enabled && Mathf.Abs(localPos.x) < 0.5f && Mathf.Abs(localPos.y) < 0.5f && Mathf.Abs(localPos.z) < 0.5f)
            return true;
        else
            return false;
    }

    private Vector3[] getColliderPoints(GameObject go)
    {
        Vector3[] verts = new Vector3[4];        // Array that will contain the BOX Collider Vertices
        //BoxCollider b = go.GetComponent<BoxCollider>();

        verts[0] = go.transform.position + new Vector3(go.transform.localScale.x, -go.transform.localScale.y, go.transform.localScale.z) * 0.5f;
        verts[1] = go.transform.position + new Vector3(-go.transform.localScale.x, -go.transform.localScale.y, go.transform.localScale.z) * 0.5f;
        verts[2] = go.transform.position + new Vector3(-go.transform.localScale.x, -go.transform.localScale.y, -go.transform.localScale.z) * 0.5f;
        verts[3] = go.transform.position + new Vector3(go.transform.localScale.x, -go.transform.localScale.y, -go.transform.localScale.z) * 0.5f;

        return verts;
    }

    //private Vector3[] Bla2(GameObject g)
    //{
    //    var vertices = new Vector3[8];
    //    var thisMatrix = g.transform.localToWorldMatrix;
    //    var storedRotation = g.transform.rotation;
    //    g.transform.rotation = Quaternion.identity;

    //    var extents = g.GetComponent<BoxCollider>().bounds.extents;
    //    vertices[0] = thisMatrix.MultiplyPoint3x4(extents);
    //    vertices[1] = thisMatrix.MultiplyPoint3x4(Vector3(-extents.x, extents.y, extents.z));
    //    vertices[2] = thisMatrix.MultiplyPoint3x4(Vector3(extents.x, extents.y, -extents.z));
    //    vertices[3] = thisMatrix.MultiplyPoint3x4(Vector3(-extents.x, extents.y, -extents.z));
    //    vertices[4] = thisMatrix.MultiplyPoint3x4(Vector3(extents.x, -extents.y, extents.z));
    //    vertices[5] = thisMatrix.MultiplyPoint3x4(Vector3(-extents.x, -extents.y, extents.z));
    //    vertices[6] = thisMatrix.MultiplyPoint3x4(Vector3(extents.x, -extents.y, -extents.z));
    //    vertices[7] = thisMatrix.MultiplyPoint3x4(-extents);

    //    g.transform.rotation = storedRotation;
    //    return vertices;
        

    //}

}
