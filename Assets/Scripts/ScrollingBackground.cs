using UnityEngine;

public class ScrollingBackground : MonoBehaviour
{
    [SerializeField] private float scrollSpeed = 2f; // Speed of scrolling
    private float spriteHeight;
    [SerializeField] private GameObject secondBackground; // Second background reference

    private void Start()
    {
        spriteHeight = GetComponent<SpriteRenderer>().bounds.size.y; // Get background sprite height
    }

    private void Update()
    {
        // Move both backgrounds downward
        transform.position += Vector3.down * scrollSpeed * Time.deltaTime;
        secondBackground.transform.position += Vector3.down * scrollSpeed * Time.deltaTime;

        // Check if the first background has fully moved out of the screen
        if (transform.position.y <= -spriteHeight)
        {
            // Reset position by moving it above the second background
            transform.position = new Vector3(transform.position.x, secondBackground.transform.position.y + spriteHeight, transform.position.z);
        }

        // Check if the second background has fully moved out of the screen
        if (secondBackground.transform.position.y <= -spriteHeight)
        {
            // Reset position by moving it above the first background
            secondBackground.transform.position = new Vector3(secondBackground.transform.position.x, transform.position.y + spriteHeight, secondBackground.transform.position.z);
        }
    }
}
