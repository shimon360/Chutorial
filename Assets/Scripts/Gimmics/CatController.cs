using UnityEngine;
using System.Collections;

public class CatController : MonoBehaviour
{
    public enum CatState
    {
        Normal,
        Warning,
        Threatening
    }

    public CatState currentState;

    public Sprite normalSprite;
    public Sprite warningSprite;
    public Sprite threateningSprite;

    public ActorController actorController;

    private SpriteRenderer spriteRenderer;

    [Header("追従設定")]
    [Tooltip("追従対象のトランスフォーム（未設定の場合は自動でメインカメラを対象にします）")]
    public Transform target;
    
    [Tooltip("追従の強さ（1: カメラと完全に同期して画面に固定、0.8: パララックス効果で少し遅れて動き奥行きが出る）")]
    [Range(0f, 1f)]
    public float followRate = 0.5f;

    [Tooltip("滑らかに追従（補間）させるかどうか")]
    public bool smoothFollow = true;

    [Tooltip("追随する滑らかさ（値が大きいほど素早く追従）")]
    public float smoothSpeed = 5.0f;

    private float initialSelfX;
    private float initialTargetX;
    private StageManager stageManager;
    private Coroutine catRoutine;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        stageManager = FindFirstObjectByType<StageManager>();
        
        if (actorController == null)
        {
            actorController = FindFirstObjectByType<ActorController>();
        }
    }

    private void OnEnable()
    {
        // 追従対象が未設定の場合はメインカメラをターゲットにする
        if (target == null && Camera.main != null)
        {
            target = Camera.main.transform;
        }

        // アクティブ化された時点の座標を初期位置として記憶する
        initialSelfX = transform.position.x;
        if (target != null)
        {
            initialTargetX = target.position.x;
        }

        // コルーチンを開始
        if (catRoutine != null)
        {
            StopCoroutine(catRoutine);
        }
        catRoutine = StartCoroutine(CatRoutine());
    }

    private void OnDisable()
    {
        // 非アクティブ化されたらコルーチンを停止
        if (catRoutine != null)
        {
            StopCoroutine(catRoutine);
            catRoutine = null;
        }
    }

    private void LateUpdate()
    {
        if (target == null) return;

        // エリア遷移中の場合は追従を停止し、その場に留まらせてスクロールアウトさせる
        if (stageManager != null && stageManager.IsChangingArea)
        {
            Debug.Log($"【猫ログ】 {gameObject.name} 追従停止（エリア遷移中を検知）");
            return;
        }

        if (stageManager == null)
        {
            Debug.LogWarning($"【猫警告】 {gameObject.name} StageManager が見つからないため遷移を検知できません！");
        }

        // ターゲットの初期位置からの移動量（Delta）を計算
        float targetDeltaX = target.position.x - initialTargetX;

        // 移動量に追従率（followRate）を乗算し、初期の自身のX座標に足すことで目標X座標を決定
        float targetX = initialSelfX + (targetDeltaX * followRate);

        Vector3 newPos = transform.position;
        if (smoothFollow)
        {
            newPos.x = Mathf.Lerp(transform.position.x, targetX, Time.deltaTime * smoothSpeed);
        }
        else
        {
            newPos.x = targetX;
        }
        transform.position = newPos;
    }

    private IEnumerator CatRoutine()
    {
        while (true)
        {
            // --- 1. Normal（安全状態） ---
            currentState = CatState.Normal;
            spriteRenderer.sprite = normalSprite;
            
            float timer = 0f;
            float normalTime = 5f;
            while (timer < normalTime)
            {
                // 猫じゃらし発動中はタイマーの進行をストップさせ、安全状態を維持
                if (actorController != null && actorController.isCatTeaserActive)
                {
                    spriteRenderer.sprite = normalSprite;
                    yield return null;
                    continue;
                }
                timer += Time.deltaTime;
                yield return null;
            }

            // --- 2. Warning（警告状態） ---
            currentState = CatState.Warning;
            spriteRenderer.sprite = warningSprite;
            
            timer = 0f;
            float warningTime = 2f;
            while (timer < warningTime)
            {
                // 警告中に猫じゃらしが発動したら強制的にNormal状態へ引き戻す
                if (actorController != null && actorController.isCatTeaserActive)
                {
                    currentState = CatState.Normal;
                    spriteRenderer.sprite = normalSprite;
                    yield return null;
                    continue;
                }
                // 効果が切れたらWarning状態に復帰
                if (currentState == CatState.Normal && !actorController.isCatTeaserActive)
                {
                    currentState = CatState.Warning;
                    spriteRenderer.sprite = warningSprite;
                }
                timer += Time.deltaTime;
                yield return null;
            }

            // --- 3. Threatening（脅威状態） ---
            currentState = CatState.Threatening;
            spriteRenderer.sprite = threateningSprite;

            timer = 0f;
            float threateningTime = 3f;
            while (timer < threateningTime)
            {
                // 脅威中に猫じゃらしが発動したら強制的にNormal状態へ引き戻して判定回避
                if (actorController != null && actorController.isCatTeaserActive)
                {
                    currentState = CatState.Normal;
                    spriteRenderer.sprite = normalSprite;
                    yield return null;
                    continue;
                }
                // 効果が切れたらThreatening状態に復帰
                if (currentState == CatState.Normal && !actorController.isCatTeaserActive)
                {
                    currentState = CatState.Threatening;
                    spriteRenderer.sprite = threateningSprite;
                }

                // 脅威状態で、かつ隠れていない場合のみ即死判定
                if (currentState == CatState.Threatening && !actorController.isInsideHideSpot)
                {
                    actorController.Damaged();
                    Time.timeScale = 0;
                    yield break;
                }

                timer += Time.deltaTime;
                yield return null;
            }
        }
    }
}
