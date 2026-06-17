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
            // StageManagerを取得
            StageManager stageManager = FindFirstObjectByType<StageManager>();
            if (stageManager != null && nextArea != null)
            {
                // 次のエリアへの遷移を要求
                stageManager.ChangeArea(nextArea);
            }
        }
    }
}
