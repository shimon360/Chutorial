using TMPro;
using UnityEngine;

public class ScoreUIController : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI scoreText;


    private void Update()
    {
        if(GameManager.Instance == null)
            return;


        scoreText.text =
            "Score : " + GameManager.Instance.Score;
    }
}