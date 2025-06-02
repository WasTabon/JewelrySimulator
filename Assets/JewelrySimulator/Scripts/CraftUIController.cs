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
    
    [SerializeField] private Image _resultImage;
    
    private int _currentIndex = 1;
    private bool _isRight;

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
    }
    
}
