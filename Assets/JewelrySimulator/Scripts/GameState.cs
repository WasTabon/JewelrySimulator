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

    [SerializeField] private TextMeshProUGUI _levelText;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        state = State.Default;
    }

    private void Update()
    {
        _levelText.text = level.ToString();
    }
}
