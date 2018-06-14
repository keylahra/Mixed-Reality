using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceFloe : MonoBehaviour {

    public Color pathColor;
    public Color badColor;
    private Color originalColor;

    private Vector3 position;
    private int floeID = -1;
    private bool isGoodFloe = false;
    private bool playerWasOnFloe = false;

    private AudioSource audioSource;
    private playStepParticle particle;

    //public bool isVisible = true;

    private MeshRenderer mesh;
    private Renderer rend;
    private PlayerManager manager;


    private void OnTriggerEnter(Collider other)
    {
        audioSource.Play();
        playerWasOnFloe = true;

        manager.SendMessage("FloeEnter", floeID, SendMessageOptions.RequireReceiver);

        if (isGoodFloe)
        {
            particle.PlayParticle(true);
        }
        else
        {
            particle.PlayParticle(false);
            BadReaction();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        manager.SendMessage("FloeExit", floeID);
    }

    private void Awake()
    {
        //mesh = this.transform.Find("Spindle001").gameObject.GetComponent<MeshRenderer>();
        rend = this.transform.Find("Spindle001").gameObject.GetComponent<Renderer>();
        originalColor = rend.material.color;
    }

    void Start () {

        manager = GameObject.Find("Manager").GetComponent<PlayerManager>();
        audioSource = GetComponent<AudioSource>();
        particle = GetComponent<playStepParticle>();
    }

    void Update () {

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
    }

    private void BadReaction()
    {
        Material mat = rend.material;
        mat.color = badColor;
    }

    public void ChangeColor(bool pathVisible)
    {
        if (pathVisible)
        {
            Material mat2 = rend.material;
            mat2.color = pathColor;
        }
        else if(!playerWasOnFloe)
        {
            Material mat2 = rend.material;
            mat2.color = originalColor;
        }

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
