using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// だるまさんが転んだ（赤信号・青信号）ギミック
/// 背景色を切り替え、赤信号の時にプレイヤーが移動キーを入力していたら即死させる
/// </summary>
public class Gimmic_RedLightGreenLight : MonoBehaviour
{
    [Header("背景・信号の設定")]
    [Tooltip("画像を切り替える対象の背景（または信号）のSpriteRenderer")]
    public SpriteRenderer backgroundSprite;
    
    [Tooltip("青信号時の専用画像（スプライト）")]
    public Sprite greenLightSprite;
    
    [Tooltip("赤信号時の専用画像（スプライト）")]
    public Sprite redLightSprite;

    [Header("タイミング設定")]
    [Tooltip("青信号が続く最小時間（秒）")]
    public float minGreenTime = 2.0f;
    [Tooltip("青信号が続く最大時間（秒）")]
    public float maxGreenTime = 4.0f;
    
    [Tooltip("赤信号が続く最小時間（秒）")]
    public float minRedTime = 1.0f;
    [Tooltip("赤信号が続く最大時間（秒）")]
    public float maxRedTime = 2.5f;

    private bool isRedLight = false;
    private ActorController actorController;

    private void OnEnable()
    {
        // アクターの参照を取得
        if (actorController == null)
        {
            actorController = FindFirstObjectByType<ActorController>();
        }
        
        if (backgroundSprite == null)
        {
            Debug.LogWarning("【警告】Gimmic_RedLightGreenLight: Background Spriteが設定されていません！インスペクターから背景画像をセットしてください。");
        }

        // 信号切り替えコルーチンを開始
        StartCoroutine(LightCycleCoroutine());
    }

    private void OnDisable()
    {
        // 無効化された時はコルーチンを止める
        StopAllCoroutines();
    }

    private void Update()
    {
        // プレイヤーが存在しない、または既に死亡（無効化）している場合は処理しない
        if (actorController == null || !actorController.enabled) return;

        // 赤信号中かチェック
        if (isRedLight)
        {
            // プレイヤーが影（Layer_Shadow）に隠れている場合は、動いても安全（即死をバイパス）
            if (actorController.isHiddenFromRedLight) return;

            // プレイヤーが能動的に移動キー（左右やジャンプ）を押しているかチェック
            // （ジャンプ後の滞空や、キーを離した後の慣性移動はセーフとする仕様）
            bool isInputting = Keyboard.current.leftArrowKey.isPressed || 
                               Keyboard.current.rightArrowKey.isPressed || 
                               Keyboard.current.upArrowKey.isPressed;

            if (isInputting)
            {
                // 動いたのでアウト（即死）
                actorController.Damaged();
            }
        }
    }

    private IEnumerator LightCycleCoroutine()
    {
        while (true)
        {
            // --- 青信号フェーズ ---
            isRedLight = false;
            if (backgroundSprite != null && greenLightSprite != null)
            {
                backgroundSprite.sprite = greenLightSprite;
            }
            Debug.Log("青信号になりました！動いても安全です。");

            // 青信号の持続時間をランダムに決定して待機
            float greenTime = Random.Range(minGreenTime, maxGreenTime);
            Debug.Log($"青信号で {greenTime} 秒待機します...");
            yield return new WaitForSecondsRealtime(greenTime); // Time.timeScaleに依存しない待機に変更

            // --- 赤信号フェーズ ---
            isRedLight = true;
            if (backgroundSprite != null && redLightSprite != null)
            {
                backgroundSprite.sprite = redLightSprite;
            }
            Debug.Log("赤信号になりました！動くとアウトです。");

            // 赤信号の持続時間をランダムに決定して待機
            float redTime = Random.Range(minRedTime, maxRedTime);
            Debug.Log($"赤信号で {redTime} 秒待機します...");
            yield return new WaitForSecondsRealtime(redTime); // Time.timeScaleに依存しない待機に変更
        }
    }
}
