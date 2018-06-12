using HoloToolkit.Unity.SpatialMapping;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour {

    private GameObject gameOverUI;
    private GameObject loadingUI;
    private GameObject finishUI;
    public AudioSource gameOverAudio;
    public AudioSource spawnAudio;

    //public delegate void PlayerDied();
    //public static event PlayerDied OnPlayerDeath;

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

        loadingUI = GameObject.Find("UI").transform.Find("Loading").gameObject;
        gameOverUI = GameObject.Find("UI").transform.Find("Game Over").gameObject;
        finishUI = GameObject.Find("UI").transform.Find("Finish").gameObject;
        StartCoroutine(ActivationRoutine());
    }

    private IEnumerator ActivationRoutine()
    {
        yield return new WaitForSeconds(7);
        loadingUI.SetActive(false);
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
        if(id == iceFloeManager.finalFloeID)
        {
            Finish();
        }
        currentFloeID = id;
        waitingForDeath = false;
    }

    // Player exits a "good" floe or enters a "bad" floe
    void FloeExit(int id)
    {
        currentFloeID = -1;
        if (!playerDead && !waitingForDeath)
        {
            waitingForDeath = true;
        }

    }

    public void Reset()
    {
        iceFloeManager.Reset();
        playerDead = false;
        waitingForDeath = false;
        ResetUI();
    }

    private void ResetUI()
    {
        gameOverUI.SetActive(false);
        loadingUI.SetActive(false);
        finishUI.SetActive(false);
    }

    private void PlayerDeathFeedback()
    {    
        if (!playerDead)
        {
            playerDead = true;
            waitingForDeath = false;
            print("you are dead.");
            gameOverUI.SetActive(true);

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

    private void Finish()
    {
        print("finish!");
        finishUI.SetActive(true);
    }
}
