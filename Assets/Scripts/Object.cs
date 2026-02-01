using UnityEngine;

public class Object : MonoBehaviour
{
    [Header("Status")]
    [SerializeField] private MinalData _minaldata;

    [SerializeField] private int _level;
    public int getLevel { get => _level; }

    [SerializeField] private string _name;
    public string name { get => _name; }

    [SerializeField] private float _price;
    public float price { get => _price; }

    // 상자 안에 있느냐??
    [SerializeField] private bool _boxIn;
    public bool boxIn { 
        set => _boxIn = value;
        get => _boxIn; }

    // 오브젝트 충돌시 중복 생성을 막기 위한 Lock
    [SerializeField] private bool _lock;
    public bool getLock { get => _lock; }
    public void setLock(bool state) { _lock = state; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _level = _minaldata.level;
        _name = _minaldata.name;
        _price = _minaldata.price;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Objects")) {
            return;
        }

        Object collisionObject = collision.gameObject.GetComponent<Object>();
        int collisionLevel = collisionObject.getLevel;

        if (collisionLevel != getLevel ||  collisionObject.getLock == true) {
            return;
        }

        setLock(true);
        collisionObject.setLock(true);


        if (PoolManager.instance.prefabs.Length > getLevel + 1 && 5 >= getLevel + 1) { 
            PoolManager.instance.Get(getLevel+1, transform);
            AudioManager.instance.SetSFX("combine");
            gameObject.SetActive(false);
            collisionObject.gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            _boxIn = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            _boxIn = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 vp = Camera.main.WorldToViewportPoint(transform.position);

        bool isOut =
            vp.x < 0 || vp.x > 1 ||
            vp.y < 0 ; 

        if (isOut && boxIn == false)
        {
            gameObject.SetActive(false);
            setLock(false);
        }
    }
}
