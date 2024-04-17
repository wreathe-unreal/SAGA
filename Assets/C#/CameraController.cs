using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CameraController : MonoBehaviour
{
    public Vector3 StartPositionOffset = new Vector3(0, 0, 0);
    public float HorizontalMoveSpeed = 50.0f;
    public float VerticalMoveSpeed = 50.0f;
    public TMP_InputField InputField;

    public float ZoomVelocity = 15f;
    public float MinZoom = 60f;
    public float MaxZoom = 175.0f;

    public float MinX = -100f;
    public float MaxX = 100f;
    public float MinY = -100f;
    public float MaxY = 100f;
    
    // Base bounds before scaling
    public float BaseMinX = -175f;
    public float BaseMaxX = 175f;
    public float BaseMinY = -150f;
    public float BaseMaxY = 15f;

    public float ScreenEdgeThreshold = 20f; 

    private Camera cam;
    private Vector3 newPosition;

    void Start()
    {
        cam = GetComponent<Camera>();
        cam.transform.position = StartPositionOffset + new Vector3(0, 0, -10); // Ensure Z position keeps camera focused
    }

    void Update()
    {
        if (InputField != null && InputField.isFocused)
            return;

        HandleMovement();
        HandleZoom();
        UpdateBounds();
    }

    void HandleMovement()
    {
        float moveX = Input.GetAxis("Horizontal") * HorizontalMoveSpeed * Time.deltaTime;
        float moveY = Input.GetAxis("Vertical") * VerticalMoveSpeed * Time.deltaTime;

        // Check for mouse position at the edges of the screen
        if (Input.mousePosition.x <= ScreenEdgeThreshold)
            moveX -= HorizontalMoveSpeed * Time.deltaTime;
        if (Input.mousePosition.x >= Screen.width - ScreenEdgeThreshold)
            moveX += HorizontalMoveSpeed * Time.deltaTime;
        if (Input.mousePosition.y <= ScreenEdgeThreshold)
            moveY -= VerticalMoveSpeed * Time.deltaTime;
        if (Input.mousePosition.y >= Screen.height - ScreenEdgeThreshold)
            moveY += VerticalMoveSpeed * Time.deltaTime;

        newPosition = transform.position + new Vector3(moveX, moveY, 0);
        newPosition.x = Mathf.Clamp(newPosition.x, MinX, MaxX);
        newPosition.y = Mathf.Clamp(newPosition.y, MinY, MaxY);

        // Apply the clamped position
        transform.position = newPosition;
    }

    void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel") * ZoomVelocity;
        if (cam.orthographic)
        {
            cam.orthographicSize -= scroll;
            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, MinZoom, MaxZoom);
        }
    }

    // Update the movement bounds based on the current zoom level
    void UpdateBounds()
    {
        float zoomScale = (MaxZoom - cam.orthographicSize) / (MaxZoom - MinZoom);
        MinX = BaseMinX * zoomScale;
        MaxX = BaseMaxX * zoomScale;
        MinY = BaseMinY * zoomScale;
        MaxY = BaseMaxY * zoomScale;
    }
}