using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [SerializeField] private Camera _main_camera;
    public Camera main_camera { get => _main_camera; }

    [SerializeField] private TMP_Text _priceUI;
    public TMP_Text priceUI { get => _priceUI; }
    [SerializeField] private TMP_Text _timerUI;
    public TMP_Text timerUI { get => _timerUI; }

    [SerializeField] private Slider _acceleratorUI;

    [SerializeField] private Slider _distanceUI;

    [SerializeField] private GameObject _MenuUI;
    public GameObject MenuUI { get => _MenuUI; }

    [SerializeField] private GameObject _ResultUI;
    public GameObject ResultUI { get => _ResultUI; }

    [SerializeField] private TMP_Text _ResultTextUI;
    public TMP_Text ResultTextUI { 
        set => _ResultTextUI = value;
        get => _ResultTextUI; }

    public void setText(TMP_Text textUI, string text) {
        textUI.text = text;
    }

    [SerializeField] TMP_Text _overheatText;   // 과열 UI 텍스트
    public TMP_Text overheatText { get => _overheatText; }
    [SerializeField] float blinkSpeed = 2f; // 깜빡임 속도 (클수록 빠름)

    float blinkTimer;

    private void Awake()
    {
        if (SceneManager.GetActiveScene().name == "GameScene")
        {
            GameManager.instance.player.GetComponent<PlayerInput>().actions.FindActionMap("UI").Enable();
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (SceneManager.GetActiveScene().name == "GameScene")
        {
            float maxAccelerateGauge = GameManager.instance.player.maxAccelerateGauge;
            float accelerateGauge = GameManager.instance.player.accelerateGauge;
            _acceleratorUI.value = accelerateGauge / maxAccelerateGauge;

            float maxDistance = GameManager.instance.maxDistance;
            _distanceUI.value = GameManager.instance.player.transform.position.y / maxDistance;
            if (GameManager.instance.player.transform.position.y >= maxDistance)
            {
                GameManager.instance.isClear = true;
            }

            if (GameManager.instance.player.isOverhearting)
            {
                BlinkOverheatUI();
            }
            else
            {
                ResetOverheatUI();
            }
        }
    }

    void BlinkOverheatUI()
    {
        blinkTimer += Time.deltaTime * blinkSpeed;

        // 0~1 왕복 값
        float t = Mathf.PingPong(blinkTimer, 1f);

        // R값을 155 ~ 255 사이로 보간
        float r = Mathf.Lerp(155f, 255f, t) / 255f;

        Color c = overheatText.color;
        c.r = r;
        overheatText.color = c;
    }

    void ResetOverheatUI()
    {
        blinkTimer = 0f;

        Color c = overheatText.color;
        c.r = 255f / 255f;
        overheatText.color = c;
    }


    public void OnEsc(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {

            float axisValue = ctx.ReadValue<float>();
            RectTransform rectTransform = MenuUI.GetComponent<RectTransform>();

            if (!MenuUI.activeSelf)
            {
                Time.timeScale = 0;
                MenuUI.SetActive(true);
                rectTransform.GetChild(0).gameObject.SetActive(true);
            }
            else
            {
                for (int i = 0; i < rectTransform.childCount-1; i++)
                {
                    if (rectTransform.GetChild(i).gameObject.activeSelf) {
                        rectTransform.GetChild(i).gameObject.SetActive(false);
                    }
                }
                Time.timeScale = 1;
                MenuUI.SetActive(false);
            }

        }


    }

}
