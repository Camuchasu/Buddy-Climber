using UnityEngine;

public class PlatformPickup : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Playerタグ判定
        if (other.CompareTag("Player"))
        {
            PlatformItem item =
                other.GetComponent<PlatformItem>();

            if (item != null)
            {
                // 使用可能化
                item.enabled = true;

                Debug.Log("足場アイテム取得！");
            }

            // アイテム消す
            Destroy(gameObject);
        }
    }
}