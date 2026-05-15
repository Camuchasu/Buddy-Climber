using UnityEngine;

public class HookItem : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Playerタグなら
        if (other.CompareTag("Player"))
        {
            // プレイヤーのHook取得
            Hook hook = other.GetComponent<Hook>();

            if (hook != null)
            {
                // Hookを使えるようにする
                hook.enabled = true;

                Debug.Log("フック取得！");
            }

            // アイテム消す
            Destroy(gameObject);
        }
    }
}
