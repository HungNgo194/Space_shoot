using UnityEngine;

public class RingController : MonoBehaviour
{
    
    [SerializeField] private int ringValue;
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
            GameManager.instance.AddScore(ringValue);
            Destroy(gameObject); // Remove ring after collection
        }
    }
}
