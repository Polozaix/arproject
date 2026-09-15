using PersonalAR.UI;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace PersonalAR.Editor
{
    /// <summary>Isolated world-space canvas preview; never instantiates an XR rig.</summary>
    public sealed class HudPreviewWindow : EditorWindow
    {
        private ModularHud source;
        private ModularHud previewHud;
        private PreviewRenderUtility renderer;
        private bool showFocus, glass, showLibrary;
        private Color background = new Color(.15f, .18f, .21f, 1f);

        [MenuItem("PersonalAR/HUD Preview")]
        public static void Open()
        {
            var window = GetWindow<HudPreviewWindow>("HUD Preview");
            window.minSize = new Vector2(640, 430);
            window.FindSource();
            window.Show();
        }

        public static void Open(ModularHud hud)
        {
            var window = GetWindow<HudPreviewWindow>("HUD Preview");
            window.minSize = new Vector2(640, 430);
            window.source = hud;
            window.Rebuild();
            window.Show();
        }

        private void OnEnable()
        {
            EditorApplication.playModeStateChanged += OnPlayModeChanged;
            EditorApplication.delayCall += FindSource;
        }

        private void FindSource()
        {
            if (this == null || EditorApplication.isPlayingOrWillChangePlaymode) return;
            if (source == null)
                foreach (var candidate in Object.FindObjectsByType<ModularHud>(FindObjectsInactive.Include))
                    if (!EditorUtility.IsPersistent(candidate) && !UnityEditor.SceneManagement.EditorSceneManager.IsPreviewScene(candidate.gameObject.scene))
                    { source = candidate; break; }
            Rebuild();
        }

        private void Rebuild()
        {
            Cleanup();
            if (EditorApplication.isPlayingOrWillChangePlaymode || source == null || source.EditorPreviewFont == null) return;
            renderer = new PreviewRenderUtility();
            var camera = renderer.camera;
            camera.orthographic = true;
            camera.nearClipPlane = .01f;
            camera.farClipPlane = 10f;
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.transform.SetPositionAndRotation(new Vector3(0, 0, -2), Quaternion.identity);
            var host = new GameObject("HUD layout preview", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler));
            host.hideFlags = HideFlags.HideAndDontSave;
            renderer.AddSingleGO(host);
            var rect = host.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(1000, 600);
            rect.localScale = Vector3.one * .001f;
            var canvas = host.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.worldCamera = camera;
            canvas.additionalShaderChannels = AdditionalCanvasShaderChannels.TexCoord1 |
                AdditionalCanvasShaderChannels.Normal | AdditionalCanvasShaderChannels.Tangent;
            previewHud = host.AddComponent<ModularHud>();
            previewHud.BuildEditorPreview(source.EditorPreviewFont);
            previewHud.SetEditorPreviewOptions(showFocus, glass, showLibrary);
            Canvas.ForceUpdateCanvases();
            Repaint();
        }

        private void OnGUI()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                EditorGUILayout.HelpBox("Stop Play Mode to use this layout preview. Use Game view for runtime testing.", MessageType.Info);
                return;
            }
            EditorGUI.BeginChangeCheck();
            source = (ModularHud)EditorGUILayout.ObjectField("HUD source", source, typeof(ModularHud), true);
            if (EditorGUI.EndChangeCheck()) Rebuild();
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUI.BeginChangeCheck();
                showFocus = GUILayout.Toggle(showFocus, "Focus", EditorStyles.miniButton);
                glass = GUILayout.Toggle(glass, "Glass", EditorStyles.miniButton);
                showLibrary = GUILayout.Toggle(showLibrary, "Library", EditorStyles.miniButton);
                if (EditorGUI.EndChangeCheck() && previewHud != null)
                {
                    previewHud.SetEditorPreviewOptions(showFocus, glass, showLibrary);
                    Canvas.ForceUpdateCanvases();
                    Repaint();
                }
                if (GUILayout.Button("Refresh", EditorStyles.miniButton)) FindSource();
            }
            background = EditorGUILayout.ColorField("Test background", background);
            EditorGUILayout.HelpBox("Visual preview with sample values. Buttons and tracking run only in Play Mode. Preview changes are not saved to the scene.", MessageType.None);
            if (renderer == null || previewHud == null)
            {
                EditorGUILayout.HelpBox("Open PersonalARPrototype and assign its HUD component, then click Refresh. The HUD needs a font source.", MessageType.Info);
                return;
            }
            var area = GUILayoutUtility.GetRect(100, 10000, 100, 10000, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            if (Event.current.type != EventType.Repaint || area.width < 1 || area.height < 1) return;
            var camera = renderer.camera;
            camera.backgroundColor = background;
            camera.orthographicSize = Mathf.Max(.34f, .54f / (area.width / area.height));
            renderer.BeginPreview(area, GUIStyle.none);
            renderer.Render(true);
            var image = renderer.EndPreview();
            GUI.DrawTexture(area, image, ScaleMode.StretchToFill, false);
        }

        private void OnPlayModeChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingEditMode) Cleanup();
            if (state == PlayModeStateChange.EnteredEditMode) FindSource();
            Repaint();
        }

        private void Cleanup()
        {
            // PreviewRenderUtility owns the preview scene and everything added to it.
            renderer?.Cleanup();
            renderer = null;
            previewHud = null;
        }

        private void OnDisable()
        {
            EditorApplication.delayCall -= FindSource;
            EditorApplication.playModeStateChanged -= OnPlayModeChanged;
            Cleanup();
        }
    }

    [CustomEditor(typeof(ModularHud))]
    public sealed class ModularHudEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            using (new EditorGUI.DisabledScope(EditorApplication.isPlayingOrWillChangePlaymode))
                if (GUILayout.Button("Open HUD Preview")) HudPreviewWindow.Open((ModularHud)target);
        }
    }
}
