using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    [SerializeField]
    Transform cameraTransform;
    [SerializeField]
    Transform cameraPivotTransform;
    float cameraSpeed = 10f;
    float cameraSensitivity = 0.25f;

    Vector3 lastMouse = new Vector3(255, 255, 255);

    private void Update() {
        Vector3 p = GetKeyboardInput();
        p *= cameraSpeed * Time.deltaTime;
        transform.Translate(p);

        cameraTransform.Translate(Vector3.forward * Input.mouseScrollDelta.y);
        if (Input.GetMouseButton(2)) {
            /*
            lastMouse = Input.mousePosition;
            lastMouse = new Vector3(-lastMouse.y * cameraSensitivity, lastMouse.x * cameraSensitivity, 0);
            lastMouse = new Vector3(cameraTransform.eulerAngles.x + lastMouse.x, cameraTransform.eulerAngles.y + lastMouse.y, 0);
            cameraTransform.eulerAngles = lastMouse;
            lastMouse = Input.mousePosition;
            */

            lastMouse = Input.mousePosition - lastMouse;
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y - (lastMouse.x * cameraSensitivity), 0);
            cameraPivotTransform.eulerAngles = new Vector3(cameraPivotTransform.eulerAngles.x + (lastMouse.y * cameraSensitivity), transform.eulerAngles.y, 0);
            lastMouse = Input.mousePosition;


        }
    }



    private Vector3 GetKeyboardInput() {
        Vector3 input = new Vector3();

        if (Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S))
            input += new Vector3(0, 0, 1);
        else if (Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.W))
            input += new Vector3(0, 0, -1);
        if (Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
            input += new Vector3(-1, 0, 0);
        else if (Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A))
            input += new Vector3(1, 0, 0);
        if (Input.GetKey(KeyCode.Q) && !Input.GetKey(KeyCode.E))
            input += new Vector3(0, -1, 0);
        else if (Input.GetKey(KeyCode.E) && !Input.GetKey(KeyCode.Q))
            input += new Vector3(0, 1, 0);

        return input;
    }
}
