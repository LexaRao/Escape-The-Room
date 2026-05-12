using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public sealed class MazeController : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 50f;
    [SerializeField] private float maxRotation = 10f;

    private float rotationX;
    private float rotationZ;

    private void Start()
    {
        Vector3 initialRotation = transform.localEulerAngles;
        rotationX = NormalizeAngle(initialRotation.x);
        rotationZ = NormalizeAngle(initialRotation.z);
    }

    private void Update()
    {
        Vector2 input = GetMoveInput();

        rotationX = Mathf.Clamp(rotationX + input.y * rotationSpeed * Time.deltaTime, -maxRotation, maxRotation);
        rotationZ = Mathf.Clamp(rotationZ - input.x * rotationSpeed * Time.deltaTime, -maxRotation, maxRotation);

        transform.localRotation = Quaternion.Euler(rotationX, transform.localEulerAngles.y, rotationZ);
    }

    private static Vector2 GetMoveInput()
    {
    #if ENABLE_INPUT_SYSTEM
        if (Keyboard.current == null)
        {
            return Vector2.zero;
        }

        Vector2 input = Vector2.zero;

        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
            input.x -= 1f;

        if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
            input.x += 1f;

        if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed)
            input.y -= 1f;

        if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed)
            input.y += 1f;

        return Vector2.ClampMagnitude(input, 1f);
    #else
        return Vector2.zero;
    #endif
    }

    private static float NormalizeAngle(float angle)
    {
        return angle > 180f ? angle - 360f : angle;
    }
}
