using UnityEngine;

public class Goal : MonoBehaviour
{
    public Cheese cheese;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if(cheese.isCollected)
            {
                Debug.Log("ゲームクリア！");
            }
        }
    }
    
}
