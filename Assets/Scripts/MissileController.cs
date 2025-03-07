using System.Collections;
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
        audioManager.PlaySFX(audioManager.hit);
        if (collision.gameObject.CompareTag("Enemy"))
        {
            EnemyController enemy = collision.gameObject.GetComponent<EnemyController>();
            if (enemy != null)
            {
                enemy.TakeDamage();
                SpriteRenderer sr = enemy.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    sr.color = Color.red; // Change to red
                    enemy.StartCoroutine(ResetColor(sr)); // Reset after delay
                }
            }
            GameObject explosion = Instantiate(GameManager.instance.explosion, transform.position, transform.rotation);
            Destroy(explosion, 1f);
            Destroy(gameObject);
        }
        else if (collision.gameObject.CompareTag("Asteroids"))
        {
            AsteroidsController asteroid = collision.gameObject.GetComponent<AsteroidsController>();
            if (asteroid != null)
            {
                asteroid.TakeDamage();
                SpriteRenderer sr = asteroid.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    sr.color = Color.red; // Change to red
                    asteroid.StartCoroutine(ResetColor(sr)); // Reset after delay
                }
            }
            GameObject explosion = Instantiate(GameManager.instance.explosion, transform.position, transform.rotation);
            Destroy(explosion, 1f);
            Destroy(gameObject);
        }
        else if (collision.gameObject.CompareTag("Boss"))
        {
            BossController boss = collision.gameObject.GetComponent<BossController>();
            if (boss != null)
            {
                boss.TakeDamage();
                SpriteRenderer sr = boss.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    sr.color = Color.red; // Change to red
                    boss.StartCoroutine(ResetColor(sr)); // Reset after delay
                }
            }
            GameObject explosion = Instantiate(GameManager.instance.explosion, transform.position, transform.rotation);
            Destroy(explosion, 1f);
            Destroy(gameObject); // Missile is always destroyed on impact
            
        }

    }

    private IEnumerator ResetColor(SpriteRenderer sr)
    {
        yield return new WaitForSeconds(0.05f);
        sr.color = Color.white; // Reset to original color
    }

}
