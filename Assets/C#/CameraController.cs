using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CameraController : MonoBehaviour
{
    public float HorizontalMoveSpeed = 150.0f;
    public float VerticalMoveSpeed = 150.0f;
    public TMP_InputField InputField;

    public float ZoomVelocity = 80f;
    public float MinZoom = 65f;
    public float MaxZoom = 159.0f;

    public float MinX = -100f;
    public float MaxX = 100f;
    public float MinY = -100f;
    public float MaxY = 100f;

    public float BaseMinX = -150f;
    public float BaseMaxX = 150f;
    public float BaseMinY = -125f;
    public float BaseMaxY = 125f;

    public float ScreenEdgeThreshold = 20f; 
    public float ZoomSmoothing = 10f;
    public float RollAngle = 7f; 
    public float RollReturnSpeed = 10f;
    public float PitchAngle = 7f; 
    public float PitchReturnSpeed = 10f;

    private Camera cam;
    private Vector3 newPosition;
    private float targetOrthographicSize;
    public float AnimSpeed = 50;
    private float AnimTimer;

    void Awake()
    {
        cam = GetComponent<Camera>();
        targetOrthographicSize = cam.orthographicSize; // Initialize target size
        UpdateBounds();
        HandleMovement();
        HandleZoom();
    }
    

    void Start()
    {
        cam.orthographic = false;
        cam.fieldOfView = 179;
        


    }

    void Update()
    {
        if(cam.orthographic == false)
        {
            AnimTimer += Time.deltaTime * AnimSpeed; 
            
            cam.fieldOfView = Mathf.Lerp(178, 50, AnimTimer);

            if (AnimTimer >= 1)
            {
                cam.orthographic = true;
            }
            else
            {
                return;
            }
        }
        
        if (InputField != null && InputField.isFocused)
            return;

        HandleMovement();
        HandleZoom();
        UpdateBounds();
        HandleRoll();
        HandlePitch();

    }

    void HandleMovement()
    {
        float moveX = Input.GetAxis("Horizontal") * HorizontalMoveSpeed * Time.deltaTime;
        float moveY = Input.GetAxis("Vertical") * VerticalMoveSpeed * Time.deltaTime;

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

        transform.position = newPosition;
    }

    void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel") * ZoomVelocity;
        if (cam.orthographic)
        {
            targetOrthographicSize -= scroll;
            targetOrthographicSize = Mathf.Clamp(targetOrthographicSize, MinZoom, MaxZoom);
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetOrthographicSize, Time.deltaTime * ZoomSmoothing);
        }
    }

    void UpdateBounds()
    {
        float zoomScale = (MaxZoom - cam.orthographicSize) / (MaxZoom - MinZoom);
        MinX = BaseMinX * zoomScale;
        MaxX = BaseMaxX * zoomScale;
        MinY = BaseMinY * zoomScale;
        MaxY = BaseMaxY * zoomScale;
    }
    
    void HandleRoll()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float targetYRotation = horizontalInput * RollAngle;
        Quaternion targetRotation = Quaternion.Euler(0, targetYRotation, 0);
        cam.transform.rotation = Quaternion.Lerp(cam.transform.rotation, targetRotation, Time.deltaTime * RollReturnSpeed);
    }
    
    void HandlePitch()
    {
        float verticalInput = Input.GetAxis("Vertical");
        float targetXRotation = -verticalInput * PitchAngle;
        Quaternion targetRotation = Quaternion.Euler(targetXRotation, 0, 0);
        cam.transform.rotation = Quaternion.Lerp(cam.transform.rotation, targetRotation, Time.deltaTime * PitchReturnSpeed);
    }
}