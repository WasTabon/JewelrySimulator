using UnityEngine;

public class ClothFollower : MonoBehaviour
{
    public Camera cam;                       // Камера
    public Transform targetObject;           // Сюда положи тряпку
    public Transform crystalObjectRed;          // Объект кристалла
    public Transform crystalObjectBlue;          // Объект кристалла
    public Transform crystalObjectGreen;          // Объект кристалла
    public float followSpeed = 10f;          // Насколько быстро тряпка двигается
    public float returnSpeed = 5f;           // Насколько быстро возвращается
    public Vector3 defaultPosition;          // Где тряпка по умолчанию
    public Quaternion defaultRotation;       // Изначальный поворот

    private bool isFollowing = false;

    private void Start()
    {
        defaultPosition = targetObject.position;
        defaultRotation = targetObject.rotation;
    }

    private void Update()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        if (Input.GetMouseButton(0))
        {
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.transform == crystalObjectRed || hit.transform == crystalObjectBlue || hit.transform == crystalObjectGreen)
                {
                    isFollowing = true;
                    Vector3 targetPos = hit.point + hit.normal * 0.01f; // немного отступаем от поверхности
                    Vector3 forward = Vector3.Cross(cam.transform.right, hit.normal);
                    Quaternion targetRot = Quaternion.LookRotation(forward, hit.normal);

                    targetObject.position = Vector3.Lerp(targetObject.position, targetPos, Time.deltaTime * followSpeed);
                    targetObject.rotation = Quaternion.Slerp(targetObject.rotation, targetRot, Time.deltaTime * followSpeed);
                }
                else
                {
                    isFollowing = true;
                    MoveToCursor(ray);
                }
            }
            else
            {
                isFollowing = true;
                MoveToCursor(ray);
            }
        }
        else
        {
            isFollowing = false;
            ReturnToStart();
        }
    }

    void MoveToCursor(Ray ray)
    {
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Vector3 targetPos = hit.point;
            targetObject.position = Vector3.Lerp(targetObject.position, targetPos, Time.deltaTime * followSpeed);
            targetObject.rotation = Quaternion.Slerp(targetObject.rotation, defaultRotation, Time.deltaTime * followSpeed);
        }
        else
        {
            Plane plane = new Plane(cam.transform.forward * -1, defaultPosition);
            if (plane.Raycast(ray, out float enter))
            {
                Vector3 point = ray.GetPoint(enter);
                targetObject.position = Vector3.Lerp(targetObject.position, point, Time.deltaTime * followSpeed);
                targetObject.rotation = Quaternion.Slerp(targetObject.rotation, defaultRotation, Time.deltaTime * followSpeed);
            }
        }
    }

    void ReturnToStart()
    {
        targetObject.position = Vector3.Lerp(targetObject.position, defaultPosition, Time.deltaTime * returnSpeed);
        targetObject.rotation = Quaternion.Slerp(targetObject.rotation, defaultRotation, Time.deltaTime * returnSpeed);
    }
}
