using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceFloe : MonoBehaviour {

    private Vector3 position;
    private int floeID = -1;
    private bool isGoodFloe = false;

    private AudioSource audioSource;
    private playStepParticle particle;

    //public bool isVisible = true;

    private MeshRenderer mesh;
    private Renderer rend;
    private PlayerManager manager;


    private void OnTriggerEnter(Collider other)
    {
        audioSource.Play();

        if (isGoodFloe)
        {
            manager.SendMessage("FloeEnter", floeID, SendMessageOptions.RequireReceiver);
            particle.PlayParticle(true);
        }
        else
        {
            manager.SendMessage("FloeExit", floeID);
            particle.PlayParticle(false);
            BadReaction();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (isGoodFloe)
        {
            manager.SendMessage("FloeExit", floeID);
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

    public void Reset()
    {
        Destroy(this.gameObject);
        //Material mat = rend.material;
        //mat.color = normalColor;
        //isGoodFloe = false;
    }

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

    private void PlayAudio()
    {
        audioSource.Play();
    }
    public int GetID()
    {
        return floeID;
    }

    public void SetID(int id)
    {
        floeID = id;
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
        if (!isGood)
            floeID = -1;
    }
}
