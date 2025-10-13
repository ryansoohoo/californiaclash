using UnityEngine;

/// <summary>
/// Attach to the Main Camera.
/// Rotates the camera based on mouse position (without locking the cursor),
/// adds a breathing bob effect, and optionally replaces the system cursor with a custom sprite.
/// </summary>
[RequireComponent(typeof(Camera))]
public class MouseLookFollowCursor : MonoBehaviour
{
    [Header("Mouse Look")]
    [Tooltip("How sensitive the camera is to cursor movement.")]
    public float sensitivity = 0.2f;

    [Tooltip("Maximum yaw (horizontal) and pitch (vertical) rotation from center.")]
    public Vector2 maxRotation = new Vector2(45f, 25f);

    [Tooltip("How smoothly the camera follows the cursor.")]
    [Range(0.1f, 30f)] public float rotationLerpSpeed = 8f;

    [Header("Breathing Bob (Position)")]
    public Vector3 breatheAmplitude = new Vector3(0f, 0.025f, 0f);
    public float breatheFrequency = 0.18f;

    [Header("Breathing Bob (Roll)")]
    public float breatheRollAmplitude = 0.5f;
    public float rollPhaseDegrees = 90f;

    [Header("Timing")]
    public bool useUnscaledTime = true;

    [Header("Custom Cursor")]
    [Tooltip("Enable to replace the default OS cursor with a custom texture.")]
    public bool useCustomCursor = true;
    public Texture2D customCursorTexture;
    [Tooltip("Hotspot offset in pixels from the top-left of the texture.")]
    public Vector2 cursorHotspot = Vector2.zero;
    [Tooltip("Choose between hardware and software cursor rendering.")]
    public CursorMode cursorMode = CursorMode.Auto;

    private Vector3 _basePos;
    private Quaternion _baseRot;
    private float _yaw;
    private float _pitch;

    void Start()
    {
        _basePos = transform.localPosition;
        _baseRot = transform.localRotation;

        // Make sure cursor is visible and unlocked
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // Apply custom cursor if enabled
        if (useCustomCursor && customCursorTexture != null)
        {
            Cursor.SetCursor(customCursorTexture, cursorHotspot, cursorMode);
        }
        else
        {
            // fallback to default system cursor
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }
    }

    void Update()
    {
        HandleMouseLook();
        ApplyBreathing();
    }

    void HandleMouseLook()
    {
        // Get mouse position relative to screen center (normalized -0.5..0.5)
        Vector2 mousePos = Input.mousePosition;
        float x = (mousePos.x / Screen.width) - 0.5f;
        float y = (mousePos.y / Screen.height) - 0.5f;

        // Map to rotation angles
        float targetYaw = x * maxRotation.x * 2f * sensitivity;
        float targetPitch = -y * maxRotation.y * 2f * sensitivity;

        // Smooth interpolation
        float dt = useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
        _yaw = Mathf.Lerp(_yaw, targetYaw, 1f - Mathf.Exp(-rotationLerpSpeed * dt));
        _pitch = Mathf.Lerp(_pitch, targetPitch, 1f - Mathf.Exp(-rotationLerpSpeed * dt));

        // Apply rotation relative to base
        transform.localRotation = _baseRot * Quaternion.Euler(_pitch, _yaw, 0f);
    }

    void ApplyBreathing()
    {
        float t = useUnscaledTime ? Time.unscaledTime : Time.time;
        float omega = Mathf.PI * 2f * breatheFrequency;

        Vector3 posOffset = new Vector3(
            breatheAmplitude.x * Mathf.Sin(omega * t),
            breatheAmplitude.y * Mathf.Sin(omega * t),
            breatheAmplitude.z * Mathf.Sin(omega * t)
        );

        float rollPhase = rollPhaseDegrees * Mathf.Deg2Rad;
        float roll = breatheRollAmplitude * Mathf.Sin(omega * t + rollPhase);

        transform.localPosition = _basePos + posOffset;
        transform.localRotation = transform.localRotation * Quaternion.Euler(0f, 0f, roll);
    }
}
