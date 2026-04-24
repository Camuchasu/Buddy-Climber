using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class StartButton : MonoBehaviour
{
    private void Start()
    {
        Button button = GetComponent<Button>();

        //ボタンを押下した時のリスナーを設定する
        button.onClick.AddListener(() =>
        {
            //シーン遷移の際にはSceneManagerを使用する
            //SceneManager.LoadScene("MainScene");
            SceneManager.LoadScene("0");
        });
    }
}
