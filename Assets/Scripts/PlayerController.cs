using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float destroyTime = 2f;
    [SerializeField] private float invincibilityDuration = 2f;
    [SerializeField] private float blinkInterval = 0.2f;

    [Header("Missile")]
    [SerializeField] private GameObject missile;
    [SerializeField] private Transform missileSpawnPosition;
    [SerializeField] private Transform muzzleSpawnPosition;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Collider2D playerCollider;
    private bool isInvincible = false;
    private bool doubleMissiles = false;
    private float powerUpTime = 0;

    AudioManager audioManager;


    private void Awake()
    {
            audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerCollider = GetComponent<Collider2D>();
    }

    private void Update()
    {
        PlayerMovement();
        PlayerShoot();

        if (doubleMissiles && Time.time > powerUpTime)
        {
            doubleMissiles = false;
        }
    }

    private void PlayerMovement()
    {
        float xpos = Input.GetAxis("Horizontal");
        float ypos = Input.GetAxis("Vertical");

        Vector2 movement = new Vector2(xpos, ypos) * speed;
        rb.linearVelocity = movement;

        // Keep Player in Bounds
        Vector3 viewPos = transform.position;
        float screenWidth = Camera.main.orthographicSize * Screen.width / Screen.height;
        float screenHeight = Camera.main.orthographicSize;

        viewPos.x = Mathf.Clamp(viewPos.x, -screenWidth, screenWidth);
        viewPos.y = Mathf.Clamp(viewPos.y, -screenHeight, screenHeight);

        transform.position = viewPos;

        transform.rotation = Quaternion.Euler(0, 0, 0);
    }

    private void PlayerShoot()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            audioManager.PlaySFX(audioManager.shoot);
            SpawnMissile();
            SpawnMuzzle();
        }
    }

    private void SpawnMissile()
    {
        if (doubleMissiles)
        {
            Instantiate(missile, missileSpawnPosition.position + new Vector3(-0.3f, 0, 0), Quaternion.identity);
            Instantiate(missile, missileSpawnPosition.position + new Vector3(0.3f, 0, 0), Quaternion.identity);
        }
        else
        {
            Instantiate(missile, missileSpawnPosition.position, Quaternion.identity);
        }
    }

    private void SpawnMuzzle()
    {
        GameObject gm = Instantiate(GameManager.instance.muzzleFlash, muzzleSpawnPosition);
        gm.transform.SetParent(null);
        Destroy(gm, destroyTime);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if ((collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("EnemyBullet") || collision.gameObject.CompareTag("Asteroids") || collision.gameObject.CompareTag("Boss")) && !isInvincible)
        {
            audioManager.PlaySFX(audioManager.explode);
            GameObject gm = Instantiate(GameManager.instance.explosion, transform.position, transform.rotation);
            Destroy(gm, 1f);

            GameManager.instance.ReducePlayerHealth();

            if (GameManager.instance.playerHealth <= 0)
            {
                Destroy(this.gameObject);
            }
            else
            {
                StartCoroutine(BecomeInvincible());
            }
        }
    }

    private IEnumerator BecomeInvincible()
    {
        isInvincible = true;
        playerCollider.enabled = false;

        float timer = 0;
        while (timer < invincibilityDuration)
        {
            spriteRenderer.color = new Color(1, 1, 1, 0.5f);
            yield return new WaitForSeconds(blinkInterval);
            spriteRenderer.color = new Color(1, 1, 1, 1);
            yield return new WaitForSeconds(blinkInterval);
            timer += blinkInterval * 2;
        }

        playerCollider.enabled = true;
        isInvincible = false;
    }

    public void ActivateDoubleMissile()
    {
        doubleMissiles = true;
        powerUpTime = Time.time + 5f;
    }
}
