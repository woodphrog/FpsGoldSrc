using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private Vector3 coordinates;

    public Vector3 GetCoordinates()
    {
        return this.coordinates;
    }
    
    // Start is called before the first frame update
    private void Start()
    {
        this.coordinates = GetComponent<Transform>().position;
    }

    // Update is called once per frame
    private void OnTriggerEnter(Collider other)
    {
        this.GetComponentInParent<CheckpointManager>().Push(this.gameObject);
        

    }
}
