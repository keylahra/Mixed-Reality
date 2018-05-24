using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceFloe : MonoBehaviour {

    private Vector3 position;

    private int number;

    private enum floeColor
    {
        Orange = 0,
        Purple = 1,
        Green = 3,
    }

    private bool userEntered = false;

    private void OnTriggerEnter(Collider other)
    {
        
    }

    void Start () {
		
	}
	

	void Update () {
		
	}



    public int GetID()
    {
        return number;
    }

    public void SetID(int id)
    {
        number = id;
    }

    public Vector3 GetPosition()
    {
        return position;
    }

    public void SetPosition(Vector3 pos)
    {
        position = pos;
    }
}
