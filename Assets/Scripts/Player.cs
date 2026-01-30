using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    Vector2 move;
    Rigidbody2D rigid;

    [SerializeField] private float _speed = 5f;

    [SerializeField] float baseSpeed = 5f;
    [SerializeField] float speedUpMultiplier = 1.5f;
    [SerializeField] float acceleration = 10f; // 가속 제한 (클수록 빨리 변함)

    float targetSpeed;
    float currentSpeed;

    public float speed
    {
        get => _speed;
        set => _speed = value;
    }

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        //baseSpeed = _speed; // 시작 시 기본 속도 저장

        // ⭐ 시작부터 기본 속도로 움직이도록 초기화
        targetSpeed = baseSpeed;
        currentSpeed = baseSpeed;
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
            targetSpeed = baseSpeed;
            return;
        }

        if (ctx.performed)
        {
            targetSpeed = baseSpeed * speedUpMultiplier;
        }
        else if (ctx.canceled)
        {
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
}
