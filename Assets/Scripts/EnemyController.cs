using System.Reflection;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private float enemySpeed;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint; // FirePoint for shooting
    [SerializeField] private float fireRate;
    [SerializeField] private int pointPerKill;
    private float nextFireTime;

    private Transform player; // Reference to the player

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform; // Find the player
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
    private void OnDestroy()
    {
         GameManager.instance.AddScore(pointPerKill);   
    }
    void ShootAtPlayer()
    {
        if (firePoint == null || bulletPrefab == null || player == null) return;

        Vector2 direction = (player.position - firePoint.position).normalized; // Aim at player
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        bullet.GetComponent<EnemyBullet>().SetDirection(direction);
    }
}
