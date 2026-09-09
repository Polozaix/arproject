using System;
using TMPro;
using UnityEngine;

namespace PersonalAR.UI
{
    public class ClockWidget : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text clockText;

        private float nextUpdateTime;

        private void Start()
        {
            UpdateClock();
        }

        private void Update()
        {
            if (Time.unscaledTime < nextUpdateTime)
                return;

            UpdateClock();
            nextUpdateTime = Time.unscaledTime + 1f;
        }

        private void UpdateClock()
        {
            clockText.text = DateTime.Now.ToString("HH:mm");
        }
    }
}