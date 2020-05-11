using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraMovement : MonoBehaviour
{
    public Transform target;
    private Camera gameCamera;
    private Vector3 mouseDragOrigin;

    /// <summary>
    /// Gets current camera at the beginning
    /// </summary>
    private void Awake() =>
        gameCamera = GetComponent<Camera>();

    void Update()
    {
        // Depending on game mode
        if (gameCamera.orthographic)
        {
            gameCamera.orthographicSize += -Input.mouseScrollDelta.y;
            gameCamera.orthographicSize = Mathf.Max(gameCamera.orthographicSize, 1.0f);
        }
        else
            gameCamera.transform.Translate(new Vector3(0, 0, Input.mouseScrollDelta.y));

        // Camera view
        if (Input.GetKey(KeyCode.LeftAlt))
        {
            if (Input.GetMouseButtonDown(1))
                mouseDragOrigin = Input.mousePosition;
            else if (Input.GetMouseButton(1))
            {
                Vector3 mouseMovement = mouseDragOrigin - Input.mousePosition;
                mouseDragOrigin = Input.mousePosition;
                gameCamera.transform.Translate(mouseMovement);
                gameCamera.transform.LookAt(target);
            }
        }
        else
        {
            if (Input.GetMouseButtonDown(1))
                mouseDragOrigin = Input.mousePosition;
            else if (Input.GetMouseButton(1))
            {
                Vector3 mouseMovement = mouseDragOrigin - Input.mousePosition;
                mouseDragOrigin = Input.mousePosition;
                gameCamera.transform.Translate(mouseMovement * 0.1f);
            }
        }
    }
}
