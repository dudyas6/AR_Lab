using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainScreen : MonoBehaviour
{
    public Button Btn_installation;
    public Button Btn_operation;
    public GameObject InstallationScreen;
    public GameObject OperationScreen;

    // Start is called before the first frame update
    void Start()
    {
        Btn_installation.onClick.AddListener(InstallationCMD);
        Btn_operation.onClick.AddListener(OperationCMD);

    }

    private void InstallationCMD()
    {
        Manager.Instance().ShowNextScreen(InstallationScreen);
    }

    private void OperationCMD()
    {
        Manager.Instance().ShowNextScreen(OperationScreen);
    }


}
