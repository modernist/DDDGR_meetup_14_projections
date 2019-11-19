using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace cli
{
    internal class EventStore
    {
        private readonly IEnumerable<Action<Event>> projections;

        public EventStore(IEnumerable<Action<Event>> projections)
        {
            this.projections = projections;
        }

        public void Replay(string filePath)
        {
            using (var stream = File.OpenText(filePath))
            using (var reader = new JsonTextReader(stream))
            {
                var serializer = new JsonSerializer();
                while (reader.Read())
                {
                    if (reader.TokenType == JsonToken.StartObject)
                    {
                        Project(serializer.Deserialize<Event>(reader));
                    }
                }
            }
        }

        private void Project(Event @event)
        {
            foreach (var projection in projections)
            {
                projection(@event);
            }
        }
    }
}