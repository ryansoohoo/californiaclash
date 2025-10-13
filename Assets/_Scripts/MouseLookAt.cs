using UnityEngine;

public class MouseLookAt : MonoBehaviour
{
    [Header("Pivot Reference")]
    [Tooltip("The transform that acts as the pivot for rotation. If left empty, this object will be used.")]
    public Transform pivot;

    [Header("Rotation Limits (relative to start)")]
    [Tooltip("Horizontal rotation limits (Yaw) in degrees.")]
    public Vector2 yawLimits = new Vector2(-60f, 60f);
    [Tooltip("Vertical rotation limits (Pitch) in degrees.")]
    public Vector2 pitchLimits = new Vector2(-20f, 30f);

    [Header("Rotation Behavior")]
    [Tooltip("How quickly the pivot rotates toward the mouse position.")]
    public float rotationSpeed = 10f;
    [Tooltip("Sensitivity of rotation toward mouse movement.")]
    public float sensitivity = 1f;
    [Tooltip("Lock vertical rotation (for only horizontal look).")]
    public bool lockVertical = false;

    private Camera _mainCam;
    private Transform _pivot;
    private Vector2 _baseRotation; // (pitch, yaw)
    private Vector2 _currentRotation;

    void Start()
    {
        _mainCam = Camera.main;
        _pivot = pivot != null ? pivot : transform;

        Vector3 startRot = _pivot.localEulerAngles;
        _baseRotation = new Vector2(NormalizeAngle(startRot.x), NormalizeAngle(startRot.y));
        _currentRotation = _baseRotation;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    void Update()
    {
        if (_mainCam == null || _pivot == null) return;

        // 1. Get mouse position as world point at pivot's distance
        float distance = Vector3.Distance(_mainCam.transform.position, _pivot.position);
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Mathf.Max(0.01f, distance);
        Vector3 targetWorld = _mainCam.ScreenToWorldPoint(mousePos);

        // 2. Calculate direction in local space
        Vector3 direction = targetWorld - _pivot.position;
        if (direction.sqrMagnitude < 0.0001f) return;

        Vector3 localDir = _pivot.InverseTransformDirection(direction.normalized);

        // 3. Convert to yaw/pitch
        float targetYaw = Mathf.Atan2(-localDir.x, -localDir.z) * Mathf.Rad2Deg;
        float targetPitch = Mathf.Atan2(localDir.y, new Vector2(localDir.x, localDir.z).magnitude) * Mathf.Rad2Deg;

        if (lockVertical) targetPitch = 0f;

        // 4. Apply sensitivity and clamp
        float desiredYaw = _baseRotation.y + Mathf.Clamp(targetYaw * sensitivity, yawLimits.x, yawLimits.y);
        float desiredPitch = _baseRotation.x + Mathf.Clamp(targetPitch * sensitivity, pitchLimits.x, pitchLimits.y);

        // 5. Smoothly interpolate toward target
        _currentRotation.y = Mathf.LerpAngle(_currentRotation.y, desiredYaw, 1f - Mathf.Exp(-rotationSpeed * Time.deltaTime));
        _currentRotation.x = Mathf.LerpAngle(_currentRotation.x, desiredPitch, 1f - Mathf.Exp(-rotationSpeed * Time.deltaTime));

        // 6. Apply rotation
        _pivot.localRotation = Quaternion.Euler(-_currentRotation.x, -_currentRotation.y, 0f);
    }

    private static float NormalizeAngle(float angle)
    {
        angle %= 360f;
        if (angle > 180f) angle -= 360f;
        return angle;
    }
}
