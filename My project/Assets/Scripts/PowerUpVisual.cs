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
        // create a small point light if one was not assigned
        if (glowLight == null)
        {
            GameObject lightObj = new GameObject("PowerupGlow");
            lightObj.transform.SetParent(transform);
            lightObj.transform.localPosition = Vector3.zero;

            glowLight = lightObj.AddComponent<Light>();
            glowLight.type = LightType.Point;
        }

        glowLight.color = glowColor;
        glowLight.intensity = glowIntensity;
        glowLight.range = glowRange;
    }

    void Update()
    {
        // rotate powerup so it is easier to notice
        transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime, Space.World);
    }
}