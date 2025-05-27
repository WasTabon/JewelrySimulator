using UnityEngine;

public class SwipeCutter : MonoBehaviour
{
    public RectTransform swipeLine;
    public Canvas canvas;
    public GemCutter cutter;

    private Vector2 screenSwipeStart;
    private Vector2 screenSwipeEnd;
    private bool isSwiping = false;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isSwiping = true;
            screenSwipeStart = Input.mousePosition;

            swipeLine.gameObject.SetActive(true);
        }
        else if (Input.GetMouseButton(0) && isSwiping)
        {
            screenSwipeEnd = Input.mousePosition;

            // Преобразуем screen → local для отрисовки в UI
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform,
                screenSwipeStart,
                canvas.worldCamera,
                out Vector2 localStart
            );

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform,
                screenSwipeEnd,
                canvas.worldCamera,
                out Vector2 localEnd
            );

            Vector2 dir = localEnd - localStart;
            float length = dir.magnitude;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

            swipeLine.sizeDelta = new Vector2(length, 5); // 5 — толщина
            swipeLine.anchoredPosition = localStart + dir * 0.5f;
            swipeLine.localRotation = Quaternion.Euler(0, 0, angle);
        }
        else if (Input.GetMouseButtonUp(0) && isSwiping)
        {
            isSwiping = false;
            swipeLine.gameObject.SetActive(false);

            // Возьмём первый попавшийся объект с тегом Gem
            GameObject referenceGem = GameObject.FindWithTag("Gem");
            if (referenceGem == null) return;

            Vector3 planeOrigin = referenceGem.transform.position;
            Vector3 planeNormal = Vector3.up; // или direction камеры, если объект вертикальный

            Vector3 worldStart = GetWorldPoint(screenSwipeStart, planeOrigin, planeNormal);
            Vector3 worldEnd = GetWorldPoint(screenSwipeEnd, planeOrigin, planeNormal);

            Vector3 cutNormal = Vector3.Cross(worldEnd - worldStart, Camera.main.transform.forward).normalized;
            Vector3 cutCenter = (worldStart + worldEnd) * 0.5f;

            Plane cutPlane = new Plane(cutNormal, cutCenter);
            GameObject[] gems = GameObject.FindGameObjectsWithTag("Gem");
            foreach (var gem in gems)
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
        Plane plane = new Plane(planeNormal, planeOrigin);
        if (plane.Raycast(ray, out float enter))
        {
            return ray.GetPoint(enter);
        }
        return Vector3.zero;
    }
}
