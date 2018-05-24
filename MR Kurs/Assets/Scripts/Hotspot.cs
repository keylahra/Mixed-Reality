using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hotspot : MonoBehaviour {

    public delegate void HotspotEntered();
    public static event HotspotEntered OnEntered;

    public delegate void HotspotExited();
    public static event HotspotExited OnExited;

    private AudioSource audioSource;

    playStepParticle particle;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        //OnEntered += PlayAudio;
        //OnEntered += ActivateParticles;

        particle = transform.parent.gameObject.GetComponent<playStepParticle>();
    }

    private void OnTriggerEnter(Collider other)
    {
        particle.PlayParticle();
        audioSource.Play();
        //if (OnEntered != null)
        //{
        //    OnEntered();
        //    }
    }

    private void ActivateParticles()
    {
        particle.PlayParticle();
    }

    private void PlayAudio()
    {
        audioSource.Play();
    }

    private void OnTriggerExit(Collider other)
    {
        if (OnExited != null)
        {
            OnExited();
        }
    }
}
