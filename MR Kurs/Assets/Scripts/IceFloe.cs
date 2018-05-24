using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceFloe : MonoBehaviour {

    private Vector3 position;

    private int number;

    //private enum floeColor
    //{
    //    Orange = 0,
    //    Purple = 1,
    //    Green = 3,
    //}

    private AudioSource audioSource;

    playStepParticle particle;

    public bool userEntered = false;
    public bool isVisible = true;

    private MeshRenderer mesh;
    private PlayerManager manager;

    private void OnTriggerEnter(Collider other)
    {
        particle.PlayParticle();
        audioSource.Play();
        userEntered = true;
        manager.SendMessage("FloeEnter", number, SendMessageOptions.RequireReceiver);
    }

    private void OnTriggerExit(Collider other)
    {
        userEntered = false;
        manager.SendMessage("FloeExit", number);
    }

    void Start () {

        mesh = this.transform.Find("Spindle001").gameObject.GetComponent<MeshRenderer>();
        manager = GameObject.Find("Manager").GetComponent<PlayerManager>();

        audioSource = GetComponent<AudioSource>();

        particle = GetComponent<playStepParticle>();
    }
	

	void Update () {

        if (isVisible)
        {
            mesh.enabled = true;
        }
        else
        {
            mesh.enabled = false;
        }
    }


    public void SetVisible(bool visible)
    {
        isVisible = visible;
    }

    private void ActivateParticles()
    {
        particle.PlayParticle();
    }

    private void PlayAudio()
    {
        audioSource.Play();
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
