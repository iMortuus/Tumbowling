using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TinCan : MonoBehaviour
{
    [SerializeField] private float resetTime = 2f;
    public LayerMask whatIsFlipFlops;
    public Quaternion  originalRotationValue;
    public bool displaced;
    public bool gotDisplaced;
    public bool gotPickedUp;
    public bool scoreAdded;
    public Transform parent;
    public Transform Barrier;
    public Throwing throwingScript;
    public PointsHandler pointsHandler;
    public TargetArea targetArea;
    public Throwing throwScript;
    [SerializeField] private float timer;

    // Start is called before the first frame update
    void Start()
    {
        parent = transform.parent.transform;
        originalRotationValue = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        CheckIfDisplaced();
        if(displaced){
            if(!scoreAdded){
                scoreAdded = true;   
                pointsHandler.totalPoints += 1;
                pointsHandler.carriedPoints += 1;
                pointsHandler.timer += 5;
            }
        }

        void OutOFThrows(){
            if(pointsHandler.transform.GetComponent<Throwing>().totalThrows <= 0 && !gotDisplaced){
                displaced = true;
            }
        }
        void CheckIfDisplaced(){
            if(transform.localRotation != originalRotationValue){
                if(!gotDisplaced){
                    timer += Time.deltaTime * 2f;
                    //throwScript.canThrow = false;
                    displaced = true;
                }
                if(timer >= resetTime){
                    ResetTinCanPos();
                    timer = 0f;
                }
            }
        }
        void ResetTinCanPos(){
            //throwScript.canThrow = true;
            //transform.position = transform.parent.position;
            transform.GetComponent<Rigidbody>().isKinematic = true;
            transform.localPosition = new Vector3(0,0,0);
            transform.rotation = Quaternion.Euler(0,0,0);
            transform.GetComponent<Rigidbody>().isKinematic = false;
            scoreAdded = false;
            displaced = false;
        }
        
        void OnCollisionEnter(Collision other)
        {
            if(displaced){
                if(other.transform.CompareTag("FlipFlops")){
                    other.transform.GetComponent<FlipFlops>().GoToPlayer();
                }
            }
        }
    }   
}