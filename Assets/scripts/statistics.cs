using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class statistics : MonoBehaviour
{
    public int Player_health = 100;
    public int Player_money = 500;
    public int level =1;
    void Start()
    {
        //get text component
        TextMeshProUGUI text = GetComponent<TextMeshProUGUI>();
        //set text
        text.text = "Health: " + Player_health + "\n" + "Money: " + Player_money;

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
