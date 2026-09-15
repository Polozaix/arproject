using UnityEngine;

namespace PersonalAR.XR
{
    public class SoftHeadFollow : MonoBehaviour
    {
        [Header("Target")]
        [SerializeField] private Transform head;

        [Header("Placement")]
        [SerializeField] private float distance = 1.2f;
        [SerializeField] private float verticalOffset = 0f;

        [Header("Smoothing")]
        [SerializeField] private float positionSmoothTime = 0.10f;
        [SerializeField] private float rotationSharpness = 12f;

        [SerializeField, Range(1f, 30f)] private float maximumAngularLag = 12f;
        private Vector3 velocity;
        private bool positioned;

        private void OnEnable()
        {
            positioned = false;
            velocity = Vector3.zero;
        }

        private void LateUpdate()
        {
            if (head == null)
                return;

            Vector3 targetPosition =
                head.position +
                head.forward * distance +
                head.up * verticalOffset;

            // Start at the head instead of flying in from the scene origin.
            if (!positioned)
            {
                transform.SetPositionAndRotation(targetPosition, head.rotation);
                positioned = true;
                return;
            }

            transform.position = Vector3.SmoothDamp(
                transform.position,
                targetPosition,
                ref velocity,
                Mathf.Max(0.001f, positionSmoothTime),
                Mathf.Infinity,
                Time.unscaledDeltaTime
            );

            // Bound angular drift and keep a constant viewing distance through fast turns.
            Vector3 direction = transform.position - head.position;
            Vector3 desired = targetPosition - head.position;
            direction = Vector3.RotateTowards(desired.normalized, direction.normalized,
                maximumAngularLag * Mathf.Deg2Rad, 0f);
            transform.position = head.position + direction * desired.magnitude;

            Quaternion targetRotation = head.rotation;

            float rotationT =
                1f - Mathf.Exp(-Mathf.Max(0f, rotationSharpness) * Time.unscaledDeltaTime);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationT
            );
            transform.rotation = Quaternion.RotateTowards(targetRotation, transform.rotation, maximumAngularLag);
        }

        public void Recenter() { positioned = false; velocity = Vector3.zero; }
    }
}
