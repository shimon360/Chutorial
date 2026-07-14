using UnityEngine;

public class CrumbController : MonoBehaviour
{
    [Header("獲得スコア")]
    [SerializeField] private int score = 100;


    private bool isCollected = false;


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isCollected)
            return;


        if (!other.CompareTag("Player"))
            return;


        isCollected = true;


        GameManager.Instance.CollectCrumb(score);


        ActorController actor = other.GetComponent<ActorController>();

        if(actor != null)
        {
            actor.UpdateSpeedMultiplier(
                GameManager.Instance.GetCurrentSpeedMultiplier()
            );
        }


        Destroy(gameObject);
    }
}