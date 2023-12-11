using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathScreen : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Restart(){
        SceneManager.LoadScene("GameScene");
    }
    public void MainMenu(){
        SceneManager.LoadScene("MainMenu");
    }
    public void Quit(){
        Application.Quit();
    }
}
