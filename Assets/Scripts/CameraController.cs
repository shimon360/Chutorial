using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ParallaxLayer
{
    [Tooltip("動かすオブジェクト")]
    public Transform target;

    [Tooltip("視差係数（0～1）")]
    [Range(0f, 1f)]
    public float factor = 1.0f;
}

/// <summary>
/// メインカメラ制御クラス(Main Cameraにアタッチ)
/// </summary>
public class CameraController : MonoBehaviour
{
	[Header("パララックス設定")]
	public List<ParallaxLayer> parallaxLayers = new List<ParallaxLayer>();

	[Header("Objectループ設定")]
	public float objectLoopWidth = 20f; // 画面横幅（要調整）
	public float objectResetOffset = 20f;

    [Header("追従対象")]
    [SerializeField]
    private Transform player;
	private ActorController actorController;

	// 各種変数
	private Vector3 previousCameraPos; // 前フレームのカメラ座標

	private float fixedY;

	private bool isFocusing = false;     // フォーカス中フラグ

	[Header("カメラ範囲")]
	[SerializeField]
	private Transform leftLimit;
	[SerializeField]
	private Transform rightLimit;

	[SerializeField]
	private float cameraOffset = 2.5f; // Playerを画面の左側に表示する量

	[Header("フォーカス設定")]
	[SerializeField]
	private float focusTime = 1.0f;
	[SerializeField]
	private float focusMoveDuration = 0.5f;
	
	[HideInInspector] public bool isScrolling = false; // エリア間スクロール中かどうか
	private Vector2 scrollTargetPos; // スクロール時の目標座標

	private void Start ()
	{
		// 最初のカメラ位置を記憶しておく
		previousCameraPos = transform.position;

		if (player != null)
		{
			fixedY = transform.position.y;
			actorController = player.GetComponent<ActorController>();
		}
	}

	/// <summary>
	/// エリア間スクロールを開始する
	/// </summary>
	public void StartScroll(Vector2 targetPos)
	{
		scrollTargetPos = targetPos;
		isScrolling = true;
	}

	/// <summary>
	/// エリア間スクロールを終了し、通常追従に戻る
	/// </summary>
	public void EndScroll()
	{
		isScrolling = false;
	}

	// FixedUpdate
	private void FixedUpdate ()
	{
		if(isFocusing)
		{
			return;
		}

		// カメラの目標座標を決定する
		Vector3 pos = transform.localPosition;

		if (!isScrolling)
		{
			// 通常時：アクターの現在位置より少し右上を映すようにX・Y座標を補正
			float offset = cameraOffset;

			if (actorController != null && !actorController.rightFacing)
			{
				offset = -cameraOffset;
			}

			float targetX = player.position.x + offset;
			float halfWidth = Camera.main.orthographicSize * Camera.main.aspect;
			targetX = Mathf.Clamp(targetX, leftLimit.position.x + halfWidth, rightLimit.position.x - halfWidth);

			pos.x = targetX; // X座標
			pos.y = fixedY; // Y座標
		}
		else
		{
			// スクロール時：指定された目標座標へ向かう
			pos.x = scrollTargetPos.x;
			pos.y = scrollTargetPos.y;
		}

		// Z座標は現在値(transform.localPosition)をそのまま使用

		// 計算後のカメラ目標座標へ滑らかに移動(Lerp)
		transform.localPosition = Vector3.Lerp (transform.localPosition, pos, 0.08f);

		// --- パララックス（視差）スクロール処理 ---
		Vector3 deltaCameraPos = transform.position - previousCameraPos;
		Debug.Log(deltaCameraPos);

		foreach (var layer in parallaxLayers)
		{
			if (layer.target == null) continue;

			layer.target.position += new Vector3(
				deltaCameraPos.x * layer.factor,
        		0,
        		0
    		);

			if (layer.target.name.Contains("object"))
			{
				LoopObject(layer.target);
			}
		}

		// 次のフレームの計算用に現在のカメラ座標を保存
		previousCameraPos = transform.position;

	}

	private void LoopObject(Transform obj)
	{
	    float cameraX = transform.position.x;

	    if (obj.position.x < cameraX - objectResetOffset)
    	{
        	obj.position += new Vector3(objectLoopWidth * 2f, 0, 0);
    	}
	}

	public void FocusOn(Transform target, System.Action onFinished = null)
	{
		StartCoroutine(FocusCoroutine(target, onFinished));
	}

	private IEnumerator FocusCoroutine(Transform target, System.Action onFinished)
	{
		isFocusing = true;
		if(actorController != null)
		{
			actorController.SetControlEnabled(false);
		}
		
		// 開始位置
		Vector3 startPosition = transform.localPosition;

		// 猫を見る位置
		Vector3 targetPosition = new Vector3(
			target.position.x,
			target.position.y,
			transform.localPosition.z
		);


		// プレイヤー位置から猫位置へ移動
		float timer = 0f;

		while(timer < focusMoveDuration)
		{
			timer += Time.deltaTime;

			transform.localPosition = Vector3.Lerp(
				startPosition,
				targetPosition,
				timer / focusMoveDuration
			);

			yield return null;
		}


		// 猫を見る時間
		yield return new WaitForSeconds(focusTime);


		// 元の位置へ戻る
		timer = 0f;

		while(timer < focusMoveDuration)
		{
			timer += Time.deltaTime;

			transform.localPosition = Vector3.Lerp(
				targetPosition,
				startPosition,
				timer / focusMoveDuration
			);

			yield return null;
		}


		transform.localPosition = startPosition;

		isFocusing = false;

		actorController.SetControlEnabled(true);

		onFinished?.Invoke();
	}
}