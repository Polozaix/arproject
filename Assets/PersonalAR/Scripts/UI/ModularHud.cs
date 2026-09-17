using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using PersonalAR.XR;

namespace PersonalAR.UI
{
    /// <summary>Shared head-following shell with the concept's four attention tiers.</summary>
    [RequireComponent(typeof(Canvas))]
    public class ModularHud : MonoBehaviour
    {
        [SerializeField] private TMP_Text fontSource;
        [SerializeField] private SpatialObjectives objectives;
        [Header("Play Mode inspection")]
        [Tooltip("Stop the refresh loop from rewriting clock, date, session and task text, so labels edited by hand in the Play Mode Hierarchy stay as typed. Live time and task counts freeze while this is on.")]
        [SerializeField] private bool pauseLiveTextUpdates;
        private readonly Dictionary<string, HudModule> modules = new Dictionary<string, HudModule>();
        private readonly Dictionary<HudSlot, HudModule> slots = new Dictionary<HudSlot, HudModule>();
        private TMP_Text clock, date, session, progress, focusBody, focusTitle;
        private TMP_Text focusToggle;
        private Transform root;
        private RectTransform library;
        private readonly Dictionary<string, TMP_Text> libraryLabels = new Dictionary<string, TMP_Text>();
        private readonly Dictionary<string, TMP_Text> headings = new Dictionary<string, TMP_Text>();
        private bool twelveHourClock, showDate = true;
        private UnityAction undoDismiss;
        private TMP_Text undoLabel;
        private int libraryPage;
        private bool glassMode;
        private readonly Dictionary<string, float> opaqueModeAlpha = new Dictionary<string, float>();
        private float nextTick, started;
        private TMP_FontAsset font;
        private NavigationWidget navigation;
        private static readonly Color Accent = new Color(.40f, .91f, .86f);
        private static readonly Color Muted = new Color(.59f, .72f, .76f);
        private static readonly Color White = new Color(.91f, .96f, .96f);

        private void Start()
        {
            if (fontSource == null) { Debug.LogError("Modular HUD requires a font source.", this); enabled = false; return; }
            font = fontSource.font;
            BuildHud();
            fontSource.gameObject.SetActive(false);
        }

        private void BuildHud()
        {
            root = Rect(transform, "Modular shell", Vector2.zero, new Vector2(1000, 600));
            var brand = Card("identity", HudSlot.TopLeft, "PERSONAL / AR");
            Label(brand, "Spatial workspace", 18, White, 16, 43, 202, 32, "workspace");
            var time = Card("clock", HudSlot.TopRight, "LOCAL TIME");
            clock = Label(time, "", 26, White, 16, 36, 148, 34, "time");
            date = Label(time, "", 16, Muted, 16, 67, 196, 24, "date");
            var elapsed = Card("session", HudSlot.BottomRight, "DEMO SESSION");
            session = Label(elapsed, "", 20, White, 16, 39, 196, 36, "elapsed");
            var top = Card("workspace", HudSlot.Top, "WORKSPACE  /  01");
            Label(top, "Make room for focus.", 28, White, 18, 37, 420, 44, "banner");
            var left = Card("objectives", HudSlot.Left, "01  /  OBJECTIVES");
            progress = Label(left, "", 26, White, 16, 47, 196, 44, "progress");
            Label(left, "A little progress,\none task at a time.", 18, Muted, 16, 98, 196, 66, "note");
            Action(left, "Open tasks", 16, 194, 196, () => objectives?.ShowList());
            var right = Card("controls", HudSlot.Right, "02  /  MODULES");
            Action(right, "Focus", 16, 44, 196, ToggleFocus);
            focusToggle = right.GetChild(right.childCount - 1).GetComponentInChildren<TMP_Text>();
            Action(right, "Nav route", 16, 98, 196, () => {
                bool show = !IsDeployed("navigation");
                SetDeployed("navigation", show);
            }, "toggle nav");
            Action(right, "Context on / off", 16, 152, 196, () => {
                bool show = !IsDeployed("workspace");
                SetDeployed("workspace", show); SetDeployed("activity", show);
            });
            Action(right, "Recenter HUD", 16, 206, 196, () => GetComponentInParent<SoftHeadFollow>()?.Recenter());
            var focus = Card("focus", HudSlot.Focus, "FOCUS  /  CURRENT OBJECTIVE");
            focusTitle = Label(focus, "", 30, White, 20, 52, 410, 85, "title");
            focusBody = Label(focus, "", 19, Muted, 20, 141, 410, 44, "body");
            Action(focus, "View objective", 20, 194, 250, () => {
                if (objectives != null) objectives.Open(objectives.NextIncomplete);
            });
            var bottom = Card("activity", HudSlot.Bottom, "SESSION PROGRESS");
            Label(bottom, "Sample objectives · saved for this session", 19, White, 18, 40, 420, 37, "note");
            BuildNavigationCard();
            SetDeployed("focus", false);
            focusToggle.text = "Show focus";
            BuildLibrary();
            started = Time.unscaledTime;
            Refresh();
        }

