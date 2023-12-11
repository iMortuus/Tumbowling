using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointsHandler : MonoBehaviour
{
    public Throwing throwingScript;
    public PlayerUIHandler uIScript;
    public float carriedPoints;
    public float totalPoints;
    public float timer;
    public bool strike;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;
        if(timer >= 5f){
            timer = 5f;
        }
        if(timer > 0f){
            uIScript.scoreTimer.enabled = true;
        }else if(timer <= 0f){
            uIScript.scoreTimer.enabled = false;
        }

        if(timer <= 0){
            timer = 5f;
            carriedPoints = 0f;
            strike = false;
            timer = 0f;
        }
        
        if(carriedPoints >= 10){
            throwingScript.totalThrows += 3;
            carriedPoints = 0;
        }
    }

    private void addCarriedPoints(float amount){
        carriedPoints += amount;
    }
    private void addTotalPoints(float amount){
        totalPoints += amount;
    }
}
