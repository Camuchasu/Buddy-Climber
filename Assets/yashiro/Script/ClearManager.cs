using UnityEngine;
using UnityEngine.SceneManagement;

public class ClearManager : MonoBehaviour
{
    [SerializeField] GameObject clearUI;

    public void Clear()
    {
        clearUI.SetActive(true);

        Time.timeScale = 0f;
    }


    public void ReturnTitle()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene("Title");
    }
}