using System;
using DG.Tweening;
using UnityEngine;

public class LavaController : MonoBehaviour
{
    [SerializeField] private Transform _lava;

    private void Start()
    {
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
        Debug.Log("Start Move");
        _lava.DOLocalMoveY(0.09f, 1f)
            .SetEase(Ease.OutQuad)
            .OnComplete((() =>
            {
                Debug.Log("Movement Finsihed");
            }));
    }

    private void ResetLava()
    {
        _lava.DOLocalMoveY(-0.05f, 0f);
    }
}
