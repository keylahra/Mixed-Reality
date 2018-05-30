using HoloToolkit.Unity.SpatialMapping;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour {

    public GameObject myObject;
    public GameObject button;
    public GameObject loading;
    public AudioSource gameOverAudio;
    public AudioClip gameOver;

    public delegate void PlayerDied();
    public static event PlayerDied OnPlayerDeath;

    private bool playerOnFloe = true;
    private string sceneName = "SpatialMapping2";
    public float timeUntilPlayerDies = 2f;

    private bool playerDead = false;

    int currentFloeID = -1;

    void Start()
    {
        gameOverAudio = GetComponent<AudioSource>();
        gameOverAudio.clip = gameOver;

        loading = GameObject.Find("InputManager").transform.Find("SpatialUILoading").gameObject;
        button = GameObject.Find("InputManager").transform.Find("SpatialUI").gameObject;
        StartCoroutine(ActivationRoutine());
    }

    private IEnumerator ActivationRoutine()
    {
        yield return new WaitForSeconds(7);
        loading.SetActive(false);
    }

    
    private void OnEnable()
    {
        //OnPlayerDeath += EndScene;
        OnPlayerDeath += PlayerDeathFeedback;
    }

    void Update () {

        //if(SpatialMappingManager.Instance.IsObserverRunning())
        //        {
        //    print ("observerrunning");
        //    // If running, Stop the observer by calling
        //    // StopObserver() on the SpatialMappingManager.Instance.
        //    //SpatialMappingManager.Instance.StopObserver();
        //}
        //else
        //{
        //    print("observer stopped");
        //}
    }

    void FloeEnter(int id)
    {
        currentFloeID = id;
    }

    void FloeExit(int id)
    {
        currentFloeID = -1;
        if(!playerDead)
            StartCoroutine(WaitForDeath());
    }

    private IEnumerator WaitForDeath()
    {
        yield return new WaitForSecondsRealtime(timeUntilPlayerDies);
        if(currentFloeID < 0)
        {
            OnPlayerDeath();
            playerDead = true;
        }
    }

    private void CheckIfPlayerOnFloe()
    {
    }

    private void PlayerDeathFeedback()
    {
        
        if (!playerDead)
        {   print("you are dead.");
            //myObject.SetActive(true);
            button.SetActive(true);
            gameOverAudio.Play();
        }


    }

    private void EndScene()
    {
        // start game over UI

        StartCoroutine(WaitAndReloadScene());
    }

    private IEnumerator WaitAndReloadScene()
    {
        yield return new WaitForSecondsRealtime(5);
        // reload scene
        SceneManager.LoadScene(sceneName);
    }

    public void FirePlayerDeath()
    {
        OnPlayerDeath();
    }

    public void SetPlayerDead(bool dead)
    {
        playerDead = dead;
    }
}
