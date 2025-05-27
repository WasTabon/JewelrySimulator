using EzySlice;
using UnityEngine;
using DG.Tweening;

public class GemCutter : MonoBehaviour
{
    public Material cutMaterial;
    public float pushForce = 2f;

    public void Cut(GameObject gem, Vector3 point, Vector3 normal)
    {
        SlicedHull hull = gem.Slice(point, normal);
        if (hull == null) return;

        GameObject upper = hull.CreateUpperHull(gem, cutMaterial);
        GameObject lower = hull.CreateLowerHull(gem, cutMaterial);

        float upperSize = CalculateSize(upper);
        float lowerSize = CalculateSize(lower);

        GameObject smallPiece = upperSize < lowerSize ? upper : lower;
        GameObject largePiece = upperSize < lowerSize ? lower : upper;

        SetupPiece(largePiece, normal);
        SetupPiece(smallPiece, -normal, true);

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
}