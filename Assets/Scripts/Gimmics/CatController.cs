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
    public float followRate = 0.9f;

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
            currentState = CatState.Normal;
            spriteRenderer.sprite = normalSprite;
            yield return new WaitForSeconds(5f);

            currentState = CatState.Warning;
            spriteRenderer.sprite = warningSprite;
            yield return new WaitForSeconds(2f);

            currentState = CatState.Threatening;
            spriteRenderer.sprite = threateningSprite;

            float threateningTime = 3f;
            float timer = 0f;

            while (timer < threateningTime)
            {
                if (!actorController.isInsideHideSpot)
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
