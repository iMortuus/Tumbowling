using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafeZone : MonoBehaviour
{
    public Transform Barrier;
    public TinCan TinCan;
    public Transform player;
    public PointsHandler pointsHandler;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(player.GetComponent<Throwing>().totalThrows <= 0){
            Barrier.GetComponent<Collider>().enabled = false;
        }
    }
    private void OnTriggerEnter(Collider other){
        if(other.tag == "Player" && player.transform.GetComponent<Throwing>().totalThrows > 0){
            Barrier.GetComponent<Collider>().enabled = true;
            pointsHandler.totalPoints += pointsHandler.carriedPoints;
            player.transform.GetComponent<Throwing>().totalThrows += pointsHandler.carriedPoints;
            pointsHandler.carriedPoints = 0;
        }
    }
}
