using UnityEngine;
using UnityEngine.SceneManagement;

public class Goal : MonoBehaviour
{
    [SerializeField] private string titleSceneName = "Title"; // タイトルシーン名

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // プレイヤーだけ反応
        {
            SceneManager.LoadScene(titleSceneName);
        }
    }
}