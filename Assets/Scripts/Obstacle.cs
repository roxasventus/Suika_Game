using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Obstacle : MonoBehaviour
{
    [SerializeField] private int _level;
    public int getLevel => _level;

    [SerializeField] private AudioSource _audioSource;
    public AudioSource audioSource{ get => _audioSource; }

    // 상자 안에 있느냐??
    [SerializeField] private bool _boxIn;
    public bool boxIn
    {
        set => _boxIn = value;
        get => _boxIn;
    }

    [Header("Explosion Settings")]
    [SerializeField] private float _timer = 1.0f;
    public float timer => _timer;

    [SerializeField] private bool _isExploding;
    public bool isExploding => _isExploding;

    [SerializeField] private float radius = 5f;
    [SerializeField] private float explosionForce = 10f;
    //[SerializeField] private LayerMask affectedLayers; // 광물 레이어만 넣는 걸 추천

    private Coroutine _explodeRoutine;

    [SerializeField] private GameObject warningUI;

    // Update is called once per frame
    void Update()
    {
        Vector3 vp = Camera.main.WorldToViewportPoint(transform.position);

        bool isOut =
            vp.x < 0 || vp.x > 1 ||
            vp.y < 0 || 
            vp.z < 0; // 카메라 뒤쪽

        if (isOut && boxIn == false)
        {
            gameObject.SetActive(false);
        }

        if (vp.y > 1 && isExploding == false)
        {

            if (!warningUI.activeSelf)
            {
                AudioManager.instance.SetSFX(audioSource, "warning");
                warningUI.SetActive(true);
            }
        }
        else {

            if (warningUI.activeSelf)
            {
                warningUI.SetActive(false);
            }

        }
    }

    void LateUpdate()
    {
        // 다이너마이트 경고 ui 위치 배치
        if (_level == 0 && warningUI.activeSelf) {
            Vector3 dynPos = gameObject.transform.position;
            Camera cam = UIManager.instance.main_camera;
            float topOffset = 0.5f; // 화면 상단에서 살짝 아래

            // 화면 최상단 (Viewport Y = 1)
            Vector3 topWorldPos = cam.ViewportToWorldPoint(
                new Vector3(0.5f, 1f, cam.nearClipPlane)
            );

            warningUI.transform.position = new Vector3(
                dynPos.x,
                topWorldPos.y - topOffset,
                transform.position.z
            );

            warningUI.transform.rotation = Quaternion.identity;
        }
    }

    private void OnEnable()
    {
        _boxIn = false;
        _isExploding = false;
        _explodeRoutine = null;
    }

    private void OnDisable()
    {
        if (_explodeRoutine != null)
        {
            StopCoroutine(_explodeRoutine);
            _explodeRoutine = null;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 이미 점화/폭발 중이면 중복 방지
        if (_isExploding) return;

        // 상자(플레이어) 안에 들어왔을 때 점화
        if (collision.CompareTag("Player"))
        {
            _boxIn = true;
            _isExploding = true; // ✅ 코루틴 시작 순간부터 잠금(중복 코루틴 방지)
            _explodeRoutine = StartCoroutine(ExplodeCoroutine());
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            _boxIn = false;
        }
    }

    private IEnumerator ExplodeCoroutine()
    {
        AudioManager.instance.SetSFX(audioSource, "timer");
        yield return new WaitForSeconds(_timer);

        Vector2 center = transform.position;

        // ✅ 레이어 마스크로 1차 필터링 (성능/정확도 ↑)
        Collider2D[] colliders = Physics2D.OverlapCircleAll(center, radius);

        foreach (var col in colliders)
        {
            // 태그로 2차 필터(원하면 레이어만 써도 됨)
            if (!col.CompareTag("Objects")) continue;

            Rigidbody2D rb = col.attachedRigidbody;
            if (rb == null) continue;
            if (rb.bodyType != RigidbodyType2D.Dynamic) continue;

            Vector2 toTarget = (Vector2)col.transform.position - center;
            float dist = toTarget.magnitude;
            if (dist <= 0.0001f) continue;

            // ✅ 방향 정규화
            Vector2 dir = toTarget / dist;

            // ✅ 가까울수록 강, 멀수록 약 (거리 감쇠)
            float t = Mathf.Clamp01(1f - (dist / radius)); // 0~1
            float force = explosionForce * t;

            // ✅ 위로 살짝 보정(원치 않으면 0으로)
            Vector2 launchDir = (dir + Vector2.up * 1.25f).normalized;

            rb.AddForce(launchDir * force, ForceMode2D.Impulse);
        }
        if(gameObject.activeSelf)
            AudioManager.instance.SetSFX(audioSource, "explosion");
        gameObject.transform.GetChild(0).gameObject.SetActive(false);
        gameObject.transform.GetChild(1).gameObject.SetActive(true);

        yield return new WaitForSeconds(1f);
        gameObject.transform.GetChild(0).gameObject.SetActive(true);
        gameObject.transform.GetChild(1).gameObject.SetActive(false);
        _boxIn = false;
        gameObject.SetActive(false);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
