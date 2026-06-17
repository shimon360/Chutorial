using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ステージギミック：ダメージ床
/// ブロックに乗ったアクター(またはトリガーに進入したアクター)にダメージを与える
/// </summary>
public class Gimmic_DamageBlock : MonoBehaviour
{
	// アクター接触時処理(Trigger)
	private void OnTriggerStay2D (Collider2D collision)
	{
		// タグの設定忘れを防ぐため、ActorControllerを持っているかで判定する
		ActorController actor = collision.gameObject.GetComponent<ActorController>();
		if (actor != null)
		{
			actor.Damaged();
		}
	}
	// アクター接触時処理(Collider)
	private void OnCollisionStay2D (Collision2D collision)
	{
		ActorController actor = collision.gameObject.GetComponent<ActorController>();
		if (actor != null)
		{
			actor.Damaged();
		}
	}
}