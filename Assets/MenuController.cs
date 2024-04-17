using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using EasyUI.Popup;


public class MenuOption
{
    public string title;
    public string body;
    public GameObject arrowGameObject;
    public GameObject modelMarker;

    public MenuOption(string title, string body, GameObject arrowGameObject)
    {
        this.title = title;
        this.body = body;
        this.arrowGameObject = arrowGameObject;
        this.modelMarker = null;
    }

    public MenuOption(string title, string body, GameObject arrowGameObject, GameObject modelMarker)
    {
        this.title = title;
        this.body = body;
        this.arrowGameObject = arrowGameObject;
        this.modelMarker = modelMarker;
    }
}

public class MenuController : MonoBehaviour
{
    private string btnPressedName;

    private GameObject floatingPopUp;
    private TextMeshPro popUpTitle, popUpBody;
    private MenuOption currentMenuOption;
    private Dictionary<string, MenuOption> menuOptionsContainersDict;

    // Use this for initialization
    void Start()
    {
        floatingPopUp = GameObject.Find("FloatingPopUp");
        popUpTitle = GameObject.Find("PopUpTitle").GetComponent<TextMeshPro>();
        popUpBody = GameObject.Find("PopUpBody").GetComponent<TextMeshPro>();

        floatingPopUp.SetActive(false);

        InitializeMenuOptionsContainersDict();
        InitiaizeScene();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("Updated");
        // capture first click on screen
        if (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
            RaycastHit Hit;
            if (Physics.Raycast(ray, out Hit))
            {
                btnPressedName = Hit.transform.name;
                if (btnPressedName == "CloseButton")
                {
                    closePopUp();
                    return;
                }

                if (menuOptionsContainersDict.ContainsKey(btnPressedName))
                {
                    // set floatin pop up as active in first click
                    if (!floatingPopUp.activeSelf)
                    {
                        floatingPopUp.SetActive(true);
                    }

                    handleButtonClick(menuOptionsContainersDict[btnPressedName]);
                }
            }
        }
    }

    void closePopUp()
    {
        floatingPopUp.SetActive(false);
        currentMenuOption.arrowGameObject.SetActive(false);
        
        if (currentMenuOption.modelMarker != null)
        {
            currentMenuOption.modelMarker.SetActive(false);
        }
    }

    void handleButtonClick(MenuOption popUpInfo)
    {
        // Orchestrate all scene changes required for a button change
        setPopUpData(popUpInfo.title, popUpInfo.body);
        enableArrow(popUpInfo.arrowGameObject);
        enableModelMarker(popUpInfo.modelMarker);

        currentMenuOption = popUpInfo;
    }

    void setPopUpData(string title, string body)
    {
        // set pop up data
        popUpTitle.text = title;
        popUpBody.text = body;
    }

    void enableArrow(GameObject arrowGameObject) 
    {
        // Disable previous arrow and enable current arrow
        if (currentMenuOption != null && currentMenuOption.arrowGameObject != null)
        {
            currentMenuOption.arrowGameObject.SetActive(false);
        }

        if (arrowGameObject != null)
        {
            arrowGameObject.SetActive(true);
        }
    }

    void enableModelMarker(GameObject modelMarker)
    {
        // Disable previous marker and enable current market
        if(currentMenuOption != null && currentMenuOption.modelMarker != null)
        {
            currentMenuOption.modelMarker.SetActive(false);
        }

        if (modelMarker != null)
        {
            modelMarker.SetActive(true);
        }
    }

    // Initialize dictionary for Floating pop up values and related game objects
    void InitializeMenuOptionsContainersDict()
    {
        menuOptionsContainersDict = new Dictionary<string, MenuOption>();

        menuOptionsContainersDict["IOLinkTxtContainer"] = new MenuOption(
            "I/O Link",
            "This unit has an IO-Link communication interface which requires an IO-Link capable module (IO-Link master) for operation.\r\nThe IO-Link interface enables direct access to \r\nthe process and diagnostic data and provides the possibility to set the parameters of the unit during operation.",
            GameObject.Find("IOLinkArrow")
            );

        menuOptionsContainersDict["DisplayPanelTxtContainer"] = new MenuOption(
            "Display Panel",
            "bar/kPa/psi/in Hg = system pressure / differential pressure in the unit of measurement which is indicated in the label.\r\np ext = not used.\r\nOUT 1/OUT 2 = switching status of the output.",
            GameObject.Find("DisplayPanelArrow")
            );

        menuOptionsContainersDict["SetTxtContainer"] = new MenuOption(
            "Set",
            "Setting of the parameter values.\r\n(scrolling by holding pressed; incrementally by pressing once).",
            GameObject.Find("SetArrow"),
            GameObject.Find("OnModelContainer/Set")
        );

        menuOptionsContainersDict["ModeTxtContainer"] = new MenuOption(
            "Mode",
            "Selection of the parameters and acknowledgement of the parameter values",
            GameObject.Find("ModeArrow"),
            GameObject.Find("OnModelContainer/Mode")
        );

        menuOptionsContainersDict["LeftMountTxtContainer"] = new MenuOption(
            "Left Mount",
            "Used to fix the unit with 2 M4 x 35 screws to the rear panel.\r\nMaximum tightening torque: 0.5 Nm",
            GameObject.Find("LeftMountArrow"),
            GameObject.Find("OnModelContainer/LeftMount")
        );

        menuOptionsContainersDict["RightMountTxtContainer"] = new MenuOption(
            "Right Mount",
            "Used to fix the unit with 2 M4 x 35 screws to the rear panel.\r\nMaximum tightening torque: 0.5 Nm",
            GameObject.Find("RightMountArrow"),
            GameObject.Find("OnModelContainer/RightMount")
        );
    }

    void InitiaizeScene()
    {
        // Deactivate all game objects not necessary in the scene init
        foreach (var menuOption in menuOptionsContainersDict.Values)
        {
            if (menuOption.arrowGameObject != null)
            {
                menuOption.arrowGameObject.SetActive(false);
            }

            if (menuOption.modelMarker != null)
            {
                menuOption.modelMarker.SetActive(false);
            }
        }
    }
}
