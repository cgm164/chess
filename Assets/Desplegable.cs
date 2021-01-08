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
    public GameObject panel;
    public GameObject ButtonWooden;
    public GameObject ButtonMetal;
    public GameObject ButtonMarble;
    public GameObject ButtonReset;
    public GameObject panelTexture;

    private bool activadoButton;

    void Start()
    {
        ButtonReanudar.SetActive(false);
        ButtonNueva.SetActive(false);
        ButtonSalir.SetActive(false);
        ButtonTexture.SetActive(false);
        panel.SetActive(false);
        ButtonWooden.SetActive(false);
        ButtonMetal.SetActive(false);
        ButtonMarble.SetActive(false);
        ButtonReset.SetActive(false);
        panelTexture.SetActive(false);
        activadoButton = false;
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
            panel.SetActive(true);
            activadoButton = true;
        }
        else
        {
            ButtonReanudar.SetActive(false);
            ButtonNueva.SetActive(false);
            ButtonSalir.SetActive(false);
            panel.SetActive(false);
            activadoButton = false;
            ButtonWooden.SetActive(false);
            ButtonMetal.SetActive(false);
            ButtonMarble.SetActive(false);
            ButtonReset.SetActive(false);
            panelTexture.SetActive(false);
        }
    }

    public void OnClickTexture()
    {
        ButtonWooden.SetActive(true);
        ButtonMetal.SetActive(true);
        ButtonMarble.SetActive(true);
        ButtonReset.SetActive(true);
        panelTexture.SetActive(true);
    }

    }
