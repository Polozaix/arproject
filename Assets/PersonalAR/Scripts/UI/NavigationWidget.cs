using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace PersonalAR.UI
{
    [Serializable]
    public class NavigationStep
    {
        public string instruction;
        public string maneuverSymbol; // e.g. "▲", "↱", "↰", "★"
        public string distance;
        public double latitude;
        public double longitude;

        public NavigationStep(string instruction, string maneuverSymbol, string distance, double lat, double lng)
        {
            this.instruction = instruction;
            this.maneuverSymbol = maneuverSymbol;
            this.distance = distance;
            this.latitude = lat;
            this.longitude = lng;
        }
    }

    /// <summary>
    /// Glanceable navigation controller for PersonalAR HUD with Google Maps Static API integration and simulated fallback.
    /// </summary>
    [DisallowMultipleComponent]
    public class NavigationWidget : MonoBehaviour
    {
        [Header("Google Maps Configuration")]
        [Tooltip("Optional Google Maps API Key or free Maps Demo Key. Leave blank to use offline simulation mode.")]
        [SerializeField] private string apiKey = "";

        [Header("Route Context")]
        [SerializeField] private string destinationName = "Campus Tech Lab";
        [SerializeField] private string etaText = "14 min (1.1 km)";

        private readonly List<NavigationStep> steps = new List<NavigationStep>
        {
            new NavigationStep("Head north on University Way", "▲", "250 m", 51.5074, -0.1278),
            new NavigationStep("Turn right onto Innovation Blvd", "↱", "400 m", 51.5085, -0.1265),
            new NavigationStep("Continue straight past Central Plaza", "▲", "300 m", 51.5098, -0.1240),
            new NavigationStep("Turn left into Tech Gate", "↰", "150 m", 51.5110, -0.1235),
            new NavigationStep("Arrived at Campus Tech Lab", "★", "0 m", 51.5115, -0.1230)
        };

        private int currentStepIndex;
        private TMP_Text instructionText;
        private TMP_Text maneuverText;
        private TMP_Text distanceText;
        private TMP_Text etaLabel;
        private RawImage mapImage;
        private Texture2D simulatedTexture;
        private Coroutine fetchCoroutine;

        public string ApiKey
        {
            get => apiKey;
            set { apiKey = value; RefreshView(); }
        }

        public string DestinationName => destinationName;
        public string EtaText => etaText;
        public int StepCount => steps.Count;
        public int CurrentStepIndex => currentStepIndex;
        public NavigationStep CurrentStep => steps[Mathf.Clamp(currentStepIndex, 0, steps.Count - 1)];

        public void BindUI(TMP_Text instruction, TMP_Text maneuver, TMP_Text distance, TMP_Text eta, RawImage map = null)
        {
            instructionText = instruction;
            maneuverText = maneuver;
            distanceText = distance;
            etaLabel = eta;
            mapImage = map;
            RefreshView();
        }

        public void NextStep()
        {
            if (currentStepIndex < steps.Count - 1)
            {
                currentStepIndex++;
                RefreshView();
            }
        }

        public void PrevStep()
        {
            if (currentStepIndex > 0)
            {
                currentStepIndex--;
                RefreshView();
            }
        }

        public void RefreshView()
        {
            var step = CurrentStep;
            if (instructionText != null) instructionText.text = step.instruction;
            if (maneuverText != null) maneuverText.text = step.maneuverSymbol;
            if (distanceText != null) distanceText.text = step.distance;
            if (etaLabel != null) etaLabel.text = etaText;

            if (mapImage != null)
            {
                if (!string.IsNullOrWhiteSpace(apiKey))
                {
                    if (fetchCoroutine != null) StopCoroutine(fetchCoroutine);
                    fetchCoroutine = StartCoroutine(FetchStaticMapRoutine(step.latitude, step.longitude));
                }
                else
                {
                    RenderSimulatedMap();
                }
            }
        }

        private IEnumerator FetchStaticMapRoutine(double lat, double lng)
        {
            // Google Maps Static API with dark high-contrast styling designed for AR glanceability
            string style = "style=element:geometry%7Ccolor:0x1a2126&" +
                           "style=element:labels.text.fill%7Ccolor:0x8ba2aa&" +
                           "style=element:labels.text.stroke%7Ccolor:0x12171a&" +
                           "style=feature:road%7Celement:geometry%7Ccolor:0x2c3b42&" +
                           "style=feature:road.highway%7Celement:geometry%7Ccolor:0x3f5863&" +
                           "style=feature:water%7Celement:geometry%7Ccolor:0x0b1114";

            string url = $"https://maps.googleapis.com/maps/api/staticmap?center={lat:F6},{lng:F6}&zoom=16&size=400x160&scale=2&maptype=roadmap&{style}&markers=color:0x66e8dc%7Csize:mid%7C{lat:F6},{lng:F6}&key={apiKey}";

            using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(url))
            {
                yield return uwr.SendWebRequest();

                if (uwr.result == UnityWebRequest.Result.Success)
                {
                    Texture2D tex = DownloadHandlerTexture.GetContent(uwr);
                    if (mapImage != null && tex != null)
                    {
                        mapImage.texture = tex;
                        mapImage.color = Color.white;
                    }
                }
                else
                {
                    Debug.LogWarning($"[PersonalAR Navigation] Maps Static API failed: {uwr.error}. Falling back to simulation map.");
                    RenderSimulatedMap();
                }
            }
        }

        private void RenderSimulatedMap()
        {
            if (mapImage == null) return;

            if (simulatedTexture == null)
            {
                int w = 256, h = 128;
                simulatedTexture = new Texture2D(w, h, TextureFormat.RGBA32, false);
                simulatedTexture.wrapMode = TextureWrapMode.Clamp;
                simulatedTexture.filterMode = FilterMode.Bilinear;

                Color bg = new Color(0.04f, 0.08f, 0.11f, 0.95f);
                Color grid = new Color(0.12f, 0.22f, 0.28f, 0.40f);
                Color route = new Color(0.40f, 0.91f, 0.86f, 1f);
                Color pin = new Color(1f, 0.35f, 0.45f, 1f);

                Color[] pixels = new Color[w * h];
                for (int y = 0; y < h; y++)
                {
                    for (int x = 0; x < w; x++)
                    {
                        Color c = bg;
                        if (x % 32 == 0 || y % 32 == 0) c = grid;
                        float targetY = 24f + (float)x / w * (h - 48f);
                        if (Mathf.Abs(y - targetY) < 3.5f) c = route;
                        if (Mathf.Abs(x - 24) < 5 && Mathf.Abs(y - 24) < 5) c = route;
                        if (Mathf.Abs(x - (w - 24)) < 6 && Mathf.Abs(y - (h - 24)) < 6) c = pin;
                        pixels[y * w + x] = c;
                    }
                }
                simulatedTexture.SetPixels(pixels);
                simulatedTexture.Apply();
            }

            mapImage.texture = simulatedTexture;
            mapImage.color = Color.white;
        }

        private void OnDestroy()
        {
            if (fetchCoroutine != null) StopCoroutine(fetchCoroutine);
            if (simulatedTexture != null)
            {
                Destroy(simulatedTexture);
                simulatedTexture = null;
            }
        }
    }
}
