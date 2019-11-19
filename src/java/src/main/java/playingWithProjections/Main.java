package playingWithProjections;

public class Main {
    public static void main(String[] args) {
        String file = FilePathFrom(args);
        CountEvents projector = new CountEvents();
        new EventStore(projector::projection)
                .replay(file);

        System.out.printf("number of events: %d%n", projector.getResult());
    }

    private static String FilePathFrom(String[] args) {
        if (args.length < 1) throw new IllegalArgumentException("Please specify a file to replay");
        return args[0];
    }

    private static class CountEvents {
        private int counter = 0;

        int getResult() {
            return counter;
        }

        void projection(Event event) {
            counter++;
        }
    }
}
