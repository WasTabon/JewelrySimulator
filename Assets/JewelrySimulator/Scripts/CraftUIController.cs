using System;
using UnityEngine;
using UnityEngine.UI;

public class CraftUIController : MonoBehaviour
{
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
    
    private int _currentIndex = 1;
    private bool _isRight;

    private void Update()
    {
        if ((_circleLeft.gameObject.activeSelf && _squareRight.gameObject.activeSelf) ||
            (_circleRight.gameObject.activeSelf && _squareLeft.gameObject.activeSelf))
        {
            _resultPendant.gameObject.SetActive(true);
            _resultCrown.gameObject.SetActive(false);
            _resultRing.gameObject.SetActive(false);
        }
        else if ((_circleLeft.gameObject.activeSelf && _triangleRight.gameObject.activeSelf) ||
            (_circleRight.gameObject.activeSelf && _triangleLeft.gameObject.activeSelf))
        {
            _resultPendant.gameObject.SetActive(false);
            _resultCrown.gameObject.SetActive(true);
            _resultRing.gameObject.SetActive(false);
        }
        else if ((_circleLeft.gameObject.activeSelf && _circleRight.gameObject.activeSelf) ||
                 (_circleRight.gameObject.activeSelf && _circleLeft.gameObject.activeSelf))
        {
            _resultPendant.gameObject.SetActive(false);
            _resultCrown.gameObject.SetActive(false);
            _resultRing.gameObject.SetActive(true);
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
}
