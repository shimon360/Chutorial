using UnityEngine;

public class CheeseController : MonoBehaviour
{
    [Header("取得前")]
    [SerializeField] private Sprite cheeseSprite;

    [Header("取得後")]
    [SerializeField] private Sprite dishSprite;

    private SpriteRenderer spriteRenderer;

    private bool isCollected = false;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // 最初はチーズ付き
        spriteRenderer.sprite = cheeseSprite;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isCollected)
            return;

        if (!other.CompareTag("Player"))
            return;

        CollectCheese();
    }

    private void CollectCheese()
    {
        isCollected = true;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.CollectCheese();
        }

        // 皿だけに変更
        spriteRenderer.sprite = dishSprite;
    }
}