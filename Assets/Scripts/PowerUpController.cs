using UnityEngine;

public class PowerUpController : MonoBehaviour
{
    [SerializeField] private float speed;
    private void Update()
    {
        // Move enemy downward
        transform.Translate(Vector3.down * speed * Time.deltaTime);
        transform.rotation = Quaternion.Euler(0, 0, 0);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();

            if (player != null)
            {
                player.ActivateDoubleMissile();
            }

            Destroy(gameObject); // Remove power-up after collection
        }
    }
}
