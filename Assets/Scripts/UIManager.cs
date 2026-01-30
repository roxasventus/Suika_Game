using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [SerializeField] private TMP_Text _priceUI;
    public TMP_Text priceUI { get => _priceUI; }
    [SerializeField] private TMP_Text _timerUI;
    public TMP_Text timerUI { get => _timerUI; }

    [SerializeField] private Slider _acceleratorUI;

    [SerializeField] private Slider _distanceUI;

    [SerializeField] private GameObject _ResultUI;
    public GameObject ResultUI { get => _ResultUI; }

    [SerializeField] private TMP_Text _ResultTextUI;
    public TMP_Text ResultTextUI { 
        set => _ResultTextUI = value;
        get => _ResultTextUI; }

    public void setText(TMP_Text textUI, string text) {
        textUI.text = text;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        float maxAccelerateGauge = GameManager.instance.player.maxAccelerateGauge;
        float accelerateGauge = GameManager.instance.player.accelerateGauge;
        _acceleratorUI.value = accelerateGauge/maxAccelerateGauge;

        float maxDistance = GameManager.instance.maxDistance;
        _distanceUI.value = GameManager.instance.player.transform.position.y / maxDistance;
        if (GameManager.instance.player.transform.position.y >= maxDistance) {
            GameManager.instance.isClear = true;
        }
    }
}
