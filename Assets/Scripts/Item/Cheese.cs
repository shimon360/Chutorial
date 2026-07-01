using UnityEngine;

public class Cheese : MonoBehaviour
{
    [System.NonSerialized] // または [HideInInspector]
    [HideInInspector] public bool isCollected = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isCollected = true;
            Debug.Log($"【チーズログ】 チーズが {collision.gameObject.name} によって回収されました！");

            gameObject.SetActive(false);
        }
    }
}
