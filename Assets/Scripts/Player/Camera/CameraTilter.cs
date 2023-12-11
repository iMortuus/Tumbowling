using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
public class CameraTilter : MonoBehaviour
{
    [ShowNonSerializedField] private float tiltX;
    [ShowNonSerializedField] private float tiltZ;
    private void Update()
    {
    }

    #region  Public Voids

    public void SlideTilt(float upAmount, float sideAmount, float upSpeed, float sideSpeed){
        tiltX += upSpeed * Time.deltaTime;
        tiltZ += sideSpeed * Time.deltaTime;
        tiltX = Mathf.Clamp(tiltX, 0, upAmount);
        tiltZ = Mathf.Clamp(tiltZ, 0, sideAmount);
        transform.localRotation = Quaternion.Euler(-tiltX, 0, -tiltZ);
    }

    public void ResetSlideTilt(float speed){
        tiltX -= speed * Time.deltaTime;
        tiltZ -= speed * Time.deltaTime;
        if(tiltX <= 0){
            tiltX = 0;
        }
        if(tiltZ <= 0){
            tiltZ = 0;
        }
        transform.localRotation = Quaternion.Euler(-tiltX, 0, -tiltZ);
    }
    #endregion
}
