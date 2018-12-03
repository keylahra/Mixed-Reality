using UnityEngine;

public class IceFloe : MonoBehaviour {

    // individual ID, stays the same during the game
    private int floeID = -1;

    // -1 if floe is not part of the path, otherwise pathID
    private int pathID = -1;

    // true if floe is part of the path
    private bool isGoodFloe = false;

    private Vector3 position;

    // different colors
    public Color pathColor;
    public Color badColor;
    private Color originalColor;

    // other components
    private AudioSource audioSource;
    private playStepParticle particle;
    private MeshRenderer mesh;
    private Renderer rend;

    private PlayerManager manager;

    private void Awake()
    {
        rend = this.transform.Find("Spindle001").gameObject.GetComponent<Renderer>();
        originalColor = rend.material.color;
    }

    void Start () {

        manager = GameObject.Find("Manager").GetComponent<PlayerManager>();
        audioSource = GetComponent<AudioSource>();
        particle = GetComponent<playStepParticle>();
    }

    private void OnTriggerEnter(Collider other)
    {
        audioSource.Play();

        // send message to PlayerManager with the entered IceFloe as a parameter
        manager.SendMessage("FloeEnter", this, SendMessageOptions.RequireReceiver);

        if (isGoodFloe)
        {
            particle.PlayParticle(true);
        }
        else
        {
            particle.PlayParticle(false);
            ChangeToBadColor();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        manager.SendMessage("FloeExit", floeID);
    }

    public void Reset()
    {
        ChangeColor(false);
        isGoodFloe = false;
        pathID = -1;
    }

    private void ChangeToBadColor()
    {
        Material mat = rend.material;
        mat.color = badColor;
    }

    public void ChangeColor(bool isPath)
    {
        if (isPath)
        {
            Material mat2 = rend.material;
            mat2.color = pathColor;
        }
        else
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

    public int GetPathID()
    {
        return pathID;
    }

    public void SetPathID(int id)
    {
        pathID = id;
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
