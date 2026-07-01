using UnityEngine;

public class Cheese : MonoBehaviour
{
    public bool isCollected = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isCollected = true;

            gameObject.SetActive(false);
        }
    }
}
