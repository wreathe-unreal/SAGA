using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

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

    public float ScreenEdgeThreshold = 1f; 
    public float ZoomSmoothing = 10f;
    public float RollAngle = 7f; 
    public float RollReturnSpeed = 10f;
    public float PitchAngle = 7f; 
    public float PitchReturnSpeed = 10f;
    public float ZoomMovementSpeed = 40f;
    public float SkyBoxRotationSpeed = -.4f;

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
        if (ActionGUI.PanelState != EPanelState.EndState)
        {
            RenderSettings.skybox.SetFloat("_Rotation", Time.time * SkyBoxRotationSpeed);
        
            if(cam.orthographic == false)
            {
                AnimTimer += Time.unscaledDeltaTime * AnimSpeed; 
            
                cam.fieldOfView = Mathf.Lerp(178, 59, AnimTimer);

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
            {
                return;
            }

            HandleMovement();
            HandleZoom();
            HandleZoomMovement();
            UpdateBounds();
            HandleRoll();
            HandlePitch();  
        }
    }

    void HandleMovement()
    {
        float moveX = Input.GetAxisRaw("Horizontal") * HorizontalMoveSpeed * Time.unscaledDeltaTime;
        float moveY = Input.GetAxisRaw("Vertical") * VerticalMoveSpeed * Time.unscaledDeltaTime;

        if (Input.mousePosition.x <= ScreenEdgeThreshold)
            moveX -= HorizontalMoveSpeed * Time.unscaledDeltaTime;
        if (Input.mousePosition.x >= Screen.width - ScreenEdgeThreshold)
            moveX += HorizontalMoveSpeed * Time.unscaledDeltaTime;
        if (Input.mousePosition.y <= ScreenEdgeThreshold)
            moveY -= VerticalMoveSpeed * Time.unscaledDeltaTime;
        if (Input.mousePosition.y >= Screen.height - ScreenEdgeThreshold)
            moveY += VerticalMoveSpeed * Time.unscaledDeltaTime;

        newPosition = transform.position + new Vector3(moveX, moveY, 0);
        newPosition.x = Mathf.Clamp(newPosition.x, MinX, MaxX);
        newPosition.y = Mathf.Clamp(newPosition.y, MinY, MaxY);

        transform.position = newPosition;
    }

    void HandleZoom()
    {
        
        float scroll = Input.GetAxisRaw("Mouse ScrollWheel") * ZoomVelocity;
        if (cam.orthographic)
        {
            targetOrthographicSize -= scroll;
            targetOrthographicSize = Mathf.Clamp(targetOrthographicSize, MinZoom, MaxZoom);
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetOrthographicSize, Time.unscaledDeltaTime * ZoomSmoothing);
        }
    }

    void HandleZoomMovement()
    {
        // for moving while zooming
        Vector3 MousePos = Input.mousePosition;
        MousePos = cam.ScreenToViewportPoint(MousePos);
        MousePos.x -= .5f;
        MousePos.y -= .5f;
        
        if (targetOrthographicSize > cam.orthographicSize)
        {
            MousePos *= -1;
        }
        
        MousePos *= ZoomMovementSpeed * 10 * Time.unscaledDeltaTime;

        
        
        if (Mathf.Abs(cam.orthographicSize - targetOrthographicSize) > .5f)
        {
            newPosition = transform.position + new Vector3(MousePos.x, MousePos.y, 0);
            newPosition.x = Mathf.Clamp(newPosition.x, MinX, MaxX);
            newPosition.y = Mathf.Clamp(newPosition.y, MinY, MaxY);

            transform.position = newPosition;
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
        cam.transform.rotation = Quaternion.Lerp(cam.transform.rotation, targetRotation, Time.unscaledDeltaTime * RollReturnSpeed);
    }
    
    void HandlePitch()
    {
        float verticalInput = Input.GetAxis("Vertical");
        float targetXRotation = -verticalInput * PitchAngle;
        Quaternion targetRotation = Quaternion.Euler(targetXRotation, 0, 0);
        cam.transform.rotation = Quaternion.Lerp(cam.transform.rotation, targetRotation, Time.unscaledDeltaTime * PitchReturnSpeed);
    }
}