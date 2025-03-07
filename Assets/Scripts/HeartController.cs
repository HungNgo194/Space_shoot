using UnityEngine;

public class HeartController : MonoBehaviour
{
    [SerializeField] private float speed;

    AudioManager audioManager;

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }
    private void Update()
    {
        transform.Translate(Vector3.down * speed * Time.deltaTime);
        transform.rotation = Quaternion.Euler(0, 0, 0);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            GameManager.instance.IncreasePlayerHealth();
            Destroy(gameObject);
            audioManager.PlaySFX(audioManager.collect);
        }
    }
}
