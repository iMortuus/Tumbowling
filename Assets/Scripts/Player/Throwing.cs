using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using NaughtyAttributes;

public class Throwing : MonoBehaviour
{
    [Header("References")]
    public PointsHandler pointsHandler;
    public Transform cam;
    public Transform attackPoint;
    public GameObject objectToThrow;
    public PlayerStats playerStats;
    public LayerMask[] IgnoreLayerMask;
    public GameManager gameManager;
    private Rigidbody projectileRb;
    private GameObject projectile;

    [Header("Settings")]
    public float totalThrows;
    public float throwCooldown;
    public float timer;
    public float averageAddForceTime;

    [Header("Throwing")]
    public KeyCode throwKey = KeyCode.Mouse0;
    public float throwForce;
    public float maxForce;
    public float throwUpwardForce;
    public bool canThrow = true;

    [Header("Slide Jumping")]
    [Foldout("Sound Effects")] public AudioSource throwingAudioSource;
    [Foldout("Sound Effects")] public AudioClip throwingSound;
    [Foldout("Sound Effects")] [Range(0f , 1f)] public float throwingPitchRandomization;
    [Foldout("Sound Effects")] [Range(0f, 1f)] public float throwingVolumeRandomization;

    bool readyToThrow;

    private void Start()
    {
        readyToThrow = true;
    }

    private void Update()
    {
        if(throwForce >= maxForce){
            throwForce = maxForce;
        }
        if(totalThrows <= 0 && pointsHandler.carriedPoints <= 0){
            playerStats.TakeDamage(playerStats.maxHealth);
        }
        if(!Paused()){
            if(Input.GetKey(throwKey) && readyToThrow && totalThrows > 0 && canThrow)
            {
                GetFlipFlops();
                timer += Time.deltaTime;
                averageAddForceTime += Time.deltaTime;
                if(timer != 0f ){
                    averageAddForceTime = 0f;
                    throwForce += 0.8f;
                }
            }else if(Input.GetKeyUp(throwKey) && readyToThrow && totalThrows > 0 && canThrow){
                attackPoint.rotation = Quaternion.Euler(0,0,0);
                Throw();
                PlayThrowSound();
                timer = 0f;
                averageAddForceTime = 0f;
                throwForce = 0f;
            }
        }
        if(totalThrows > 5f){
            totalThrows = 5f;
        }
    }

    private void GetFlipFlops(){
        if(timer == 0){
            projectile = Instantiate(objectToThrow, attackPoint.position, attackPoint.rotation, attackPoint.transform);
            projectileRb = projectile.GetComponent<Rigidbody>();
            projectileRb.isKinematic = true;
            
        }
    }
    private void Throw()
    {
        if(!Paused()){
            readyToThrow = false;

            // instantiate object to throw
            

            // get rigidbody component
            projectileRb.isKinematic = false;
            projectile.GetComponent<FlipFlops>().canRotate = true;
            projectile.transform.SetParent(null);
            
            // calculate direction
            Vector3 forceDirection = cam.transform.forward;

            RaycastHit hit;

            if(Physics.Raycast(cam.position + cam.forward   , cam.forward, out hit, 500f))
            {
                forceDirection = (hit.point - attackPoint.position).normalized;
            }

            // add force
            Vector3 forceToAdd = forceDirection * throwForce + transform.up * throwUpwardForce;

            projectileRb.AddForce(forceToAdd, ForceMode.Impulse);
            totalThrows--;

            // implement throwCooldown
            Invoke(nameof(ResetThrow), throwCooldown);
        }
    }

    private void ResetThrow()
    {
        readyToThrow = true;
    }

    bool Paused(){
        return gameManager.paused;
    }

    void PlayThrowSound(){
        throwingAudioSource.clip = throwingSound;
        throwingAudioSource.volume = 1 - throwingVolumeRandomization + Random.Range(-throwingVolumeRandomization, throwingVolumeRandomization);
        throwingAudioSource.pitch = 1 - throwingPitchRandomization + Random.Range(-throwingPitchRandomization, throwingPitchRandomization);
        throwingAudioSource.Play();
    }
}