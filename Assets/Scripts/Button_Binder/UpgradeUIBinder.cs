
using UnityEngine;
using UnityEngine.UI;

public class UpgradeUIBinder : MonoBehaviour
{
    [SerializeField] Button btn;
    [SerializeField] int index;

    private void Start()
    {
        if(index == 0)
            btn.onClick.AddListener(() => UpgradeManager.instance.initStatus());
        else
            btn.onClick.AddListener(() => UpgradeManager.instance.levelUP(index));
    }
}