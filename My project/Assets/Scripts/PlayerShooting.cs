using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    [Header("Gun Settings")]
    [SerializeField] float fireRate = 0.3f;
    [SerializeField] float range = 100f;
    [SerializeField] int damage = 1;

    Camera cam;
    float nextTimeToFire = 0f;

    void Start()
    {
        cam = GetComponentInChildren<Camera>();
    }

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.isGameOver)
            return;

        if (Input.GetButtonDown("Fire1") && Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + fireRate;
            Shoot();
        }
    }

    void Shoot()
    {
        // raycast from center of screen
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, range))
        {
            // check if we hit a zombie
            Zombie zombie = hit.collider.GetComponent<Zombie>();
            if (zombie != null)
            {
                zombie.TakeDamage(damage);
            }
        }

        // quick muzzle flash
        StartCoroutine(MuzzleFlash());
    }

    System.Collections.IEnumerator MuzzleFlash()
    {
        // just a small light at the gun position for feedback
        GameObject flash = new GameObject("Flash");
        flash.transform.position = cam.transform.position + cam.transform.forward * 0.5f;
        Light light = flash.AddComponent<Light>();
        light.color = new Color(1f, 0.9f, 0.5f);
        light.intensity = 5f;
        light.range = 15f;

        yield return new WaitForSeconds(0.12f);
        Destroy(flash);
    }
}
