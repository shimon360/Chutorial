# Chutorial_demo - スクリプト・ギミック解説

このプロジェクトは、Unityで制作された2D横スクロールゲームです。
リファクタリングおよび新規実装した主要スクリプトの構成、仕様、およびUnityエディタ上での設定手順をここにまとめます。

---

## 📁 スクリプト構成と役割

### 1. アクター・プレイヤー制御
* **[ActorController.cs](Assets/Scripts/Actor/ActorController.cs)**
  * プレイヤー（ネズミ）の移動、ジャンプ、被ダメージ即死処理を制御します。
  * `Layer_Shadow`（影・遮蔽レイヤー）による「隠れ状態（`isInsideHideSpot`）」を一元管理します。
  * スコア（`score`）および食べ残し数（`leftoverCount`）を保持し、食べ残しを拾うほど歩行速度・ジャンプ力が低下するペナルティロジックを実装しています。
  * 死亡時は、1.5秒間の被弾点滅演出を見せたのち、自動的にゲームオーバーシーン（`GameOverScene`）へ遷移します。
* **[ActorSprite.cs](Assets/Scripts/Actor/ActorSprite.cs)**
  * プレイヤーの歩行・待機アニメーションを制御します。また、死亡時の「実時間（タイムスケール=0の影響を受けない）ベースでの点滅アニメーション」を実行します。

### 2. ゲーム・エリア管理
* **[StageManager.cs](Assets/Scripts/GameManager/StageManager.cs)**
  * ステージ全体の初期化、エリア遷移（スクロール）演出を制御します。
  * エリア遷移中フラグ（`IsChangingArea`）を外部に公開し、遷移中の時間カウント停止や猫の追従停止を可能にします。
  * シーン遷移後もデータを保持する静的変数（`finalScore`, `finalClearTime`）を定義し、プレイ時間（`playTime`）を自動計測します。
* **[AreaManager.cs](Assets/Scripts/GameManager/AreaManager.cs)**
  * 各エリアのアクティブ/非アクティブ化を管理します。
* **[ScrollEnter.cs](Assets/Scripts/GameManager/ScrollEnter.cs)**
  * エリア間スクロールを引き起こす境界トリガー。インスペクターで次のエリア（`Next Area`）をアタッチして使用します。

### 3. ギミック・アイテム
* **[CatController.cs](Assets/Scripts/Cat/CatController.cs)**
  * 猫の監視（だるまさんが転んだ）ギミック。
  * プレイヤー（カメラ）への自動追従（パララックス比率・滑らかな補間移動）をサポート。
  * エリア遷移中は自動で追従を一時停止し、古い猫が画面外へスクロールアウトする演出を行います。
  * `OnEnable`/`OnDisable` で正しく初期化・コルーチンの開始停止を行い、エリアごとの配置に対応します。
* **[Cheese.cs](Assets/Scripts/Cheese/Cheese.cs)**
  * キーアイテムのチーズ。接触すると消滅し、`isCollected` を有効にします。インスペクターの誤操作を防ぐためフラグは非表示です。
* **[Leftover.cs](Assets/Scripts/Item/Leftover.cs)**
  * 食べ残しスコアアイテム。拾うとスコアが加算され、アクターの速度・ジャンプ力が段階的に減衰します。
* **[Goal.cs](Assets/Scripts/House/Goal.cs)**
  * ゴールハウス。チーズ（`Cheese`）を持っている場合のみクリア判定になり、クリアデータ（スコア・時間）を静的変数に保存してクリアシーン（`ClearScene`）へ遷移します。

### 4. シーン管理
* **[ClearSceneManager.cs](Assets/Scripts/GameManager/ClearSceneManager.cs)**
  * `ClearScene` でクリア時の最終スコアとクリア時間をUIテキストに表示します（TextMeshPro / Legacy Text 双方に対応）。

---

## ⚙️ Unityエディタでの設定の重要ポイント

### エリアの往復スクロール
1. 各エリアの「左端」と「右端」の両方に `ScrollEnter` オブジェクトを配置します。
2. 右端の `ScrollEnter` の `Next Area` には「次のエリア」、左端の `ScrollEnter` の `Next Area` には「手前のエリア」をアタッチします。

### 床コライダーの引っかかり防止（コライダー結合）
1. 床（Tilemap）の `Tilemap Collider 2D` コンポーネントにある **`Composite Operation`** を **`Merge`** に変更します。
2. 同オブジェクトに **`Composite Collider 2D`** をアタッチします。
3. 自動追加される **`Rigidbody 2D`** の **`Body Type`** を **`Kinematic`** に設定します。
