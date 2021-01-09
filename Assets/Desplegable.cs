using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Desplegable : MonoBehaviour
{
    public GameObject ButtonReanudar;
    public GameObject ButtonNueva;
    public GameObject ButtonSalir;
    public GameObject ButtonTexture;
    public GameObject panelMenu;
    public GameObject ButtonWooden;
    public GameObject ButtonMetal;
    public GameObject ButtonMarble;
    public GameObject panelTexture;

    private bool activadoButton;
    private bool activadoButtonTexture;

    void Start()
    {
        ButtonReanudar.SetActive(false);
        ButtonNueva.SetActive(false);
        ButtonSalir.SetActive(false);
        ButtonTexture.SetActive(false);
        panelMenu.SetActive(false);
        ButtonWooden.SetActive(false);
        ButtonMetal.SetActive(false);
        ButtonMarble.SetActive(false);
        panelTexture.SetActive(false);
        activadoButton = false;
        activadoButtonTexture = false;
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClick()
    {
        if (activadoButton == false)
        {
            ButtonReanudar.SetActive(true);
            ButtonNueva.SetActive(true);
            ButtonTexture.SetActive(true);
            ButtonSalir.SetActive(true);
            panelMenu.SetActive(true);
            activadoButton = true;
        }
        else
        {
            ButtonReanudar.SetActive(false);
            ButtonNueva.SetActive(false);
            ButtonSalir.SetActive(false);
            panelMenu.SetActive(false);
            activadoButton = false;
            ButtonWooden.SetActive(false);
            ButtonMetal.SetActive(false);
            ButtonMarble.SetActive(false);
            panelTexture.SetActive(false);
        }
    }

    public void OnClickTexture()
    {
        if (activadoButtonTexture == false)
        {
            ButtonWooden.SetActive(true);
            ButtonMetal.SetActive(true);
            ButtonMarble.SetActive(true);
            panelTexture.SetActive(true);
            activadoButtonTexture = true;
        }
        else
        {
            ButtonWooden.SetActive(false);
            ButtonMetal.SetActive(false);
            ButtonMarble.SetActive(false);
            panelTexture.SetActive(false);
            activadoButtonTexture = false;
        }
    }

    }
