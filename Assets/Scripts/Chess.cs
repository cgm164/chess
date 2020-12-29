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

    public void Move(string movement, BoardManager.EndToMove callback, BoardManager.EndToMove error)
    {
        string nextStatus = status + movement;

        Debug.Log(nextStatus);

        StartCoroutine(Request($"http://chess-api.herokuapp.com/valid_move/{nextStatus}", result =>
        {
            if (result["validMove"])
            {
                status = nextStatus;
                callback();
            }
            else
            {
                error();
            }
        }));
    }

    public delegate void MoveEvent(string oldC, string newC);

    public void Move(MoveEvent moveEvent)
    {
        StartCoroutine(Request($"http://chess-api.herokuapp.com/next_best/{status}", result =>
        {
            status += result["bestNext"];
            
            string r = result["bestNext"];
            string oldC = $"{r[0]}{r[1]}";
            string newC = $"{r[2]}{r[3]}";
            Debug.Log(oldC);
            Debug.Log(newC);
            Debug.Log(r);
            moveEvent(oldC, newC);
        }));
    }
}
