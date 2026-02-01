using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class HowtoPlay : MonoBehaviour
{
    [SerializeField] string[] explainContents;
    [SerializeField] private TMP_Text contentTextUI;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button previousButton;
    [SerializeField] private GameObject keyUI;
    [SerializeField] private Image oreUI;
    [SerializeField] private Image dynamiteUI;
    int contentIndex = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnEnable()
    {
        contentIndex = 0;
        contentTextUI.text = explainContents[contentIndex];
        keyUI.gameObject.SetActive(false);
        oreUI.gameObject.SetActive(false);
        previousButton.gameObject.SetActive(false);
        nextButton.gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
           
    }

    public void OnIndex(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            float axisValue = ctx.ReadValue<float>();

            if (axisValue > 0)
            {
                indexIncrease(1);
            }
            else if (axisValue < 0) {
                indexIncrease(-1);
            }
        }


    }

    public void indexIncrease(int num) {

        if (explainContents.Length > contentIndex + num && 0 <= contentIndex + num)
        {
            contentIndex+=num;
            contentTextUI.text = explainContents[contentIndex];
            if (explainContents.Length == contentIndex + 1)
                nextButton.gameObject.SetActive(false);
            else if (contentIndex == 0)
                previousButton.gameObject.SetActive(false);
            else {
                nextButton.gameObject.SetActive(true);
                previousButton.gameObject.SetActive(true);
            }

            if (contentIndex == 3)
                keyUI.gameObject.SetActive(true);
            else
                keyUI.gameObject.SetActive(false);

            if (contentIndex == 4)
                oreUI.gameObject.SetActive(true);
            else
                oreUI.gameObject.SetActive(false);

            if (contentIndex == 5)
                dynamiteUI.gameObject.SetActive(true);
            else
                dynamiteUI.gameObject.SetActive(false);


        }
    }
}
