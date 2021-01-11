using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropdownSelection : MonoBehaviour
{

    private void Start()
    {
        PlayerPrefs.SetInt("tiempo", 60);
    }
    // Start is called before the first frame update
    public void OnValueChange()
    {
        Dropdown drop = GetComponent<Dropdown>();
        string text = drop.options[drop.value].text;
        Debug.Log(text);
        string []parse = text.Split(' ');
        int time = int.Parse(parse[0]);
        PlayerPrefs.SetInt("tiempo", time * 60);
    }
}
