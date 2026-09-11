using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.Interaction.Toolkit.UI;

namespace PersonalAR.UI
{
    // Dragging uses a fixed world plane so moving your head does not move the card.
    public class SpatialPanelHandle : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public Transform panel;
        public Action dismissed;
        private Plane plane;
        private Vector3 offset, lastPoint, speed;
        private float lastTime;
        private int? owner;

        public void OnBeginDrag(PointerEventData e)
        {
            if (owner.HasValue || panel == null) return;
            plane = new Plane(panel.forward, panel.position);
            if (!Point(e, out var p)) return;
            owner = e.pointerId;
            offset = panel.position - p;
            lastPoint = p;
            lastTime = Time.unscaledTime;
            speed = Vector3.zero;
        }

        public void OnDrag(PointerEventData e)
        {
            if (owner != e.pointerId || !Point(e, out var p)) return;
            float dt = Time.unscaledTime - lastTime;
            if (dt > 0.001f) speed = Vector3.Lerp(speed, (p - lastPoint) / dt, 0.5f);
            panel.position = p + offset;
            lastPoint = p;
            lastTime = Time.unscaledTime;
        }

        public void OnEndDrag(PointerEventData e)
        {
            if (owner != e.pointerId) return;
            owner = null;
            // A deliberate quick flick dismisses the view, never the underlying task.
            if (Time.unscaledTime - lastTime < 0.12f && speed.magnitude > 1.8f)
                dismissed?.Invoke();
        }

        private bool Point(PointerEventData e, out Vector3 p)
        {
            Ray ray;
            if (e is TrackedDeviceEventData tracked && tracked.rayPoints != null && tracked.rayPoints.Count > 1)
                ray = new Ray(tracked.rayPoints[0], tracked.rayPoints[1] - tracked.rayPoints[0]);
            else if (e.pressEventCamera != null)
                ray = e.pressEventCamera.ScreenPointToRay(e.position);
            else { p = default; return false; }
            if (plane.Raycast(ray, out float distance) && distance < 8f)
            { p = ray.GetPoint(distance); return true; }
            p = default;
            return false;
        }

        private void OnDisable() { owner = null; speed = Vector3.zero; }
    }
}
