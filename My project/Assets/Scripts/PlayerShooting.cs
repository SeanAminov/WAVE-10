using UnityEngine;
using System.Collections;

public class PlayerShooting : MonoBehaviour
{
    [Header("Gun Settings")]
    [SerializeField] float fireRate = 0.1f;
    [SerializeField] float range = 100f;
    [SerializeField] int damage = 1;

    [Header("Ammo")]
    [SerializeField] int magazineSize = 30;
    [SerializeField] float reloadTime = 1.5f;

    [Header("VFX Prefabs")]
    [SerializeField] GameObject muzzleFlashPrefab;
    [SerializeField] GameObject bulletImpactConcretePrefab;
    [SerializeField] GameObject bulletImpactFleshPrefab;

    [Header("Audio")]
    [SerializeField] AudioClip shootSound;
    [SerializeField] AudioClip reloadSound;
    [SerializeField] AudioClip emptyClickSound;

    [Header("References")]
    [SerializeField] Camera cam;
    [SerializeField] AudioSource audioSource;
    [SerializeField] Animator gunAnimator;

    float nextTimeToFire = 0f;
    int currentAmmo;
    bool isReloading = false;
    Light muzzleLight;

    void Start()
    {
        currentAmmo = magazineSize;

        // muzzle flash light
        var lightObj = new GameObject("MuzzleLight");
        lightObj.transform.SetParent(cam.transform, false);
        lightObj.transform.localPosition = new Vector3(0.4f, -0.15f, 0.8f);
        muzzleLight = lightObj.AddComponent<Light>();
        muzzleLight.type = LightType.Point;
        muzzleLight.color = new Color(1f, 0.8f, 0.4f);
        muzzleLight.intensity = 0f;
        muzzleLight.range = 10f;

        if (UIManager.Instance != null)
            UIManager.Instance.UpdateAmmo(currentAmmo);
    }

    void Update()
    {
        if (GameManager.Instance != null &&
        (GameManager.Instance.isGameOver || GameManager.Instance.isPaused))
            return;
        
        if (GameManager.Instance != null && GameManager.Instance.isGameOver)
            return;

        if (isReloading)
            return;

        if (Input.GetKeyDown(KeyCode.R) && currentAmmo < magazineSize)
        {
            StartCoroutine(Reload());
            return;
        }

        if (currentAmmo <= 0)
        {
            if (Input.GetButtonDown("Fire1") && emptyClickSound != null)
                audioSource.PlayOneShot(emptyClickSound, 0.35f);

            StartCoroutine(Reload());
            return;
        }

        // auto fire - hold to shoot
        if (Input.GetButton("Fire1") && Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + fireRate;
            Shoot();
        }

        // fade muzzle light
        if (muzzleLight != null && muzzleLight.intensity > 0)
            muzzleLight.intensity = Mathf.Max(0, muzzleLight.intensity - Time.deltaTime * 20f);
    }

    void Shoot()
    {
        if (isReloading || currentAmmo <= 0)
            return;

        currentAmmo--;

        if (UIManager.Instance != null)
            UIManager.Instance.UpdateAmmo(currentAmmo);

        if (shootSound != null)
            audioSource.PlayOneShot(shootSound, 0.4f);

        SpawnMuzzleFlash();

        // raycast from center of screen
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, range))
        {
            Zombie zombie = hit.collider.GetComponent<Zombie>();
            if (zombie == null)
                zombie = hit.collider.GetComponentInParent<Zombie>();

            if (zombie != null)
            {
                zombie.TakeDamage(damage);
                SpawnImpact(bulletImpactFleshPrefab, hit.point, hit.normal);
            }
            else
            {
                SpawnImpact(bulletImpactConcretePrefab, hit.point, hit.normal);
            }
        }
    }

    void SpawnMuzzleFlash()
    {
        if (muzzleLight != null)
            muzzleLight.intensity = 3f;

        if (muzzleFlashPrefab != null)
        {
            GameObject fx = Instantiate(muzzleFlashPrefab, cam.transform);
            fx.transform.localPosition = new Vector3(0.4f, -0.15f, 0.8f);
            fx.transform.localRotation = Quaternion.identity;
            Destroy(fx, 0.5f);
        }
    }

    void SpawnImpact(GameObject prefab, Vector3 pos, Vector3 normal)
    {
        if (prefab == null) return;

        GameObject fx = Instantiate(prefab, pos, Quaternion.LookRotation(normal));
        Destroy(fx, 2f);
    }

    IEnumerator Reload()
    {
        if (isReloading)
            yield break;

        isReloading = true;

        if (reloadSound != null)
            audioSource.PlayOneShot(reloadSound, 0.24f);

        if (gunAnimator != null)
            gunAnimator.SetTrigger("Reload");

        yield return new WaitForSeconds(reloadTime);

        currentAmmo = magazineSize;
        isReloading = false;

        if (UIManager.Instance != null)
            UIManager.Instance.UpdateAmmo(currentAmmo);
    }

    public int GetCurrentAmmo() { return currentAmmo; }
    public int GetMagazineSize() { return magazineSize; }
    public bool IsReloading() { return isReloading; }
}
