using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// アクター操作・制御クラス
/// </summary>
public class ActorController : MonoBehaviour
{
	// オブジェクト・コンポーネント参照
	private Rigidbody2D rigidbody2D;
	private SpriteRenderer spriteRenderer;
	private ActorGroundSensor groundSensor;
	private ActorSprite actorSprite;
	public CameraController cameraController; // カメラ制御クラス

	// ステータス関連変数
	private bool isDead = false; // 死亡済みかどうかのフラグ
	
	[Header("信号隠れ設定")]
	[Tooltip("影や遮蔽物（HideSpot）に隠れているかどうかのフラグ")]
	public bool isInsideHideSpot = false;

	private int shadowLayerId;
	private int shadowOverlapCount = 0;

	[Header("スコア・食べ残し設定")]
	[Tooltip("現在のスコア")]
	public int score = 0;

	[Tooltip("回収した食べ残しの数")]
	public int leftoverCount = 0;

	[Header("速度減衰設定")]
	[Tooltip("ベースの移動速度")]
	public float baseSpeed = 6.0f;

	[Tooltip("食べ残し1個あたりの速度低下率 (0.1 = 10%低下)")]
	public float speedPenaltyRate = 0.1f;

	[Tooltip("どれだけ食べ残しを拾っても保証される最低速度の割合 (0.3 = ベース速度の30%は維持)")]
	public float minSpeedRatio = 0.3f;

	[Header("ジャンプ減衰設定")]
	[Tooltip("ベースのジャンプ力")]
	public float baseJumpPower = 10.0f;

	[Tooltip("どれだけ食べ残しを拾っても保証される最低ジャンプ力の割合 (0.5 = ベースジャンプ力の50%は維持)")]
	public float minJumpPowerRatio = 0.5f;

	// 移動関連変数
	[HideInInspector] public float xSpeed; // X方向移動速度
	[HideInInspector] public bool rightFacing; // 向いている方向(true.右向き false:左向き)

	// Start（オブジェクト有効化時に1度実行）
	void Start()
	{
		// コンポーネント参照取得
		rigidbody2D = GetComponent<Rigidbody2D> ();
		spriteRenderer = GetComponent<SpriteRenderer> ();
		groundSensor = GetComponentInChildren<ActorGroundSensor> ();
		actorSprite = GetComponent<ActorSprite> ();

		// 配下コンポーネント初期化
		if (actorSprite != null)
		{
			actorSprite.Init (this);
		}

		cameraController.SetPosition (transform.position);
		
		// 変数初期化
		rightFacing = true; // 最初は右向き

		// 影レイヤーのIDを取得
		shadowLayerId = LayerMask.NameToLayer("Layer_Shadow");
		if (shadowLayerId == -1)
		{
			Debug.LogWarning("【警告】Unityのエディタ側で 'Layer_Shadow' レイヤーが作成されていません。エディタの [Project Settings] -> [Tags and Layers] で 'Layer_Shadow' レイヤーを追加し、隠れ場所に設定してください。");
		}
	}

    // Update（1フレームごとに1度ずつ実行）
    void Update()
	{
		// 左右移動処理
		MoveUpdate ();
		// ジャンプ入力処理
		JumpUpdate ();

		cameraController.SetPosition (transform.position);
	}
	
	/// <summary>
	/// Updateから呼び出される左右移動入力処理
	/// </summary>
	private void MoveUpdate ()
	{
		// 食べ残しのペナルティを加味した現在の移動速度を計算
		float speedPenalty = 1f - (leftoverCount * speedPenaltyRate);
		float currentSpeed = baseSpeed * Mathf.Max(minSpeedRatio, speedPenalty);

		// X方向移動入力
		if (Keyboard.current.rightArrowKey.isPressed)
		{// 右方向の移動入力
			// X方向移動速度をプラスに設定
			xSpeed = currentSpeed;

			// 右向きフラグon
			rightFacing = true;

			// スプライトを通常の向きで表示
			spriteRenderer.flipX = false;
		}
		else if (Keyboard.current.leftArrowKey.isPressed)
		{// 左方向の移動入力
			// X方向移動速度をマイナスに設定
			xSpeed = -currentSpeed;

			// 右向きフラグoff
			rightFacing = false;

			// スプライトを左右反転した向きで表示
			spriteRenderer.flipX = true;
		}
		else
		{// 入力なし
			// X方向の移動を停止
			xSpeed = 0.0f;
		}
	}

