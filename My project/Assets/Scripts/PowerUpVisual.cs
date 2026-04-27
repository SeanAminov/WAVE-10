using UnityEngine;

public class PowerUpVisual : MonoBehaviour
{
    [Header("Rotation")]
    [SerializeField] float rotateSpeed = 90f;

    [Header("Glow")]
    [SerializeField] Light glowLight;
    [SerializeField] Color glowColor = Color.yellow;
    [SerializeField] float glowIntensity = 1.5f;
    [SerializeField] float glowRange = 3f;

    void Start()
    {
        if (glowLight != null)
        {
            glowLight.color = glowColor;
            glowLight.intensity = glowIntensity;
            glowLight.range = glowRange;
        }
    }

    void Update()
    {
        // spin slowly so the powerup catches the player's eye
        transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime, Space.World);
    }
}
