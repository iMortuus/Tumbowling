using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TagScene : MonoBehaviour
{
    void Awake(){
        StartCoroutine(Load());
    }    
   IEnumerator Load(){
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene("MainMenu");
   } 
}
