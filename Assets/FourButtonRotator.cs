using UnityEngine;
using TMPro;

public class FourButtonRotator : MonoBehaviour
{
    [Header("Object To Rotate")]
    public Transform targetObject;

    [Header("Rotation Settings")]
    public float rotationSpeed = 100f;

    [Header("UI")]
    public TMP_Text rotationText;

    [Header("Rotation Guides")]
    public bool showRotationGuides = true;
    public float guideRadius = 2f;
    public int guideSegments = 64;
    public float guideWidth = 0.02f;

    [Tooltip("Ring for Up/Down rotation")]
    public Color pitchColorNormal = new Color(0.2f, 0.6f, 1f, 0.15f);   // faint blue
    public Color pitchColorActive = new Color(0.2f, 0.6f, 1f, 1f);      // bright blue

    [Tooltip("Ring for Left/Right rotation")]
    public Color yawColorNormal = new Color(1f, 0.6f, 0.1f, 0.15f);     // faint orange
    public Color yawColorActive = new Color(1f, 0.6f, 0.1f, 1f);        // bright orange

    private LineRenderer pitchRing; // up/down rotation
    private LineRenderer yawRing;   // left/right rotation

    // Flags set true while a button is held down
    private bool holdLeft;
    private bool holdRight;
    private bool holdUp;
    private bool holdDown;

    // Reset support
    private bool isResetting;
    private static readonly Quaternion ZeroRotation = Quaternion.identity;

    private void Start()
    {
        if (showRotationGuides && targetObject != null)
        {
            pitchRing = CreateRing("PitchRing", Vector3.right);
            yawRing = CreateRing("YawRing", Vector3.up);
        }
    }

    private void Update()
    {
        if (targetObject == null)
            return;

        bool anyHeld = holdUp || holdDown || holdLeft || holdRight;

        if (anyHeld && isResetting)
            isResetting = false;

        if (isResetting)
        {
            RotateTowardsZero();
        }
        else
        {
            if (holdUp) RotateUp();
            if (holdDown) RotateDown();
            if (holdLeft) RotateLeft();
            if (holdRight) RotateRight();
        }

        UpdateRotationText();
        UpdateRingHighlight();
    }

    // ---- Actual rotation logic ----
    private void RotateLeft()
    {
        targetObject.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
    }

    private void RotateRight()
    {
        targetObject.Rotate(Vector3.down * rotationSpeed * Time.deltaTime);
    }

    private void RotateUp()
    {
        targetObject.Rotate(Vector3.right * rotationSpeed * Time.deltaTime);
    }

    private void RotateDown()
    {
        targetObject.Rotate(Vector3.left * rotationSpeed * Time.deltaTime);
    }

    // ---- Reset logic ----
    private void RotateTowardsZero()
    {
        targetObject.localRotation = Quaternion.RotateTowards(
            targetObject.localRotation,
            ZeroRotation,
            rotationSpeed * Time.deltaTime
        );

        if (Quaternion.Angle(targetObject.localRotation, ZeroRotation) < 0.01f)
        {
            targetObject.localRotation = ZeroRotation;
            isResetting = false;
        }
    }

    // Hook this up to the Reset button's OnClick event
    public void ResetRotation()
    {
        isResetting = true;
    }

    // ---- UI display ----
    private void UpdateRotationText()
    {
        if (rotationText == null)
            return;

        Vector3 euler = targetObject.localEulerAngles;
        rotationText.text = $"X: {euler.x:F0}  Y: {euler.y:F0}  Z: {euler.z:F0}";
    }

    // ---- Rotation guide rings ----
    private LineRenderer CreateRing(string name, Vector3 axis)
    {
        GameObject ringObj = new GameObject(name);
        ringObj.transform.SetParent(targetObject, false);

        LineRenderer lr = ringObj.AddComponent<LineRenderer>();
        lr.loop = true;
        lr.useWorldSpace = false;
        lr.positionCount = guideSegments;
        lr.widthMultiplier = guideWidth;
        lr.material = new Material(Shader.Find("Sprites/Default"));

        Vector3[] points = new Vector3[guideSegments];
        for (int i = 0; i < guideSegments; i++)
        {
            float angle = (i / (float)guideSegments) * Mathf.PI * 2f;
            Vector3 point = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * guideRadius;

            if (axis == Vector3.right) point = new Vector3(0, point.y, point.x);   // pitch plane
            else if (axis == Vector3.up) point = new Vector3(point.x, 0, point.y); // yaw plane

            points[i] = point;
        }
        lr.SetPositions(points);

        return lr;
    }

    private void UpdateRingHighlight()
    {
        if (pitchRing != null)
        {
            bool pitchActive = holdUp || holdDown;
            SetRingColor(pitchRing, pitchActive ? pitchColorActive : pitchColorNormal);
        }

        if (yawRing != null)
        {
            bool yawActive = holdLeft || holdRight;
            SetRingColor(yawRing, yawActive ? yawColorActive : yawColorNormal);
        }
    }

    private void SetRingColor(LineRenderer lr, Color color)
    {
        lr.startColor = color;
        lr.endColor = color;
    }

    // ---- Hook these up via EventTrigger (PointerDown / PointerUp) on each button ----
    public void SetHoldLeft(bool isHeld) { holdLeft = isHeld; }
    public void SetHoldRight(bool isHeld) { holdRight = isHeld; }
    public void SetHoldUp(bool isHeld) { holdUp = isHeld; }
    public void SetHoldDown(bool isHeld) { holdDown = isHeld; }
}