using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialScreen : MonoBehaviour
{
    void Awake(){
        StartCoroutine(Game());
    }    
   IEnumerator Game(){
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene("GameScene");
   } 
}
