using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using EZCameraShake;
using Photon.Pun;

public class PlayerMovement : MonoBehaviour
{
    #region Player Movement Settings
    [Header("Movement Settings")]
    public float speedSmoothTransmition;
    public float crouchSmoothAmount;
    public float slidingCrouchSmoothAmount;
    public float slidingSpeedDecreaseAmount;
    #endregion

    #region Script References
    [Header("Script References")]
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private CharacterController controller;
    [SerializeField] private PhotonView photonView;
    #endregion

    #region Player States
    [Header("Player States")]
    public PlayerStateHandler playerState;
    #endregion

    #region Player SFX
    [Header("Foot Steps")]
    [Foldout("Sound Effects")] public AudioSource footStepAudioSource;
    [Foldout("Sound Effects")] public AudioClip footStepSound;
    [Foldout("Sound Effects")] [Range(0f , 1f)] public float footStepsPitchRandomization;
    [Foldout("Sound Effects")] [Range(0f, 1f)] public float footsStepsVolumeRandomization;
    [Foldout("Sound Effects")] [Range(0f, 1f)] public float modifier;
    [Foldout("Sound Effects")] private float footStepSpeed;
    [Foldout("Sound Effects")] private float distanceCovered;

    [Header("Sliding")]
    [Foldout("Sound Effects")] public AudioSource slidingAudioSource;
    [Foldout("Sound Effects")] public AudioClip slidingSound;
    [Foldout("Sound Effects")] [Range(0f , 1f)] public float slidingPitchRandomization;
    [Foldout("Sound Effects")] [Range(0f, 1f)] public float slidingVolumeRandomization;

    [Header("Jumping")]
    [Foldout("Sound Effects")] public AudioSource jumpingAudioSource;
    [Foldout("Sound Effects")] public AudioClip jumpingSound;
    [Foldout("Sound Effects")] [Range(0f , 1f)] public float jumpingPitchRandomization;
    [Foldout("Sound Effects")] [Range(0f, 1f)] public float jumpingVolumeRandomization;

    [Header("Slide Jumping")]
    [Foldout("Sound Effects")] public AudioSource slideJumpingAudioSource;
    [Foldout("Sound Effects")] public AudioClip slideJumpingSound;
    [Foldout("Sound Effects")] [Range(0f , 1f)] public float slideJumpingPitchRandomization;
    [Foldout("Sound Effects")] [Range(0f, 1f)] public float slideJumpingVolumeRandomization;

    [Header("Double Jumping")]
    [Foldout("Sound Effects")] public AudioSource doubleJumpingAudioSource;
    [Foldout("Sound Effects")] public AudioClip doubleJumpingSound;
    [Foldout("Sound Effects")] [Range(0f , 1f)] public float doubleJumpingPitchRandomization;
    [Foldout("Sound Effects")] [Range(0f, 1f)] public float doubleJumpingVolumeRandomization;

    [Header("Dash")]
    [Foldout("Sound Effects")] public AudioSource dashAudioSource;
    [Foldout("Sound Effects")] public AudioClip dashJumpingSound;
    [Foldout("Sound Effects")] [Range(0f , 1f)] public float dashPitchRandomization;
    [Foldout("Sound Effects")] [Range(0f, 1f)] public float dashVolumeRandomization;

    [Header("Falling")]
    [Foldout("Sound Effects")] public AudioSource landingAudioSource;
    [Foldout("Sound Effects")] public AudioClip landingSound;
    [Foldout("Sound Effects")] [Range(0f , 1f)] public float landingPitchRandomization;
    [Foldout("Sound Effects")] [Range(0f, 1f)] public float landingVolumeRandomization;
    [Foldout("Sound Effects")] [Range(0f, 1f)] public float landingSoundDistance = 0.25f;
    #endregion

    #region  Player VFX
    [Header("Effects")]
    //Foldout("Visual Effects")] [SerializeField] private Volume dashVFX;
    #endregion

    #region Player Checks
    [Header("Player Checks")]
    public Transform groundCheck;
    public Transform ceilingCheck;
    private Transform playerTransform;

    public float ceilingDistance = 0.4f; // the distance of the player to the ground to consider if the player is on the ground
    public float groundDistance = 0.4f; // the distance of the player to the ceiling to consider if the player has a ceiling
    public float crouchingGroundDistance;
    public LayerMask groundMask;
    [Space(2f)]

    public bool isGrounded;
    public bool wasGrounded;
    public bool hasCeiling;
    
    #endregion

