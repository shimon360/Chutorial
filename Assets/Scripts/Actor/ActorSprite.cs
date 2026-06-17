using System.Collections;
using UnityEngine;

/// <summary>
/// アクターのスプライト（画像）アニメーションを管理するクラス
/// </summary>
public class ActorSprite : MonoBehaviour
{
    [Header("スプライト画像の設定")]
    [Tooltip("待機時（止まっている時）の画像")]
    public Sprite idleSprite;
    
    [Tooltip("歩行時のアニメーション画像配列")]
    public Sprite[] walkSprites;
    
    [Tooltip("やられた時（死亡時）の画像")]
    public Sprite deadSprite;
    
    [Header("アニメーション速度")]
    [Tooltip("画像が切り替わる間隔（秒）")]
    public float animationSpeed = 0.1f;
    
    private SpriteRenderer spriteRenderer;
    private ActorController actorController;
    
    private float animationTimer;
    private int currentWalkFrame;
    private bool isDead = false; // 死亡状態フラグ

    /// <summary>
    /// ActorControllerから呼び出される初期化処理
    /// </summary>
    public void Init(ActorController controller)
    {
        actorController = controller;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (isDead) return; // 死亡している時は通常のアニメーション更新をストップする

        // ActorControllerのX方向の移動速度を見て、歩いているか判定する
        bool isWalking = Mathf.Abs(actorController.xSpeed) > 0.1f;

        if (isWalking)
        {
            // --- 歩行中のアニメーション処理 ---
            
            // タイマーを進める
            animationTimer += Time.deltaTime;
            
            // 指定した秒数(animationSpeed)が経過したら画像を切り替える
            if (animationTimer >= animationSpeed)
            {
                // タイマーをリセットして次のコマへ
                animationTimer = 0f;
                currentWalkFrame++;
                
                // 最後の画像まで表示したら、最初の画像(0番目)に戻る
                if (currentWalkFrame >= walkSprites.Length)
                {
                    currentWalkFrame = 0;
                }
                
                // 配列に画像が設定されていれば、SpriteRendererの画像を更新
                if (walkSprites.Length > 0 && walkSprites[currentWalkFrame] != null)
                {
                    spriteRenderer.sprite = walkSprites[currentWalkFrame];
                }
            }
        }
        else
        {
            // --- 停止中の処理 ---
            
            // 待機用スプライトが設定されていれば、それに切り替える
            if (idleSprite != null)
            {
                spriteRenderer.sprite = idleSprite;
            }
            
            // 再び歩き出した時に最初からアニメーションするようにリセットしておく
            animationTimer = 0f;
            currentWalkFrame = 0;
        }
    }

    /// <summary>
    /// 死亡時の画像に切り替える処理
    /// </summary>
    public void PlayDeadAnimation()
    {
        isDead = true;
        if (deadSprite != null)
        {
            spriteRenderer.sprite = deadSprite;
        }

        // キャラクターをチカチカ（点滅）させる
        StartCoroutine(BlinkCoroutine());
    }

    /// <summary>
    /// スプライトを一定時間点滅させるコルーチン
    /// </summary>
    private IEnumerator BlinkCoroutine()
    {
        float blinkDuration = 1.5f; // 点滅させる合計時間（秒）
        float interval = 0.1f;     // 点滅の切り替えスピード（秒） 少しゆったりとした間隔に変更
        float timePassed = 0f;

        while (timePassed < blinkDuration)
        {
            // スプライトの表示・非表示を反転させてチカチカさせる
            spriteRenderer.enabled = !spriteRenderer.enabled;
            
            yield return new WaitForSeconds(interval);
            timePassed += interval;
        }

        // 点滅終了後、キャラクターを完全に見えなくする
        spriteRenderer.enabled = false;

        // ※もしエンディング画面へ移行する場合は、ここでシーン遷移を呼ぶのがベストタイミングです
        // UnityEngine.SceneManagement.SceneManager.LoadScene("EndingScene");
    }
}
