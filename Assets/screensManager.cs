using EasyUI.Popup;
using System.Linq;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Gpm.WebView;
using static System.Net.WebRequestMethods;
public class ScreensManager : MonoBehaviour
{
    [Header("Game Objects")]
    public GameObject ARScreen;
    public GameObject Screen3D;
    public GameObject HomeScreen;
    public GameObject Others;
    public GameObject rotationPanel;
    public GameObject menuPanel;
    public GameObject quizScreen;
    [Header("Text")]
    public TextMeshProUGUI mainScreenTxt;
    [Header("Buttons")]
    public Button btn_return;
    public Button btn_3d;
    public Button btn_ar;
    public Button btn_quiz;
    public Button btn_about;
    public Button btn_quit;
    public Button btn_pdfOperation;
    public Button btn_pdfInstallation;

    private bool showMenuFlag = true;
    private bool showRotationFlag = true;
    private bool isFirstStart = true;
    private GameObject buttons;
    private static ScreensManager instance = null;
    private bool isPopupActive = false;

    public static ScreensManager Instance()
    {
        if (instance == null)
            instance = new ScreensManager();
        return instance;
    }
    private ScreensManager() { instance = this; }


    void Start()
    {
        // This declares AR screen as the first screen by default, and displays General information only for first start//
        ReturnToHomeScreen();
        btn_return.onClick.AddListener(ReturnToHomeScreen);
        btn_3d.onClick.AddListener(Start3D);
        btn_ar.onClick.AddListener(StartAR);
        btn_quiz.onClick.AddListener(() => ToggleScreens(false, false, false, false, true));
        btn_about.onClick.AddListener(AboutUs);
        btn_quit.onClick.AddListener(exitProgram);
        btn_pdfOperation.onClick.AddListener(OpenOperationPDF);
        btn_pdfInstallation.onClick.AddListener(OpenInstallationPDF);

        for (int i = 0; i < menuPanel.transform.childCount; i++)
        {
            GameObject button = menuPanel.transform.GetChild(i).gameObject;
            Button buttonComponent = button.GetComponent<Button>(); // Assuming child buttons have Button components
            int buttonIndex = i;
            // Add listeners for button click events or perform other operations as needed
            buttonComponent.onClick.AddListener(delegate { OnButtonClick(buttonIndex); });
        }


        rotationPanel.SetActive(false);
        GameObject buttonsOnStart = menuPanel.transform.GetChild(1).gameObject;
        buttonsOnStart.SetActive(false);
        buttonsOnStart = menuPanel.transform.GetChild(2).gameObject;
        buttonsOnStart.SetActive(false);
        buttonsOnStart = menuPanel.transform.GetChild(3).gameObject;
        buttonsOnStart.SetActive(false);

        if (ARScreen.activeSelf)
        {
            buttonsOnStart = menuPanel.transform.GetChild(3).gameObject;
            buttonsOnStart.SetActive(false);
        }

    }

    // Update is called once per frame
    public void ReturnToHomeScreen()
    {
        ToggleScreens(false, false, false, true);
    }

    public void Start3D()
    {
        Debug.Log("3D");
        CloseMenu();
        ToggleScreens(true, true, false, false);
        rotationPanel.SetActive(false);
        showRotationFlag = true;
    }

    public void StartAR()
    {
        Debug.Log("AR");
        ToggleScreens(true, false, true, false);
        if (isFirstStart)
        {
            // DisplayGeneralInformationPopUp();
            DisplaySafetyPopUp();
            isFirstStart = false;
        }
        CloseMenu();

    }

    public void AboutUs()
    {
        string aboutUs = "Duda\nGal\nThe PoizeX\nBigDaddyNate";
        Popup.Show("Our Team:", aboutUs, "Close", PopupColor.Blue);
    }

    private void ToggleScreens(bool others, bool screen3D, bool arScreen, bool homeScreen, bool showQuiz = false)
    {
        Others.SetActive(others);
        Screen3D.SetActive(screen3D);
        ARScreen.SetActive(arScreen);
        HomeScreen.SetActive(homeScreen);
        quizScreen.SetActive(showQuiz);
    }

    public void ShowRotationPanel()
    {
        for (int i = 0; i < rotationPanel.transform.childCount; i++)
        {
            GameObject button = rotationPanel.transform.GetChild(i).gameObject;
            button.SetActive(true);
            // Add listeners for button click events or perform other operations as needed
        }
    }

