using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [SerializeField] bool _isPlay;
    public bool isPlay { get => _isPlay; }

    [SerializeField] Player _player;
    public Player player { get => _player; }

    [SerializeField] float _time;
    public float time { get => _time; }

    [SerializeField] double _totalPrice;
    public double totalPrice { get => _totalPrice; }

    public void addPrice(double price) { _totalPrice += price; }

    [SerializeField] private GameObject _exitBackground;

    private void Awake()
    {
        instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(SceneManager.GetActiveScene().name == "GameScene")
            AudioManager.instance.SetBGM("BGM1");
    }

    // Update is called once per frame
    void Update()
    {
        if (_isPlay) {
            if (_time < 1)
            {
                _time = 0;
                _isPlay = false;
                UpdateTimerUI(time);
                _exitBackground.gameObject.SetActive(true);
                StartCoroutine(exitCoroutine());
            }
            else {
                _time -= Time.deltaTime;
                UpdateTimerUI(time);
            }
        }

    }

    void UpdateTimerUI(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);


        UIManager.instance.setText(UIManager.instance.timerUI, "Time : " + $"{minutes:00}:{seconds:00}");

    }

    IEnumerator exitCoroutine()
    {
        float speed = 3f;   // 이동 속도

        Vector3 targetPos = new Vector3(
            _exitBackground.transform.position.x,
            _exitBackground.transform.position.y,
            player.transform.position.z
        );

        while (Vector3.Distance(player.transform.position, targetPos) > 0.01f)
        {
            player.transform.position =
                Vector3.MoveTowards(
                    player.transform.position,
                    targetPos,
                    speed * Time.deltaTime
                );

            yield return null; // 프레임마다 양보
        }

        // 마무리로 정확히 맞추기
        player.transform.position = targetPos;
        ResultGame();
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f; // ⭐ 일시정지 상태였다면 반드시 복구
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void ExitGame()
    {
        Time.timeScale = 1f; // ⭐ 일시정지 상태였다면 반드시 복구
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
                Application.Quit(); // 어플리케이션 종료
        #endif
    }

    public void ResultGame() { 
        UIManager.instance.ResultUI.gameObject.SetActive( true );

        UIManager.instance.ResultTextUI.text = "Game Clear";
    
    }

}
