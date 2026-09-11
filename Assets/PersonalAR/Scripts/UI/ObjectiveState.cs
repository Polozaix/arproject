namespace PersonalAR.UI
{
    // Session-only sample data; closing a view does not modify completion.
    public sealed class ObjectiveState
    {
        public string Title { get; }
        public string Description { get; }
        public bool Completed { get; private set; }
        public ObjectiveState(string title, string description)
        { Title = title; Description = description; }
        public void ToggleCompleted() { Completed = !Completed; }
    }
}
