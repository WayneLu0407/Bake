namespace Bake.ViewModel
{
    public class MyHostedEventsViewModel
    {
        public int TotalCount { get; set; }
        public int PublishedCount { get; set; }
        public int UnpublishedCount { get; set; }

        public List<MyHostedEventItemViewModel> Items { get; set; } = new();
    }

    public class MyHostedEventItemViewModel
    {
        public int PostId { get; set; }
        public int EventId { get; set; }

        public string Title { get; set; } = string.Empty;
        public string? CoverImageUrl { get; set; }

        public string EventTypeName { get; set; } = string.Empty;

        public DateTime EventTime { get; set; }
        public DateTime EventEndTime { get; set; }
        public DateTime SignupStart { get; set; }
        public DateTime SignupDeadline { get; set; }

        public string LocationText { get; set; } = string.Empty;

        public int ParticipantCount { get; set; }
        public int MaxParticipants { get; set; }

        public bool IsPublished { get; set; }

        public string StatusText { get; set; } = string.Empty;
        public string BadgeClass { get; set; } = "secondary";
    }
}