        private void BuildNavigationCard()
        {
            var nav = Card("navigation", HudSlot.Top, "03  /  NAVIGATION");
            navigation = nav.gameObject.AddComponent<NavigationWidget>();
            var maneuver = Label(nav, "▲", 26, Accent, 16, 38, 36, 40, "maneuver");
            maneuver.alignment = TextAlignmentOptions.Center;
            var instr = Label(nav, "Head north on University Way", 17, White, 56, 36, 260, 32, "instruction");
            var dist = Label(nav, "250 m", 14, Muted, 56, 68, 70, 22, "distance");
            var eta = Label(nav, "14 min", 14, Muted, 130, 68, 140, 22, "eta");
            Action(nav, ">", nav.sizeDelta.x - 110, 0, 48, () => navigation.NextStep(), "next step");
            navigation.BindUI(instr, maneuver, dist, eta, null);
            SetDeployed("navigation", false);
        }

#if UNITY_EDITOR
        public TMP_FontAsset EditorPreviewFont => fontSource != null ? fontSource.font : null;

        // Called only on an isolated preview object, never on the scene's live component.
        public void BuildEditorPreview(TMP_FontAsset previewFont)
        {
            if (root != null || previewFont == null) return;
            font = previewFont;
            BuildHud();
            clock.text = "10:24";
            date.text = "Tue, 15 Sep";
            session.text = "00:12:34";
            progress.text = "0 / 3 done";
            focusTitle.text = "Explore your workspace";
        }

        public void SetEditorPreviewOptions(bool showFocus, bool glass, bool showLibrary)
        {
            if (root == null) return;
            SetDeployed("focus", showFocus);
            SetGlassMode(glass);
            library.gameObject.SetActive(showLibrary);
            if (showLibrary) library.SetAsLastSibling();
        }
#endif

        // Register prefab content in the same shell. Duplicate IDs and invalid slots are rejected.
        public bool Register(string id, HudSlot slot, RectTransform content)
        {
            if (root == null || content == null || string.IsNullOrWhiteSpace(id) ||
                !Enum.IsDefined(typeof(HudSlot), slot) || modules.ContainsKey(id)) return false;
            content.SetParent(root, false);
            content.localRotation = Quaternion.identity; content.localScale = Vector3.one;
            content.anchorMin = content.anchorMax = content.pivot = new Vector2(.5f, .5f);
            Layout(slot, out var position, out var size);
            content.anchoredPosition = position; content.sizeDelta = size;
            var module = content.GetComponent<HudModule>() ?? content.gameObject.AddComponent<HudModule>();
            module.Configure(id, slot); modules.Add(id, module); slots[slot] = module;

            if (library != null) BuildLibraryRows();
            return true;
        }

        public bool SetDeployed(string id, bool deployed)
        {
            if (!modules.TryGetValue(id, out var module)) return false;
            if (deployed)
            {
                foreach (var pair in modules)
                {
                    if (pair.Key != id && pair.Value.Slot == module.Slot && pair.Value.gameObject.activeSelf)
                        pair.Value.SetDeployed(false);
                }
                slots[module.Slot] = module;
            }
            module.SetDeployed(deployed);
            if (!deployed) undoDismiss = () => SetDeployed(id, true);
            RefreshLibrary();
            return true;
        }

        public bool IsDeployed(string id) => modules.TryGetValue(id, out var module) && module.gameObject.activeSelf;

        /// <summary>Free a slot before registering a replacement prefab. The removed view is destroyed.</summary>
        public bool Unregister(string id)
        {
            if (!modules.TryGetValue(id, out var module)) return false;
            module.SetDeployed(false);
            if (slots.TryGetValue(module.Slot, out var current) && current == module)
                slots.Remove(module.Slot);
            modules.Remove(id); Destroy(module.gameObject);

            if (library != null) BuildLibraryRows();
            return true;
        }

