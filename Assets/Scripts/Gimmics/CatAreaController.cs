using UnityEngine;

public class CatAreaController : MonoBehaviour
{
    [SerializeField]
    private CatController cat;

    [SerializeField]
    private CameraController cameraController;

    [Header("再フォーカス設定")]
    [SerializeField]
    private Transform player;

    [SerializeField]
    private float resetDistance = 8f;

    // 一度フォーカスしたか
    private bool hasFocused = false;

    // フォーカス終了後に距離判定を開始するためのフラグ
    private bool canReset = false;

    private void Update()
    {
        if (!hasFocused || !canReset)
        {
            return;
        }

        float distance = Mathf.Abs(
            player.position.x - cat.transform.position.x
        );

        if (distance > resetDistance)
        {
            hasFocused = false;
            canReset = false;

            cat.StopCat();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
        {
            return;
        }

        if (hasFocused)
        {
            return;
        }

        hasFocused = true;
        canReset = false;

        cat.StartCat();

        cameraController.FocusOn(cat.transform, OnFocusFinished);
    }

    private void OnFocusFinished()
    {
        canReset = true;
    }
}