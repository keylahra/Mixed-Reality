using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour {

    public GameObject myObject;

    public delegate void PlayerDied();
    public static event PlayerDied OnPlayerDeath;

    private bool playerOnFloe = true;
    private string sceneName = "SpatialMapping2";

    int currentFloeID = -1;

    void Start () {

  
    }

    private void OnEnable()
    {
        //OnPlayerDeath += EndScene;
        OnPlayerDeath += PlayerDeathFeedback;
    }

    void Update () {

	}

    void FloeEnter(int id)
    {
        print("FloeEnter");
        currentFloeID = id;
    }

    void FloeExit(int id)
    {
        print("FloeExit");
        currentFloeID = -1;
        StartCoroutine(WaitForDeath());
    }

    private IEnumerator WaitForDeath()
    {
        yield return new WaitForSecondsRealtime(1);
        if(currentFloeID < 0)
            OnPlayerDeath();
    }

    private void CheckIfPlayerOnFloe()
    {

    }

    private void PlayerDeathFeedback()
    {
        print("you are dead.");
        myObject.gameObject.SetActive(true);

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
}