    #region Debugers
    //float
    public bool showDebuggers;
    [ShowIf("showDebuggers")] [ReadOnly] [SerializeField] private float jumpCount;
    [ShowIf("showDebuggers")] [ReadOnly] [SerializeField] private float nextJump;
    [ShowIf("showDebuggers")] [ReadOnly] [SerializeField] public float playerCurrentSpeed;
    [ShowIf("showDebuggers")] [ReadOnly] [SerializeField] private float airTime;
    [ShowIf("showDebuggers")] [ReadOnly] [SerializeField] private float coyoteTime;
    [ShowIf("showDebuggers")] [ReadOnly] [SerializeField] public float targetSpeed;
    [ShowIf("showDebuggers")] [ReadOnly] [SerializeField] public float weaponSpeed;
    [ShowIf("showDebuggers")] [ReadOnly] public float nextDash;
    [ShowIf("showDebuggers")] [ReadOnly] public float uiDashCooldown;
    [ShowIf("showDebuggers")] [ReadOnly] public float playerCurrentDashAmount;

    //bool
    [ShowIf("showDebuggers")] [ReadOnly] [SerializeField]  public bool moving;
    [ShowIf("showDebuggers")] [ReadOnly] [SerializeField] private bool slid;
    [ShowIf("showDebuggers")] [ReadOnly] [SerializeField] private bool slidSoundPlayed;
    [ShowIf("showDebuggers")] [ReadOnly] [SerializeField] private bool canDoubleJump;
    [ShowIf("showDebuggers")] [ReadOnly] [SerializeField] private bool canCoyoteJump;
    #endregion

    [HideInInspector] public Vector3 velocity;
    [HideInInspector] public Vector3 characterVelocityMomentum;
    [HideInInspector] public Vector3 moveDir;
    [HideInInspector] public Vector3 moveForward;

    private void Start()
    {
        ResetDash();
    }

    private void Awake()
    {
        ResetDash();
        crouchingGroundDistance = controller.height / 2;
        playerTransform = GetComponent<Transform>();
        photonView = GetComponent<PhotonView>();
    }

    private void Update()
    {
        if(!photonView.IsMine) return;
        MovementChecks();
        CapMovementSpeed();
        HandleGravity();
        HandleMovement();
        HandleMovementSpeed();
        HandleCoyoteJump();
        HandleDashCooldown();
        //HandleDashVFX();
        HandleFallSounds();

        if(playerState.states != PlayerStates.Dead){
            switch(playerState.states){
                default:
                case PlayerStates.Normal:
                    Walk();
                    break;
                case PlayerStates.Sprinting:
                    Sprint();
                    break;
                case PlayerStates.Crouching:
                    Crouch();
                    break;
                case PlayerStates.Sliding:
                    Slide();
                    break;
            }
        }
        //Debug.Log(moving);
    }

    
     
