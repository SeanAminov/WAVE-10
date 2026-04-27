using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float sprintSpeed = 8.5f;
    [SerializeField] float gravity = -9.81f;

    [Header("Mouse Look")]
    [SerializeField] float mouseSensitivity = 2f;
    [SerializeField] float maxLookAngle = 80f;

    [Header("References")]
    [SerializeField] CharacterController controller;
    [SerializeField] Camera cam;

    float xRotation;
    Vector3 velocity;

    float speedMultiplier = 1f;
    Coroutine speedBoostRoutine;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (GameManager.Instance != null &&
            (GameManager.Instance.isPaused || GameManager.Instance.isGameOver))
            return;

        HandleMouseLook();
        HandleMovement();
    }

    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // body turns left/right, camera tilts up/down
        transform.Rotate(Vector3.up * mouseX);

        xRotation = Mathf.Clamp(xRotation - mouseY, -maxLookAngle, maxLookAngle);
        cam.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }

    void HandleMovement()
    {
        // small downward push to stay grounded
        if (controller.isGrounded && velocity.y < 0)
            velocity.y = -2f;

        Vector3 input = transform.right * Input.GetAxis("Horizontal") +
                        transform.forward * Input.GetAxis("Vertical");

        float baseSpeed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : moveSpeed;
        float speed = baseSpeed * speedMultiplier;

        controller.Move(input * speed * Time.deltaTime);

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    public void ApplySpeedBoost(float multiplier, float duration)
    {
        if (speedBoostRoutine != null)
            StopCoroutine(speedBoostRoutine);

        if (UIManager.Instance != null)
            UIManager.Instance.ShowSpeedBoostBar(duration);

        speedBoostRoutine = StartCoroutine(SpeedBoostRoutine(multiplier, duration));
    }

    IEnumerator SpeedBoostRoutine(float multiplier, float duration)
    {
        speedMultiplier = multiplier;
        yield return new WaitForSeconds(duration);
        speedMultiplier = 1f;
        speedBoostRoutine = null;
    }
}
