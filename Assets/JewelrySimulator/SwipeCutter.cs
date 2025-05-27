using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class SwipeCutter : MonoBehaviour
{
    public RectTransform swipeLine;
    public Canvas canvas;
    public GemCutter cutter;

    Vector2 swipeStart;
    Vector2 swipeEnd;
    bool isSwiping;
    CanvasGroup swipeGroup;

    void Start()
    {
        swipeGroup = swipeLine.GetComponent<CanvasGroup>();
        swipeLine.gameObject.SetActive(false);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isSwiping = true;
            swipeStart = Input.mousePosition;

            swipeLine.gameObject.SetActive(true);
            swipeLine.localScale = Vector3.zero;
            swipeGroup.alpha = 0f;

            swipeLine.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack);
            swipeGroup.DOFade(1f, 0.2f);
        }
        else if (Input.GetMouseButton(0) && isSwiping)
        {
            swipeEnd = Input.mousePosition;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform, swipeStart, canvas.worldCamera, out Vector2 localStart);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform, swipeEnd, canvas.worldCamera, out Vector2 localEnd);

            Vector2 dir = localEnd - localStart;
            swipeLine.sizeDelta = new Vector2(dir.magnitude, 5f);
            swipeLine.anchoredPosition = localStart + dir * 0.5f;
            swipeLine.localRotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);
        }
        else if (Input.GetMouseButtonUp(0) && isSwiping)
        {
            isSwiping = false;
            
            swipeLine.DOPunchScale(Vector3.one * 0.2f, 0.15f, 8, 1f);

            swipeGroup.DOFade(0f, 0.2f).OnComplete(() =>
            {
                swipeLine.gameObject.SetActive(false);
            });

            GameObject referenceGem = GameObject.FindWithTag("Gem");
            if (!referenceGem) return;

            Vector3 origin = referenceGem.transform.position;
            Vector3 normal = Camera.main.transform.forward;

            Vector3 worldStart = GetWorldPoint(swipeStart, origin, normal);
            Vector3 worldEnd = GetWorldPoint(swipeEnd, origin, normal);
            Vector3 cutNormal = Vector3.Cross(worldEnd - worldStart, Camera.main.transform.forward).normalized;
            Vector3 cutCenter = (worldStart + worldEnd) * 0.5f;

            Plane cutPlane = new Plane(cutNormal, cutCenter);
            foreach (var gem in GameObject.FindGameObjectsWithTag("Gem"))
            {
                if (cutPlane.GetDistanceToPoint(gem.transform.position) < 0.5f)
                {
                    cutter.Cut(gem, cutCenter, cutNormal);
                }
            }
        }
    }

    Vector3 GetWorldPoint(Vector2 screenPos, Vector3 planeOrigin, Vector3 planeNormal)
    {
        Ray ray = Camera.main.ScreenPointToRay(screenPos);
        return new Plane(planeNormal, planeOrigin).Raycast(ray, out float enter)
            ? ray.GetPoint(enter)
            : Camera.main.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, 10f));
    }
}
