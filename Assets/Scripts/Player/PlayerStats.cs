using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using EZCameraShake;
using Photon.Pun;

public class PlayerStats : MonoBehaviour
{
    [Header("Script References")]
    public PlayerStateHandler playerState;
    public PlayerMovement playerMovement;
    public PlayerUIHandler playerUIHandler;

    [Header ("Player Health Settings")]
    public float maxHealth;
    public float lowHealth;
    public float criticalHealth;
    public float maxShield;
    public float maxStamina;
    
    [Header("Level And Xp Settings")]
    public int currentLevel;
    public float currentXp;
    public float xpNeeded;

    public float currentHealth;
    public float currentShield;
    public float currentStamina;

    [Header("Movement Settings")]    
    public float defaultWalkSpeed = 8f; // your default walking speed
    public float sprintSpeed = 13f; // basically your speed when you sprint
    public float slidingSpeed = 15f; // basically your speed when you slide
    public float crouchingWalkingSpeed = 3f; // walking while crouching settings
    public float maxJumpCount = 2f; // the number of jumps that your player can do;
    public float jumpHeight = 3f; // Default jumping settings
    public float crouchJump = 4f; // Jumping after crouching settings

    
    [Header("DashSettings")]
    public float dashSpeed; // the speed of your dash
    public float dashTime;  // the amount of time that you will dash
    public float dashAmount; // the amount of dashes the player can do before cooldown
    
    [Header("Movement Cooldown Settings")]
    public float jumpCooldown = .62f; //this is the time each time you jump
    public float staminaCooldown = 2f; //the time needed for you to use stamina again after you drain it 
    public float oneDashCooldown = 1f;
    public float dashCooldown = 2f; // this is the time needed for dash to cooldown

    [Header("Movement Stamina Settings")]
    public float sprintingStaminaCost = 5f; // this is the amount of stamina that you will will use when you sprint
    public float slidingStaminaCost = 25f; // this is the amount of stamina that you will use when you slide
    public float slideJumpingStaminaCost = 10f;
    public float doubleJumpingStaminaCost = 5f;
    public float staminaRegenerateOverTime = 10f; // this is your default stamina regeneration when you are not springint nor sliding
    public float notMovingStaminaRegenerationOverTime = 15f; // this the the amount of stamina that you will refill when you aren't moving

    [Header ("Player Gravity Settings")]
    public float gravity = -60f;
    public float coyoteJumpTime = 1f;
    public float slopeForce = 1f;
    public float slopeForceRaylength = 1.5f;
    public float playerMomentumDrag = 3f;

    
    [Header("Damage")]
    public AudioSource damageAudioSource;
    public AudioClip damageSound;
    [Range(0f , 1f)] public float damagePitchRandomization;
    [Range(0f, 1f)] public float damageVolumeRandomization;


    [Header("Effects")]
    [SerializeField] private Volume damageVFX;
    [SerializeField] private Volume lowHealthVFX;
    [SerializeField] private Volume deathVFX;
    #region  Debugers
    #endregion
    private void Awake()
    {
        SetHealthToMax();
        SetStaminaToMax();
    }

    private void Update()
    {
        CapHealth();
        CapShield();
        CapStamina();
        NotMovingStaminaIncrease();
        HandleDamageVFX();
        HandlePlayerLow();
        HandlePlayerStamina();
        HandleDeath();
        switch(playerState.states){
            default:
            case PlayerStates.Normal:
                IncreaseStamina(staminaRegenerateOverTime * Time.deltaTime);
                break;
            case PlayerStates.Sprinting:
                if(playerMovement.isGrounded){
                    DecreaseStamina(sprintingStaminaCost * Time.deltaTime);
                }
                break;
            case PlayerStates.Crouching:
                IncreaseStamina(notMovingStaminaRegenerationOverTime * Time.deltaTime);
                break;
            case PlayerStates.Sliding:
                if(playerMovement.isGrounded){
                    //DecreaseStamina(slidingStaminaCost * Time.deltaTime);
                }
                break;
        }
    }

