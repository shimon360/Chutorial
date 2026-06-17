using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// メインカメラ制御クラス(Main Cameraにアタッチ)
/// </summary>
public class CameraController : MonoBehaviour
{
	// オブジェクト・コンポーネント
	[Tooltip("動かしたい背景オブジェクト")]
	public Transform background;

	// 各種変数
	private Vector2 basePos; // 基点座標（通常時のアクター座標）
	private Vector3 previousCameraPos; // 前フレームのカメラ座標
	
	[HideInInspector] public bool isScrolling = false; // エリア間スクロール中かどうか
	private Vector2 scrollTargetPos; // スクロール時の目標座標
	
	[Tooltip("背景のスクロール量（0.5ならカメラの半分の速度で移動）")]
	public float parallaxFactor = 0.5f;

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
		if (background != null)
		{
			// 今回のフレームでのカメラ移動量を計算
			Vector3 deltaCameraPos = transform.position - previousCameraPos;
			
			// 背景をカメラの移動量 × パララックス係数 の分だけ動かす
			background.position += new Vector3 (deltaCameraPos.x * parallaxFactor, deltaCameraPos.y * parallaxFactor, 0);
		}

		// 次のフレームの計算用に現在のカメラ座標を保存
		previousCameraPos = transform.position;
	}
}