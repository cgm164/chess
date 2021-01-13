using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;
using UnityEngine.UI;

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

        if (request.isHttpError)
            Debug.Log("Not API work");
        if (request.isNetworkError)
            Debug.Log("Not Internet work");

        yield return request.SendWebRequest();
        
        callback(JSONNode.Parse(request.downloadHandler.text));
        
    }

    public void Move(string movement, BoardManager.EndToMove callback, BoardManager.EndToMoveStatus error)
    {
        string nextStatus = status + movement; 

        StartCoroutine(Request($"http://chess-api.herokuapp.com/valid_move/{nextStatus}", result =>
        {
            if (result["validMove"])
            {
                status = nextStatus;
                callback();
            }
            else
            {
                error(true);
            }
        }));
    }

    public enum GameStatus {
        white_won,
        black_won,
        stalemate,
        in_progress
    };

    public delegate void StatusEvent(GameStatus nn);

    public void GetStatusGame(StatusEvent cb)
    {
        StartCoroutine(Request($"http://chess-api.herokuapp.com/status/{status}", result =>
        {
            cb((GameStatus)Enum.Parse(typeof(GameStatus), result["gameStatus"]));
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
            moveEvent(oldC, newC);
        }));
    }
}
