using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{

    public GameObject[] gameobjects;
    private Stack<GameObject> navStack = new Stack<GameObject>();
    private static Manager instance = null;
    public static Manager Instance()
    {
        if (instance == null)
            instance = new Manager();
        return instance;
    }

    // TODO: handle file for installation progress


    // TODO: handle file for operation progress



    public bool IsInstallationComplete { get; set; } = false;
    public bool IsOperationComplete { get; set; } = false;



    public Stack<GameObject> GetStack => navStack;

    private Manager() { instance = this; }

    public void ShowNextScreen(GameObject gameobj)
    {
        navStack.Peek().gameObject.SetActive(false); // hide current
        gameobj.gameObject.SetActive(true); // show next
        navStack.Push(gameobj); // save next screen
    }

    public void HideScreen()
    {
        // dont use it
        if (navStack.Count >= 1)
            navStack.Pop().gameObject.SetActive(false);  // hide current
    }

    public void GoBack()
    {
        if (navStack.Count >= 2)
        {
            navStack.Pop().gameObject.SetActive(false); // current screen
            navStack.Peek().gameObject.SetActive(true); // show previous
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        foreach (GameObject gameobject in gameobjects)
        {
            gameobject.gameObject.SetActive(false);
        }
        if (gameobjects != null) gameobjects[0].gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {

    }


}
