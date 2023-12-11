using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    public Transform cam;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    private void Update()
    {
        var cameraPos = GameObject.FindWithTag("MainCamera").transform;
        cam = cameraPos;
    }
    
    // Update is called once per frame
    private void LateUpdate()
    {
        transform.LookAt(transform.position + cam.forward);
    }
}
    