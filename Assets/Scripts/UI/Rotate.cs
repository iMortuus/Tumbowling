using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Rotate : MonoBehaviour
{
    public float rotationSpeed = 30;

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0,0,rotationSpeed);
    }
}
