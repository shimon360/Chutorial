using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// エリア管理クラス
/// </summary>
public class AreaManager : MonoBehaviour
{
    [Tooltip("このエリアのカメラ位置を示すアンカーオブジェクト（アタッチしたオブジェクトの座標を参照します）")]
    public Transform cameraMoveArea;

    /// <summary>
    /// カメラの基準座標（アタッチされた cameraMoveArea の座標を返します）
    /// </summary>
    public Vector2 cameraBasePos
    {
        get
        {
            if (cameraMoveArea != null)
            {
                return new Vector2(cameraMoveArea.position.x, cameraMoveArea.position.y);
            }
            // 未設定の場合は自分自身（AreaManagerオブジェクト）の現在地を返す（エラー防止のフォールバック）
            return new Vector2(transform.position.x, transform.position.y);
        }
    }

    private StageManager stageManager; // 親となるStageManagerの参照

    /// <summary>
    /// StageManagerから呼ばれる初期化処理
    /// </summary>
    public void Init(StageManager manager)
    {
        stageManager = manager;
    }

    /// <summary>
    /// このエリアをアクティブ（有効）にする処理
    /// </summary>
    public void ActiveArea()
    {
        // 1. まず他の全てのエリアを一旦無効化する
        if (stageManager != null)
        {
            stageManager.DeactivateAllAreas();
        }

        // 2. このエリア（自分自身）だけを有効化する
        gameObject.SetActive(true);
    }
}
