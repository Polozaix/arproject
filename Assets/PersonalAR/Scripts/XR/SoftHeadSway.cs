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

        private Vector3 velocity;

        private void LateUpdate()
        {
            if (head == null)
                return;

            Vector3 targetPosition =
                head.position +
                head.forward * distance +
                head.up * verticalOffset;

            transform.position = Vector3.SmoothDamp(
                transform.position,
                targetPosition,
                ref velocity,
                positionSmoothTime,
                Mathf.Infinity,
                Time.unscaledDeltaTime
            );

            Quaternion targetRotation = head.rotation;

            float rotationT =
                1f - Mathf.Exp(-rotationSharpness * Time.unscaledDeltaTime);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationT
            );
        }
    }
}