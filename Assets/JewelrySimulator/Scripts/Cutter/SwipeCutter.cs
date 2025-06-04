using UnityEngine;
using DG.Tweening;

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
        if (GameState.Instance.state != State.Cut)
            return;
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameObject gem = GameObject.FindWithTag("Gem");
            if (gem != null)
            {
                Vector3 center = gem.GetComponent<Renderer>().bounds.center;
                cutter.Cut(gem, center, Vector3.up);
            }
        }
        
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

            float gemY = referenceGem.GetComponent<Renderer>().bounds.center.y;
            Vector3 worldStart = ScreenToWorldAtHeight(swipeStart, gemY);
            Vector3 worldEnd = ScreenToWorldAtHeight(swipeEnd, gemY);
            
            Vector3 swipeMid = (worldStart + worldEnd) * 0.5f;
            Vector3 swipeDir = (worldEnd - worldStart);
            swipeDir.y = 0f; // обнуляем высоту, чтобы вектор был в XZ плоскости
            swipeDir.Normalize();
            
            Vector3 cutCenter = (worldStart + worldEnd) * 0.5f;
            Vector3 cutNormal = new Vector3(-swipeDir.z, 0f, swipeDir.x);

            GameObject closestGem = null;
            float closestDistance = float.MaxValue;

            foreach (var gem in GameObject.FindGameObjectsWithTag("Gem"))
            {
                Vector3 gemPos = gem.GetComponent<Renderer>().bounds.center;
                float dist = Vector3.Distance(gemPos, swipeMid);
                if (dist < closestDistance)
                {
                    closestDistance = dist;
                    closestGem = gem;
                }
            }

            if (closestGem != null && closestDistance < 1f)
            {
                cutter.Cut(closestGem, swipeMid, cutNormal);
            }
            Debug.DrawLine(worldStart, worldEnd, Color.red, 2f);
            Debug.DrawRay(swipeMid, Vector3.up * 2f, Color.green, 2f);
        }

    }

    Vector3 ScreenToWorldAtHeight(Vector2 screenPos, float heightY)
    {
        Ray ray = Camera.main.ScreenPointToRay(screenPos);
        Plane plane = new Plane(Vector3.up, new Vector3(0f, heightY, 0f));
        return plane.Raycast(ray, out float enter) ? ray.GetPoint(enter) : Vector3.zero;
    }
    
    Vector3 ScreenToWorldFlat(Vector2 screenPos)
    {
        Ray ray = Camera.main.ScreenPointToRay(screenPos);
        Plane ground = new Plane(Vector3.up, Vector3.zero); // плоскость XZ
        return ground.Raycast(ray, out float enter) ? ray.GetPoint(enter) : Vector3.zero;
    }
    
    Vector3 GetWorldPoint(Vector2 screenPos, Vector3 planeOrigin, Vector3 planeNormal)
    {
        Ray ray = Camera.main.ScreenPointToRay(screenPos);
        return new Plane(planeNormal, planeOrigin).Raycast(ray, out float enter)
            ? ray.GetPoint(enter)
            : Camera.main.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, 10f));
    }
}
