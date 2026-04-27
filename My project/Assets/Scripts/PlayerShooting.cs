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

    int currentAmmo;
    bool isReloading;
    float nextTimeToFire;

    float damageMultiplier = 1f;
    Coroutine damageBoostRoutine;

    Light muzzleLight;

    void Start()
    {
        currentAmmo = magazineSize;
        CreateMuzzleLight();

        if (UIManager.Instance != null)
            UIManager.Instance.UpdateAmmo(currentAmmo);
    }

    // dynamic point light that flashes on each shot for muzzle feedback
    void CreateMuzzleLight()
    {
        var lightObj = new GameObject("MuzzleLight");
        lightObj.transform.SetParent(cam.transform, false);
        lightObj.transform.localPosition = new Vector3(0.4f, -0.15f, 0.8f);

        muzzleLight = lightObj.AddComponent<Light>();
        muzzleLight.type = LightType.Point;
        muzzleLight.color = new Color(1f, 0.8f, 0.4f);
        muzzleLight.intensity = 0f;
        muzzleLight.range = 10f;
    }

    void Update()
    {
        if (GameManager.Instance != null &&
            (GameManager.Instance.isGameOver || GameManager.Instance.isPaused))
            return;

        // fade muzzle light every frame, even while reloading or out of ammo
        if (muzzleLight != null && muzzleLight.intensity > 0)
            muzzleLight.intensity = Mathf.Max(0, muzzleLight.intensity - Time.deltaTime * 20f);

        if (isReloading) return;

        if (Input.GetKeyDown(KeyCode.R) && currentAmmo < magazineSize)
        {
            StartCoroutine(Reload());
            return;
        }

        // out of ammo: play click and auto-reload
        if (currentAmmo <= 0)
        {
            if (Input.GetButtonDown("Fire1") && emptyClickSound != null)
                audioSource.PlayOneShot(emptyClickSound, 0.35f);

            StartCoroutine(Reload());
            return;
        }

        // hold to fire (auto)
        if (Input.GetButton("Fire1") && Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + fireRate;
            Shoot();
        }
    }

    void Shoot()
    {
        if (isReloading || currentAmmo <= 0) return;

        currentAmmo--;
        if (UIManager.Instance != null)
            UIManager.Instance.UpdateAmmo(currentAmmo);

        if (shootSound != null)
            audioSource.PlayOneShot(shootSound, 0.4f);

        SpawnMuzzleFlash();

        // raycast straight from the center of the screen
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

        if (Physics.Raycast(ray, out RaycastHit hit, range, ~0, QueryTriggerInteraction.Ignore))
        {
            Zombie zombie = hit.collider.GetComponent<Zombie>() ?? hit.collider.GetComponentInParent<Zombie>();

            if (zombie != null)
            {
                zombie.TakeDamage(Mathf.RoundToInt(damage * damageMultiplier));
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

        if (muzzleFlashPrefab == null) return;

        // parent to camera so the flash sticks to the gun
        GameObject fx = Instantiate(muzzleFlashPrefab, cam.transform);
        fx.transform.localPosition = new Vector3(0.4f, -0.15f, 0.8f);
        fx.transform.localRotation = Quaternion.identity;
        Destroy(fx, 0.5f);
    }

    void SpawnImpact(GameObject prefab, Vector3 pos, Vector3 normal)
    {
        if (prefab == null) return;
        GameObject fx = Instantiate(prefab, pos, Quaternion.LookRotation(normal));
        Destroy(fx, 2f);
    }

    IEnumerator Reload()
    {
        if (isReloading) yield break;

        isReloading = true;

        // kill any leftover muzzle flash so it doesn't act like a flashlight while reloading
        if (muzzleLight != null) muzzleLight.intensity = 0f;

        if (reloadSound != null) audioSource.PlayOneShot(reloadSound, 0.24f);
        if (gunAnimator != null) gunAnimator.SetTrigger("Reload");

        yield return new WaitForSeconds(reloadTime);

        currentAmmo = magazineSize;
        isReloading = false;

        if (UIManager.Instance != null)
            UIManager.Instance.UpdateAmmo(currentAmmo);
    }

    public void ApplyDamageBoost(float multiplier, float duration)
    {
        if (damageBoostRoutine != null)
            StopCoroutine(damageBoostRoutine);

        if (UIManager.Instance != null)
            UIManager.Instance.ShowDamageBoostBar(duration);

        damageBoostRoutine = StartCoroutine(DamageBoostRoutine(multiplier, duration));
    }

    IEnumerator DamageBoostRoutine(float multiplier, float duration)
    {
        damageMultiplier = multiplier;
        yield return new WaitForSeconds(duration);
        damageMultiplier = 1f;
        damageBoostRoutine = null;
    }

    public int GetCurrentAmmo() => currentAmmo;
    public int GetMagazineSize() => magazineSize;
    public bool IsReloading() => isReloading;
}
