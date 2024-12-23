using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenu; // ָ����ͣ�˵�
    public GameObject DieMenu; // �����˵�
    public GameObject PassMenu; // ͨ�ز˵�
    public GameObject Pass; // ͨ�ع�
    public GameObject fox;

    public FoxMove foxMoveScript;
    public int current_hp;
    private bool isPaused = false;

    void Start()
    {
        Time.timeScale = 1f;
        isPaused = false;
        pauseMenu.SetActive(false); // ��ʼʱ������ͣ�˵�
        DieMenu.SetActive(false); // ��ʼʱ���������˵�
        PassMenu.SetActive(false); // ��ʼʱ����ͨ�ز˵�
    }

    void Update()
    {
        RaycastHit hitInfo;
        Ray ray = new Ray(fox.transform.position, fox.transform.forward); // �ӽ�ɫλ�÷��䣬�����ǽ�ɫǰ��

        if (Physics.Raycast(ray, out hitInfo, 3f)) // ���������������
        {
            // ���������ײ���������Ƿ��ǹ��ع�
            if (hitInfo.collider.name.Contains("ͨ��"))
            {
                // ��ʾͨ�ز˵�
                PassMenu.SetActive(true);
                Time.timeScale = 0f; // ��ͣ��Ϸ
                Debug.Log("ͨ��");
            }
        }
        current_hp = foxMoveScript.current_hp;
        if (current_hp <= 0)
        {
            Time.timeScale = 0f; // ��ͣ��Ϸ
            DieMenu.SetActive(true); // ��ʾ�����˵�
        }
        else
        {
            // �����ͣ��ť
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (isPaused)
                    Resume();
                else
                    Pause();
            }
        }
        
    }

    public void Pause()
    {
        Time.timeScale = 0f; // ��ͣ��Ϸ
        isPaused = true;
        pauseMenu.SetActive(true); // ��ʾ��ͣ�˵�
    }

    public void Resume()
    {
        Time.timeScale = 1f; // �ָ���Ϸ
        isPaused = false;
        pauseMenu.SetActive(false); // ������ͣ�˵�
    }
        
    public void Restart()
    {
        isPaused = false;
        pauseMenu.SetActive(false);
        DieMenu.SetActive(false);
        Time.timeScale = 1f; // �ָ���Ϸ
        // ��ȡ��ǰ���������ֲ����¼���
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        
    }

    public void ExitGame()
    {
        // �˳���Ϸ
        SceneManager.LoadScene("Main", LoadSceneMode.Single);
    }
}
