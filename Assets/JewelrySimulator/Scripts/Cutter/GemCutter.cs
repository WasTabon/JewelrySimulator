using System.Collections.Generic;
using EzySlice;
using UnityEngine;
using DG.Tweening;
using Random = UnityEngine.Random;

public class GemCutter : MonoBehaviour
{
    public EraseDirt eraseDirt;
    
    public Transform cameraLavaPos;
    public Material cutMaterial;
    public float pushForce = 2f;

    public RectTransform nextLavaButton;
    
    public GameObject targetShapeMask;
    public GameObject targetShapeMaskRed;
    public GameObject targetShapeMaskBlue;
    public GameObject targetShapeMaskGreen;

    private GameObject _finishedGem;
    
    private bool _isGood;
    private float _oldSimilar;
    float threshold = 0.4f;

    private void Start()
    {
        nextLavaButton.DOScale(Vector3.zero, 0f);
    }

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
            _finishedGem = matchingPiece;
            nextLavaButton.DOScale(Vector3.one, 0.5f)
                .SetEase(Ease.InOutBack);
        }
        else
        {
            Debug.Log("Форма не совпадает");
            _isGood = false;
        }
        
        Destroy(gem);
    }

    public void HandleGameStateLava()
    {
        GameState.Instance.state = State.Lava;
        nextLavaButton.DOScale(Vector3.zero, 0.5f)
            .SetEase(Ease.InOutBack);
        Transform cameraTransform = Camera.main.transform;
        cameraTransform.DOMove(cameraLavaPos.position, 1f)
            .SetEase(Ease.InOutSine)
            .OnComplete((() =>
            {
                eraseDirt.ResetEraseMask();
                _finishedGem.SetActive(false);
            }));
    }
    
   private void SetupPiece(GameObject piece, Vector3 forceDir, bool isSmall = false)
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

    private Vector3[] SampleMeshPoints(GameObject obj, int count)
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

            Vector3 normalized = worldPoint - obj.transform.position;
            normalized /= scaleFactor;

            sampled.Add(normalized);
        }

        return sampled.ToArray();
    }
    
    private float GetShapeSimilarityApprox(GameObject a, GameObject b, int sampleCount = 500, float maxDistance = 0.2f)
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
    
    private float DistanceToMask(GameObject piece)
    {
        Vector3 pieceCenter = piece.GetComponent<MeshRenderer>().bounds.center;
        Vector3 maskCenter = targetShapeMask.GetComponent<Renderer>().bounds.center;

        return Vector3.Distance(pieceCenter, maskCenter);
    }
}