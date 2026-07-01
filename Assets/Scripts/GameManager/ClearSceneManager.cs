using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// クリア画面のUIテキスト表示管理クラス
/// </summary>
public class ClearSceneManager : MonoBehaviour
{
    [Header("スコア表示用UI")]
    [Tooltip("TextMeshProのテキストをアタッチしてください")]
    public TextMeshProUGUI scoreTextTMP;
    
    [Tooltip("従来のUI Textを使う場合はこちらにアタッチしてください")]
    public Text scoreTextLegacy;

    [Header("クリア時間表示用UI")]
    [Tooltip("TextMeshProのテキストをアタッチしてください")]
    public TextMeshProUGUI timeTextTMP;

    [Tooltip("従来のUI Textを使う場合はこちらにアタッチしてください")]
    public Text timeTextLegacy;

    private void Start()
    {
        // データの文字列化
        string scoreStr = $"Score: {StageManager.finalScore}";
        
        // 経過時間を 分:秒 (01:23) の形式に整形
        int minutes = Mathf.FloorToInt(StageManager.finalClearTime / 60F);
        int seconds = Mathf.FloorToInt(StageManager.finalClearTime % 60F);
        string timeStr = string.Format("Time: {0:00}:{1:00}", minutes, seconds);

        // 各UIテキストに反映（アタッチされているもののみ）
        if (scoreTextTMP != null) scoreTextTMP.text = scoreStr;
        if (scoreTextLegacy != null) scoreTextLegacy.text = scoreStr;

        if (timeTextTMP != null) timeTextTMP.text = timeStr;
        if (timeTextLegacy != null) timeTextLegacy.text = timeStr;

        Debug.Log($"【クリアシーン】 スコア: {StageManager.finalScore} | 時間: {timeStr} ({StageManager.finalClearTime:F2}秒) を表示しました。");
    }
}
