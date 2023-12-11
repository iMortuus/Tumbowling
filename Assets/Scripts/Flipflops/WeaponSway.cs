using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class WeaponSway : MonoBehaviour
{
    [Header("Weapon Sway Position")]
    public float amount = 0.02f;
    public float maxAmount = 0.06f;
    public float smoothAmount = 6f;

    [Header("Rotational Weapon Sway")]
    public float rotationalAmount = 4f;
    public float maxRotationalAmount = 5f;
    public float smoothRotation = 12f;
    
    [Space]
    public bool rotationX = true;
    public bool rotationY = true;
    public bool rotationZ = true;

    [Header("References")]
    public PhotonView photonView;

    private Vector3 initialPosition;
    private Quaternion initialRotation;

    private float InputX;
    private float InputY;

    private void Awake()
    {
        photonView = transform.root.GetComponent<PhotonView>();
        initialPosition = transform.localPosition;
        initialRotation = transform.localRotation;
    }

    private void Update()
    {
        if(!photonView.IsMine) return;
        CalculateSway();
        MoveSway();
        TiltSway();
    }

    #region Private Voids
    private void CalculateSway(){
        InputX = Input.GetAxis("Mouse X");
        InputY = Input.GetAxis("Mouse Y");
    }
    void MoveSway(){
        float moveX = Mathf.Clamp(InputX * amount, -maxAmount, maxAmount);
        float moveY = Mathf.Clamp(InputY * amount, -maxAmount, maxAmount);

        Vector3 finalPosition = new Vector3(moveX, moveY, 0);

        transform.localPosition = Vector3.Lerp(transform.localPosition, finalPosition + initialPosition, Time.deltaTime * smoothAmount);
    }
    void TiltSway(){
        float tiltY = Mathf.Clamp(InputX * rotationalAmount, -maxRotationalAmount, maxRotationalAmount);
        float tiltX = Mathf.Clamp(InputY * rotationalAmount, -maxRotationalAmount, maxRotationalAmount);

        Quaternion finalRotation = Quaternion.Euler(new Vector3(
            rotationX ? -tiltX : 0f,
            rotationY ? -tiltY :0f,
            rotationZ ? -tiltY : 0
        ));

        transform.localRotation = Quaternion.Slerp(transform.localRotation, finalRotation * initialRotation, Time.deltaTime * smoothRotation);
    }
    #endregion
}