    void MovementChecks(){
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        hasCeiling = Physics.CheckSphere(ceilingCheck.position, ceilingDistance, groundMask);
        wasGrounded = isGrounded;
        if(isGrounded){
            ResetGravity();
        }
    }
    void HandleMovement(){
        if(playerState.states != PlayerStates.Dead){
            if(playerState.states != PlayerStates.Stunned){
                float x = Input.GetAxis("Horizontal");
                float z = Input.GetAxis("Vertical");

                Vector3 move = transform.right * x + transform.forward * z;

                moveDir = move;
                moveForward = transform.forward;

                
                controller.Move(Vector3.ClampMagnitude(move, 1.0f) * playerCurrentSpeed * Time.deltaTime);

                if(z >0 || z <0 || x >0 || x <0){
                    moving = true;
                }else
                {
                    moving = false;
                }

                if(moving && OnSlope()){
                    controller.Move(Vector3.down * controller.height / 2 * playerStats.slopeForce * Time.deltaTime);
                }
            }
            #region Sliding
            if((SprintInputOnce() && CrouchInputOnce() || SprintInput() && CrouchInputOnce() || SprintInputOnce() && CrouchInput()) && isGrounded && moving && !playerState.outOfStamina){
                
            }
            if(SprintInput() && CrouchInput() && isGrounded && moving && !playerState.outOfStamina){
                playerState.states = PlayerStates.Sliding;
            }else{
                if(playerState.states != PlayerStates.Crouching && playerState.states != PlayerStates.Sprinting){
                    playerState.states = PlayerStates.Normal;
                }
                if(playerState.states != PlayerStates.Crouching && playerState.states != PlayerStates.Sprinting && !hasCeiling){
                    playerState.states = PlayerStates.Crouching;
                }
                if(playerState.states != PlayerStates.Crouching && !hasCeiling){
                    ResetControllerHeight();
                }
                slid = false;
                slidSoundPlayed = false;
            }
            #endregion
            #region Sprinting
            if(SprintInput() && playerState.states != PlayerStates.Sliding && moving && !playerState.outOfStamina){
                playerState.states = PlayerStates.Sprinting;
            }else{
                if(playerState.states != PlayerStates.Crouching && playerState.states != PlayerStates.Sliding){
                    playerState.states = PlayerStates.Normal;
                }
            }
            #endregion
            #region Crouching
            if(CrouchInput() && isGrounded && playerState.states != PlayerStates.Sliding){
                playerState.states = PlayerStates.Crouching;
            }else{
                if(playerState.states != PlayerStates.Sprinting && playerState.states != PlayerStates.Sliding && !hasCeiling){
                    playerState.states = PlayerStates.Normal;
                }
                
                if(playerState.states != PlayerStates.Sliding && !hasCeiling){
                    ResetControllerHeight();
                }
            }
            #endregion
            #region  Dashing
            /** dash
            #region Dashing
            if(DashInput() && !playerState.outOfStamina && moving){
                if(playerCurrentDashAmount > 0){
                    StartCoroutine(Dash(playerStats.dashSpeed, playerStats.dashTime, moveDir));
                }
            }**/
            #endregion
            #region Jumping
            if(JumpInput() && (isGrounded || canCoyoteJump) && playerState.states != PlayerStates.Crouching && playerState.states != PlayerStates.Sliding && !hasCeiling && !canDoubleJump){
                if(Time.time > nextJump){
                    jumpCount++;
                    canDoubleJump = true;
                    Jump();
                    
                }
            }else if(JumpInput() && (isGrounded || canCoyoteJump) && (playerState.states == PlayerStates.Crouching ||  playerState.states == PlayerStates.Sliding) && !hasCeiling && !canDoubleJump){
                if(Time.time > nextJump){
                    canDoubleJump = true;
                    jumpCount++;
                    CrouchJump();
                }
            }else if(JumpInput() && (canDoubleJump || !canCoyoteJump && canDoubleJump) && !hasCeiling){
                DoubleJump();
                
            }
            #endregion
        }
    }    
    void HandleMovementSpeed(){
        if(!moving){
            targetSpeed = 0;
        }
        playerCurrentSpeed = Mathf.Lerp(playerCurrentSpeed, targetSpeed + weaponSpeed, speedSmoothTransmition * Time.deltaTime);
    }
    void HandleGravity(){
        velocity.y += playerStats.gravity * Time.deltaTime;
        velocity += characterVelocityMomentum;
        controller.Move(velocity * Time.deltaTime);

        if(characterVelocityMomentum.magnitude > 0f){
            float momentumDrag = 3f;
            velocity -= characterVelocityMomentum;
            characterVelocityMomentum -= characterVelocityMomentum * momentumDrag * Time.deltaTime;
            if(characterVelocityMomentum.magnitude < 0f){
                characterVelocityMomentum = Vector3.zero;
            }
        }

    }
    void HandleCoyoteJump(){
        if(!isGrounded){
            coyoteTime += Time.deltaTime;
        }else{
            coyoteTime = 0;
        }

        if(coyoteTime < playerStats.coyoteJumpTime){
            canCoyoteJump = true;
        }else{
            canCoyoteJump = false;
        }
    }
    bool OnSlope(){
        if(!isGrounded)
            return false;
        RaycastHit hit;
        if(Physics.Raycast(transform.position, Vector3.down, out hit, controller.height / 2 * playerStats.slopeForceRaylength))
            if(hit.normal != Vector3.up)
                return true;
        return false;
    }
    void HandleDashCooldown(){   
        if(nextDash > 0 && playerCurrentDashAmount == 0 && playerCurrentDashAmount < 1){
            nextDash -= Time.deltaTime;
            uiDashCooldown += Time.deltaTime;
            if(nextDash <=  0 ){
                ResetDash();  
            }
        }else if(nextDash > 0 && playerCurrentDashAmount == 1 && playerCurrentDashAmount < 2 && playerCurrentDashAmount > 0){
            nextDash -= Time.deltaTime;
            uiDashCooldown += Time.deltaTime;
            if(nextDash <= 0){
                ResetDash();
            }
        }
        if(nextDash <= 0){
            nextDash = 0;
        }
    }
    /** HandleDashVFX
    void HandleDashVFX(){
        if(dashVFX.weight > 0){
            dashVFX.weight -= 1f * Time.deltaTime; 
        }
    }**/

