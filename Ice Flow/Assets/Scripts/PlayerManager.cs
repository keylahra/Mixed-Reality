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

    // how long can the player "stand" on a bad floe
    public float timeUntilPlayerDies = 2f;

    // game over because of the wrong floe, or because of being aside the right floes
    public bool onlyDieOnBadFloes;

    private bool playerDead = false;
    private bool waitingForDeath = false;
    private float timeTillDeathCounter = 0f;

    private IceFloeManager iceFloeManager;
    private List<IceFloe> iceFloePathList;

    private IceFloe currentFloe;
    private int currentFloeID = 0;
    private int currentLevel = 1;

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
        yield return new WaitForSeconds(iceFloeManager.secondsToWaitForSpawn + 0.2f);

        iceFloePathList = iceFloeManager.GetPathList();
        loadingUI.SetActive(false);
        spawnAudio.Play();
    }

    void Update () {

        if (!playerDead)
        {
            if (waitingForDeath)
            {
                timeTillDeathCounter += Time.deltaTime;
                if (timeTillDeathCounter >= timeUntilPlayerDies)
                {
                    PlayerDeathFeedback();
                }
            }
            else
            {
                timeTillDeathCounter = 0;
            }
        }
    }

    /// <summary>
    /// reaction to the step on a floe
    /// </summary>
    /// <param name="floe">entered floe</param>
    void FloeEnter(IceFloe floe)
    {
        int pathID = floe.GetPathID();

        if (iceFloePathList != null)
        {
            // enter good floe
            if (floe.GetIsGoodFloe())     
            {
                // enter final floe
                if (pathID == iceFloeManager.finalPathFloeID)
                {
                    currentFloeID = floe.GetID();
                    currentFloe = iceFloeManager.GetFloeList()[pathID];
                    Finish();
                }

                // player moved to the next floe -> change color (only after level 1)
                else if (pathID != currentFloeID && currentLevel > 1)
                {
                    // beginning of the path
                    if (currentFloeID == 0 || currentFloeID == 1)       // whole path was visible at the beginning, because player was standing on the first floe
                    {
                        for (int i = pathID + 1; i < iceFloePathList.Count - 1; i++)
                        {
                            iceFloePathList[i].ChangeColor(false);      // hide the color of all other path floes (except of the first two)
                        }

                        iceFloePathList[pathID].ChangeColor(true);      // change color of the floe the player stepped on to "good"

                        if(pathID + currentLevel <  iceFloePathList.Count)
                            iceFloePathList[pathID + currentLevel].ChangeColor(true);  // show the color of the floe after the next (gap becomes bigger with higher level)
                    }

                    // somewhere in between the path
                    else
                    {
                        iceFloePathList[pathID].ChangeColor(true);                          // change color of the floe the player stepped on to "good"
                        if (pathID + currentLevel-1 < iceFloePathList.Count -1)
                        {
                            iceFloePathList[pathID + currentLevel-1].ChangeColor(false);    // hide the color of the next floe
                            if (pathID + currentLevel < iceFloePathList.Count)
                                iceFloePathList[pathID + currentLevel].ChangeColor(true);   // show the color of the floe after the next (gap becomes bigger with higher level)
                        }
                    }
                }
                waitingForDeath = false;
            }

            // enter bad floe -> die
            else
            {
                waitingForDeath = true;
            }

            currentFloeID = floe.GetID();
            currentFloe = iceFloeManager.GetFloeList()[floe.GetID()];
       }
    }

    /// <summary>
    /// reaction when the player exits a floe
    /// </summary>
    /// <param name="id">floe ID</param>
    void FloeExit(int id)
    {
        if (!onlyDieOnBadFloes && !playerDead && !waitingForDeath)
        {
            waitingForDeath = true;
        }
    }

    /// <summary>
    /// resets parameters, UI,
    /// invokes reset function of the IceFloeManager
    /// </summary>
    public void Reset()
    {
        iceFloeManager.Reset(currentFloe);
        playerDead = false;
        waitingForDeath = false;
        currentFloeID = 0;
        ResetUI();
    }

    /// <summary>
    /// reset UI
    /// </summary>
    private void ResetUI()
    {
        gameOverUI.SetActive(false);
        loadingUI.SetActive(false);
        finishUI.SetActive(false);
    }

    /// <summary>
    /// invokes Game Over.
    /// shows UI, resets level to 1
    /// </summary>
    private void PlayerDeathFeedback()
    {    
        if (!playerDead)
        {
            playerDead = true;
            waitingForDeath = false;
            gameOverUI.SetActive(true);

            gameOverAudio.Play();
            currentLevel = 1;
        }
    }

    /// <summary>
    /// reaction after the player reached the finish floe
    /// increases level
    /// </summary>
    private void Finish()
    {
        finishUI.SetActive(true);
        spawnAudio.Play();
        playerDead = true;
        waitingForDeath = false;

        // only get to higher level if the path was longer than 4 floes.
        if(iceFloeManager.finalPathFloeID > 3)
            currentLevel++;
        print("finish! next level:" + currentLevel);
    }
}
