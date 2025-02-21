using UnityEngine;

public class BossProjectile : MonoBehaviour
{
    
    [SerializeField] private float destroyTime = 6f; // Time before disappearing

    private void Start()
    {
        Destroy(gameObject, destroyTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(gameObject);
    }
}
