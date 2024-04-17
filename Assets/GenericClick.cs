using EasyUI.Popup;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using EasyUI.Helpers;

public class CircleClick : MonoBehaviour
{
    private CircleCollider2D circleCollider;
    private CapsuleCollider capsulaCollider;
    private BoxCollider2D boxCollider;
    private Camera mainCamera;

    public Sprite sprite;
    private Transform parentObject;
    private Vector3[] cubeFaceDirections;
    private GlowEffect glowingEffect;
    bool isFaceVisible = false, isPopupActive = false;

    private bool isInDebugMode { get; set; } = true; 

    void Start()
    {
        mainCamera = Camera.main;
        boxCollider = GetComponent<BoxCollider2D>();
        circleCollider = GetComponent<CircleCollider2D>();
        capsulaCollider = GetComponent<CapsuleCollider>();
        glowingEffect = GetComponentInChildren<GlowEffect>();

        parentObject = transform.parent;
        if (parentObject.parent != null)
            parentObject = parentObject.parent;

        cubeFaceDirections = new Vector3[]
        {
            parentObject.forward,  // Front face
            -parentObject.forward, // Back face
            parentObject.right,    // Right face
            -parentObject.right,   // Left face
            parentObject.up,       // Top face
            -parentObject.up       // Bottom face
        };

    }

    void Update()
    {
        isFaceVisible = IsFaceVisibleToCamera();
        if (boxCollider)
            boxCollider.transform.rotation = transform.rotation;

        if (circleCollider)
            circleCollider.transform.rotation = transform.rotation;

        if (capsulaCollider != null && capsulaCollider.transform && mainCamera != null)
        {
            Vector3 directionToCamera = mainCamera.transform.position - capsulaCollider.transform.position;
            capsulaCollider.transform.forward = -directionToCamera.normalized;
        }
    }

    void OnMouseDown()
    {
        bool isPopupActive = ScreensManager.Instance().getPopupActive();
        if (!isFaceVisible || isPopupActive) return;
        if (glowingEffect != null)
        {
            glowingEffect.StopGlowing();
        }
        switch (gameObject.name)
        {
            case "LeftCircle":
                LeftCircleAction();
                break;
            case "RightCircle":
                RightCircleAction();
                break;
            case "IOLink":
                IOLinkAction();
                break;
            case "RightMount":
                RightMountAction();
                break;
            case "LeftMount":
                LeftMountAction();
                break;
            case "DisplayPanel":
                DisplayPanelAction();
                break;
            default:
                break;
        }

    }

    bool IsFaceVisibleToCamera()
    {
        bool isFaceVisible = false;
        Vector3[] cubeFaceDirections = new Vector3[] {
            Vector3.forward, Vector3.back,
            Vector3.left, Vector3.right,
            Vector3.up, Vector3.down
        };

        foreach (Vector3 faceDirection in cubeFaceDirections)
        {
            Vector3 worldFaceDirection = parentObject.TransformDirection(faceDirection);
            Vector3 toFace = parentObject.position + worldFaceDirection * 0.5f - mainCamera.transform.position;

            Quaternion cubeRotationInverse = Quaternion.Inverse(parentObject.rotation);
            toFace = cubeRotationInverse * toFace;

            float dotProduct = Vector3.Dot(toFace.normalized, mainCamera.transform.forward);

            if (dotProduct > 0.35f) // Adjust this threshold as needed
            {
                isFaceVisible = true;
            }
        }

        return isFaceVisible;
    }

    #region Actions
    private void IOLinkAction()
    {

        string page2Text = "This unit has an IO-Link communication interface which requires an IO-Linkcapable" +
            " module(IO - Link master) for operation.";
        string page3Text = "The IO-Link interface enables direct access to the process and diagnostic data " +
            "and provides the possibility to set the parameters of the unit during operation.";
        string page4Text = "In addition communication is possible via a point-to-point connection with a USB adapter cable.";

        Popup.Show("I/O Link info", page2Text + "\n\n" + page3Text + "\n\n" + page4Text, "Close", PopupColor.Blue);
    }

    private void RightCircleAction()
    {
        string setButtonInfo = "Setting of the parameter values \n(scrolling by holding pressed; incrementally by pressing once).";
        Popup.Show("Set info", setButtonInfo, "Close", PopupColor.Blue);
    }
    private void LeftCircleAction()
    {
        string modeButtonInfo = "Selection of the parameters and acknoledgement of the parameter values";
        Popup.Show("Mode info", modeButtonInfo, "Close", PopupColor.Blue);
    }

    private void RightMountAction()
    {
        Popup.Show("Right Mount", "Used to fix the unit with 2 M4 x 35 screws to the rear panel.\nMaximum tightening torque: 0.5 Nm", "Close", PopupColor.Blue);
    }

    private void LeftMountAction()//added by gal
    {
        Popup.Show("Left Mount", "Used to fix the unit with 2 M4 x 35 screws to the rear panel.\nMaximum tightening torque: 0.5 Nm ", "Close", PopupColor.Blue);
    }
    private void DisplayPanelAction()
    {
        Popup.Show("Display", "bar/kPa/psi/in Hg = system pressure / differential pressure in the unit of measurement which is indicated in the label.\np ext = not used.\nOUT 1/OUT 2 = switching status of the output.", "Close", PopupColor.Blue);
    }
    #endregion
}
