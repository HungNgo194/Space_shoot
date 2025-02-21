using UnityEngine;

public class AsteroidsController : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private int point;
    [SerializeField] private int hitRequired;
    void Update()
    {
        transform.Translate(Vector3.down * speed * Time.deltaTime);
    }

    public void TakeDamage()
    {
        hitRequired--; // Reduce health
        if (hitRequired <= 0)
        {
            GameManager.instance.AddScore(point);
            Destroy(gameObject);
        }
    }
}
