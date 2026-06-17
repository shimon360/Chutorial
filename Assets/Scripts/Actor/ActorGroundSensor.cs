using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// アクターの接地判定用センサー
/// </summary>
public class ActorGroundSensor : MonoBehaviour
{
    // 接地フラグ（他のスクリプトから参照する用）
    [HideInInspector] public bool isGrounded = false;

    // 地面と判定するタグ名（Unityエディタで地面オブジェクトにこのタグを設定してください）
    private string groundTag = "Ground";

    // -------------------------------------------------------------
    // トリガー内にオブジェクトが侵入した瞬間に呼ばれる
    // -------------------------------------------------------------
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 接触したオブジェクトのタグが地面(Ground)かチェック
        if (collision.CompareTag(groundTag))
        {
            isGrounded = true;
        }
    }

    // -------------------------------------------------------------
    // トリガー内にオブジェクトが留まっている間呼ばれ続ける
    // -------------------------------------------------------------
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag(groundTag))
        {
            isGrounded = true;
        }
    }

    // -------------------------------------------------------------
    // トリガー内からオブジェクトが出た瞬間に呼ばれる
    // -------------------------------------------------------------
    private void OnTriggerExit2D(Collider2D collision)
    {
        // 接触が離れたら接地フラグを解除
        if (collision.CompareTag(groundTag))
        {
            isGrounded = false;
        }
    }
}