        private void BuildLibrary()
        {
            // A separate permanent dock cannot be dismissed with an individual module.
            Action(root, "Modules", 28, 504, 112, () => {
                library.gameObject.SetActive(!library.gameObject.activeSelf);
                if (library.gameObject.activeSelf) { BuildLibraryRows(); library.SetAsLastSibling(); }
                RefreshLibrary();
            }, "dock: Modules");
            Action(root, "Undo", 148, 504, 108, () => {
                var restore = undoDismiss; undoDismiss = null; restore?.Invoke();
            }, "dock: Undo");
            undoLabel = root.GetChild(root.childCount - 1).GetComponentInChildren<TMP_Text>();
            library = Rect(root, "Module library", Vector2.zero, new Vector2(660, 540));
            var surface = library.gameObject.AddComponent<HudSurface>();
            surface.color = new Color(.025f, .055f, .075f, 1); surface.raycastTarget = true;
            BuildLibraryRows();
            library.gameObject.SetActive(false);
            undoDismiss = null;
            if (objectives != null) objectives.ViewDismissed += RememberDismissal;
        }

        private void BuildLibraryRows()
        {
            foreach (Transform child in library) { child.gameObject.SetActive(false); Destroy(child.gameObject); }
            libraryLabels.Clear();
            libraryPage = Mathf.Clamp(libraryPage, 0, Mathf.Max(0, (modules.Count - 1) / 8));
            Label(library, "YOUR MODULES", 22, Accent, 20, 12, 310, 36, "heading");
            if (modules.Count > 8)
            {
                Action(library, "Prev", 354, 8, 84, () => { libraryPage--; BuildLibraryRows(); }, "prev page");
                Action(library, "Next", 446, 8, 84, () => { libraryPage++; BuildLibraryRows(); }, "next page");
            }
            Action(library, "Close", 544, 8, 96, () => library.gameObject.SetActive(false), "close library");
            int index = 0;
            foreach (var pair in modules)
            {
                int pageIndex = index++ - libraryPage * 8;
                if (pageIndex < 0 || pageIndex >= 8) continue;
                string id = pair.Key;
                float x = 20 + pageIndex % 2 * 320, y = 60f + pageIndex / 2 * 94;
                libraryLabels[id] = Label(library, "", 16, White, x, y, 300, 30, "row: " + id);
                Action(library, "Show / hide", x, y + 32, 156, () => SetDeployed(id, !IsDeployed(id)), "row: " + id + " show / hide");
                Action(library, "Settings", x + 164, y + 32, 136, () => ShowSettings(id), "row: " + id + " settings");
            }
            Action(library, "Open tasks", 20, 476, 190, () => { library.gameObject.SetActive(false); objectives?.ShowList(); }, "open tasks");
            Action(library, "Dismiss all", 230, 476, 190, () => {
                var visible = new List<string>();
                foreach (var pair in modules) if (IsDeployed(pair.Key)) visible.Add(pair.Key);
                foreach (var id in new List<string>(modules.Keys)) SetDeployed(id, false);
                undoDismiss = null;
                objectives?.DismissAll();
                var restoreCard = undoDismiss;
                undoDismiss = () => { foreach (var id in visible) SetDeployed(id, true); restoreCard?.Invoke(); };
                library.gameObject.SetActive(false);
            }, "dismiss all");
            Action(library, "Restore all", 440, 476, 200, () => {
                foreach (var id in new List<string>(modules.Keys)) SetDeployed(id, true);
                library.gameObject.SetActive(false);
            }, "restore all");
            RefreshLibrary();
        }

        private void RememberDismissal(UnityAction restore) { undoDismiss = restore; }

