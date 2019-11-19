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
                new CountRegisteredUsers()
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
}