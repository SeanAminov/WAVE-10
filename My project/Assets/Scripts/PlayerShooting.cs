using UnityEngine;
using System.Collections;

public class PlayerShooting : MonoBehaviour
{
    [Header("Gun Settings")]
    [SerializeField] float fireRate = 0.15f;
    [SerializeField] float range = 100f;
    [SerializeField] int damage = 1;

    [Header("Ammo")]
    [SerializeField] int magazineSize = 30;
    [SerializeField] float reloadTime = 1.5f;

    [Header("Animation")]
    [SerializeField] Animator gunAnimator;

    Camera cam;
    float nextTimeToFire = 0f;

    int currentAmmo;
    bool isReloading = false;

    void Start()
    {
        cam = GetComponentInChildren<Camera>();
        currentAmmo = magazineSize;

        if (gunAnimator == null)
            gunAnimator = GetComponentInChildren<Animator>();

        // update ammo UI on start
        if (UIManager.Instance != null)
            UIManager.Instance.UpdateAmmo(currentAmmo);
    }

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.isGameOver)
            return;

        // don't allow input while reloading
        if (isReloading)
            return;

        // manual reload
        if (Input.GetKeyDown(KeyCode.R) && currentAmmo < magazineSize)
        {
            StartCoroutine(Reload());
            return;
        }

        // auto reload when empty
        if (currentAmmo <= 0)
        {
            StartCoroutine(Reload());
            return;
        }

        // fire input check with fire rate
        if (Input.GetButtonDown("Fire1") && Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + fireRate;
            Shoot();
        }
    }

    void Shoot()
    {
        if (isReloading || currentAmmo <= 0)
            return;

        currentAmmo--;

        // update ammo UI
        if (UIManager.Instance != null)
            UIManager.Instance.UpdateAmmo(currentAmmo);

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

    IEnumerator Reload()
    {
        if (isReloading)
            yield break;

        isReloading = true;

        // trigger reload animation
        if (gunAnimator != null)
            gunAnimator.SetTrigger("Reload");

        yield return new WaitForSeconds(reloadTime);

        currentAmmo = magazineSize;
        isReloading = false;

        // update ammo UI after reload finishes
        if (UIManager.Instance != null)
            UIManager.Instance.UpdateAmmo(currentAmmo);
    }

    IEnumerator MuzzleFlash()
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

    public int GetCurrentAmmo()
    {
        return currentAmmo;
    }

    public int GetMagazineSize()
    {
        return magazineSize;
    }

    public bool IsReloading()
    {
        return isReloading;
    }
}