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

            // ⭐ 위험근무 수당 지급
            string envelopeName;
            int hazardPay = GiveHazardPay(out envelopeName);

            // TODO: 나중에 로그라이트 전용 재화로 분리 가능
            _totalPrice += hazardPay;

            string moneyText =
                $"\n\n<size=120%><color=#FFD700>{envelopeName}</color></size>" +
                $"\n위험근무 수당 +{hazardPay:N0}";

            result = $"{title}\n\n{body}{moneyText}";
            UpgradeManager.instance.addCredit(hazardPay);
        }
        else
        {
            string title =
                "<size=160%><color=#FF5555>게임 오버</color></size>";

            string body =
                failText[UnityEngine.Random.Range(0, failText.Length)];

            // ❌ 보상 없음, 연출만
            string envelopeName = "지급 보류 통지";
            string moneyText;

            if (isClear && totalPrice < _clearPrice)
            {
                moneyText =
                    $"\n\n<size=120%><color=#AAAAAA>{envelopeName}</color></size>" +
                    $"\n※ 실적 미달로, 수당 지급은 보류됩니다.";
            }
            else 
            {
                moneyText =
                    $"\n\n<size=120%><color=#AAAAAA>{envelopeName}</color></size>" +
                    $"\n※ 본 근무는 사고 처리되었습니다.";
            }

            result = $"{title}\n\n{body}{moneyText}";
        }

        UIManager.instance.ResultTextUI.text = result.Replace("\\n", "\n");
    }

    int GiveHazardPay(out string envelopeName)
    {
        double excess = Math.Max(0, _totalPrice - _clearPrice);

        const double rate = 0.03; // ⭐ 초과분의 3%를 수당으로 지급 (추천)
        int reward = (int)Math.Floor(excess * rate);

        // 연출용 봉투 이름(액수에 따라 등급만 보여주기)
        if (reward <= 0)
            envelopeName = "지급 보류 통지";
        else if (reward < 150)
            envelopeName = "쥐꼬리 봉투";
        else if (reward < 300)
            envelopeName = "기본 봉투";
        else if (reward < 500)
            envelopeName = "두툼 봉투";
        else if (reward < 900)
            envelopeName = "위험수당 특봉";
        else
            envelopeName = "입막음 봉투";

        return reward;
    }

    /*
    int GiveHazardPay(out string envelopeName)
    {
        float r = UnityEngine.Random.value; // 0.0 ~ 1.0
        int reward;

        if (r < 0.45f)
        {
            envelopeName = "쥐꼬리 봉투";
            reward = 120;
        }
        else if (r < 0.75f) // 0.45 + 0.30
        {
            envelopeName = "기본 봉투";
            reward = 180;
        }
        else if (r < 0.90f) // +0.15
        {
            envelopeName = "두툼 봉투";
            reward = 280;
        }
        else if (r < 0.98f) // +0.08
        {
            envelopeName = "위험수당 특봉";
            reward = 420;
        }
        else
        {
            envelopeName = "입막음 봉투";
            reward = 900;
        }

        return reward;
    }
    */
}