    private void DisplayGeneralInformationPopUp()
    {
        string delimeter = ";";

        // define pop up text strings
        string IOLink1 = "This unit has an IO-Link communication interface which requires an IO-Linkcapable module (IO-Link master) for operation.";
        string IOLink2 = "The IO-Link interface enables direct access to the process and diagnostic data, and provides the possibility to set the parameters of the unit during operation.";
        string IOLink3 = "In addition communication is possible via a point-to-point connection with a USB adapter cable.\nYou will find more detailed information about IO-Link at www.ifm.com.";
        string outro = "For a more detailed information about the sensor, please read the full sensor docs, a refrence will be available once you continue.\nEnjoy!";

        // extract max text length and pad intro message to prevent message cut bug (WA)
        List<string> strings = new List<string> { IOLink1, IOLink2, IOLink3, outro };
        int maxLength = strings.Max(str => str.Length);
        string paddedWelcome = IOLink1.PadRight(maxLength);
        strings.Insert(0, paddedWelcome);

        // build final popUp content string seperated by delimeter
        string popUpContent = string.Join(delimeter, strings);
        void DebugLogAction() { Debug.Log("Display General information"); }

        // The last argument is maxText length per window, and is set to the maximum of the above strings.
        // The intro string length should match this length - pad with whitespace at the end if necessary.
        // This is an ugly workaround, and needs a good solution when time allows.
        Popup.Show("I/O Link", popUpContent, "Close", PopupColor.Magenta, DebugLogAction, delimeter, maxLength);
    }


    private void DisplaySafetyPopUp()
    {
        string delimeter = ";";
        // define pop up text strings
        string warningSign = "\u26A0";
        string safety0 = "Avoid static and dynamic overpressure exceeding the specified overload pressure by taking appropriate measures.";
        string safety1 = "The indicated bursting pressure must not be exceeded.";
        string safety2 = "Even if the bursting pressure is exceeded only for a short time, the unit may be destroyed.\nATTENTION: Risk of injury!";
        string safety3 = "Permissible overload pressure in the auxiliary connection as opposed to the main connection: 2 bar / 29 PSI.";
        string safety4 = "Bursting pressure of the auxiliary connection as opposed to the main connection: 12 bar / 174 PSI.";
        // extract max text length and pad intro message to prevent message cut bug (WA)
        List<string> strings = new List<string> { safety1, safety2, safety3, safety4 };
        int maxLength = strings.Max(str => str.Length);
        string paddedWelcome = safety0.PadRight(maxLength);
        strings.Insert(0, paddedWelcome);
        // build final popUp content string seperated by delimeter
        string popUpContent = string.Join(delimeter, strings);
        void DebugLogAction() { Debug.Log("Display General information"); }
        // The last argument is maxText length per window, and is set to the maximum of the above strings.
        // The intro string length should match this length - pad with whitespace at the end if necessary.
        // This is an ugly workaround, and needs a good solution when time allows.
        Popup.Show(warningSign + " Safety!", popUpContent, "Close", PopupColor.Red, DebugLogAction, delimeter, maxLength);
    }

    public void ShowUrlFullScreen(string pageTitle, string htmlFile)
    {
        GpmWebView.ShowUrl(
            htmlFile,
            new GpmWebViewRequest.Configuration()
            {
                style = GpmWebViewStyle.FULLSCREEN,
                orientation = GpmOrientation.UNSPECIFIED,
                isClearCookie = true,
                isClearCache = true,
                backgroundColor = "#FFFFFF",
                isNavigationBarVisible = true,
                navigationBarColor = "#4B96E6",
                title = pageTitle,
                isBackButtonVisible = true,
                isForwardButtonVisible = true,
                isCloseButtonVisible = true,
                supportMultipleWindows = true,
#if UNITY_IOS
            contentMode = GpmWebViewContentMode.MOBILE
#endif
            },
            // See the end of the code example
            OnCallback,
            new List<string>()
            {
            "USER_ CUSTOM_SCHEME"
            });
    }

