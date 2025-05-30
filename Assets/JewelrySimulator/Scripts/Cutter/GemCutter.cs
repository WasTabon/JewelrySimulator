using System.Collections.Generic;
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

    private bool _isGood;
    private float _oldSimilar;
    float threshold = 0.4f;
    
    public void Cut(GameObject gem, Vector3 point, Vector3 normal)
    {
        if (GameState.Instance.gemType == GemType.Red)
        {
            targetShapeMask = targetShapeMaskRed;
            threshold = 1f;
        }
        else if (GameState.Instance.gemType == GemType.Blue)
        {
            targetShapeMask = targetShapeMaskBlue;
            threshold = 0.8f;
        }
        else if (GameState.Instance.gemType == GemType.Green)
        {
            targetShapeMask = targetShapeMaskGreen;
            threshold = 0.7f;
        }
        else
        {
            targetShapeMask = targetShapeMaskRed;
            threshold = 1f;
        }
        
        SlicedHull hull = gem.Slice(point, normal);
        if (hull == null) return;

        GameObject upper = hull.CreateUpperHull(gem, cutMaterial);
        GameObject lower = hull.CreateLowerHull(gem, cutMaterial);

        float upperDistance = DistanceToMask(upper);
        float lowerDistance = DistanceToMask(lower);

        GameObject matchingPiece = upperDistance < lowerDistance ? upper : lower;
        GameObject otherPiece = upperDistance < lowerDistance ? lower : upper;

        float similarity = GetShapeSimilarityApprox(lower, targetShapeMask);
        Debug.Log($"Shape Similarity: {similarity}");
        
        SetupPiece(matchingPiece, normal);
        SetupPiece(otherPiece, -normal, true);
        
        if (similarity >= threshold)
        {
            Debug.Log("Вырезана нужная форма!");
            _isGood = true;
        }
        else
        {
            Debug.Log("Форма не совпадает");
            _isGood = false;
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

    Vector3[] SampleMeshPoints(GameObject obj, int count)
    {
        Mesh mesh = obj.GetComponent<MeshFilter>()?.sharedMesh;
        if (mesh == null) return new Vector3[0];

        var vertices = mesh.vertices;
        List<Vector3> sampled = new List<Vector3>();

        var bounds = mesh.bounds;
        float scaleFactor = Mathf.Max(bounds.size.x, bounds.size.y, bounds.size.z);

        for (int i = 0; i < count; i++)
        {
            Vector3 randomVertex = vertices[Random.Range(0, vertices.Length)];
            Vector3 worldPoint = obj.transform.TransformPoint(randomVertex);

            // Нормализация — отцентровать и масштабировать
            Vector3 normalized = worldPoint - obj.transform.position;
            normalized /= scaleFactor;

            sampled.Add(normalized);
        }

        return sampled.ToArray();
    }
    
    float GetShapeSimilarityApprox(GameObject a, GameObject b, int sampleCount = 500, float maxDistance = 0.2f)
    {
        Vector3[] sampleA = SampleMeshPoints(a, sampleCount);
        Vector3[] sampleB = SampleMeshPoints(b, sampleCount);

        if (sampleA.Length == 0 || sampleB.Length == 0)
            return 0f;

        float total = 0f;
        int matches = 0;

        foreach (Vector3 pointA in sampleA)
        {
            float minDist = float.MaxValue;

            foreach (Vector3 pointB in sampleB)
            {
                float dist = Vector3.Distance(pointA, pointB);
                if (dist < minDist)
                    minDist = dist;
            }

            if (minDist <= maxDistance)
                matches++;
        }

        float similarity = matches / (float)sampleA.Length;
        similarity += _oldSimilar;
        _oldSimilar = similarity;

        return similarity;
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
    
    float GetShapeSimilarity(GameObject a, GameObject b, float tolerance = 0.01f)
    {
        MeshFilter meshFilterA = a.GetComponent<MeshFilter>();
        MeshFilter meshFilterB = b.GetComponent<MeshFilter>();

        if (meshFilterA == null || meshFilterB == null)
            return 0f;

        Mesh meshA = meshFilterA.sharedMesh;
        Mesh meshB = meshFilterB.sharedMesh;

        Vector3[] verticesA = meshA.vertices;
        Vector3[] verticesB = meshB.vertices;

        if (verticesA.Length != verticesB.Length)
        {
            Debug.LogWarning("Meshes have different vertex counts");
            return 0f;
        }

        int matching = 0;

        for (int i = 0; i < verticesA.Length; i++)
        {
            // Приводим к мировым координатам, затем в локальные относительно объекта
            Vector3 va = a.transform.TransformPoint(verticesA[i]);
            Vector3 vb = b.transform.TransformPoint(verticesB[i]);

            // Сравнение с учётом допущения
            if (Vector3.Distance(va, vb) <= tolerance)
            {
                matching++;
            }
        }

        return matching / (float)verticesA.Length;
    }
    
    float DistanceToMask(GameObject piece)
    {
        Vector3 pieceCenter = piece.GetComponent<MeshRenderer>().bounds.center;
        Vector3 maskCenter = targetShapeMask.GetComponent<Renderer>().bounds.center;

        return Vector3.Distance(pieceCenter, maskCenter);
    }
}