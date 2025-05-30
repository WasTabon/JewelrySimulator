using UnityEngine;

public enum State
{
    Default,
    Clean,
    Cut
}

public class GameState : MonoBehaviour
{
    public static GameState Instance;

    public State state;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        state = State.Default;
    }
}
