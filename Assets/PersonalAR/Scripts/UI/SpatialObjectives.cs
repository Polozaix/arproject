using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit.UI;

namespace PersonalAR.UI
{
    [DisallowMultipleComponent]
    public class SpatialObjectives : MonoBehaviour
    {
        [SerializeField] private Camera headCamera;
        [SerializeField] private TMP_Text fontSource;
        [SerializeField, Min(0.5f)] private float panelDistance = 1.2f;
        private readonly ObjectiveState[] objectives =
        {
            new ObjectiveState("Explore your workspace", "Open an objective and read its details.\n\nThis is your first spatial task organiser. The cards are sample content, ready for testing in Quest Link."),
            new ObjectiveState("Place a card in the room", "Hold the trigger over the card header and move your controller ray to reposition it. Release to leave it there.\n\nTurn away and back: the card should remain in the same place. A quick flick of the header dismisses it."),
            new ObjectiveState("Complete your first objective", "Use Mark complete below. Close the card, then reopen it from Objectives to check its status.\n\nClosing or tossing away a card does not delete or complete the task. Sample progress resets when Play Mode ends.")
        };
        private RectTransform list, detail, launcher;
        private TMP_Text detailTitle, detailBody, completeLabel, countLabel;
        private readonly TMP_Text[] rowLabels = new TMP_Text[3];
        private int selected;
        private TMP_FontAsset font;
        private static readonly Color Cyan = new Color(0.35f, 0.94f, 1f);
        private static readonly Color Ink = new Color(0.025f, 0.055f, 0.085f, 0.97f);

        private void Start()
        {
            if (headCamera == null || fontSource == null)
            { Debug.LogError("SpatialObjectives needs a head camera and font source.", this); enabled = false; return; }
            font = fontSource.font;
#if UNITY_EDITOR
            if (EventSystem.current != null && EventSystem.current.TryGetComponent<XRUIInputModule>(out var input))
                input.enableMouseInput = true;
#endif
            var canvas = GetComponent<Canvas>();
            if (canvas == null) { enabled = false; return; }
            if (GetComponent<TrackedDeviceGraphicRaycaster>() == null)
                gameObject.AddComponent<TrackedDeviceGraphicRaycaster>();
            // Standard UI rays support desktop testing; tracked rays support XR controllers.

            launcher = Button(transform, "Objectives launcher", "OBJECTIVES  /  3", new Vector2(0, 0), new Vector2(290, 62), ToggleList);
            launcher.anchorMin = launcher.anchorMax = new Vector2(0.5f, 0);
            launcher.pivot = new Vector2(0.5f, 0.5f);
            launcher.anchoredPosition = new Vector2(0, -32);
            list = Panel("Objectives panel");
            Label(list, "Section", "PERSONAL AR  /  OBJECTIVES", new Vector2(24, -18), new Vector2(470, 34), 19, Cyan);
            Button(list, "Close list", "X", new Vector2(560, -12), new Vector2(56, 48), () => list.gameObject.SetActive(false));
            countLabel = Label(list, "Progress", "", new Vector2(24, -74), new Vector2(590, 36), 25, Color.white);
            for (int i = 0; i < objectives.Length; i++)
            {
                int index = i;
                var row = Button(list, "Objective " + (i + 1), "", new Vector2(24, -128 - i * 76), new Vector2(592, 64), () => Open(index));
                rowLabels[i] = row.GetComponentInChildren<TMP_Text>();
            }
            Label(list, "Session note", "SAMPLE TASKS  /  progress lasts this session", new Vector2(24, -378), new Vector2(590, 28), 17, Cyan);
            detail = Panel("Objective detail card");
            var header = Box(detail, "Drag header", new Vector2(0, 0), new Vector2(640, 58), new Color(0.07f, 0.19f, 0.23f));
            Label(header, "Handle label", "HOLD TO MOVE  /  FLICK TO DISMISS", new Vector2(20, -12), new Vector2(520, 34), 18, Cyan);
            var handle = header.gameObject.AddComponent<SpatialPanelHandle>();
            handle.panel = detail;
            handle.dismissed = () => detail.gameObject.SetActive(false);
            Button(detail, "Close detail", "X", new Vector2(560, -6), new Vector2(56, 46), () => detail.gameObject.SetActive(false));
            detailTitle = Label(detail, "Title", "", new Vector2(24, -78), new Vector2(590, 58), 28, Color.white);
            detailBody = Label(detail, "Description", "", new Vector2(24, -150), new Vector2(592, 220), 23, new Color(0.85f, 0.92f, 0.95f));
            var complete = Button(detail, "Toggle completion", "", new Vector2(24, -385), new Vector2(300, 54), ToggleComplete);
            completeLabel = complete.GetComponentInChildren<TMP_Text>();
            Button(detail, "Back to objectives", "OBJECTIVES", new Vector2(350, -385), new Vector2(266, 54), ShowList);
            detail.sizeDelta = new Vector2(640, 465);
            Refresh();
            list.gameObject.SetActive(false);
            detail.gameObject.SetActive(false);
        }

