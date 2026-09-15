using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace PersonalAR.XR
{
    /// <summary>Reversible PC/Quest Link supersampling without changing shared quality assets.</summary>
    [DisallowMultipleComponent]
    public class HudRenderQuality : MonoBehaviour
    {
        [SerializeField, Range(1f, 1.5f)] private float desktopRenderScale = 1.25f;
        private RenderPipelineAsset previousOverride;
        private UniversalRenderPipelineAsset runtimePipeline;

        private void OnEnable()
        {
            // A standalone Quest has a different GPU budget from a PC driving Quest Link.
            if (Application.isMobilePlatform && !Application.isEditor) return;
            previousOverride = QualitySettings.renderPipeline;
            var source = (previousOverride != null ? previousOverride : GraphicsSettings.defaultRenderPipeline)
                as UniversalRenderPipelineAsset;
            if (source == null)
            {
                Debug.LogWarning("HUD quality requires an active URP asset.", this);
                return;
            }
            runtimePipeline = Instantiate(source);
            runtimePipeline.name = source.name + " (HUD HD runtime)";
            runtimePipeline.hideFlags = HideFlags.DontSave;
            ApplyQuality();
            QualitySettings.renderPipeline = runtimePipeline;
        }

        private void Update()
        {
            // Inspector changes apply on the main thread, including during Play Mode.
            if (runtimePipeline != null && !Mathf.Approximately(runtimePipeline.renderScale,
                    Mathf.Clamp(desktopRenderScale, 1f, 1.5f))) ApplyQuality();
        }

        private void ApplyQuality()
        {
            runtimePipeline.renderScale = Mathf.Clamp(desktopRenderScale, 1f, 1.5f);
            runtimePipeline.msaaSampleCount = 4;
        }

        private void OnDisable()
        {
            if (runtimePipeline == null) return;
            if (QualitySettings.renderPipeline == runtimePipeline)
                QualitySettings.renderPipeline = previousOverride;
            Destroy(runtimePipeline);
            runtimePipeline = null;
        }
    }
}
