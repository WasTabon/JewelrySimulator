using DG.Tweening;
using UnityEngine;

public class LavaController : MonoBehaviour
{
    [SerializeField] private Transform _cameraPosForm;
    [SerializeField] private Transform _lava;
    [SerializeField] private RectTransform _nextButtonForm;
    [SerializeField] private Animator _lavaAnimator;

    private void Start()
    {
        _nextButtonForm.DOScale(Vector3.zero, 0f);
        ResetLava();
    }

    private void Update()
    {
        if (GameState.Instance.state != State.Lava)
            return;
        
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
        _lava.DOLocalMoveY(0.09f, 1f)
            .SetEase(Ease.OutQuad)
            .OnComplete((() =>
            {
                _nextButtonForm.DOScale(Vector3.one, 0.5f)
                    .SetEase(Ease.InOutBack);
            }));
    }

    public void HandleCameraForm()
    {
        GameState.Instance.state = State.Form;
        Camera.main.transform.DOMove(_cameraPosForm.position, 1f)
            .SetEase(Ease.InOutSine)
            .OnComplete((() =>
            {
                _lavaAnimator.SetTrigger("OnLava");
                // Доробити анімацію шоб на 3 секунді +-був спавн кольца і після того заново всьо крафтити
            }));
    }
    
    private void ResetLava()
    {
        _lava.DOLocalMoveY(-0.05f, 0f);
    }
}
