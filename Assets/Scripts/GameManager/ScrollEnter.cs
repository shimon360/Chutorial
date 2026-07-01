using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// エリア間スクロールのトリガーとなる接触検知クラス
/// </summary>
public class ScrollEnter : MonoBehaviour
{
    [Tooltip("遷移先のエリア（インスペクターでAreaManagerをアタッチしてください）")]
    public AreaManager nextArea;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 接触してきたオブジェクトがアクター（ActorControllerを持っているか）判定
        ActorController actor = collision.GetComponent<ActorController>();
        if (actor != null)
        {
            Debug.Log($"【スクロールログ】 {gameObject.name} にプレイヤーが接触しました。NextArea: {(nextArea != null ? nextArea.name : "Null")}");

            // StageManagerを取得
            StageManager stageManager = FindFirstObjectByType<StageManager>();
            if (stageManager != null && nextArea != null)
            {
                Debug.Log($"【スクロールログ】 StageManager.ChangeArea を実行します -> {nextArea.name}");
                stageManager.ChangeArea(nextArea);
            }
            else
            {
                if (stageManager == null) Debug.LogError("【スクロールエラー】 StageManager がシーン内に見つかりません！");
                if (nextArea == null) Debug.LogError($"【スクロールエラー】 {gameObject.name} の 'Next Area' がインスペクターで未設定（空）です！");
            }
        }
    }
}
