using UnityEngine;

public class AsteroidsController : MonoBehaviour
{
    [SerializeField] private float speed ;          // Falling speed
    [SerializeField] private int point;           // Points awarded on destruction
    [SerializeField] private int hitRequired;      // Hits required to destroy
    [SerializeField] private float rotationSpeed; // Rotation speed
    private AudioManager audioManager;


    [SerializeField] private GameObject heartPrefab; // Power-up to spawn
    [SerializeField] private float spawnChance = 0.2f; // 20% chance to spawn
    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio")?.GetComponent<AudioManager>();

        // Randomly assign clockwise or counterclockwise spin
        rotationSpeed *= Random.Range(0, 2) == 0 ? 1 : -1;
    }
    void Update()
    {
        MoveAndRotate();
    }

    private void MoveAndRotate()
    {
        // Move the asteroid downward
        transform.Translate(Vector3.down * speed * Time.deltaTime, Space.World);

        // Rotate the asteroid around its own axis (Z-axis spin)
        transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);
    }

    public void TakeDamage()
    {
        hitRequired--; // Reduce asteroid health
        if (hitRequired <= 0)
        {
            DestroyAsteroid();
        }
    }

    private void DestroyAsteroid()
    {   
        if (audioManager != null)
        {
            audioManager.PlaySFX(audioManager.explode);
        }
        GameObject explosion = Instantiate(GameManager.instance.explosion, transform.position, transform.rotation);
        Destroy(explosion, 1f);
        GameManager.instance?.AddScore(point);
        Destroy(gameObject);

        if (Random.value <= spawnChance && heartPrefab != null && GameManager.instance.canAddHeart)
        {
            Instantiate(heartPrefab, transform.position, Quaternion.identity);
        }
    }

    public void increaseHP(int increaseNum)
    {
        hitRequired += increaseNum;
    }
}