        private void ShowSettings(string id)
        {
            if (!modules.TryGetValue(id, out var module)) return;
            foreach (Transform child in library) { child.gameObject.SetActive(false); Destroy(child.gameObject); }
            libraryLabels.Clear();
            Label(library, id.ToUpperInvariant() + " SETTINGS", 22, Accent, 20, 16, 480, 36, "settings heading");
            Action(library, "Back", 544, 8, 96, BuildLibraryRows, "back to modules");
            var surface = module.GetComponent<HudSurface>();
            if (surface != null)
            {
                Label(library, "Panel opacity: " + Mathf.RoundToInt(surface.color.a * 100) + "%", 20, White, 20, 74, 600, 32, "settings opacity");
                Action(library, "Glass", 20, 116, 145, () => SetOpacity(id, .35f), "opacity: glass");
                Action(library, "Light", 178, 116, 145, () => SetOpacity(id, .55f), "opacity: light");
                Action(library, "Balanced", 336, 116, 145, () => SetOpacity(id, .82f), "opacity: balanced");
                Action(library, "Solid", 494, 116, 146, () => SetOpacity(id, 1f), "opacity: solid");
            }
            if (headings.TryGetValue(id, out var heading) && heading != null && id != "navigation")
            {
                Label(library, "Module title · keyboard input", 18, Muted, 20, 190, 600, 30, "title hint");
                var fieldRect = Rect(library, "title field", new Vector2(20, -228), new Vector2(620, 54));
                fieldRect.anchorMin = fieldRect.anchorMax = fieldRect.pivot = new Vector2(0, 1);
                var bg = fieldRect.gameObject.AddComponent<HudSurface>(); bg.color = new Color(.10f, .25f, .28f, glassMode ? .40f : 1f);
                var viewport = Rect(fieldRect, "text viewport", Vector2.zero, new Vector2(596, 50));
                viewport.gameObject.AddComponent<RectMask2D>();
                var value = Label(viewport, heading.text, 20, White, 0, 0, 596, 50, "value");
                value.alignment = TextAlignmentOptions.MidlineLeft;
                var field = fieldRect.gameObject.AddComponent<TMP_InputField>();
                field.textViewport = viewport; field.textComponent = value; field.targetGraphic = bg;
                field.characterLimit = 40; field.text = heading.text;
                field.onEndEdit.AddListener(text => { if (!string.IsNullOrWhiteSpace(text) && heading != null) heading.text = text.Trim(); });
            }
            if (id == "navigation" && navigation != null)
            {
                Label(library, "DESTINATION: " + navigation.DestinationName + " (" + navigation.EtaText + ")", 17, Accent, 20, 175, 600, 26, "nav dest");
                Label(library, "Step " + (navigation.CurrentStepIndex + 1) + "/" + navigation.StepCount + ": " + navigation.CurrentStep.instruction, 16, White, 20, 204, 600, 26, "nav step");

                Action(library, "< Prev Turn", 20, 236, 140, () => { navigation.PrevStep(); ShowSettings(id); }, "prev turn");
                Action(library, "Next Turn >", 170, 236, 140, () => { navigation.NextStep(); ShowSettings(id); }, "next turn");

                var mapRect = Rect(library, "static map preview", new Vector2(320, -232), new Vector2(320, 72));
                mapRect.anchorMin = mapRect.anchorMax = mapRect.pivot = new Vector2(0, 1);
                var mapImg = mapRect.gameObject.AddComponent<RawImage>();
                navigation.BindUI(null, null, null, null, mapImg);

                Label(library, "Google Maps API Key (leave blank for offline simulation)", 15, Muted, 20, 314, 600, 22, "key hint");
                var keyRect = Rect(library, "api key field", new Vector2(20, -338), new Vector2(620, 44));
                keyRect.anchorMin = keyRect.anchorMax = keyRect.pivot = new Vector2(0, 1);
                var keyBg = keyRect.gameObject.AddComponent<HudSurface>();
                keyBg.color = new Color(.10f, .25f, .28f, glassMode ? .40f : 1f);
                var keyViewport = Rect(keyRect, "text viewport", Vector2.zero, new Vector2(596, 40));
                keyViewport.gameObject.AddComponent<RectMask2D>();
                var keyValue = Label(keyViewport, string.IsNullOrEmpty(navigation.ApiKey) ? "Paste Google Maps Key here..." : navigation.ApiKey, 16, White, 0, 0, 596, 40, "value");
                keyValue.alignment = TextAlignmentOptions.MidlineLeft;
                var keyField = keyRect.gameObject.AddComponent<TMP_InputField>();
                keyField.textViewport = keyViewport; keyField.textComponent = keyValue; keyField.targetGraphic = keyBg;
                keyField.characterLimit = 120; keyField.text = navigation.ApiKey;
                keyField.onEndEdit.AddListener(text => {
                    navigation.ApiKey = text?.Trim() ?? "";
                    ShowSettings(id);
                });
            }
            if (id == "clock")
            {
                Action(library, twelveHourClock ? "Format: 12 hour" : "Format: 24 hour", 20, 322, 300,
                    () => { twelveHourClock = !twelveHourClock; Refresh(); ShowSettings(id); }, "time format");
                Action(library, showDate ? "Date: visible" : "Date: hidden", 340, 322, 300,
                    () => { showDate = !showDate; Refresh(); ShowSettings(id); }, "date visibility");
            }
            if (id == "session")
                Action(library, "Restart session timer", 20, 322, 300, () => { started = Time.unscaledTime; Refresh(); }, "restart timer");
            if (id == "controls")
                Action(library, glassMode ? "AR glass mode: on" : "AR glass mode: off", 20, 322, 300,
                    () => { SetGlassMode(!glassMode); ShowSettings(id); }, "glass mode");
            if (id == "objectives" || id == "focus")
                Action(library, "Open tasks", 20, 322, 300, () => { library.gameObject.SetActive(false); objectives?.ShowList(); }, "open tasks");
            Label(library, "Changes stay for this session. Closing keeps your settings.", 18, Muted, 20, 414, 610, 48, "settings note");
            Action(library, "Done", 440, 476, 200, () => library.gameObject.SetActive(false), "done");
        }

