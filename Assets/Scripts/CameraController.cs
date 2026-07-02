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

	// 各種変数
	private Vector2 basePos; // 基点座標（通常時のアクター座標）
	private Vector3 previousCameraPos; // 前フレームのカメラ座標
	
	[HideInInspector] public bool isScrolling = false; // エリア間スクロール中かどうか
	private Vector2 scrollTargetPos; // スクロール時の目標座標

	private void Start ()
	{
		// 最初のカメラ位置を記憶しておく
		previousCameraPos = transform.position;
	}

	/// <summary>
	/// カメラの位置を動かす
	/// </summary>
	/// <param name="targetPos">座標</param>
	public void SetPosition (Vector2 targetPos)
	{
		basePos = targetPos;
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
		Debug.Log("FixedUpdate動いている");

		// カメラの目標座標を決定する
		Vector3 pos = transform.localPosition;

		if (!isScrolling)
		{
			// 通常時：アクターの現在位置より少し右上を映すようにX・Y座標を補正
			pos.x = basePos.x + 2.5f; // X座標
			pos.y = basePos.y + 1.5f; // Y座標
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
}