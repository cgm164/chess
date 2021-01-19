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
    public GameObject ButtonOpciones;
    public GameObject ButtonStart;
    public GameObject PanelWin;

    private bool activadoButtonTexture;

    void Start()
    {
        PanelWin.SetActive(false);
        ButtonReanudar.SetActive(false);
        ButtonNueva.SetActive(false);
        ButtonSalir.SetActive(false);
        ButtonTexture.SetActive(false);
        panelMenu.SetActive(false);
        ButtonWooden.SetActive(false);
        ButtonMetal.SetActive(false);
        ButtonMarble.SetActive(false);
        panelTexture.SetActive(false);
        activadoButtonTexture = false;
        ButtonOpciones.SetActive(true);
        ButtonStart.SetActive(true);
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClick()
    {
        ButtonReanudar.SetActive(true);
        ButtonNueva.SetActive(true);
        ButtonTexture.SetActive(true);
        ButtonSalir.SetActive(true);
        panelMenu.SetActive(true);
        ButtonOpciones.SetActive(false);
        
    }

    public void OnClickReanudar()
    {
        ButtonReanudar.SetActive(false);
        ButtonNueva.SetActive(false);
        ButtonSalir.SetActive(false);
        panelMenu.SetActive(false);
        ButtonWooden.SetActive(false);
        ButtonMetal.SetActive(false);
        ButtonMarble.SetActive(false);
        panelTexture.SetActive(false);
        ButtonOpciones.SetActive(true);
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

    public void OnClickStart()
    {
        ButtonStart.SetActive(false);
    }
    
    public void BackWin()
    {
        PanelWin.SetActive(false);
    }
}
