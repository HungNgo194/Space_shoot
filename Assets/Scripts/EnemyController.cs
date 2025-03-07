using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private float enemySpeed;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint; // FirePoint for shooting
    [SerializeField] private float fireRate;
    [SerializeField] private int pointPerKill;
    [SerializeField] private int hitRequired;
    private float nextFireTime;

    private Transform player; // Reference to the player
    AudioManager audioManager;

    [SerializeField] private GameObject powerUpPrefab; // Power-up to spawn
    [SerializeField] private float spawnChance = 0.2f; // 20% chance to spawn

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform; // Find the player
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

    private void Update()
    {
        // Move enemy downward
        transform.Translate(Vector3.up * enemySpeed * Time.deltaTime);

        // Check if it's time to shoot
        if (Time.time >= nextFireTime && player != null)
        {
            ShootAtPlayer();
            nextFireTime = Time.time + fireRate;
        }

        transform.rotation = Quaternion.Euler(0, 0, 180);
    }

    public void TakeDamage()
    {
        hitRequired--;
        if (hitRequired <= 0)
        {
            DestroyEnemy();
        }
    }

    private void DestroyEnemy()
    {
        if (audioManager != null)
        {
            audioManager.PlaySFX(audioManager.explode);
        }

        GameObject explosion = Instantiate(GameManager.instance.explosion, transform.position, transform.rotation);
        Destroy(explosion, 1f);

        GameManager.instance?.AddScore(pointPerKill);
        Destroy(gameObject);

        // Attempt to spawn power-up with spawnChance probability
        if (Random.value <= spawnChance && powerUpPrefab != null)
        {
            Instantiate(powerUpPrefab, transform.position, Quaternion.identity);
        }
    }

    void ShootAtPlayer()
    {
        if (firePoint == null || bulletPrefab == null || player == null) return;

        Vector2 direction = (player.position - firePoint.position).normalized; // Aim at player
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        bullet.GetComponent<EnemyBullet>().SetDirection(direction);
    }

    public void increaseHP(int increaseNum)
    {
        hitRequired += increaseNum;
    }
}
