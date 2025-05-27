using EzySlice;
using UnityEngine;
using DG.Tweening;

public class GemCutter : MonoBehaviour
{
    public Material cutMaterial;
    public float pushForce = 2f;
    
    public GameObject targetShapeMask;
    
    public Vector3 centerReference = Vector3.zero;

    public void Cut(GameObject gem, Vector3 point, Vector3 normal)
    {
        SlicedHull hull = gem.Slice(point, normal);
        if (hull == null) return;

        GameObject upper = hull.CreateUpperHull(gem, cutMaterial);
        GameObject lower = hull.CreateLowerHull(gem, cutMaterial);

        float upperDistance = DistanceToMask(upper);
        float lowerDistance = DistanceToMask(lower);

        GameObject matchingPiece = upperDistance < lowerDistance ? upper : lower;
        GameObject otherPiece = upperDistance < lowerDistance ? lower : upper;

        SetupPiece(matchingPiece, normal);
        SetupPiece(otherPiece, -normal, true);

        Destroy(gem);
    }

    void SetupPiece(GameObject piece, Vector3 forceDir, bool isSmall = false)
    {
        piece.transform.position += forceDir * 0.01f;

        var col = piece.AddComponent<MeshCollider>();
        col.convex = true;

        var rb = piece.AddComponent<Rigidbody>();
        rb.mass = 0.1f;

        if (isSmall)
        {
            rb.AddForce(forceDir * pushForce, ForceMode.Impulse);

            piece.transform.DOScale(Vector3.zero, 1f)
                .SetEase(Ease.InBack)
                .OnComplete(() => piece.SetActive(false));
        }
        else
        {
            rb.isKinematic = true;
            piece.tag = "Gem";
        }
    }

    float CalculateSize(GameObject obj)
    {
        var bounds = obj.GetComponent<MeshRenderer>().bounds;
        return bounds.size.magnitude;
    }
    
    float GetOverlapScore(GameObject piece)
    {
        // Добавим временно MeshCollider для пересечения
        MeshCollider pieceCollider = piece.AddComponent<MeshCollider>();
        pieceCollider.convex = false;

        MeshCollider maskCollider = targetShapeMask.GetComponent<MeshCollider>();

        if (maskCollider == null || pieceCollider == null)
            return 0f;

        // Считаем количество точек на меше, находящихся внутри маски
        Mesh mesh = piece.GetComponent<MeshFilter>().sharedMesh;
        Vector3[] vertices = mesh.vertices;

        int insideCount = 0;
        foreach (Vector3 vertex in vertices)
        {
            Vector3 worldPoint = piece.transform.TransformPoint(vertex);
            if (maskCollider.bounds.Contains(worldPoint))
                insideCount++;
        }

        Destroy(pieceCollider); // убираем временный коллайдер

        return insideCount / (float)vertices.Length;
    }
    
    float DistanceToMask(GameObject piece)
    {
        Vector3 pieceCenter = piece.GetComponent<MeshRenderer>().bounds.center;
        Vector3 maskCenter = targetShapeMask.GetComponent<Renderer>().bounds.center;

        return Vector3.Distance(pieceCenter, maskCenter);
    }
}