using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ステージマネージャクラス
/// </summary>
public class StageManager : MonoBehaviour
{
	[HideInInspector] public ActorController actorController; // アクター制御クラス
	[HideInInspector] public CameraController cameraController; // カメラ制御クラス

	[Header ("初期エリアのAreaManager")]
	public AreaManager initArea; // ステージ内の最初のエリア(初期エリア)

	// ステージ内の全エリアの配列(Startで取得)
	private AreaManager[] inStageAreas;

	[Header ("クリアデータ保存用（静的変数）")]
	public static int finalScore = 0;
	public static float finalClearTime = 0f;

	[Header ("プレイ時間計測")]
	public float playTime = 0f;

	// Start
	void Start()
	{
		// 静的変数を初期化（リトライ時のリセット用）
		finalScore = 0;
		finalClearTime = 0f;
		playTime = 0f;
		// 参照取得
		actorController = GetComponentInChildren<ActorController> ();
		cameraController = GetComponentInChildren<CameraController> ();

		// ステージ内の全エリアを取得・初期化
		inStageAreas = GetComponentsInChildren<AreaManager> (true);
		foreach (var targetAreaManager in inStageAreas)
			targetAreaManager.Init (this);

		// 初期エリアをアクティブ化(その他のエリアは全て無効化)
		initArea.ActiveArea ();
	}

	void Update()
	{
		// エリア遷移中でない、かつアクターが有効な間は時間を進める
		if (!isChangingArea && actorController != null && actorController.enabled)
		{
			playTime += Time.deltaTime;
		}
	}

	/// <summary>
	/// ステージ内の全エリアを無効化する
	/// </summary>
	public void DeactivateAllAreas ()
	{
		foreach (var targetAreaManager in inStageAreas)
			targetAreaManager.gameObject.SetActive (false);
	}

	private bool isChangingArea = false; // エリア遷移中かどうかを判定するフラグ
	public bool IsChangingArea => isChangingArea;

	/// <summary>
	/// エリアの切り替え（スクロール遷移）を開始する
	/// </summary>
	public void ChangeArea(AreaManager nextArea)
	{
		if (isChangingArea) return; // すでに遷移中なら重複して実行しない
		StartCoroutine(ChangeAreaCoroutine(nextArea));
	}

	private IEnumerator ChangeAreaCoroutine(AreaManager nextArea)
	{
		isChangingArea = true;

		// 1. プレイヤーの操作を無効化し、動きを止める
		if (actorController != null)
		{
			actorController.enabled = false;
			// 物理演算による慣性移動を止める
			actorController.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
		}

		// 3. エリアのアクティブ状態を切り替え
		nextArea.ActiveArea();
		initArea = nextArea; // 現在のエリアを更新

		// 2. カメラのスクロールを開始
		if (cameraController != null)
		{
			// カメラに次のエリアの基準座標を渡してスクロールさせる
			cameraController.StartScroll(nextArea.cameraBasePos);
			
			// カメラが目標座標に十分近づくまで待機
			while (true)
			{
				Vector2 currentCamPos = new Vector2(cameraController.transform.localPosition.x, cameraController.transform.localPosition.y);
				if (Vector2.Distance(currentCamPos, nextArea.cameraBasePos) < 0.1f)
				{
					break;
				}
				yield return null;
			}

			// スクロール状態を終了
			cameraController.EndScroll();
		}



		// 4. トリガーからプレイヤーを少し押し出して連続接触を防ぐ
		if (actorController != null)
		{
			float pushDir = actorController.rightFacing ? 1.0f : -1.0f;
			actorController.transform.position += new Vector3(pushDir, 0, 0);
			
			// 少し待ってから操作を再開（画面切り替え直後の誤操作防止）
			yield return new WaitForSeconds(0.1f);
			actorController.enabled = true;
		}

		isChangingArea = false;
	}
}