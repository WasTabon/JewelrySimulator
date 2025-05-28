using UnityEngine;

public class EraseDirt : MonoBehaviour
{
    public Camera cam;
    public RenderTexture maskTexture;
    public Material eraseMaterial;
    public float brushSize = 0.1f;

    private void Start()
    {
        RenderTexture.active = maskTexture;
        GL.Clear(true, true, Color.white);
        RenderTexture.active = null;
    }
   
    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Vector2 uv = hit.textureCoord;
                DrawAtUV(uv);
            }
        }
    }

    void DrawAtUV(Vector2 uv)
    {
        RenderTexture.active = maskTexture;

        eraseMaterial.SetVector("_ErasePos", new Vector4(uv.x, uv.y, 0, 0));
        eraseMaterial.SetFloat("_EraseSize", brushSize);

        GL.PushMatrix();
        GL.LoadOrtho();

        eraseMaterial.SetPass(0);

        float size = brushSize * 0.5f;
        float x = uv.x;
        float y = uv.y;

        GL.Begin(GL.QUADS);
        GL.TexCoord2(0, 0); GL.Vertex3(x - size, y - size, 0);
        GL.TexCoord2(1, 0); GL.Vertex3(x + size, y - size, 0);
        GL.TexCoord2(1, 1); GL.Vertex3(x + size, y + size, 0);
        GL.TexCoord2(0, 1); GL.Vertex3(x - size, y + size, 0);
        GL.End();

        GL.PopMatrix();

        RenderTexture.active = null;
    }
}