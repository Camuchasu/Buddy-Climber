using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class StartButton : MonoBehaviour
{
    private void Start()
    {
        Button button = GetComponent<Button>();

        button.onClick.AddListener(() =>
        {
            //SceneManager.LoadScene("MainScene");
            SceneManager.LoadScene("0");
        });
    }
}
