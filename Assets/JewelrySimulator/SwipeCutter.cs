using UnityEngine;

public class SwipeCutter : MonoBehaviour
{
    private Vector3 swipeStart;
    private Vector3 swipeEnd;

    public GemCutter cutter;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            swipeStart = GetWorldPoint();
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    if (Physics.Raycast(ray, out RaycastHit hit))
                    {
                        Debug.Log("Hit: " + hit.collider.name);
                    }
        }
        if (Input.GetMouseButtonUp(0))
        {
            swipeEnd = GetWorldPoint();
        
            Vector3 planeNormal = Vector3.Cross(swipeEnd - swipeStart, Camera.main.transform.forward).normalized;
            Debug.DrawLine(swipeStart, swipeEnd * 1000f, Color.red, 2f);
            Debug.DrawRay((swipeStart + swipeEnd) * 0.5f, planeNormal, Color.green, 2f);
            Vector3 planeCenter = (swipeStart + swipeEnd) * 0.5f;
            Plane cutPlane = new Plane(planeNormal, planeCenter);
        
            GameObject[] gems = GameObject.FindGameObjectsWithTag("Gem");
            foreach (var gem in gems)
            {
                if (cutPlane.GetDistanceToPoint(gem.transform.position) < 0.5f)
                {
                    cutter.Cut(gem, planeCenter, planeNormal);
                }
            }
        }
    }

    Vector3 GetWorldPoint()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.up, Vector3.zero); // Плоскость XZ на Y=0, можно поменять
        if (plane.Raycast(ray, out float enter))
        {
            return ray.GetPoint(enter);
        }
        return Vector3.zero;
    }
}