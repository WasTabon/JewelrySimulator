using EzySlice;
using UnityEngine;

public class GemCutter : MonoBehaviour
{
    public Material cutMaterial;
    public float pushForce = 2f;
    public float smallPieceThreshold = 0.5f;

    public void Cut(GameObject gem, Vector3 point, Vector3 normal)
    {
        SlicedHull hull = gem.Slice(point, normal);
        if (hull != null)
        {
            GameObject upper = hull.CreateUpperHull(gem, cutMaterial);
            GameObject lower = hull.CreateLowerHull(gem, cutMaterial);

            SetupPiece(upper, normal);
            SetupPiece(lower, -normal);

            Destroy(gem);
        }
    }

    void SetupPiece(GameObject piece, Vector3 forceDir)
    {
        piece.transform.position += forceDir * 0.01f;

        var col = piece.AddComponent<MeshCollider>();
        col.convex = true;

        var rb = piece.AddComponent<Rigidbody>();
        rb.mass = 0.1f;

        float size = CalculateSize(piece);
        if (size < smallPieceThreshold)
        {
            rb.AddForce(forceDir * pushForce + Random.insideUnitSphere * 0.1f, ForceMode.Impulse);
        }
        else
        {
            rb.isKinematic = true;
        }

        piece.tag = "Gem";
    }

    float CalculateSize(GameObject obj)
    {
        var bounds = obj.GetComponent<MeshRenderer>().bounds;
        return bounds.size.magnitude;
    }
}