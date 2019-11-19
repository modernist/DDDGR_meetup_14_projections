using System;
using System.Collections.Generic;
using System.Linq;

namespace cli
{
    internal interface IProjection
    {
        void Projection(Event @event);

        string Result { get; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var projections = new List<IProjection>()
            {
                new CountEvents(),
                new CountRegisteredUsers(),
                new CountRegisteredUsersPerMonth()
            };

            new EventStore(projections.Select<IProjection, Action<Event>>(p => p.Projection))
                .Replay(FilePathFrom(args));

            foreach (var projection in projections)
            {
                Console.WriteLine(projection.Result);
            }
        }

        private static string FilePathFrom(string[] args)
        {
            if (args.Length < 1) throw new ArgumentException("Please specify a file to replay");
            return args[0];
        }
    }

    internal class CountEvents : IProjection
    {
        private int _result;

        public void Projection(Event @event)
        {
            _result++;
        }

        public string Result => $"number of events: {_result}";
    }

    internal class CountRegisteredUsers : IProjection
    {
        private int _result;

        public void Projection(Event @event)
        {
            if (@event.Type.Equals("PlayerHasRegistered", StringComparison.InvariantCultureIgnoreCase))
            {
                _result++;
            }
        }

        public string Result => $"number of registered users: {_result}";
    }

    internal class CountRegisteredUsersPerMonth : IProjection
    {
        private Dictionary<string, int> _result = new Dictionary<string, int>(); // month -> count

        public void Projection(Event @event)
        {
            if (!@event.Type.Equals("PlayerHasRegistered", StringComparison.InvariantCultureIgnoreCase))
            {
                return;
            }

            var month = @event.Timestamp.ToString("yyyy-MM");
            if (_result.TryGetValue(month, out var currentValue))
            {
                _result[month] = currentValue + 1;
            }
            else
            {
                _result.Add(month, 1);
            }
        }

        public string Result => string.Join(Environment.NewLine,
            _result.Select(kvp => $"{kvp.Key} : {kvp.Value}").OrderBy(t => t));
    }
}