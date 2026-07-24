using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class StartButton : MonoBehaviour
{
    private void Start()
    {
        Button button = GetComponent<Button>();

        //�{�^���������������̃��X�i�[��ݒ肷��
        button.onClick.AddListener(() =>
        {
            //�V�[���J�ڂ̍ۂɂ�SceneManager���g�p����
            //SceneManager.LoadScene("MainScene");
            SceneManager.LoadScene("BuddyClimer");
        });
    }
}