    public void ShowHtmlFile()
    {
        var htmlFilePath = string.Empty;
#if UNITY_IOS
        htmlFilePath = string.Format("file://{0}/{1}", Application.streamingAssetsPath, "pdfInstallation.pdf");
#elif UNITY_ANDROID
        htmlFilePath = string.Format("file:///android_asset/{0}", "pdfInstallation.pdf");
#endif//Assets/StreamingAssets/html/pdfInstallation.pdf

        GpmWebView.ShowHtmlFile(
            htmlFilePath,
            new GpmWebViewRequest.Configuration()
            {
                style = GpmWebViewStyle.FULLSCREEN,
                orientation = GpmOrientation.UNSPECIFIED,
                isClearCookie = true,
                isClearCache = true,
                backgroundColor = "#FFFFFF",
                isNavigationBarVisible = true,
                navigationBarColor = "#4B96E6",
                title = "The page title.",
                isBackButtonVisible = true,
                isForwardButtonVisible = true,
                isCloseButtonVisible = true,
                supportMultipleWindows = true,
#if UNITY_IOS
            contentMode = GpmWebViewContentMode.MOBILE
#endif
            },
            OnCallback,
            new List<string>()
            {
            "USER_ CUSTOM_SCHEME"
            });
    }
    private void OnCallback(
    GpmWebViewCallback.CallbackType callbackType,
    string data,
    GpmWebViewError error)
    {
        Debug.Log("OnCallback: " + callbackType);
        switch (callbackType)
        {
            case GpmWebViewCallback.CallbackType.Open:
                if (error != null)
                {
                    Debug.LogFormat("Fail to open WebView. Error:{0}", error);
                }
                break;
            case GpmWebViewCallback.CallbackType.Close:
                if (error != null)
                {
                    Debug.LogFormat("Fail to close WebView. Error:{0}", error);
                }
                break;
            case GpmWebViewCallback.CallbackType.PageStarted:
                if (string.IsNullOrEmpty(data) == false)
                {
                    Debug.LogFormat("PageStarted Url : {0}", data);
                }
                break;
            case GpmWebViewCallback.CallbackType.PageLoad:
                if (string.IsNullOrEmpty(data) == false)
                {
                    Debug.LogFormat("Loaded Page:{0}", data);
                }
                break;
            case GpmWebViewCallback.CallbackType.MultiWindowOpen:
                Debug.Log("MultiWindowOpen");
                break;
            case GpmWebViewCallback.CallbackType.MultiWindowClose:
                Debug.Log("MultiWindowClose");
                break;
            case GpmWebViewCallback.CallbackType.Scheme:
                if (error == null)
                {
                    if (data.Equals("USER_ CUSTOM_SCHEME") == true || data.Contains("CUSTOM_SCHEME") == true)
                    {
                        Debug.Log(string.Format("scheme:{0}", data));
                    }
                }
                else
                {
                    Debug.Log(string.Format("Fail to custom scheme. Error:{0}", error));
                }
                break;
            case GpmWebViewCallback.CallbackType.GoBack:
                Debug.Log("GoBack");
                break;
            case GpmWebViewCallback.CallbackType.GoForward:
                Debug.Log("GoForward");
                break;
            case GpmWebViewCallback.CallbackType.ExecuteJavascript:
                Debug.LogFormat("ExecuteJavascript data : {0}, error : {1}", data, error);
                break;
#if UNITY_ANDROID
            case GpmWebViewCallback.CallbackType.BackButtonClose:
                Debug.Log("BackButtonClose");
                break;
#endif
        }
    }

    void OnButtonClick(int buttonIndex)
    {
        buttons = menuPanel.transform.GetChild(0).gameObject;
        switch (buttonIndex)
        {
            case 0://Menu
                if (showMenuFlag == true) // open menu
                {
                    buttons.SetActive(true);
                    buttons = menuPanel.transform.GetChild(1).gameObject;
                    buttons.SetActive(true);
                    buttons = menuPanel.transform.GetChild(2).gameObject;
                    buttons.SetActive(true);
                    buttons = menuPanel.transform.GetChild(3).gameObject;
                    buttons.SetActive(true);
                    showMenuFlag = false;
                }
                else //close menu 
                {
                    CloseMenu();
                }
                break;

            case 1://pdfInstallation

                break;
            case 2://pdfOperation

                break;

            case 3://rotate object
                if (showRotationFlag == true)
                {
                    rotationPanel.SetActive(true);
                    showRotationFlag = false;
                }
                else
                {
                    rotationPanel.SetActive(false);
                    showRotationFlag = true;
                }
                break;
            default:
                break;
        }

    }

    private void OpenOperationPDF()
    {
        string operationHTML = "https://pdfoperation.tiiny.site";
        ScreensManager.Instance().ShowUrlFullScreen("Operation PDF", operationHTML);
    }

    private void OpenInstallationPDF()
    {
        string installationHTML = "https://salmon-dorolice-28.tiiny.site";
        ScreensManager.Instance().ShowUrlFullScreen("Installation PDF", installationHTML);
    }

    private void OpenRotationPanel()
    {
        ScreensManager.Instance().ShowRotationPanel();
    }
    private void exitProgram()
    {
        Popup.Show("Exiting Program", "Thank you for using our program!", "Close", PopupColor.Blue, QuitGame, null, -1, true);
    }
    public void QuitGame()
    {
    #if UNITY_EDITOR

        UnityEditor.EditorApplication.isPlaying = false;
    #else
         Application.Quit();
    #endif
    }

    void Update()
    {
        // Check if the object is active
        if (ARScreen.activeSelf)
        {
            // The object is active
            buttons = menuPanel.transform.GetChild(3).gameObject;
            buttons.SetActive(false);
            rotationPanel.SetActive(false);
        }

    }

    void CloseMenu()
    {
        buttons = menuPanel.transform.GetChild(1).gameObject;
        buttons.SetActive(false);
        buttons = menuPanel.transform.GetChild(2).gameObject;
        buttons.SetActive(false);
        buttons = menuPanel.transform.GetChild(3).gameObject;
        buttons.SetActive(false);
        showMenuFlag = true;
    }

    public void setPopupActive(bool active)
    {
        isPopupActive = active;
    }

    public bool getPopupActive()
    {
        return isPopupActive;
    }
}
