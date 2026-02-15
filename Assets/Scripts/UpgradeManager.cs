using UnityEngine;


public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager instance;

    [SerializeField] private int _credit;
    public int credit
    {
        get => _credit;
    }

    [Header("Level")]

    [SerializeField] private int _widthLevel;
    public int widthLevel
    {
        get => _widthLevel;
        set => _widthLevel = value;
    }

    [SerializeField] private int _heightLevel;
    public int heightLevel
    {
        get => _heightLevel;
        set => _heightLevel = value;
    }

    [SerializeField] private int _accelerateLevel;
    public int accelerateLevel
    {
        get => _accelerateLevel;
        set => _accelerateLevel = value;
    }

    [SerializeField] private int _accelerateFuelLevel;
    public int accelerateFuelLevel
    {
        get => _accelerateFuelLevel;
        set => _accelerateFuelLevel = value;
    }

    [Header("Status")]

    [SerializeField] private float _widthRate;
    public float widthRate
    {
        get => _widthRate;
        set => _widthRate = value;
    }

    [SerializeField] private float _heightRate;
    public float heightRate
    {
        get => _heightRate;
        set => _heightRate = value;
    }

    [SerializeField] private float _accelerateRate;
    public float accelerateRate
    {
        get => _accelerateRate;
        set => _accelerateRate = value;
    }

    [SerializeField] private float _accelerateFuelRate;
    public float accelerateFuelRate
    {
        get => _accelerateFuelRate;
        set => _accelerateFuelRate = value;
    }

    [Header("Cost")]
    [SerializeField] private int[] _upgradePrice;
    public int[] upgradePrice { 
        get => _upgradePrice;
    }


    public void addCredit(int value) { 
        _credit += value;
    }
    public bool useCredit(int value)
    {
        if (credit - value >= 0)
        {
            _credit -= value;
            return true;
        }
        else {
            Debug.LogWarning("크레딧이 부족");
            return false;
        }
    }


    public void initStatus()
    {
        if (UIManager.instance == null) return;
        UIManager.instance.refreshStatus(this);
    }

    public void levelUP(int index) {

        if (index == 1) {

            if (widthLevel < upgradePrice.Length && useCredit(upgradePrice[widthLevel]))
            {
                widthLevel += 1;
                widthRate = 1+ 2.5f * widthLevel / 100f;
            }
            else {
                return;
            }

        }
        else if (index == 2)
        {
            if (heightLevel < upgradePrice.Length && useCredit(upgradePrice[heightLevel]))
            {
                heightLevel += 1;
                heightRate = 1 + 2.5f * heightLevel / 100f;
            }
            else
            {
                return;
            }

        }
        else if (index == 3)
        {
            if (accelerateLevel < upgradePrice.Length && useCredit(upgradePrice[accelerateLevel]))
            {
                accelerateLevel += 1;
                accelerateRate = 1 + 2.5f * accelerateLevel / 100f;
            }
            else
            {
                return;
            }

        }
        else if (index == 4)
        {
            if (accelerateFuelLevel < upgradePrice.Length && useCredit(upgradePrice[accelerateFuelLevel]))
            {
                accelerateFuelLevel += 1;
                accelerateFuelRate = 1 - 2.5f * accelerateFuelLevel / 100f;
            }
            else
            {
                return;
            }

        }
        else {
            Debug.LogWarning("접근되지 않는 레벨업");
        }

        initStatus();
    }

    private void Awake()
    {
        if (instance == null)
        {
            // 인스턴스가 없으면 이 객체를 인스턴스로 설정
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // 인스턴스가 이미 존재하면 새로운 객체를 파괴
            Destroy(gameObject);
        }

    }

    private void OnDestroy()
    {
        if (instance == this) instance = null;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
