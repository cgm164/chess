using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public void PulsarReturn()
    {
        SceneManager.LoadScene("Menu", LoadSceneMode.Single);
    }

    public void Pulsar1Player()
    {
        PlayerPrefs.SetInt("modoJuego",1);
        SceneManager.LoadScene("Game", LoadSceneMode.Single);
    }

    public void Pulsar2Player()
    {
        PlayerPrefs.SetInt("modoJuego", 0);
        SceneManager.LoadScene("Game", LoadSceneMode.Single);
    }

    public void PulsarPlay()
    {
        SceneManager.LoadScene("Mode", LoadSceneMode.Single);
    }

    public void PulsarTextures()
    {
        SceneManager.LoadScene("Textures", LoadSceneMode.Single);
    }

    public void PulsarCredits()
    {
        SceneManager.LoadScene("Credits", LoadSceneMode.Single);
    }

    public void PulsarHome()
    {
        SceneManager.LoadScene("Menu", LoadSceneMode.Single);
    }

    public void OnExit()
    {
        Application.Quit();
    }
}
