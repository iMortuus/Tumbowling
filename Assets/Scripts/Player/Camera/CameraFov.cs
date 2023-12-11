using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFov : MonoBehaviour
{
    [Header("Field Of View Settings")]
    public float fovTransmitionSpeed;
    public float defaultFov;
    public float sprintFov;
    public float slidingFov;
    

    [Header("Script References")]
    public Camera myCam;
    public PlayerStateHandler playerState;
    public PlayerMovement playerMovement;
    public CameraTilter cameraTilter;

    [HideInInspector] public bool gunADSstate;
    [HideInInspector] public float targetFov;

    private void Awake()
    {
        myCam = GetComponent<Camera>();
        targetFov = defaultFov;
    }

    private void Update()
    {
        HandleFov();
        HandleResetSlideTilt();
        if(!gunADSstate){
            switch(playerState.states){
                default:
                case PlayerStates.Normal:
                    DefaultFov();
                    break;
                case PlayerStates.Sprinting:
                    SprintFov();
                    break;
                case PlayerStates.Crouching:
                    DefaultFov();
                    break;
                case PlayerStates.Sliding:
                    SlidingFov();
                    cameraTilter.SlideTilt(15f, 5f, 25f, 25f);
                    break;
            }
        }
    }

    void HandleFov(){
        myCam.fieldOfView = Mathf.Lerp(myCam.fieldOfView, targetFov, fovTransmitionSpeed * Time.deltaTime);
    }
    void DefaultFov(){
        targetFov = defaultFov;
    }
    void SprintFov(){
        if(playerMovement.moving){
            targetFov = sprintFov;
        }
    }
    void SlidingFov(){
        if(playerMovement.moving){
            targetFov = slidingFov;
        }
    }
    void HandleResetSlideTilt(){
        if(playerState.states != PlayerStates.Sliding){
            cameraTilter.ResetSlideTilt(40f);
        }
    }
}
