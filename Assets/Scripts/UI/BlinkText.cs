using System.Collections;
using TMPro;
using UnityEngine;

public class BlinkText : MonoBehaviour
{
    [Header("1回のフェード時間")]
    [SerializeField]
    private float fadeDuration = 0.8f;

    [Header("表示している時間")]
    [SerializeField]
    private float visibleTime = 0.5f;

    [Header("非表示の時間")]
    [SerializeField]
    private float invisibleTime = 0.5f;

    private TextMeshProUGUI textUI;

    private void Awake()
    {
        textUI = GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        StartCoroutine(BlinkCoroutine());
    }

    private IEnumerator BlinkCoroutine()
    {
        while (true)
        {
            // 表示状態
            SetAlpha(1f);
            yield return new WaitForSeconds(visibleTime);

            // フェードアウト
            yield return Fade(1f, 0f);

            // 非表示状態
            yield return new WaitForSeconds(invisibleTime);

            // フェードイン
            yield return Fade(0f, 1f);
        }
    }

    private IEnumerator Fade(float start, float end)
    {
        float time = 0f;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;

            float alpha = Mathf.Lerp(start, end, time / fadeDuration);

            SetAlpha(alpha);

            yield return null;
        }

        SetAlpha(end);
    }

    private void SetAlpha(float alpha)
    {
        Color color = textUI.color;
        color.a = alpha;
        textUI.color = color;
    }
}