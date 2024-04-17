using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenAR : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject textObj;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void HideText()
    {
        textObj.SetActive(false);
    }

    public void ShowText()
    {
        textObj.SetActive(true);

    }

}
