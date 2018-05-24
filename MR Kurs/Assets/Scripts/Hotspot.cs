using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hotspot : MonoBehaviour {

    public delegate void HotspotEntered();
    public static event HotspotEntered OnEntered;

    public delegate void HotspotExited();
    public static event HotspotExited OnExited;

    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        OnEntered += PlayAudio;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Entered!");
        if (OnEntered != null)
        {
            OnEntered();
            }
    }

    private void PlayAudio()
    {
        audioSource.Play();
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Exited!");
        if (OnExited != null)
        {
            OnExited();
        }
    }
}
