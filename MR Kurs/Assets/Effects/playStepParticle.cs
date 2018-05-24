using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playStepParticle : MonoBehaviour {

	public GameObject particlesObject;
	public int destroyDelay = 4;
	private ParticleSystem particles;
	
	void Start () {

	}

    public void PlayParticle()
    {
        particles = particlesObject.GetComponentInChildren<ParticleSystem>();
        GameObject particlesObjectClone = Instantiate(particlesObject, transform.position, transform.rotation);
        particles.Play();
        Destroy(particlesObjectClone, destroyDelay);
    }
}
