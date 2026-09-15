using UnityEngine;
using UnityEngine.UI;

namespace PersonalAR.UI
{
    /// <summary>Texture-free clipped glass panel, fine rim and quiet accent seam.</summary>
    [RequireComponent(typeof(CanvasRenderer))]
    public class HudSurface : MaskableGraphic
    {
        protected override void OnEnable()
        {
            // Also repair instances created before this dependency was declared.
            if (GetComponent<CanvasRenderer>() == null)
                gameObject.AddComponent<CanvasRenderer>();
            base.OnEnable();
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
            var r = rectTransform.rect;
            const float cut = 10f;
            Vector2[] points = {
                new Vector2(r.xMin + cut, r.yMin), new Vector2(r.xMax - cut, r.yMin),
                new Vector2(r.xMax, r.yMin + cut), new Vector2(r.xMax, r.yMax - cut),
                new Vector2(r.xMax - cut, r.yMax), new Vector2(r.xMin + cut, r.yMax),
                new Vector2(r.xMin, r.yMax - cut), new Vector2(r.xMin, r.yMin + cut)
            };
            vh.AddVert(r.center, color, Vector2.zero);
            foreach (var point in points) vh.AddVert(point, color, Vector2.zero);
            for (int i = 0; i < 8; i++) vh.AddTriangle(0, i + 1, (i + 1) % 8 + 1);
            for (int i = 0; i < 8; i++)
                Line(vh, points[i], points[(i + 1) % 8], new Color(.40f, .76f, .78f, color.a * .32f), 1.5f);
            Line(vh, new Vector2(r.xMin + 16, r.yMax), new Vector2(r.xMin + 58, r.yMax),
                new Color(.40f, .91f, .86f, color.a * .85f), 2);
        }

        private static void Line(VertexHelper vh, Vector2 a, Vector2 b, Color tint, float width)
        {
            Vector2 n = new Vector2(a.y - b.y, b.x - a.x).normalized * width * .5f;
            int v = vh.currentVertCount;
            vh.AddVert(a - n, tint, Vector2.zero); vh.AddVert(a + n, tint, Vector2.zero);
            vh.AddVert(b + n, tint, Vector2.zero); vh.AddVert(b - n, tint, Vector2.zero);
            vh.AddTriangle(v, v + 1, v + 2); vh.AddTriangle(v, v + 2, v + 3);
        }
    }
}
