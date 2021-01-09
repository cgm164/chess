﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceBehaviour : MonoBehaviour
{

    public GameObject capturedPrefab;

    public IEnumerator Capture()
    {
        
        // Make original object invisible
        var meshRendererComponent = gameObject.GetComponent<MeshRenderer>();
        meshRendererComponent.enabled = false;

        var capturedObject = Instantiate(capturedPrefab, gameObject.transform);
        capturedObject.transform.localScale = new Vector3(0.025f, 0.025f, 0.025f);
        foreach (var childRenderer in capturedObject.GetComponentsInChildren<MeshRenderer>()){
            childRenderer.material = meshRendererComponent.material;
        }

        // Play animation
        var animationComponent = capturedObject.GetComponent<Animation>();
        animationComponent["Captured"].speed = .5f;
        animationComponent.Play("Captured");
        while (animationComponent.isPlaying) {
            yield return null;
        }

        //Destroy(gameObject);
    }

}
