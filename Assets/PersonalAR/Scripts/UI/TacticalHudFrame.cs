using UnityEngine;
using UnityEngine.UI;

namespace PersonalAR.UI
{
    /// <summary>Optional decorative mesh. No input handling or simulated telemetry.</summary>
    [AddComponentMenu("PersonalAR/UI/Tactical HUD Frame")]
    public class TacticalHudFrame : MaskableGraphic
    {
        [SerializeField, Range(0.5f, 4f)] private float lineWidth = 1.5f;
        [SerializeField, Range(8f, 40f)] private float inset = 12f;

        protected override void OnEnable()
        {
            base.OnEnable();
            raycastTarget = false;
        }

        protected override void OnPopulateMesh(VertexHelper mesh)
        {
            mesh.Clear();
            Rect r = rectTransform.rect;
            float left = r.xMin + inset, right = r.xMax - inset;
            float bottom = r.yMin + inset, top = r.yMax - inset;
            if (right - left < 140f || top - bottom < 100f)
                return;

            // Broken perimeter keeps the middle of the view unobstructed.
            foreach (float x in new[] { left, right })
            foreach (float y in new[] { bottom, top })
            {
                float dx = x == left ? 1f : -1f;
                float dy = y == bottom ? 1f : -1f;
                Line(mesh, new Vector2(x + dx * 44f, y), new Vector2(x + dx * 8f, y), 1f);
                Line(mesh, new Vector2(x + dx * 8f, y), new Vector2(x, y + dy * 8f), 1f);
                Line(mesh, new Vector2(x, y + dy * 8f), new Vector2(x, y + dy * 36f), 1f);
                Line(mesh, new Vector2(x + dx * 52f, y), new Vector2(x + dx * 72f, y), 0.35f);
            }

            // Decorative scales, deliberately without numeric sensor labels.
            for (int i = -3; i <= 3; i++)
            {
                float y = r.center.y + i * 13f;
                float length = i == 0 ? 14f : 6f;
                Line(mesh, new Vector2(left, y), new Vector2(left + length, y), 0.5f);
                Line(mesh, new Vector2(right - length, y), new Vector2(right, y), 0.5f);
            }
            for (int i = -4; i <= 4; i++)
            {
                float x = r.center.x + i * 15f;
                Line(mesh, new Vector2(x, top), new Vector2(x, top - (i == 0 ? 8f : 4f)), 0.4f);
            }
            for (int i = 0; i < 5; i++)
            {
                float x = left + 80f + i * 11f;
                Line(mesh, new Vector2(x, bottom + 2f), new Vector2(x + 4f, bottom + 8f), 0.65f);
            }
            Line(mesh, new Vector2(right - 140f, bottom), new Vector2(right - 80f, bottom), 0.35f);
        }

        private void Line(VertexHelper mesh, Vector2 a, Vector2 b, float opacity)
        {
            Vector2 normal = new Vector2(-(b - a).y, (b - a).x).normalized * lineWidth * 0.5f;
            Color tint = color;
            tint.a *= opacity;
            int start = mesh.currentVertCount;
            mesh.AddVert(a - normal, tint, Vector2.zero);
            mesh.AddVert(a + normal, tint, Vector2.zero);
            mesh.AddVert(b + normal, tint, Vector2.zero);
            mesh.AddVert(b - normal, tint, Vector2.zero);
            mesh.AddTriangle(start, start + 1, start + 2);
            mesh.AddTriangle(start, start + 2, start + 3);
        }
    }
}