        public void ToggleList()
        { if (list != null && list.gameObject.activeSelf) list.gameObject.SetActive(false); else ShowList(); }
        public void ShowList()
        { if (list == null) return; detail.gameObject.SetActive(false); Place(list); Refresh(); }
        public void Open(int index)
        {
            if (detail == null || index < 0 || index >= objectives.Length) return;
            selected = index;
            list.gameObject.SetActive(false);
            detailTitle.text = objectives[index].Title;
            detailBody.text = objectives[index].Description;
            Refresh(); Place(detail);
        }
        private void ToggleComplete() { objectives[selected].ToggleCompleted(); Refresh(); }
        private void Refresh()
        {
            int done = 0;
            for (int i = 0; i < objectives.Length; i++)
            {
                if (objectives[i].Completed) done++;
                rowLabels[i].text = (objectives[i].Completed ? "DONE  /  " : "0" + (i + 1) + "  /  ") + objectives[i].Title;
            }
            countLabel.text = done + " / " + objectives.Length + " complete";
            completeLabel.text = objectives[selected].Completed ? "MARK INCOMPLETE" : "MARK COMPLETE";
        }
        private void Place(RectTransform panel)
        {
            panel.SetPositionAndRotation(headCamera.transform.position + headCamera.transform.forward * panelDistance, headCamera.transform.rotation);
            panel.gameObject.SetActive(true);
        }
        private RectTransform Panel(string name)
        {
            var p = new GameObject(name, typeof(RectTransform), typeof(Canvas), typeof(GraphicRaycaster), typeof(TrackedDeviceGraphicRaycaster)).GetComponent<RectTransform>();
            p.sizeDelta = new Vector2(640, 430);
            p.localScale = Vector3.one * 0.0012f;
            var c = p.GetComponent<Canvas>(); c.renderMode = RenderMode.WorldSpace; c.worldCamera = headCamera;
            var bg = p.gameObject.AddComponent<Image>(); bg.color = Ink;
            return p;
        }
        private RectTransform Box(Transform parent, string name, Vector2 pos, Vector2 size, Color color)
        {
            var r = new GameObject(name, typeof(RectTransform), typeof(Image)).GetComponent<RectTransform>();
            r.SetParent(parent, false); r.anchorMin = r.anchorMax = r.pivot = new Vector2(0, 1);
            r.anchoredPosition = pos; r.sizeDelta = size; r.GetComponent<Image>().color = color; return r;
        }
        private TMP_Text Label(Transform parent, string name, string text, Vector2 pos, Vector2 size, float fontSize, Color color)
        {
            var t = new GameObject(name, typeof(RectTransform), typeof(TextMeshProUGUI)).GetComponent<TextMeshProUGUI>();
            t.rectTransform.SetParent(parent, false); t.rectTransform.anchorMin = t.rectTransform.anchorMax = t.rectTransform.pivot = new Vector2(0, 1);
            t.rectTransform.anchoredPosition = pos; t.rectTransform.sizeDelta = size;
            t.font = font; t.fontSize = fontSize; t.color = color; t.text = text; t.raycastTarget = false;
            return t;
        }
        private RectTransform Button(Transform parent, string name, string text, Vector2 pos, Vector2 size, UnityAction action)
        {
            var r = Box(parent, name, pos, size, new Color(0.055f, 0.14f, 0.19f, 0.98f));
            var button = r.gameObject.AddComponent<Button>(); button.targetGraphic = r.GetComponent<Image>();
            var colors = button.colors; colors.highlightedColor = new Color(0.45f, 0.95f, 1f); colors.pressedColor = new Color(0.2f, 0.7f, 0.8f); button.colors = colors;
            var textLabel = Label(r, "Label", text, new Vector2(14, -6), size - new Vector2(28, 12), 22, Color.white);
            textLabel.alignment = TextAlignmentOptions.MidlineLeft;
            button.onClick.AddListener(action); return r;
        }
        private void OnDisable()
        { if (list != null) list.gameObject.SetActive(false); if (detail != null) detail.gameObject.SetActive(false); if (launcher != null) launcher.gameObject.SetActive(false); }
        private void OnEnable() { if (launcher != null) launcher.gameObject.SetActive(true); }
        private void OnDestroy()
        { if (list != null) Destroy(list.gameObject); if (detail != null) Destroy(detail.gameObject); if (launcher != null) Destroy(launcher.gameObject); }
    }
}
