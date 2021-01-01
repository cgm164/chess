using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State : MonoBehaviour
{
    public GameObject piece;
    public Color color;

    public float speed = 0.1f;
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
            Vector3 target_v = new Vector3(target.position.x, transform.position.y, target.position.z);
            transform.position = Vector3.MoveTowards(transform.position, target_v , step);

            // Check if the position of the cube and sphere are approximately equal.
            if (Vector3.Distance(transform.position, target_v) < 0.0000001f)
            {
                isMoving = false;
                cb();
            }    
        }
    }

}