    #region Handlers
    void HandleDamageVFX(){
        if(damageVFX.weight > 0){
            damageVFX.weight -= 1f * Time.deltaTime; 
        }
    }
    void HandlePlayerLow(){
        if(currentHealth <= lowHealth && currentHealth > criticalHealth){
            LowHealthVFX();
        }else if(currentHealth <= criticalHealth){
            CriticalHealthVFX();
        }else if (currentHealth > lowHealth || playerState.states == PlayerStates.Dead){
            RemoveLowHealthVFX();
        }
        if (currentHealth <= 0){
            currentHealth = 0f;
            playerState.states = PlayerStates.Dead;
        }
    }  
    void HandleDeath(){
        if(currentHealth <= 0f){
            playerState.states = PlayerStates.Dead;
            DeathVFX();
        }
    }
    void HandlePlayerStamina(){
        if(currentStamina <= 0){
            StartCoroutine(RegenerateStamina());
        }
    }
    IEnumerator RegenerateStamina(){
        playerState.outOfStamina = true;
        yield return new WaitForSeconds(2);
        playerState.outOfStamina = false;
    }
    #endregion

    #region Public Voids
    //Damage
    [PunRPC] public void TakeDamage(float damage){
        if(CheckIfNotDead() && CheckIfNotInvincible()){
            if(!playerState.Shield){
                PlayDamageSound();
                damageVFX.weight = 1;
                CameraShaker.Instance.ShakeOnce(3,3, .1f, 1f);
                playerUIHandler.lerpTimer = 0;
                currentHealth -= damage;
            }else{
                DamageShied(damage);
                playerUIHandler.lerpTimer = 0;
            }
        }
    }
    public void DamageShied(float amount){
        currentShield -= amount;
    }
    public void DecreaseStamina(float amount){
        currentStamina -= amount;
    }
    //Add
    public void Heal(float amount){
        if(CheckIfNotDead()){
            currentHealth += amount;
            playerUIHandler.lerpTimer = 0;
        }
    }
    public void AddShield(float amount){
        if(CheckIfNotDead()){
            currentShield += amount;
        }
    }
    public void IncreaseStamina(float amount){
        if(currentStamina <= maxStamina){
            currentStamina += amount;
        }
    }
    #endregion

    #region Private Voids
    void SetHealthToMax(){
        currentHealth = maxHealth;
    }
    void SetStaminaToMax(){
        currentStamina = maxStamina;
    }
    void CapHealth(){
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        if(currentHealth > maxHealth){
            currentHealth = maxHealth;
        }
        if(currentHealth < 0){
            currentHealth = 0;
        }
    }
    void CapStamina(){
        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
        if(currentStamina > maxStamina){
            currentStamina = maxStamina;
        }
        if(currentStamina < 0){
            currentStamina = 0;
        }
    }
    void CapShield(){
        currentShield = Mathf.Clamp(currentShield, 0, maxShield);
        if(currentShield > maxShield){
            currentShield = maxShield;
        }
        if(currentShield < 0){
            currentShield = 0;
        }
    }
    void LowHealthVFX(){
        if(lowHealthVFX.weight < 0.75f){
            lowHealthVFX.weight += .5f * Time.deltaTime;
        }else{
            lowHealthVFX.weight -= .5f * Time.deltaTime;
        }
    }
    void CriticalHealthVFX(){
        if(lowHealthVFX.weight < 1f){
            lowHealthVFX.weight += .5f * Time.deltaTime;
        }
    }
    void RemoveLowHealthVFX(){
        if(lowHealthVFX.weight > 0f){
            lowHealthVFX.weight -= .5f * Time.deltaTime;
        }
    }
    void DeathVFX(){
        deathVFX.weight += .05f * Time.deltaTime;
        lowHealthVFX.weight = 0f;
    }
    void NotMovingStaminaIncrease(){
        if(!playerMovement.moving){
            IncreaseStamina(notMovingStaminaRegenerationOverTime * Time.deltaTime);
        }
    }
    void PlayDamageSound(){
        damageAudioSource.clip = damageSound;
        damageAudioSource.volume = 1 - damageVolumeRandomization + Random.Range(-damageVolumeRandomization, damageVolumeRandomization);
        damageAudioSource.pitch = 1 - damagePitchRandomization + Random.Range(-damagePitchRandomization, damagePitchRandomization);
        damageAudioSource.Play();   
    }
    #endregion

    #region Check States
    bool CheckIfNotDead(){
        return playerState.states != PlayerStates.Dead;
    }
    bool CheckIfNotInvincible(){
        return !playerState.Invincible;
    }
    #endregion
}

