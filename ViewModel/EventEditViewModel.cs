namespace Bake.ViewModel
{
    public class EventEditViewModel : EventCreateViewModel
    {
        public int PostId { get; set; }
        public int EventId { get; set; }
        public string? ExistingPhotoUrl { get; set; }
        public bool HasRegistrations { get; set; }
    }
}
