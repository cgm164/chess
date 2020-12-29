using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;

public class Chess : MonoBehaviour
{
    private string status = "";

    public Chess() { }
    
    IEnumerator Request(string url, Action<JSONNode> callback)
    {
        var request = new UnityWebRequest(url, "GET")
        {
            downloadHandler = new DownloadHandlerBuffer()
        };
        request.SetRequestHeader("Content-Type", "application/json");
        yield return request.SendWebRequest();
        
        callback(JSONNode.Parse(request.downloadHandler.text));
        
    }

    public void Move(string movement)
    {
        string nextStatus = status + movement;

        StartCoroutine(Request($"http://chess-api.herokuapp.com/valid_move/{nextStatus}", result =>
        {
            if (result["validMove"])
            {
                status = nextStatus;
            }
                
            Debug.Log(status);
        }));
    }

    public void Move()
    {
        StartCoroutine(Request($"http://chess-api.herokuapp.com/next_best/{status}", result =>
        {
            status += result["bestNext"];
            Debug.Log(status);
        }));
    }
}
