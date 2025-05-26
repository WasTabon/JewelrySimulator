using UnityEngine;
using RayFire;

public class SwipeToCut : MonoBehaviour
{
    private Vector3 swipeStart;
    private Vector3 swipeEnd;

    // Сила отталкивания кусков после разрушения
    public float pushForce = 2f;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            swipeStart = GetWorldPos();
        }

        if (Input.GetMouseButtonUp(0))
        {
            swipeEnd = GetWorldPos();
            Vector3 cutNormal = Vector3.Cross(swipeEnd - swipeStart, Camera.main.transform.forward).normalized;
            Vector3 cutCenter = (swipeStart + swipeEnd) * 0.5f;

            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100f))
            {
                RayfireRigid rigid = hit.collider.GetComponent<RayfireRigid>();
                if (rigid != null)
                {
                    // Создаем массив для плоскости реза
                    Vector3[] slicePlane = new Vector3[2];
                    slicePlane[0] = cutCenter;
                    slicePlane[1] = cutNormal;

                    // Добавляем плоскость реза
                    rigid.AddSlicePlane(slicePlane);

                    // Выполняем разрез
                    rigid.Demolish();

                    // Ждем короткое время, чтобы куски появились
                    StartCoroutine(ApplyPushForce(rigid, cutNormal));
                }
            }
        }
    }

    Vector3 GetWorldPos()
    {
        Vector3 pos = Input.mousePosition;
        pos.z = 5f; // Расстояние до объекта
        return Camera.main.ScreenToWorldPoint(pos);
    }

    // Применяем силу отталкивания кускам
    System.Collections.IEnumerator ApplyPushForce(RayfireRigid rigid, Vector3 direction)
    {
        yield return new WaitForSeconds(0.05f); // Даем время на появление кусков

        foreach (var frag in rigid.fragments)
        {
            if (frag != null)
            {
                Rigidbody rb = frag.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.AddForce(direction * pushForce, ForceMode.Impulse);
                }
            }
        }
    }
}
