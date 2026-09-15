using UnityEngine;

namespace PersonalAR.UI
{
    public enum HudSlot { TopLeft, Top, TopRight, Left, Focus, Right, BottomLeft, Bottom, BottomRight }

    /// <summary>A deployable UI root. Multiple modules coexist in distinct slots.</summary>
    [DisallowMultipleComponent]
    public class HudModule : MonoBehaviour
    {
        public string Id { get; private set; }
        public HudSlot Slot { get; private set; }
        public int Attention => Slot == HudSlot.Focus ? 4 :
            Slot == HudSlot.Top || Slot == HudSlot.Bottom ? 3 :
            Slot == HudSlot.Left || Slot == HudSlot.Right ? 2 : 1;

        public void Configure(string id, HudSlot slot) { Id = id; Slot = slot; }
        public void SetDeployed(bool deployed) => gameObject.SetActive(deployed);
    }
}
