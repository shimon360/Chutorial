using UnityEngine;
using UnityEngine.SceneManagement; // シーン遷移に必要

public class Goal : MonoBehaviour
{
    [Tooltip("クリア条件となるチーズオブジェクトをアタッチしてください")]
    public Cheese cheese;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            ActorController actor = collision.GetComponent<ActorController>();
            if (actor == null)
            {
                actor = collision.GetComponentInParent<ActorController>();
            }

            if (cheese != null && actor != null)
            {
                // チーズの現在の回収状態をログに出力して検証する
                Debug.Log($"【ゴールログ】 プレイヤーがゴールに接触しました。 チーズ所持フラグ(isCollected): {cheese.isCollected}");

                if (cheese.isCollected)
                {
                    // 静的変数にクリア時のデータを代入して保存
                    StageManager.finalScore = actor.score;
                    
                    StageManager stageManager = FindFirstObjectByType<StageManager>();
                    if (stageManager != null)
                    {
                        StageManager.finalClearTime = stageManager.playTime;
                    }
                    else
                    {
                        StageManager.finalClearTime = Time.timeSinceLevelLoad;
                    }

                    Debug.Log($"【ゴールログ】 ゲームクリア！ スコア: {StageManager.finalScore} | 時間: {StageManager.finalClearTime:F2}秒。 仮のシーン(ClearScene)へ遷移します。");
                    SceneManager.LoadScene("ClearScene");
                }
                else
                {
                    Debug.Log("【ゴールログ】 チーズを持っていません！ ゲームクリアをスキップします。");
                }
            }
            else
            {
                if (cheese == null) Debug.LogError("【ゴールエラー】 Goal スクリプトの 'cheese' フィールドが未設定（Null）です！");
                if (actor == null) Debug.LogError("【ゴールエラー】 プレイヤーの ActorController が取得できませんでした！");
            }
        }
    }
}
