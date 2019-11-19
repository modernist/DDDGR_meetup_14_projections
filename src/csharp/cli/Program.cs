using System;

namespace cli
{
    class Program
    {
        static void Main(string[] args)
        {
            var projector = new CountEvents();
            var registrationsProjector = new CountRegisteredUsers();

            new EventStore(e => projector.Projection(e), e => registrationsProjector.Projection(e))
                .Replay(FilePathFrom(args));

            Console.WriteLine("number of events: {0}", projector.Result);
            Console.WriteLine("number of registered users: {0}", registrationsProjector.Result);
        }

        private static string FilePathFrom(string[] args)
        {
            if (args.Length < 1) throw new ArgumentException("Please specify a file to replay");
            return args[0];
        }
    }

    internal class CountEvents
    {
        public int Result { get; private set; }

        public void Projection(Event @event)
        {
            Result++;
        }
    }

    internal class CountRegisteredUsers
    {
        public int Result { get; private set; }

        public void Projection(Event @event)
        {
            if (@event.Type.Equals("PlayerHasRegistered", StringComparison.InvariantCulture))
            {
                Result++;
            }
        }
    }
}