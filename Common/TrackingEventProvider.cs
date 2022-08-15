using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Common
{
    public sealed class TrackingEventProvider
    {
        private TrackingEventProvider(Dictionary<EventId, TrackingEvent> dict)
        {
            _dict = dict;
        }

        private readonly Dictionary<EventId, TrackingEvent> _dict;

        private static readonly Lazy<TrackingEventProvider> lazy = new Lazy<TrackingEventProvider>(() => 
        {
            using (StreamReader r = new StreamReader(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "event_descriptions.json")))
            {
                string json = r.ReadToEnd();
                List<TrackingEvent> items = JsonSerializer.Deserialize<List<TrackingEvent>>(json);
                return new TrackingEventProvider(items.ToDictionary(x => (EventId)x.Id, x => x));
            }
        });

        public static TrackingEventProvider Instance
        {
            get
            {
                return lazy.Value;
            }
        }

        public TrackingEvent GetEventDetail(EventId eventId)
        {
            return Instance._dict[eventId];
        }
    }
}
