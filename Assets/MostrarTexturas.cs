using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MostrarTexturas : MonoBehaviour
{

    public GameObject king;
    public Material material_madera_oscura;
    public Material material_marmol_oscuro;
    public Material material_oro;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MaterialWooden()
    {
        king.GetComponent<MeshRenderer>().material = material_madera_oscura;
        PlayerPrefs.SetInt("textura", 1);
    }

    public void MaterialMetal()
    {
        king.GetComponent<MeshRenderer>().material = material_oro;
        PlayerPrefs.SetInt("textura", 2);
    }

    public void MaterialMarble()
    {
        king.GetComponent<MeshRenderer>().material = material_marmol_oscuro;
        PlayerPrefs.SetInt("textura", 3);
    }

}
