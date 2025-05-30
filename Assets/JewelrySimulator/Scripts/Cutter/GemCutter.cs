using EzySlice;
using UnityEngine;
using DG.Tweening;

public class GemCutter : MonoBehaviour
{
    public Material cutMaterial;
    public float pushForce = 2f;
    
    public GameObject targetShapeMask;
    public GameObject targetShapeMaskRed;
    public GameObject targetShapeMaskBlue;
    public GameObject targetShapeMaskGreen;

    public void Cut(GameObject gem, Vector3 point, Vector3 normal)
    {
        if (GameState.Instance.gemType == GemType.Red)
        {
            targetShapeMask = targetShapeMaskRed;
        }
        else if (GameState.Instance.gemType == GemType.Blue)
        {
            targetShapeMask = targetShapeMaskBlue;
        }
        else if (GameState.Instance.gemType == GemType.Green)
        {
            targetShapeMask = targetShapeMaskGreen;
        }
        else
        {
            targetShapeMask = targetShapeMaskRed;
        }
        
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
        
        float lowerScore = GetOverlapScore(lower);
        
        Debug.Log($"Lower Score: {lowerScore}");

        float threshold = 0.75f;

        if (lowerScore >= threshold)
        {
            Debug.Log("Вырезана нужная форма!");
        }
        else
        {
            Debug.Log("Форма не совпадает");
        }
        
        Destroy(gem);
    }

    void SetupPiece(GameObject piece, Vector3 forceDir, bool isSmall = false)
    {
        if (isSmall)
            piece.transform.position += forceDir * 0.01f;

        var col = piece.AddComponent<MeshCollider>();

        var rb = piece.AddComponent<Rigidbody>();
        rb.mass = 0.1f;

        if (isSmall)
        {
            col.convex = true;
                    col.isTrigger = true;
            
            rb.AddForce(forceDir * pushForce, ForceMode.Impulse);

            piece.transform.DOScale(Vector3.zero, 1f)
                .SetEase(Ease.InBack)
                .OnComplete(() => piece.SetActive(false));
        }
        else
        {
            rb.isKinematic = true;
            rb.constraints = RigidbodyConstraints.FreezeAll;
            piece.tag = "Gem";
        }
    }

    float CalculateSize(GameObject obj)
    {
        var bounds = obj.GetComponent<MeshRenderer>().bounds;
        return bounds.size.magnitude;
    }
    
    float GetOverlapScore(GameObject piece,float maxDistance = 0.5f)
    {
        MeshCollider maskCollider = targetShapeMask.GetComponent<MeshCollider>();
        if (maskCollider == null)
        {
            Debug.LogWarning("Mask collider missing!");
            return 0f;
        }

        Mesh mesh = piece.GetComponent<MeshFilter>().sharedMesh;
        Vector3[] vertices = mesh.vertices;

        int insideCount = 0;

        foreach (Vector3 vertex in vertices)
        {
            Vector3 worldPoint = piece.transform.TransformPoint(vertex);
            Vector3 closestPoint = maskCollider.ClosestPoint(worldPoint);
            float dist = Vector3.Distance(worldPoint, closestPoint);

            if (dist <= maxDistance)
                insideCount++;
        }

        return insideCount / (float)vertices.Length;
    }
    
    float DistanceToMask(GameObject piece)
    {
        Vector3 pieceCenter = piece.GetComponent<MeshRenderer>().bounds.center;
        Vector3 maskCenter = targetShapeMask.GetComponent<Renderer>().bounds.center;

        return Vector3.Distance(pieceCenter, maskCenter);
    }
}