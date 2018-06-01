using HoloToolkit.Unity.SpatialMapping;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour {

    private GameObject buttonParent;
    private GameObject loading;
    public AudioSource gameOverAudio;
    public AudioSource spawnAudio;

    //public delegate void PlayerDied();
    //public static event PlayerDied OnPlayerDeath;

    private bool playerOnFloe = true;
    private string sceneName = "SpatialMapping2";
    public float timeUntilPlayerDies = 2f;

    private bool playerDead = false;
    private bool waitingForDeath = false;
    private float counter = 0f;

    private IceFloeManager iceFloeManager;

    int currentFloeID = -1;

    void Start()
    {
        iceFloeManager = GetComponent<IceFloeManager>();

        loading = GameObject.Find("UI").transform.Find("SpatialUILoading").gameObject;
        buttonParent = GameObject.Find("UI").transform.Find("SpatialUI").gameObject;
        StartCoroutine(ActivationRoutine());
    }

    private IEnumerator ActivationRoutine()
    {
        yield return new WaitForSeconds(7);
        loading.SetActive(false);
        spawnAudio.Play();
    }

    
    private void OnEnable()
    {
        //OnPlayerDeath += PlayerDeathFeedback;
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
        if (!playerDead)
        {
            if (waitingForDeath)
            {
                counter += Time.deltaTime;
                if (counter >= timeUntilPlayerDies)
                {
                    PlayerDeathFeedback();
                }
            }
            else
            {
                counter = 0;
            }
        }
    }

    // Player enters a "good" floe
    void FloeEnter(int id)
    {
        currentFloeID = id;
        waitingForDeath = false;
    }

    // Player exits a "good" floe or enters a "bad" floe
    void FloeExit(int id)
    {
        currentFloeID = -1;
        if (!playerDead && !waitingForDeath)
        {
            //StartCoroutine(WaitForDeath());
            waitingForDeath = true;
        }

    }

    //private IEnumerator WaitForDeath()
    //{
    //    yield return new WaitForSecondsRealtime(timeUntilPlayerDies);
    //    if(currentFloeID < 0)
    //    {
    //        OnPlayerDeath();
    //        playerDead = true;
    //    }
    //}

    public void Reset()
    {
        iceFloeManager.Reset();
        playerDead = false;
        ResetUI();
    }

    private void ResetUI()
    {
        buttonParent.SetActive(false);
        loading.SetActive(false);
    }

    private void PlayerDeathFeedback()
    {    
        if (!playerDead)
        {
            playerDead = true;
            print("you are dead.");
            buttonParent.SetActive(true);

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
}
