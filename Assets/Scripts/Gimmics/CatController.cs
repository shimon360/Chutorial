using UnityEngine;

/// <summary>
/// 猫の状態管理クラス
/// </summary>
public class CatController : MonoBehaviour
{
    /// <summary>
    /// 猫の状態
    /// </summary>
    private enum CatState
    {
        Idle,       // 通常
        Alert,      // 警戒
        Hunting     // 狩り
    }

    [Header("スプライト")]
    [SerializeField] private Sprite cat0;
    [SerializeField] private Sprite cat1;
    [SerializeField] private Sprite alertCat;
    [SerializeField] private Sprite huntingCat0;
    [SerializeField] private Sprite huntingCat1;

    [Header("時間設定")]
    [SerializeField] private float minIdleTime = 5f;
    [SerializeField] private float maxIdleTime = 10f;
    [SerializeField] private float alertTime = 1.5f;
    [SerializeField] private float huntingTime = 3f;

    [Header("アニメーション")]
    [SerializeField] private float animationSpeed = 0.3f;

    [Header("プレイヤー")]
    [SerializeField] private ActorController player;

    private SpriteRenderer spriteRenderer;

    private CatState currentState;

    private float stateTimer;
    private float animationTimer;

    private bool animationFrame;

    private bool isActive = false;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        ChangeState(CatState.Idle);
    }

    void Update()
    {
        if (!isActive)
        {
            return;
        }

        stateTimer -= Time.deltaTime;
        animationTimer += Time.deltaTime;

        switch (currentState)
        {
            case CatState.Idle:
                UpdateIdle();
                break;

            case CatState.Alert:
                UpdateAlert();
                break;

            case CatState.Hunting:
                UpdateHunting();
                break;
        }
    }

    /// <summary>
    /// 通常状態
    /// </summary>
    private void UpdateIdle()
    {
        Animate(cat0, cat1);

        if (stateTimer <= 0f)
        {
            ChangeState(CatState.Alert);
        }
    }

    /// <summary>
    /// 警戒状態
    /// </summary>
    private void UpdateAlert()
    {
        spriteRenderer.sprite = alertCat;

        if (stateTimer <= 0f)
        {
            ChangeState(CatState.Hunting);
        }
    }

    /// <summary>
    /// 狩状態
    /// </summary>
    private void UpdateHunting()
    {
        Animate(huntingCat0, huntingCat1);

        if (!player.isHiddenFromRedLight)
        {
            player.Damaged();
            return;
        }

        if (stateTimer <= 0f)
        {
            ChangeState(CatState.Idle);
        }
    }

    /// <summary>
    /// 状態変更
    /// </summary>
    private void ChangeState(CatState nextState)
    {
        currentState = nextState;

        animationTimer = 0f;
        animationFrame = false;

        switch (currentState)
        {
            case CatState.Idle:

                stateTimer = Random.Range(minIdleTime, maxIdleTime);
                spriteRenderer.sprite = cat0;

                break;

            case CatState.Alert:

                stateTimer = alertTime;
                spriteRenderer.sprite = alertCat;

                break;

            case CatState.Hunting:

                stateTimer = huntingTime;
                spriteRenderer.sprite = huntingCat0;

                break;
        }
    }

    /// <summary>
    /// 2枚アニメーション
    /// </summary>
    private void Animate(Sprite sprite0, Sprite sprite1)
    {
        if (animationTimer < animationSpeed)
        {
            return;
        }

        animationTimer = 0f;

        animationFrame = !animationFrame;

        spriteRenderer.sprite = animationFrame ? sprite1 : sprite0;
    }

    /// <summary>
    /// 猫の活動開始
    /// </summary>
    public void StartCat()
    {
        isActive = true;

        ChangeState(CatState.Idle);
    }


    /// <summary>
    /// 猫の活動停止
    /// </summary>
    public void StopCat()
    {
        isActive = false;
    }
}