        private void SetOpacity(string id, float opacity)
        {
            if (!modules.TryGetValue(id, out var module)) return;
            var surface = module.GetComponent<HudSurface>();
            if (surface != null) { var tint = surface.color; tint.a = opacity; surface.color = tint; }
            ShowSettings(id);
        }

        public void SetGlassMode(bool enabled)
        {
            if (glassMode == enabled) return;
            glassMode = enabled;
            foreach (var pair in modules)
            {
                var surface = pair.Value.GetComponent<HudSurface>();
                if (surface == null) continue;
                var tint = surface.color;
                if (enabled)
                {
                    opaqueModeAlpha[pair.Key] = tint.a;
                    tint.a = pair.Value.Slot == HudSlot.Focus ? .60f : .40f;
                }
                else if (opaqueModeAlpha.TryGetValue(pair.Key, out var original)) tint.a = original;
                surface.color = tint;
            }
            if (!enabled) opaqueModeAlpha.Clear();
            // Buttons are separate graphics; changing only module surfaces leaves them opaque.
            foreach (var button in root.GetComponentsInChildren<Button>(true))
            {
                var graphic = button.targetGraphic;
                if (graphic == null) continue;
                var tint = graphic.color; tint.a = enabled ? .40f : 1f; graphic.color = tint;
            }
            if (library != null)
            {
                var surface = library.GetComponent<HudSurface>();
                var tint = surface.color; tint.a = enabled ? .80f : 1f; surface.color = tint;
            }
            objectives?.SetPanelOpacity(enabled ? .65f : .97f);
        }



        private void RefreshLibrary()
        {
            foreach (var pair in libraryLabels)
                if (pair.Value != null && modules.TryGetValue(pair.Key, out var module))
                    pair.Value.text = (IsDeployed(pair.Key) ? "ON  " : "OFF  ") + pair.Key.ToUpperInvariant() + " / " + module.Slot;
            if (focusToggle != null) focusToggle.text = IsDeployed("focus") ? "Hide focus" : "Show focus";
            if (undoLabel != null) undoLabel.text = "Undo";
        }

        private void ToggleFocus()
        {
            bool show = !IsDeployed("focus");
            SetDeployed("focus", show);
            if (focusToggle != null) focusToggle.text = show ? "Hide focus" : "Show focus";
        }

        private void Update()
        {
            if (root == null || Time.unscaledTime < nextTick) return;
            // Left alone while inspecting in Play Mode, so hand-edited labels are not overwritten.
            if (pauseLiveTextUpdates) return;
            Refresh();
        }
        private void Refresh()
        {
            nextTick = Time.unscaledTime + .5f;
            var now = DateTime.Now;
            if (clock != null) clock.text = now.ToString(twelveHourClock ? "h:mm tt" : "HH:mm");
            if (date != null) { date.text = now.ToString("ddd, dd MMM"); date.gameObject.SetActive(showDate); }
            var elapsed = TimeSpan.FromSeconds(Time.unscaledTime - started);
            if (session != null) session.text = elapsed.ToString(@"hh\:mm\:ss");
            if (progress != null) progress.text = objectives == null ? "Unavailable" : objectives.CompletedCount + " / " + objectives.ObjectiveCount + " done";
            if (focusTitle != null) focusTitle.text = objectives == null ? "Your focus space" : objectives.NextTitle;
            if (focusBody != null) focusBody.text = "Open a task to explore, place or complete it.";
        }

