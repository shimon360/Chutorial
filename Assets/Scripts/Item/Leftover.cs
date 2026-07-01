using UnityEngine;

/// <summary>
/// 食べ残し（スコア加算＋重量ペナルティ）アイテムクラス
/// </summary>
public class Leftover : MonoBehaviour
{
    [Tooltip("回収時に加算されるスコア値")]
    public int scoreValue = 100;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // プレイヤーと接触したか判定
        if (collision.CompareTag("Player"))
        {
            ActorController actor = collision.GetComponent<ActorController>();
            if (actor != null)
            {
                // アクター側に回収処理を通知
                actor.CollectLeftover(scoreValue);

                // オブジェクトを非アクティブ化して回収済みにする
                gameObject.SetActive(false);
            }
        }
    }
}
