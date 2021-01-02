using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class prueba : MonoBehaviour
{
    public string mensaje;
    public GameObject obj;

    public void OnPrueba()
    {
        Debug.Log(mensaje);
        obj.transform.position += new Vector3(0, 180, 0);
        PlayerPrefs.SetInt("modoJuego", 1);
        int a = PlayerPrefs.GetInt("modoJuego");
        SceneManager.LoadScene(0);
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(gameObject.GetComponent<Rotation>().Rotate(gameObject.transform, Quaternion.Euler(0, 180, 0), 1.0f));
        StartCoroutine(gameObject.GetComponent<Rotation>().Translation(gameObject.transform, new Vector3(200, 20, 200), 1.0f, Rotation.MoveType.Time));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
