﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatePiece : MonoBehaviour
{
    public GameObject cell;
    public Color color;

    // Adjust the speed for the application.
    public float speed = 1.0f;

    // The target (cylinder) position.
    public Transform target;

    private bool isMoving = false;
    private BoardManager.EndToMove cb;
    void Start()
    {
       
    }

    public void Move(GameObject obj, BoardManager.EndToMove callback)
    {
        target = obj.transform;
        cb = callback;
        isMoving = true;
    }

    void FixedUpdate()
    {
        if (isMoving)
        {
            // Move our position a step closer to the target.
            float step = speed * Time.deltaTime; // calculate distance to move
            transform.position = Vector3.MoveTowards(transform.position, target.position, step);

            // Check if the position of the cube and sphere are approximately equal.
            if (Vector3.Distance(transform.position, target.position) < 0.0000001f)
            {
                isMoving = false;
                cb();
            }    
        }
    }

}
