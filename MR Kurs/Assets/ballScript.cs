using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ballScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
        GetComponent<Rigidbody>().useGravity = false;
        StartCoroutine(wait());
	}

    private IEnumerator wait()
    {
        yield return new WaitForSecondsRealtime(8);
        GetComponent<Rigidbody>().useGravity = true;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
