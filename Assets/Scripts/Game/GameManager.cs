using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public Transform player;
    public TextMeshProUGUI FinalPoints;
    public PointsHandler pointsHandler;
    public GameObject[] playerUI;
    public GameObject deathScreen;
    public GameObject pauseScreen;
    public GameObject tutorialScreen;
    public bool paused;
    public bool pauseTutorial;
    // Start is called before the first frame update
    private void Awake()
    {
        Time.timeScale = 1f;
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(playerIsDead()){
            Cursor.lockState = CursorLockMode.None;
            DisablePlayerUI();
            deathScreen.SetActive(true);
            FinalPoints.SetText((pointsHandler.totalPoints + " points").ToString());
        }
        if(Input.GetKeyDown(KeyCode.Escape) && !paused && !playerIsDead()){
            Pause();
        }else if (Input.GetKeyDown(KeyCode.Escape) && (paused || pauseTutorial)&& !playerIsDead()){
            Resume();
        }
    }

    public void Resume(){
        Cursor.lockState = CursorLockMode.Locked;
        EnablePlayerUI();
        pauseScreen.SetActive(false);
        tutorialScreen.SetActive(false);
        Time.timeScale = 1f;
        pauseTutorial = false;
        paused = false;
    }
    public void Pause(){
        paused = true;
        Cursor.lockState = CursorLockMode.None;
        DisablePlayerUI();
        pauseScreen.SetActive(true);
        Time.timeScale = 0f;
    }
    public bool playerIsDead(){
        return player.GetComponent<PlayerStateHandler>().states == PlayerStates.Dead;
    }

    void DisablePlayerUI(){
        foreach (var obj in playerUI){
            obj.SetActive(false);
        }
    }
    void EnablePlayerUI(){
        foreach (var obj in playerUI){
            obj.SetActive(true);
        }
    }
    public void Tutorial(){
        pauseScreen.SetActive(false);
        tutorialScreen.SetActive(true);
        pauseTutorial = true;
    }
}
