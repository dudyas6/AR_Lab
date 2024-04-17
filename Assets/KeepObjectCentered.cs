using EasyUI.Helpers;
using System;
using UnityEngine;
using UnityEngine.UI;
public class KeepObjectCentered : MonoBehaviour
{
    private Camera arCamera; // Reference to the AR camera
    private bool isRotating = false;
    private Vector2 lastTouchPosition;
    private bool isInDebugMode { get; set; } = true; // debug for us, meaning we need to use mouse
    public GameObject rotationPanel;
    private Quaternion defaultRotation;
    public Slider slider;
    Vector3 newObjectPosition;
    private float sensitivty = 0.8f;
    public static bool blocker = false;
    void Start()
    {
        arCamera = Camera.main;
        defaultRotation = transform.rotation;
        slider.onValueChanged.AddListener(OnSliderValueChanged);
        // Accessing children buttons using the parent panel
        for (int i = 0; i < rotationPanel.transform.childCount - 1; i++)
        {
            GameObject button = rotationPanel.transform.GetChild(i).gameObject;
            Button buttonComponent = button.GetComponent<Button>(); // Assuming child buttons have Button components
            int buttonIndex = i;
            // Add listeners for button click events or perform other operations as needed
            buttonComponent.onClick.AddListener(delegate { OnButtonClick(buttonIndex); });
        }


    }

    public Vector3 lastMousePosition;
    void Update()
    {
        bool isPopupActive = ScreensManager.Instance().getPopupActive();
        if (isPopupActive) {
            return;
        }
        // Check if the AR camera is available
        if (arCamera != null && Camera.main != null)
        {
            // keep positions and rotations
            newObjectPosition = arCamera.transform.position + arCamera.transform.forward * (70 - slider.value) - arCamera.transform.up * 15;
            Quaternion cameraRotation = arCamera.transform.rotation;

            transform.position = newObjectPosition;  // current object

            if (isInDebugMode)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    isRotating = true;
                    lastTouchPosition = Input.mousePosition;
                }
                else if (Input.GetMouseButtonUp(0))
                {
                    isRotating = false;
                }

                if (isRotating && Input.GetMouseButton(0))
                {
                    Vector2 touchDelta = ((Vector2)Input.mousePosition - lastTouchPosition) * sensitivty;
                    Vector3 rotation = new Vector3(touchDelta.y, -touchDelta.x, 0f); // Adjust the axis of rotation

                    transform.Rotate(rotation, Space.World);
                    lastTouchPosition = Input.mousePosition;
                }
            }
            else
            {
                if (Input.touchCount > 0)
                {
                    Touch touch = Input.GetTouch(0);

                    switch (touch.phase)
                    {
                        case TouchPhase.Began:
                            isRotating = true;
                            lastTouchPosition = touch.position;
                            break;

                        case TouchPhase.Moved:
                            Vector2 touchDelta = (touch.position - lastTouchPosition) * 0.05f;
                            Vector3 rotation = new Vector3(touchDelta.y, -touchDelta.x, 0f); // Adjust the axis of rotation

                            transform.Rotate(rotation, Space.World);

                            lastTouchPosition = touch.position;
                            break;

                        case TouchPhase.Ended:
                        case TouchPhase.Canceled:
                            isRotating = false;
                            break;
                    }
                }
            }
        }
    }

    void OnButtonClick(int buttonIndex)
    {

        // Determine which child button was clicked based on its index
        float rotationSpeed = 5000f;
        // You can perform different actions based on the clicked button index
        switch (buttonIndex)
        {
            case 0://UP
                transform.rotation = Quaternion.Euler(new Vector3(-90f, 0f, 0f));
                //transform.Rotate(Vector3.right, rotationSpeed * Time.deltaTime);
                break;
            case 1://DOWN
                transform.rotation = Quaternion.Euler(new Vector3(90f, 0f, 0f));
                //transform.Rotate(-Vector3.right, rotationSpeed * Time.deltaTime);
                break;
            case 2://LEFT
                transform.rotation = Quaternion.Euler(new Vector3(0f, 90f, 0f));
                // transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
                break;
            case 3://RIGHT 
                transform.rotation = Quaternion.Euler(new Vector3(0f, -90f, 0f));
                //transform.Rotate(Vector3.up, -rotationSpeed * Time.deltaTime);
                break;
            case 4://CENTER               
                transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));;
                break;
            default:
                break;
        }

    }

    void OnSliderValueChanged(float value)
    {
        isRotating = false;
    }

}
