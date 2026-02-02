using UnityEngine;

[CreateAssetMenu(fileName = "MinalData", menuName = "Scriptable Objects/MinalData")]
public class MinalData : ScriptableObject
{
    [SerializeField] private int _level;
    public int level { get => _level; }

    [SerializeField] private string _name;

    public string name { get => _name; }

    [SerializeField] private float _price;
    public float price { get => _price; }

    [SerializeField] private float _maxFallSpeed; // 아래로 떨어지는 최대 속도(절댓값)
    public float maxFallSpeed { get => _maxFallSpeed; }
}
