using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour {

    public delegate void PlayerDied();
    public static event PlayerDied OnPlayerDeath;

    private bool playerOnFloe = true;
    private string sceneName = "SpatialMapping2";

    void Start () {
		
	}

    private void OnEnable()
    {
        //OnPlayerDeath += EndScene;
        OnPlayerDeath += PlayerDeathFeedback;
    }

    void Update () {

        CheckIfPlayerOnFloe();
	}

    private void CheckIfPlayerOnFloe()
    {

    }

    private void PlayerDeathFeedback()
    {

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
