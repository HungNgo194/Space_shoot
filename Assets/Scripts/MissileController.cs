using UnityEngine;

public class MissileController : MonoBehaviour
{
    [SerializeField] private float missileSpeed;

    AudioManager audioManager;


    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

    private void Update()
    {
        transform.Translate(Vector3.up * missileSpeed * Time.deltaTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        audioManager.PlaySFX(audioManager.explode);
        if (collision.gameObject.CompareTag("Enemy"))
        {
            GameObject explosion = Instantiate(GameManager.instance.explosion, transform.position, transform.rotation);
            Destroy(explosion, 1f);
            Destroy(collision.gameObject);
            Destroy(gameObject);
        }
        else if (collision.gameObject.CompareTag("Asteroids"))
        {
            AsteroidsController asteroid = collision.gameObject.GetComponent<AsteroidsController>();
            if (asteroid != null)
            {
                asteroid.TakeDamage(); // Reduce asteroid HP
            }
            GameObject explosion = Instantiate(GameManager.instance.explosion, transform.position, transform.rotation);
            Destroy(explosion, 1f);
            Destroy(gameObject); // Missile is always destroyed on impact
        }
        else if (collision.gameObject.CompareTag("Boss"))
        {
            BossController boss = collision.gameObject.GetComponent<BossController>();
            if (boss != null)
            {
                boss.TakeDamage(); // Reduce boss HP
            }
            GameObject explosion = Instantiate(GameManager.instance.explosion, transform.position, transform.rotation);
            Destroy(explosion, 1f);
            Destroy(gameObject); // Missile is always destroyed on impact
        }
    }
}
