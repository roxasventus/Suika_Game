using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [SerializeField] bool _isPlay;
    public bool isPlay { get => _isPlay; }

    [SerializeField] bool _isClear;
    public bool isClear { 
        set => _isClear = value; 
        get => _isClear; }

    [SerializeField] Player _player;
    public Player player { get => _player; }

    [SerializeField] float _maxDistance;  // 목표 거리
    public float maxDistance { get => _maxDistance; }

    [SerializeField] float _distance;  // 현재 거리
    public float distance { get => _distance; }

    [SerializeField] float _time;
    public float time { get => _time; }

    [SerializeField] double _clearPrice = 10000;

    [SerializeField] double _totalPrice;
    public double totalPrice { get => _totalPrice; }

    public void addPrice(double price) { _totalPrice += price; }

    [SerializeField] private GameObject _exitBackground;


    [SerializeField] private string[] clearText;
    [SerializeField] private string[] failText;


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

        if (_isClear && _isPlay )
        {
            _time = 0;
            _isPlay = false;
            UpdateTimerUI(time);
            gameClear();
        }

        else {
            if (_isPlay)
            {
                if (_time < 1)
                {
                    _time = 0;
                    _isPlay = false;
                    UpdateTimerUI(time);
                    Debug.Log("fail");
                    ResultGame();
                }
                else
                {
                    _time -= Time.deltaTime;
                    UpdateTimerUI(time);
                }
            }

        }

    }

    void UpdateTimerUI(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);


        UIManager.instance.setText(UIManager.instance.timerUI, "Time : " + $"{minutes:00}:{seconds:00}");

    }

    public void gameClear() {
        _exitBackground.gameObject.SetActive(true);
        StartCoroutine(exitCoroutine());
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

    public void ResultGame()
    {
        Time.timeScale = 0f;

        UIManager.instance.ResultUI.gameObject.SetActive(true);

        string result;

        if (isClear && totalPrice >= _clearPrice)
        {
            string title =
                "<size=160%><color=#FFD700>게임 클리어</color></size>";

            string body =
                clearText[UnityEngine.Random.Range(0, clearText.Length)];

            result = $"{title}\n\n{body}";
        }
        else
        {
            string title =
                "<size=160%><color=#FF5555>게임 오버</color></size>";

            string body =
                failText[UnityEngine.Random.Range(0, failText.Length)];

            result = $"{title}\n\n{body}";
        }

        UIManager.instance.ResultTextUI.text = result.Replace("\\n", "\n");
    }


}
