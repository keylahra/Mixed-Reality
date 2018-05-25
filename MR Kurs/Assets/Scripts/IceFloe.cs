using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceFloe : MonoBehaviour {

    private Vector3 position;

    private int number;

    public bool isGoodFloe = true;

    //private enum floeColor
    //{
    //    Orange = 0,
    //    Purple = 1,
    //    Green = 3,
    //}

    private AudioSource audioSource;

    playStepParticle particle;

    //public bool isVisible = true;

    private MeshRenderer mesh;
    private Renderer rend;
    private PlayerManager manager;

    private void OnTriggerEnter(Collider other)
    {
        particle.PlayParticle();
        audioSource.Play();

        if (isGoodFloe)
        {
            manager.SendMessage("FloeEnter", number, SendMessageOptions.RequireReceiver);
        }
        else
        {
            manager.SendMessage("FloeExit", number);
            BadReaction();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (isGoodFloe)
        {
            manager.SendMessage("FloeExit", number);
        }
    }

    void Start () {

        //mesh = this.transform.Find("Spindle001").gameObject.GetComponent<MeshRenderer>();
        rend = this.transform.Find("Spindle001").gameObject.GetComponent<Renderer>();
        manager = GameObject.Find("Manager").GetComponent<PlayerManager>();

        audioSource = GetComponent<AudioSource>();

        particle = GetComponent<playStepParticle>();
    }
	

	void Update () {
        if (GetIsGoodFloe() == true)
        {
            PathColor();
        }
        //if (isVisible)
        //{
        //    mesh.enabled = true;
        //}
        //else
        //{
        //    mesh.enabled = false;
        //}
    }


    //public void SetVisible(bool visible)
    //{
    //    isVisible = visible;
    //}

    private void BadReaction()
    {
        Material mat = rend.material;
        mat.color = new Color(0.2311f, 0.4062f, 0.458f, 0.8117f);
        //rend.material.shader = Shader.Find("_Color");
        //rend.material.SetColor("_Color", new Color(59f,104f,117f,207f));
    }

    public void PathColor()
    {
        Material mat2 = rend.material;
        mat2.color = new Color(0.9529f, 0.4549f, 0.7433f, 0.8117f);
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

    public bool GetIsGoodFloe()
    {
        return isGoodFloe;
    }

    public void SetIsGoodFloe(bool isGood)
    {
        isGoodFloe = isGood;

    }
}
