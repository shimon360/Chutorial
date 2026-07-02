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
