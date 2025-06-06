using DG.Tweening;
using UnityEngine;

public class EraseDirt : MonoBehaviour
{
    public GameObject tutorialPanel;
    
    [SerializeField] private AudioClip _cleanSoundClip;

    private float _soundCooldown = 0.5f;
    private float _soundTimer = 0f;
    
    [SerializeField] private RectTransform _nextButton;
    
    public Camera cam;
    public RenderTexture maskTexture;
    public Material eraseMaterial;
    public float brushSize = 0.1f;

    private int wasTutorial;
    
    private bool _hasScaledNextButton = false;
    
    private void Start()
    {
        _nextButton.DOScale(Vector3.zero, 0f);
        RenderTexture.active = maskTexture;
        GL.Clear(true, true, Color.white);
        RenderTexture.active = null;

        wasTutorial = PlayerPrefs.GetInt("clean", 0);
    }
   
    private void Update()
    {
        if (GameState.Instance.state != State.Clean)
            return;

        if (wasTutorial == 0)
        {
            wasTutorial = 1;
            PlayerPrefs.SetInt("clean", 1);
            PlayerPrefs.Save();
            tutorialPanel.SetActive(true);
        }
    
        bool inputHeld = false;
        Vector2 inputPos = Vector2.zero;
    
    #if UNITY_EDITOR || UNITY_STANDALONE
        inputHeld = Input.GetMouseButton(0);
        inputPos = Input.mousePosition;
    #else
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            inputHeld = touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary;
            inputPos = touch.position;
        }
    #endif
    
        _soundTimer -= Time.deltaTime;
        
        if (inputHeld)
        {
            if (!_hasScaledNextButton)
            {
                _hasScaledNextButton = true;
                _nextButton.DOScale(Vector3.one, 0.5f)
                    .SetEase(Ease.InOutBack);
            }
    
            Ray ray = cam.ScreenPointToRay(inputPos);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Vector2 uv = hit.textureCoord;
                DrawAtUV(uv);
                if (_soundTimer <= 0f)
                {
                    float randomPitch = Random.Range(0.85f, 1.2f);
                    MusicController.Instance.PlaySpecificSound(_cleanSoundClip, randomPitch);
                    _soundTimer = _soundCooldown;
                }
            }
        }
        else
        {
            _hasScaledNextButton = false;
        }
    }

    public void HandleStateNextCut()
    {
        _nextButton.DOScale(Vector3.zero, 0.5f)
            .SetEase(Ease.InOutBack);
        GameState.Instance.state = State.Cut;
    }
    
    public void ResetEraseMask()
    {
        RenderTexture.active = maskTexture;
        GL.Clear(true, true, Color.white);
        RenderTexture.active = null;
    }
    
    private void DrawAtUV(Vector2 uv)
    {
        RenderTexture.active = maskTexture;

        eraseMaterial.SetVector("_ErasePos", new Vector4(uv.x, uv.y, 0, 0));
        eraseMaterial.SetFloat("_EraseSize", brushSize);

        GL.PushMatrix();
        GL.LoadOrtho();

        eraseMaterial.SetPass(0);

        float size = brushSize * 0.5f;
        float x = uv.x;
        float y = uv.y;

        GL.Begin(GL.QUADS);
        GL.TexCoord2(0, 0); GL.Vertex3(x - size, y - size, 0);
        GL.TexCoord2(1, 0); GL.Vertex3(x + size, y - size, 0);
        GL.TexCoord2(1, 1); GL.Vertex3(x + size, y + size, 0);
        GL.TexCoord2(0, 1); GL.Vertex3(x - size, y + size, 0);
        GL.End();

        GL.PopMatrix();

        RenderTexture.active = null;
    }
}