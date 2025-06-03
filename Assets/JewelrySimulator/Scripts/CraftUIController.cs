using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class CraftUIController : MonoBehaviour
{
    [SerializeField] private RectTransform _rewardPanel;
    [SerializeField] private Transform _cameraTransform;
    [SerializeField] private Transform _cameraCraftPos;
    [SerializeField] private Transform _cameraMainPos;
    [SerializeField] private Transform _spawnPos;

    [SerializeField] private GameObject _craftPanel;
    
    [SerializeField] private GameObject _crown;
    [SerializeField] private GameObject _pendant;
    [SerializeField] private GameObject _ring;
    
    [SerializeField] private Image _circleLeft;
    [SerializeField] private Image _squareLeft;
    [SerializeField] private Image _triangleLeft;

    [SerializeField] private Image _circleRight;
    [SerializeField] private Image _squareRight;
    [SerializeField] private Image _triangleRight;
    
    [SerializeField] private Image _resultCrown;
    [SerializeField] private Image _resultPendant;
    [SerializeField] private Image _resultRing;
    
    [SerializeField] private Image _circle;
    [SerializeField] private Image _square;
    [SerializeField] private Image _triangle;

    public RectTransform canvas;
    public TextMeshProUGUI text;
    
    private GameObject _spawned;
    
    private int _currentIndex = 1;
    private bool _isRight;

    private void Start()
    {
        _rewardPanel.DOScale(Vector3.zero, 0f);
    }

    private void Update()
    {
        string left = GetActiveShape(_circleLeft, _squareLeft, _triangleLeft);
        string right = GetActiveShape(_circleRight, _squareRight, _triangleRight);

        if ((left == "circle" && right == "square") || (left == "square" && right == "circle"))
        {
            ShowResult(_resultPendant);
        }
        else if ((left == "circle" && right == "triangle") || (left == "triangle" && right == "circle"))
        {
            ShowResult(_resultCrown);
        }
        else if (left == "circle" && right == "circle")
        {
            ShowResult(_resultRing);
        }
        else
        {
            ShowResult(null);
        }
    }

    public void SetCurrentImageLeft()
    {
        _isRight = false;
    }
    
    public void SetCurrentImageRight()
    {
        _isRight = true;
    }

    public void Spawn()
    {
        if (!_resultCrown.gameObject.activeSelf && !_resultPendant.gameObject.activeSelf && !_resultRing.gameObject.activeSelf)
            return;
        
        _craftPanel.gameObject.SetActive(false);
        Sequence sequence = DOTween.Sequence();
      
        sequence.Join(_cameraTransform.DOMove(_cameraCraftPos.position, 1f)
            .SetEase(Ease.InOutSine));
      
        sequence.Join(_cameraTransform.DORotate(_cameraCraftPos.eulerAngles, 1f)
            .SetEase(Ease.InOutSine));

        sequence.OnComplete(() =>
        {
            DOTween.Sequence()
                .AppendInterval(0.5f)
                .AppendCallback(() =>
                {
                    if (_resultCrown.gameObject.activeSelf)
                    {
                        _spawned = Instantiate(_crown, _spawnPos.position, Quaternion.identity);
                        Invoke("SellItem", 0.5f);
                    }
                    else if (_resultPendant.gameObject.activeSelf)
                    {
                        _spawned = Instantiate(_pendant, _spawnPos.position, Quaternion.identity);
                        Invoke("SellItem", 0.5f);
                    }
                    else if (_resultRing.gameObject.activeSelf)
                    {
                        _spawned = Instantiate(_ring, _spawnPos.position, Quaternion.identity);
                        Invoke("SellItem", 0.5f);
                    }
                    else
                    {
                        _spawned = Instantiate(_crown, _spawnPos.position, Quaternion.identity);
                        Invoke("SellItem", 0.5f);
                    }
                });
        });
    }

    private void SellItem()
    {
        int random = Random.Range(0, 3);
        switch (random)
        {
            case 0:
                text.text = "This is great. Thanks!";
                break;
            case 1:
                text.text = "Not bad. Thanks";
                break;
            case 2:
                text.text = "This is awful";
                break;
            default:
                text.text = "This is great. Thanks!";
                break;
        }
        
        Sequence sequence = DOTween.Sequence();
      
        sequence.Join(_cameraTransform.DOMove(_cameraMainPos.position, 1f)
            .SetEase(Ease.InOutSine));
      
        sequence.Join(_cameraTransform.DORotate(_cameraMainPos.eulerAngles, 1f)
            .SetEase(Ease.InOutSine));

        sequence.OnComplete((() =>
        {
            canvas.DOScale(Vector3.one, 0.5f)
                .SetEase(Ease.InOutBack)
                .OnComplete((() =>
                {
                    _rewardPanel.DOScale(Vector3.one, 0.5f)
                        .SetDelay(1f)
                        .SetEase(Ease.InOutBack)
                        .OnComplete((() =>
                        {
                            GameState.Instance.money += 50;
                        }));
                }));
        }));
    }

    public void CloseCraftPanel()
    {
        _rewardPanel.DOScale(Vector3.zero, 0.5f)
            .SetEase(Ease.InOutBack);
    }

    public void SetImageToCraftNext()
    {
        _currentIndex++;
        if (_currentIndex > 3)
            _currentIndex = 1;
        
        if (!_isRight)
        {
            switch (_currentIndex)
            {
                case 1:
                    _circleLeft.gameObject.SetActive(true);
                    _squareLeft.gameObject.SetActive(false);
                    _triangleLeft.gameObject.SetActive(false);
                    break;
                case 2:
                    _circleLeft.gameObject.SetActive(false);
                    _squareLeft.gameObject.SetActive(true);
                    _triangleLeft.gameObject.SetActive(false);
                    break;
                case 3:
                    _circleLeft.gameObject.SetActive(false);
                    _squareLeft.gameObject.SetActive(false);
                    _triangleLeft.gameObject.SetActive(true);
                    break;
            }
        }
        else
        {
            switch (_currentIndex)
            {
                case 1:
                    _circleRight.gameObject.SetActive(true);
                    _squareRight.gameObject.SetActive(false);
                    _triangleRight.gameObject.SetActive(false);
                    break;
                case 2:
                    _circleRight.gameObject.SetActive(false);
                    _squareRight.gameObject.SetActive(true);
                    _triangleRight.gameObject.SetActive(false);
                    break;
                case 3:
                    _circleRight.gameObject.SetActive(false);
                    _squareRight.gameObject.SetActive(false);
                    _triangleRight.gameObject.SetActive(true);
                    break;
            }
        }
        
        ChangeMainIcon();
    }
    public void SetImageToCraftPrevious()
    {
        _currentIndex--;
        if (_currentIndex < 1)
            _currentIndex = 3;
        
        if (!_isRight)
        {
            switch (_currentIndex)
            {
                case 1:
                    _circleLeft.gameObject.SetActive(true);
                    _squareLeft.gameObject.SetActive(false);
                    _triangleLeft.gameObject.SetActive(false);
                    break;
                case 2:
                    _circleLeft.gameObject.SetActive(false);
                    _squareLeft.gameObject.SetActive(true);
                    _triangleLeft.gameObject.SetActive(false);
                    break;
                case 3:
                    _circleLeft.gameObject.SetActive(false);
                    _squareLeft.gameObject.SetActive(false);
                    _triangleLeft.gameObject.SetActive(true);
                    break;
            }
        }
        else
        {
            switch (_currentIndex)
            {
                case 1:
                    _circleRight.gameObject.SetActive(true);
                    _squareRight.gameObject.SetActive(false);
                    _triangleRight.gameObject.SetActive(false);
                    break;
                case 2:
                    _circleRight.gameObject.SetActive(false);
                    _squareRight.gameObject.SetActive(true);
                    _triangleRight.gameObject.SetActive(false);
                    break;
                case 3:
                    _circleRight.gameObject.SetActive(false);
                    _squareRight.gameObject.SetActive(false);
                    _triangleRight.gameObject.SetActive(true);
                    break;
            }
        }
        
        ChangeMainIcon();
    }

    private void ChangeMainIcon()
    {
        if (_currentIndex == 1)
        {
            _circle.gameObject.SetActive(true);
            _square.gameObject.SetActive(false);
            _triangle.gameObject.SetActive(false);
        }
        else if (_currentIndex == 2)
        {
            _circle.gameObject.SetActive(false);
            _square.gameObject.SetActive(true);
            _triangle.gameObject.SetActive(false);
        }
        else if (_currentIndex == 3)
        {
            _circle.gameObject.SetActive(false);
            _square.gameObject.SetActive(false);
            _triangle.gameObject.SetActive(true);
        }
    }
    
    private string GetActiveShape(Image circle, Image square, Image triangle)
    {
        if (circle.gameObject.activeSelf) return "circle";
        if (square.gameObject.activeSelf) return "square";
        if (triangle.gameObject.activeSelf) return "triangle";
        return "";
    }

    private void ShowResult(Image result)
    {
        _resultPendant.gameObject.SetActive(result == _resultPendant);
        _resultCrown.gameObject.SetActive(result == _resultCrown);
        _resultRing.gameObject.SetActive(result == _resultRing);
    }
}