	/// <summary>
	/// Updateから呼び出されるジャンプ入力処理
	/// </summary>
	private void JumpUpdate ()
	{
		// 接地している時に上キーが「押された瞬間」のみジャンプ
		if (groundSensor != null && groundSensor.isGrounded && Keyboard.current.upArrowKey.wasPressedThisFrame)
		{// ジャンプ開始
			// 食べ残しのペナルティを加味した現在のジャンプ力を計算
			float jumpPenalty = 1f - (leftoverCount * speedPenaltyRate);
			float currentJumpPower = baseJumpPower * Mathf.Max(minJumpPowerRatio, jumpPenalty);
			
			// ジャンプ力を適用
			rigidbody2D.linearVelocity = new Vector2 (rigidbody2D.linearVelocity.x, currentJumpPower);
		}

		// 上キーを「離した瞬間」にまだ上昇中なら、上方向の速度を減らしてジャンプの高さを調整する
		if (Keyboard.current.upArrowKey.wasReleasedThisFrame)
		{
			if (rigidbody2D.linearVelocity.y > 0.0f)
			{
				rigidbody2D.linearVelocity = new Vector2 (rigidbody2D.linearVelocity.x, rigidbody2D.linearVelocity.y * 0.5f);
			}
		}
	}

	// FixedUpdate（一定時間ごとに1度ずつ実行・物理演算用）
	private void FixedUpdate ()
	{
		// 移動速度ベクトルを現在値から取得
		Vector2 velocity = rigidbody2D.linearVelocity;
		// X方向の速度を入力から決定
		velocity.x = xSpeed;

		// 計算した移動速度ベクトルをRigidbody2Dに反映
		rigidbody2D.linearVelocity = velocity;
	}

	/// <summary>
	/// 外部（ギミックや敵など）からダメージを受ける処理（即死してエンディングへ）
	/// </summary>
	public void Damaged()
	{
		Debug.Log("Damaged呼ばれた: " + gameObject.name);
		Debug.Log("SpriteRenderer activeSelf: " + gameObject.activeSelf);
		Debug.Log("enabled: " + enabled);
		
		if (isDead) return; // 既に死んでいる場合は以降の処理を重複して行わない
		isDead = true;

		Debug.Log("アクターがダメージギミックに触れた！ 即死！");
		
		// やられアニメーション（画像）に切り替える
		if (actorSprite != null)
		{
			actorSprite.PlayDeadAnimation();
		}

		// 操作を受け付けなくし、物理演算による動き（慣性など）もピタッと止める
		enabled = false;
		rigidbody2D.linearVelocity = Vector2.zero;

		// TODO: ここにエンディング画面（シーン）への遷移処理を追加する
		// 例： UnityEngine.SceneManagement.SceneManager.LoadScene("EndingScene");
	}

	/// <summary>
	/// 外部から影や遮蔽物に隠れるモードを切り替えるメソッド
	/// </summary>
	/// <param name="isInside">隠れるかどうか</param>
	public void SetInsideHideSpot(bool isInside)
	{
		isInsideHideSpot = isInside;
		// 外部から直接切り替えるため、重なりカウンターもリセットまたは整合性を合わせる
		if (!isInside)
		{
			shadowOverlapCount = 0;
		}
	}

	/// <summary>
	/// トリガー衝突判定（Layer_Shadowと重なった時に隠れフラグをONにする）
	/// </summary>
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (shadowLayerId != -1 && collision.gameObject.layer == shadowLayerId)
		{
			shadowOverlapCount++;
			UpdateInsideHideSpotStatus();
		}
	}

	/// <summary>
	/// トリガー脱出判定（Layer_Shadowから離れた時に隠れフラグをOFFにする）
	/// </summary>
	private void OnTriggerExit2D(Collider2D collision)
	{
		if (shadowLayerId != -1 && collision.gameObject.layer == shadowLayerId)
		{
			shadowOverlapCount = Mathf.Max(0, shadowOverlapCount - 1);
			UpdateInsideHideSpotStatus();
		}
	}

	private void UpdateInsideHideSpotStatus()
	{
		isInsideHideSpot = (shadowOverlapCount > 0);
	}

	/// <summary>
	/// 食べ残しを回収した時の処理
	/// </summary>
	/// <param name="scoreValue">獲得スコア</param>
	public void CollectLeftover(int scoreValue)
	{
		score += scoreValue;
		leftoverCount++;
		
		// 現在のペナルティ速度・ジャンプ力をデバッグ出力
		float currentSpeed = baseSpeed * Mathf.Max(minSpeedRatio, 1f - (leftoverCount * speedPenaltyRate));
		float currentJump = baseJumpPower * Mathf.Max(minJumpPowerRatio, 1f - (leftoverCount * speedPenaltyRate));
		Debug.Log($"【食べ残し回収】 スコア +{scoreValue} (現在スコア: {score}) | 食べ残し数: {leftoverCount} | 現在速度: {currentSpeed} | 現在ジャンプ力: {currentJump}");
	}
}