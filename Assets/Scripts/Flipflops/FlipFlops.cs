using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlipFlops : MonoBehaviour
{
    public float pickUpRange;
    public LayerMask whatIsPlayer;
    public LayerMask whatIsTargetArea;
    public bool canRotate = false;
    private bool collided = false;
    private bool playerInPickUpRange, playerInTargetArea, added;
    [SerializeField] private float rotationSpeed = 20;
    float timer;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(rotationSpeed <= 0){
            canRotate = false;
        }
        if(collided){
            rotationSpeed = rotationSpeed - (Time.deltaTime * 10);
            Destroy(gameObject, 3f);
            CheckForPlayer();
        }
        if(canRotate){
            transform.Rotate(0,rotationSpeed,0);
        }
        if(playerInPickUpRange){
            GoToPlayer();
        }
        if(playerInTargetArea){
            timer += Time.deltaTime;
            if(timer >= 3){
                GoToPlayer();
                timer = 0f;
            }
        }

    }
    
    void OnCollisionEnter(Collision collision){
        collided = true;
        GiveFlipFlops();
        if(collision.transform.CompareTag("Player")){
            
            Destroy(gameObject);
        }
    }
    void CheckForPlayer(){
        playerInPickUpRange = Physics.CheckSphere(transform.position, pickUpRange, whatIsPlayer);
        playerInTargetArea = Physics.CheckSphere(transform.position, pickUpRange, whatIsTargetArea);
    }
    private void GiveFlipFlops(){
        Collider[] _colliders = Physics.OverlapSphere(transform.position, pickUpRange);
        foreach (Collider _collider in _colliders){
            if(_collider.CompareTag("Player")){
                if(!added){
                    Throwing throwScript = _collider.GetComponent<Throwing>();
                    PointsHandler pointsHandler = _collider.GetComponent<PointsHandler>();
                    throwScript.totalThrows++;
                }
                added = true;
            }
        }
    }

    public void GoToPlayer(){
        transform.position = GameObject.Find("Player").transform.position;
    }
}
