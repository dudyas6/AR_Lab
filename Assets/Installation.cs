using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Installation : MonoBehaviour
{
    public GameObject CurrentScreen;
    public GameObject NextScreen;
    public Button Btn_back;


    // Start is called before the first frame update
    void Start()
    {
        Btn_back.onClick.AddListener(Manager.Instance().GoBack);
    }

    // Update is called once per frame
    void Update()
    {

    }

  
}
