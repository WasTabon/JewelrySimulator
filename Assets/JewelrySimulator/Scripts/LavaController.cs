using DG.Tweening;
using UnityEngine;

public class LavaController : MonoBehaviour
{
    public GameObject tutorialPanel;
    private int wasTutorial;
    
    [SerializeField] private Transform _cameraTransform;
    [SerializeField] private Transform _cameraPosForm;
    [SerializeField] private Transform _lava;
    [SerializeField] private RectTransform _nextButtonForm;
    [SerializeField] private Animator _lavaAnimator;
    [SerializeField] private GameObject _createPanel;

    private void Start()
    {
        _nextButtonForm.DOScale(Vector3.zero, 0f);
        ResetLava();
        
        wasTutorial = PlayerPrefs.GetInt("lava", 0);
    }

    private void Update()
    {
        if (GameState.Instance.state != State.Lava)
            return;
        
        if (wasTutorial == 0)
        {
            wasTutorial = 1;
            PlayerPrefs.SetInt("lava", 1);
            PlayerPrefs.Save();
            tutorialPanel.SetActive(true);
        }
        
        if (Input.GetMouseButtonDown(0))
        {
            DetectClick(Input.mousePosition);
        }
        
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            DetectClick(Input.GetTouch(0).position);
        }
    }
    
    private void DetectClick(Vector3 screenPosition)
        {
            Ray ray = Camera.main.ScreenPointToRay(screenPosition);
            RaycastHit hit;
    
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.CompareTag("GoldRed"))
                {
                    HandleLavaMovement();
                }
                else if (hit.collider.CompareTag("GoldBlue"))
                {
                    HandleLavaMovement();
                }
                else if (hit.collider.CompareTag("GoldGreen"))
                {
                    HandleLavaMovement();
                }
            }
        }

    private void HandleLavaMovement()
    {
        MusicController.Instance.PlayClickSound();
        Debug.Log("Hadnle movement");
        _lava.DOLocalMoveY(0.09f, 1f)
            .SetEase(Ease.OutQuad)
            .OnComplete((() =>
            {
                Debug.Log("Finished");
                _nextButtonForm.DOScale(Vector3.one, 0.5f)
                    .SetEase(Ease.InOutBack);
            }));
    }

    public void HandleCameraForm()
    {
        GameState.Instance.state = State.Form;
        
        _nextButtonForm.DOScale(Vector3.zero, 0f);
        
        _cameraTransform.DOMove(_cameraPosForm.position, 1f)
            .SetEase(Ease.InOutSine)
            .OnComplete((() =>
            {
                _lavaAnimator.SetTrigger("OnLava");
                ResetLava();
                Invoke("ShowCreate", 5f);
            }));
    }

    private void ShowCreate()
    {
        _createPanel.SetActive(true);
    }
    
    private void ResetLava()
    {
        _lava.DOLocalMoveY(-0.055f, 0f);
    }
}
