using System;
using TMPro;
using UnityEngine;

public enum State
{
    Default,
    Clean,
    Cut,
    Lava,
    Form
}

public enum GemType
{
    Red,
    Blue,
    Green
}

public class GameState : MonoBehaviour
{
    public static GameState Instance;

    public int money;
    public int level = 1;
    
    public State state;
    public GemType gemType;

    [SerializeField] private TextMeshProUGUI _moneyText;
    [SerializeField] private TextMeshProUGUI _levelText;

    private void Awake()
    {
        Instance = this;
        LoadLevelMoney();
    }

    private void Start()
    {
        state = State.Default;
    }

    private void Update()
    {
        _moneyText.text = money.ToString();
        _levelText.text = level.ToString();
    }

    public void UpgradeLevel()
    {
        if (money >= 500)
        {
            money -= 500;
            level++;
            PlayerPrefs.SetInt("money", money);
            PlayerPrefs.SetInt("level", money);
            PlayerPrefs.Save();
        }
    }

    public void AddMoney(int amount)
    {
        money += amount;
        PlayerPrefs.SetInt("money", money);
        PlayerPrefs.Save();
    }

    public void LoadLevelMoney()
    {
        money = PlayerPrefs.GetInt("money", 0);
        level = PlayerPrefs.GetInt("level", 0);
    }
}