    #region Voids
    void ResetGravity(){
        if(isGrounded && velocity.y < 0){
            velocity.y = -2f;
        }
    }
    void ResetControllerHeight(){
        float lastHeight = controller.height;
        if(playerState.states != PlayerStates.Sliding){
            controller.height = Mathf.Lerp(controller.height , 2f, crouchSmoothAmount * Time.deltaTime);
        }else
        {
            controller.height = Mathf.Lerp(controller.height , 2f, slidingCrouchSmoothAmount * Time.deltaTime);
        }
        transform.position += new Vector3(0, (controller.height - lastHeight) / 2, 0 );
    }
    void ResetDash(){
        playerCurrentDashAmount = playerStats.dashAmount;
    } 
    void Walk(){
        if(moving){
            playerState.states = PlayerStates.Normal;
            targetSpeed = playerStats.defaultWalkSpeed;
            PlayFootStepsSounds();
        }
    }
    void Crouch(){
        if(moving){
            playerState.states = PlayerStates.Crouching;
            targetSpeed = playerStats.crouchingWalkingSpeed;
            PlayFootStepsSounds();
        }
        float lastHeight = controller.height;
        controller.height = Mathf.Lerp(controller.height , 1f, crouchSmoothAmount * Time.deltaTime);
        transform.position += new Vector3(0, (controller.height - lastHeight) / 2, 0 );
    }
    void Sprint(){
        if(moving){
            playerState.states = PlayerStates.Sprinting;
            targetSpeed = playerStats.sprintSpeed;
            PlayFootStepsSounds();
        }
    }
    void Slide(){
        if(moving){
            playSlidingSound();
            playerState.states = PlayerStates.Sliding;
            if(slid == false){
                targetSpeed = playerStats.slidingSpeed;
                playerStats.DecreaseStamina(playerStats.slidingStaminaCost);
            }
            targetSpeed -= slidingSpeedDecreaseAmount * Time.deltaTime;
            if(targetSpeed <= 0){
                targetSpeed = 0f;
                playerState.states = PlayerStates.Normal;
            }
        }
        controller.height = Mathf.Lerp(controller.height , 1f, slidingCrouchSmoothAmount * Time.deltaTime);
        slid = true;
    }
    void Jump(){
        PlayJumpingSound();
        characterVelocityMomentum = moveDir * targetSpeed * 2f; 
        //characterVelocityMomentum += Vector3.up * playerStats.jumpHeight;
        velocity.y = Mathf.Sqrt(playerStats.jumpHeight * -2f * playerStats.gravity);
        nextJump = Time.time + playerStats.jumpCooldown;
    }
    void CrouchJump(){
        PlayJumpingSound();
        playerStats.DecreaseStamina(playerStats.slideJumpingStaminaCost);
        characterVelocityMomentum = moveDir * targetSpeed * 2f; 
        //characterVelocityMomentum += Vector3.up * playerStats.crouchJump;
        velocity.y = Mathf.Sqrt(playerStats.crouchJump * -2f * playerStats.gravity);
        jumpCount++;
        nextJump = Time.time + playerStats.jumpCooldown;
    }
    void DoubleJump(){
        PlayDoubleJumpingSound();
        playerStats.DecreaseStamina(playerStats.doubleJumpingStaminaCost);
        characterVelocityMomentum = moveDir * targetSpeed * 2f; 
        //characterVelocityMomentum += Vector3.up * playerStats.jumpHeight;
        velocity.y = Mathf.Sqrt(playerStats.jumpHeight * -2f * playerStats.gravity);
        jumpCount++;
        canDoubleJump = false;
        nextJump = Time.time + playerStats.jumpCooldown;
    }
    /** Dash
    IEnumerator Dash(float speed, float time, Vector3 direction){
        if(moving){
            float startTime = Time.time;
            PlayDashSound();
            while(Time.time < startTime + time){
                dashVFX.weight = 1; // VISUAL EFFECTS FOR DASHING\\
                controller.Move(direction * speed * Time.deltaTime);
                yield return null;
            }
            playerCurrentDashAmount--;
            if(playerCurrentDashAmount <= 0){
                nextDash = playerStats.dashCooldown;
                uiDashCooldown = 0;
            }else if( playerCurrentDashAmount > 0 && playerCurrentDashAmount == 1 && playerCurrentDashAmount < 2){
                nextDash = playerStats.oneDashCooldown;
                uiDashCooldown = 0;
            }
        }
    }**/
    void PlayFootStepsSounds(){
        footStepSpeed = controller.velocity.magnitude;
        if(moving && (isGrounded || OnSlope()) ){
            distanceCovered += (footStepSpeed * Time.deltaTime) * modifier;
            if(distanceCovered > 1){
                footStepAudioSource.clip = footStepSound;
                footStepAudioSource.volume = 1 - footsStepsVolumeRandomization + Random.Range(-footsStepsVolumeRandomization, footsStepsVolumeRandomization);
                footStepAudioSource.pitch = 1 - footStepsPitchRandomization + Random.Range(-footStepsPitchRandomization, footStepsPitchRandomization);
                footStepAudioSource.Play();
                distanceCovered = 0;
            }
        }
    }
    void playSlidingSound(){
        if(isGrounded && !slidSoundPlayed){
            distanceCovered += (footStepSpeed * Time.deltaTime) * modifier;
            if(distanceCovered < 1){
                slidingAudioSource.clip = slidingSound;
                slidingAudioSource.volume = 1 - slidingVolumeRandomization + Random.Range(-slidingVolumeRandomization, slidingVolumeRandomization);
                slidingAudioSource.pitch = 1 - slidingPitchRandomization + Random.Range(-slidingPitchRandomization, slidingPitchRandomization);
                slidingAudioSource.Play();
                slidSoundPlayed = true;
            }
        }
    }
    void PlayJumpingSound(){
        if(playerState.states == PlayerStates.Sliding){
            PlaySlideJumpingSound();
        }else{
            jumpingAudioSource.clip = jumpingSound;
            jumpingAudioSource.volume = 1 - jumpingVolumeRandomization + Random.Range(-jumpingVolumeRandomization, jumpingVolumeRandomization);
            jumpingAudioSource.pitch = 1 - jumpingPitchRandomization + Random.Range(-jumpingPitchRandomization, jumpingPitchRandomization);
            jumpingAudioSource.Play();
        }
    }
    void PlaySlideJumpingSound(){
        slideJumpingAudioSource.clip = slideJumpingSound;
        slideJumpingAudioSource.volume = 1 - slideJumpingVolumeRandomization + Random.Range(-slideJumpingVolumeRandomization, slideJumpingVolumeRandomization);
        slideJumpingAudioSource.pitch = 1 - slideJumpingPitchRandomization + Random.Range(-slideJumpingPitchRandomization, slideJumpingPitchRandomization);
        slideJumpingAudioSource.Play();
    }
    void PlayDoubleJumpingSound(){
        doubleJumpingAudioSource.clip = doubleJumpingSound;
        doubleJumpingAudioSource.volume = 1 - doubleJumpingVolumeRandomization + Random.Range(-doubleJumpingVolumeRandomization, doubleJumpingVolumeRandomization);
        doubleJumpingAudioSource.pitch = 1 - doubleJumpingPitchRandomization + Random.Range(-doubleJumpingPitchRandomization, doubleJumpingPitchRandomization);
        doubleJumpingAudioSource.Play();
    }
    void PlayDashSound(){
        dashAudioSource.clip = dashJumpingSound;
        dashAudioSource.volume = 1 - dashVolumeRandomization + Random.Range(-dashVolumeRandomization, dashVolumeRandomization);
        dashAudioSource.pitch = 1 - dashPitchRandomization + Random.Range(-dashPitchRandomization, dashPitchRandomization);
        dashAudioSource.Play();
    }
    void HandleFallSounds(){
        if(!isGrounded){
            airTime += Time.deltaTime;
        }else{
            if(airTime > landingSoundDistance){
                landingAudioSource.clip = landingSound;
                landingAudioSource.volume = 1 - landingVolumeRandomization + Random.Range(-landingVolumeRandomization, landingVolumeRandomization);
                landingAudioSource.pitch = 1 - landingPitchRandomization + Random.Range(-landingPitchRandomization, landingPitchRandomization);
                landingAudioSource.Play();
                canDoubleJump = false;
                airTime = 0;
            }
        }
    }    
    #endregion

    #region Input Manager
    bool JumpInput(){
        return Input.GetKeyDown(KeyCode.Space);
    }  
    bool CrouchInput(){
        return Input.GetKey(KeyCode.LeftControl);
    }
    bool CrouchInputOnce(){
        return Input.GetKeyDown(KeyCode.LeftControl);
    }
    bool SprintInput(){
        return Input.GetKey(KeyCode.LeftShift);
    }    
    bool SprintInputOnce(){
        return Input.GetKeyDown(KeyCode.LeftShift);
    }
    bool DashInput(){
        return Input.GetKeyDown(KeyCode.LeftAlt);
    }
    #endregion

    #region Debugger Callbacks
    void CapMovementSpeed(){
        if(playerCurrentSpeed <= 0){
            playerCurrentSpeed = 0;
        }
    }
    #endregion
}
