﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using Serilog;
using Common.Utils;

namespace Common
{
    public sealed class TrackingEventProvider
    {
        private TrackingEventProvider(Dictionary<EventId, TrackingEvent> dict)
        {
            _dict = dict;
        }

        private readonly Dictionary<EventId, TrackingEvent> _dict;

        private static readonly ILogger Logger = Log.ForContext<TrackingEventProvider>();

        private static readonly Lazy<TrackingEventProvider> lazy = new Lazy<TrackingEventProvider>(() => 
        {
            var path = CommonUtils.GetAbsoletePath("event_descriptions.json");
            Logger.Information($"Loading schema from : {path}");
            using (StreamReader r = new StreamReader(path))
            {
                string json = r.ReadToEnd();
                List<TrackingEvent> items = Newtonsoft.Json.JsonConvert.DeserializeObject<List<TrackingEvent>>(json);
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
