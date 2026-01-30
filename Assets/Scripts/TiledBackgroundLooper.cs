using UnityEngine;

public class TiledBackgroundLooperY : MonoBehaviour
{
    [SerializeField] private Transform target;          // 보통 Main Camera 추천
    [SerializeField] private SpriteRenderer spriteRenderer;         // Tiled SpriteRenderer

    private float tileWidth;
    private float tileHeight;

    Sprite sprite;

    // 텍스처 크기 (픽셀)
    float textureWidth;
    float textureHeight;

    // Pixels Per Unit
    float ppu;


    // 한 패턴의 월드 크기
    float patternWidth;
    float patternHeight;


    void Start()
    {
        sprite = spriteRenderer.sprite;

        // 텍스처 크기 (픽셀)
        textureWidth = sprite.texture.width;
        textureHeight = sprite.texture.height;

        // Pixels Per Unit
        ppu = sprite.pixelsPerUnit;

        // 한 패턴의 월드 크기
        patternWidth = textureWidth / ppu;
        patternHeight = textureHeight / ppu;



        // Tiled Size가 반영된 실제 월드 길이
        tileWidth = spriteRenderer.bounds.size.x;
        tileHeight = spriteRenderer.bounds.size.y;

    }

    void LateUpdate()
    {
        if (!GameManager.instance.isPlay)
            return;

        // pivot이 어디든 상관없이 "배경의 실제 중심" 기준으로 비교
        float bgCenterX = spriteRenderer.bounds.center.x;
        float bgCenterY = spriteRenderer.bounds.center.y;

        float deltaX = target.position.x - transform.position.x;
        float deltaY = target.position.y - bgCenterY;

        // 타겟이 배경 중심에서 절반 - patternHeight 이상 벗어나면, 한 타일 높이만큼 재배치

        if (Mathf.Abs(deltaX) >= tileWidth * 0.5f - patternWidth)
        {
            float moveDir = Mathf.Sign(deltaX);

            transform.position += new Vector3(patternWidth * moveDir, 0f, 0f);

            deltaX = 0f;
        }

        if (Mathf.Abs(deltaY) >= tileHeight * 0.5f - patternHeight)
        {
            float moveDir = Mathf.Sign(deltaY);

            transform.position += new Vector3(0f, patternHeight * moveDir, 0f);

            deltaY = 0f;    
        }
    }
}