        private RectTransform Card(string id, HudSlot slot, string heading)
        {
            var panel = Rect(root, id, Vector2.zero, Vector2.zero);
            Register(id, slot, panel);
            var surface = panel.gameObject.AddComponent<HudSurface>();
            surface.color = new Color(.025f, .055f, .075f, slot == HudSlot.Focus ? .94f : .82f);
            surface.raycastTarget = false;
            headings[id] = Label(panel, heading, 14, Accent, 16, 12, panel.sizeDelta.x - 84, 26, "heading");
            Action(panel, "X", panel.sizeDelta.x - 56, 0, 48, () => SetDeployed(id, false), "close (X)");
            return panel;
        }

        private static void Layout(HudSlot slot, out Vector2 position, out Vector2 size)
        {
            int index = (int)slot, column = index % 3, row = index / 3;
            position = new Vector2(column == 0 ? -358 : column == 2 ? 358 : 0, row == 0 ? 218 : row == 2 ? -218 : 0);
            size = new Vector2(column == 1 ? 456 : 228, row == 1 ? 268 : 96);
        }

        private static RectTransform Rect(Transform parent, string name, Vector2 pos, Vector2 size)
        {
            var r = new GameObject(name, typeof(RectTransform)).GetComponent<RectTransform>();
            r.SetParent(parent, false); r.anchoredPosition = pos; r.sizeDelta = size; return r;
        }

        private TMP_Text Label(Transform parent, string text, float size, Color color, float x, float y, float width, float height, string name = null)
        {
            var r = Rect(parent, ViewName(parent, name, text), new Vector2(x, -y), new Vector2(width, height));
            r.anchorMin = r.anchorMax = r.pivot = new Vector2(0, 1);
            var label = r.gameObject.AddComponent<TextMeshProUGUI>();
            label.font = font; label.text = text; label.fontSize = size; label.color = color;
            label.raycastTarget = false; label.overflowMode = TextOverflowModes.Ellipsis;
            return label;
        }

        // Runtime views are named after their parent so every element can be found and edited in the Play Mode Hierarchy.
        private static string ViewName(Transform parent, string name, string text)
        {
            string leaf = string.IsNullOrEmpty(name) ? Shorten(text) : name;
            return parent == null ? leaf : parent.name + " / " + leaf;
        }

        private static string Shorten(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return "label";
            var flat = string.Join(" ", text.Split(new[] { ' ', '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries));
            return flat.Length > 32 ? flat.Substring(0, 32).TrimEnd() : flat;
        }

        private void Action(Transform parent, string text, float x, float y, float width, UnityAction callback, string name = null)
        {
            var r = Rect(parent, ViewName(parent, name ?? text, null), new Vector2(x, -y), new Vector2(width, 48));
            r.anchorMin = r.anchorMax = r.pivot = new Vector2(0, 1);
            var bg = r.gameObject.AddComponent<HudSurface>(); bg.color = new Color(.10f, .25f, .28f, glassMode ? .40f : 1f);
            bg.raycastTarget = true;
            var button = r.gameObject.AddComponent<Button>(); button.targetGraphic = bg;
            var colors = button.colors; colors.highlightedColor = new Color(.65f, 1, .94f);
            colors.pressedColor = new Color(.4f, .72f, .7f); button.colors = colors;
            var nav = button.navigation; nav.mode = Navigation.Mode.None; button.navigation = nav;
            button.onClick.AddListener(callback);
            var label = Label(r, text, 18, White, 12, 0, width - 24, 48, "label");
            label.alignment = TextAlignmentOptions.MidlineLeft;
        }

        private void OnEnable() { if (root != null) root.gameObject.SetActive(true); }
        private void OnDisable() { if (root != null) root.gameObject.SetActive(false); }
        private void OnDestroy()
        {
            if (objectives != null) objectives.ViewDismissed -= RememberDismissal;
            if (root != null)
            {
                if (Application.isPlaying) Destroy(root.gameObject);
                else DestroyImmediate(root.gameObject);
            }
            if (fontSource != null) fontSource.gameObject.SetActive(true);
        }
    }
}
