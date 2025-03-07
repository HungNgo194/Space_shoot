using System.Collections;
using System.Drawing;
using UnityEngine;

public class BossController : MonoBehaviour
{
    [Header("Projectile Settings")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private int numberOfProjectiles = 8;
    [SerializeField] private float shootRadius = 2f;
    [SerializeField] private float shootInterval = 2f;
    [SerializeField] private float projectileSpeed = 3f;
    [SerializeField] private int bossHealth;
    
    private float rotationSpeed = 100f;
    private void Awake()
    {
        rotationSpeed *= Random.Range(0, 2) == 0 ? 1 : -1;
        
    }

    private void Update()
    {
        transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);
    }
    private void Start()
    {
        StartCoroutine(ShootOrbitProjectiles());
    }

    private IEnumerator ShootOrbitProjectiles()
    {
        while (true)
        {
            ShootProjectilesInOrbit();
            yield return new WaitForSeconds(shootInterval);
        }
    }

    private void ShootProjectilesInOrbit()
    {
        float angleStep = 360f / numberOfProjectiles;
        float currentAngle = 0f;

        for (int i = 0; i < numberOfProjectiles; i++)
        {
            // Calculate projectile spawn position
            float projectileX = transform.position.x + Mathf.Cos(currentAngle * Mathf.Deg2Rad) * shootRadius;
            float projectileY = transform.position.y + Mathf.Sin(currentAngle * Mathf.Deg2Rad) * shootRadius;
            Vector3 spawnPosition = new Vector3(projectileX, projectileY, 0f);

            // Instantiate and move projectile
            GameObject projectile = Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);
            Vector2 moveDirection = (spawnPosition - transform.position).normalized;

            projectile.GetComponent<Rigidbody2D>().linearVelocity = moveDirection * projectileSpeed;

            currentAngle += angleStep;
        }
    }

    public void TakeDamage()
    {
        bossHealth--; // Reduce health
        if (bossHealth <= 0)
        {
            Destroy(gameObject);
            GameManager.instance.BossDefeated();
        }
    }
}
