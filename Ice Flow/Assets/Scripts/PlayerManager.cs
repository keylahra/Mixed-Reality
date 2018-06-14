using HoloToolkit.Unity.SpatialMapping;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour {

    private GameObject gameOverUI;
    private GameObject loadingUI;
    private GameObject finishUI;
    //private GameObject errorUI;
    public AudioSource gameOverAudio;
    public AudioSource spawnAudio;

    //public delegate void PlayerDied();
    //public static event PlayerDied OnPlayerDeath;

    private string sceneName = "SpatialMapping2";

    public float timeUntilPlayerDies = 2f;
    public bool onlyDieOnBadFloes;

    private bool playerDead = false;
    private bool waitingForDeath = false;
    private float counter = 0f;

    private IceFloeManager iceFloeManager;
    private List<IceFloe> iceFloePathList;

    int currentFloeID = 0;

    void Start()
    {
        iceFloeManager = GetComponent<IceFloeManager>();

        loadingUI = GameObject.Find("UI").transform.Find("Loading").gameObject;
        gameOverUI = GameObject.Find("UI").transform.Find("Game Over").gameObject;
        finishUI = GameObject.Find("UI").transform.Find("Finish").gameObject;
        //errorUI = GameObject.Find("UI").transform.Find("Error").gameObject;

        StartCoroutine(ActivationRoutine());
    }

    private IEnumerator ActivationRoutine()
    {
        yield return new WaitForSeconds(iceFloeManager.secondToWaitForSpawn + 0.2f);

        iceFloePathList = iceFloeManager.GetPathList();
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

    // Player enters a floe
    void FloeEnter(int id)
    {
        if(iceFloePathList != null)
        {
            if (id >= 0)     // enter good floe
            {
                // enter final floe
                if (id == iceFloeManager.finalFloeID)
                {
                    Finish();
                }
                // player moved to the next floe
                if (id != currentFloeID)
                {
                    if (currentFloeID == 0)      // whole path was visible at the beginning, because player was standing on the first floe
                    {
                        for (int i = id + 1; i < iceFloePathList.Count; i++)
                        {
                            iceFloePathList[i].ChangeColor(false);      // hide the color of all other path floes (except of the first two)
                        }

                        iceFloePathList[id].ChangeColor(true);          // change color of the floe the player stepped on to "good"
                        if(id + 2 <  iceFloePathList.Count)
                            iceFloePathList[id + 2].ChangeColor(true);  // show the color of the floe after the next
                    }
                    else
                    {
                        iceFloePathList[id].ChangeColor(true);          // change color of the floe the player stepped on to "good"
                        if (id + 1 < iceFloePathList.Count)
                        {
                            iceFloePathList[id + 1].ChangeColor(false);     // hide the color of the next floe
                            if (id + 2 < iceFloePathList.Count)
                                iceFloePathList[id + 2].ChangeColor(true);      // show the color of the floe after the next
                        }
                    }
                }
                waitingForDeath = false;
            }
            else            // enter bad floe -> die
            {
                waitingForDeath = true;
            }
            currentFloeID = id;
        }

    }

    // Player exits a floe 
    void FloeExit(int id)
    {
        if (!onlyDieOnBadFloes && !playerDead && !waitingForDeath)
        {
            waitingForDeath = true;
        }
        currentFloeID = -2;
    }

    public void Reset()
    {
        iceFloeManager.Reset();
        playerDead = false;
        waitingForDeath = false;
        currentFloeID = 0;
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
        spawnAudio.Play();
        playerDead = true;
        waitingForDeath = false;
    }
}
