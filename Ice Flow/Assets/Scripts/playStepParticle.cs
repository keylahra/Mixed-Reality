using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playStepParticle : MonoBehaviour {

	public GameObject particlesObject;
    public GameObject badParticlesObject;
    public int destroyDelay = 4;
	private ParticleSystem particles;
	
	void Start () {

	}

    public void PlayParticle(bool isBad)
    {
        if (!isBad)
        {
            particles = particlesObject.GetComponentInChildren<ParticleSystem>();
            GameObject particlesObjectClone = Instantiate(particlesObject, transform.position, transform.rotation);
            particles.Play();
            Destroy(particlesObjectClone, destroyDelay);
        }
        else
        {
            particles = badParticlesObject.GetComponentInChildren<ParticleSystem>();
            GameObject particlesObjectClone = Instantiate(badParticlesObject, transform.position, transform.rotation);
            particles.Play();
            Destroy(particlesObjectClone, destroyDelay);
        }
    }    
}
