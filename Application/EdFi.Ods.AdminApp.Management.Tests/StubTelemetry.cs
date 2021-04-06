using System.Collections.Generic;
using System.Threading.Tasks;
using EdFi.Ods.AdminApp.Web.Infrastructure;

namespace EdFi.Ods.AdminApp.Management.Tests
{
    public class StubTelemetry : ITelemetry
    {
        private readonly List<EventData> _events = new List<EventData>();

        public StubTelemetry()
        {
            _events = new List<EventData>();
        }

        public Task Event(string category, string action, string label)
        {
            _events.Add(new EventData
            {
                Category = category,
                Action = action,
                Label = label
            });

            return Task.CompletedTask;
        }

        public IReadOnlyList<EventData> Events => _events;

        public class EventData
        {
            public string Category { get; set; }
            public string Action { get; set; }
            public string Label { get; set; }
        }
    }
}
