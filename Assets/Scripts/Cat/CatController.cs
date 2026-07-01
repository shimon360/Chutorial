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

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        StartCoroutine(CatRoutine());
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
                if (!IsMouseHidden())
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

    private bool IsMouseHidden()
    {
        HideSpot[] hideSpots =
        FindObjectsByType<HideSpot>(FindObjectsSortMode.None);

        foreach (HideSpot hideSpot in hideSpots)
        {
            Collider2D hideCollider =
            hideSpot.GetComponent<Collider2D>();

            if (hideCollider != null &&
            hideCollider.OverlapPoint(actorController.transform.position))
            {
                return true;
            }
        }

        return false;
    }
    
}
