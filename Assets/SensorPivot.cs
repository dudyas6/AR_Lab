using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SensorPivot : MonoBehaviour
{ 
    private bool isRotating;
    private Vector3 lastMousePosition;
    private float rotationSensitivity = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Check for left mouse button down
        {
            isRotating = true;
            lastMousePosition = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0)) // Check for left mouse button up
        {
            isRotating = false;
        }

        if (isRotating && Input.GetMouseButton(0)) // Check if left mouse button is held down
        {
            Vector2 mouseDelta = (Vector2)(Input.mousePosition - lastMousePosition);
            Vector3 rotation = new Vector3(-mouseDelta.y * rotationSensitivity, -mouseDelta.x * rotationSensitivity, 0f); // Adjust the axis of rotation

            transform.Rotate(rotation, Space.Self);

            lastMousePosition = Input.mousePosition;
        }
    }
}
