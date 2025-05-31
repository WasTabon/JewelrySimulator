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

    public State state;
    public GemType gemType;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        state = State.Default;
    }
}
