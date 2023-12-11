using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetArea : MonoBehaviour
{
    public List<Transform> children = new List<Transform>();
    // Start is called before the first frame update
    void Start()
    {
        GetChildren();
    }

    // Update is called once per frame
    void Update()
    {
    }

    void GetChildren(){
        foreach (Transform child in transform)
        {
            children.Add(child.transform);
        }
    }

}
