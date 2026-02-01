using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    Vector2 move;
    Rigidbody2D rigid;

    [SerializeField] private float _speed = 5f;
    [Header("speed")]
    [SerializeField] float baseSpeed = 5f;

    [Header("acceleration")]
    [SerializeField] float speedUpMultiplier = 1.5f;
    [SerializeField] float acceleration = 10f; // 가속 제한 (클수록 빨리 변함)
    [SerializeField] float _maxAccelerateGauge = 100f;
    public float maxAccelerateGauge { get => _maxAccelerateGauge; }
    [SerializeField] float _accelerateGauge = 100f;   // 가속 게이지
    public float accelerateGauge { 
        set => _accelerateGauge = value;
        get => _accelerateGauge; }
    [SerializeField] float accelerateUse = 25f;      // 초당 소비량
    [SerializeField] float accelerateRegen = 15f;   // 초당 회복량
    [SerializeField] float overheatCooldown = 10f; // 0 도달 시 회복 정지 시간
    [SerializeField] bool _isAccelerating = false;  // 가속 중인가?
    public bool isAccelerating { 
        set => _isAccelerating = value;
        get => _isAccelerating; }
    [SerializeField] bool _isOverhearting = false;  // 과열 중인가?
    public bool isOverhearting { 
        set => _isOverhearting = value;
        get => _isOverhearting; }

    float targetSpeed;
    float currentSpeed;

    public float speed
    {
        get => _speed;
        set => _speed = value;
    }

    private void Awake()
    {
        accelerateGauge = maxAccelerateGauge;
        rigid = GetComponent<Rigidbody2D>();
        //baseSpeed = _speed; // 시작 시 기본 속도 저장

        // ⭐ 시작부터 기본 속도로 움직이도록 초기화
        targetSpeed = baseSpeed;
        currentSpeed = baseSpeed;
    }

    private void Update()
    {
        if (!GameManager.instance.isPlay) return;

        // 과열 중이면: 게이지 회복/소모 둘 다 막기 (원하면 회복은 쿨타임 후에만)
        if (isOverhearting)
            return;

        if (isAccelerating)
        {
            accelerateGauge -= accelerateUse * Time.deltaTime;
            accelerateGauge = Mathf.Max(0f, accelerateGauge);

            if (accelerateGauge <= 0f)
            {
                // 강제 가속 해제
                isAccelerating = false;
                targetSpeed = baseSpeed;

                // 코루틴 중복 실행 방지
                if (!isOverhearting)
                    StartCoroutine(overHeatingCooltimeCoroutine());
            }
        }
        else
        {
            accelerateGauge += accelerateRegen * Time.deltaTime;
            accelerateGauge = Mathf.Min(accelerateGauge, maxAccelerateGauge);
        }
    }



    public void OnMove(InputAction.CallbackContext ctx)
    {
        if (GameManager.instance.isPlay)
        {
            if (ctx.performed)
                move = ctx.ReadValue<Vector2>();
            else if (ctx.canceled)
                move = Vector2.zero;
        }
        else {
            move = Vector2.zero;
        }

    }

    public void OnSpeedUp(InputAction.CallbackContext ctx)
    {
        if (!GameManager.instance.isPlay)
        {
            isAccelerating = false;
            targetSpeed = baseSpeed;
            return;
        }

        if (ctx.performed && accelerateGauge > 0f)
        {
            isAccelerating = true;
            targetSpeed = baseSpeed * speedUpMultiplier;
        }
        else if (ctx.canceled)
        {
            isAccelerating = false;
            targetSpeed = baseSpeed;
        }
    }


    private void FixedUpdate()
    {
        // ⭐ 현재 속도를 목표 속도로 서서히 보간
        currentSpeed = Mathf.MoveTowards(
            currentSpeed,
            targetSpeed,
            acceleration * Time.fixedDeltaTime
        );

        Vector2 nextVec = move.normalized * currentSpeed * Time.fixedDeltaTime;
        rigid.MovePosition(rigid.position + nextVec);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Objects"))
        {
            float price = collision.GetComponent<Object>().price;
            GameManager.instance.addPrice(price);
            UIManager.instance.setText(UIManager.instance.priceUI, "Price : " + GameManager.instance.totalPrice);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Objects"))
        {
            float price = collision.GetComponent<Object>().price;
            GameManager.instance.addPrice(-price);
            UIManager.instance.setText(UIManager.instance.priceUI, "Price : " + GameManager.instance.totalPrice);
        }
    }

    IEnumerator overHeatingCooltimeCoroutine()
    {
        isOverhearting = true;
        AudioManager.instance.SetSFX("beep_warning");
        UIManager.instance.overheatText.gameObject.SetActive(true);

        // 여기서도 안전하게 강제 해제
        isAccelerating = false;
        targetSpeed = baseSpeed;

        yield return new WaitForSeconds(overheatCooldown);

        isOverhearting = false;
        UIManager.instance.overheatText.gameObject.SetActive(false);

        // (선택) 쿨타임 끝나면 최소 게이지 조금 채워주기
        // accelerateGauge = Mathf.Max(accelerateGauge, 5f);
    }

}
