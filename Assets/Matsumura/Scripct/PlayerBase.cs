using UnityEngine;

public class PlayerBase : MonoBehaviour
{
    public CharacterController controller;
    public Transform cam; // Main CameraのTransformをアタッチ

    public float speed = 5.0f;
    public Transform cameraTransform; // メインカメラのTransformを割り当てる
   
    void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        // 入力がある場合のみ処理
        Vector3 inputDir = new Vector3(horizontal, 0, vertical).normalized;

        if (inputDir.magnitude >= 0.1f)
        {
            // カメラの向きに基づいた移動方向を計算
            float targetAngle = Mathf.Atan2(inputDir.x, inputDir.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
            Vector3 moveDir = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;

            // 移動実行
            controller.Move(moveDir.normalized * speed * Time.deltaTime);

            // 進行方向を向かせる（もしキャラの向きも変えたいならここで行う）
            transform.rotation = Quaternion.Euler(0, targetAngle, 0);
        }
    }
}


