using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// エリア管理クラス
/// </summary>
public class AreaManager : MonoBehaviour
{
    [Tooltip("このエリアのカメラ基準座標")]
    public Vector2 cameraBasePos;

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
