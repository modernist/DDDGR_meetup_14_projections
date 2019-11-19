package playingWithProjections;

import com.fasterxml.jackson.core.JsonFactory;
import com.fasterxml.jackson.core.JsonParser;
import com.fasterxml.jackson.core.JsonToken;

import java.io.File;
import java.io.IOException;
import java.time.ZonedDateTime;
import java.util.HashMap;
import java.util.Map;
import java.util.function.Consumer;

public class EventStore {
    private Consumer<Event>[] projections;

    public EventStore(Consumer<Event>... projections) {
        this.projections = projections;
    }

    public void replay(String filePath) {
        System.out.printf("reading events from '%s' ...%n", filePath);
        File file = new File(filePath);
        try (JsonParser parser = new JsonFactory().createParser(file)) {
            if (parser.nextToken() == JsonToken.START_ARRAY) {
                Event event = null;
                while (true) {
                    JsonToken currentToken = parser.nextToken();
                    if (currentToken == JsonToken.END_ARRAY) break;

                    if (currentToken == JsonToken.START_OBJECT) {
                        event = new Event();
                    }
                    if (currentToken == JsonToken.END_OBJECT) {
                        project(event);
                        event = null;
                    }
                    if (currentToken == JsonToken.FIELD_NAME) {
                        fill(event, parser);
                    }
                }
            }
        } catch (IOException e) {
            e.printStackTrace();
        }
    }

    private void project(Event event) {
        for (Consumer<Event> projection : projections) {
            projection.accept(event);
        }
    }

    private void fill(Event event, JsonParser parser) throws IOException {
        if (parser.getCurrentName().equals("id")) {
            event.setId(readFieldData(parser));
        }
        if (parser.getCurrentName().equals("type")) {
            event.setType(readFieldData(parser));
        }
        if (parser.getCurrentName().equals("timestamp")) {
            event.setTimestamp(ZonedDateTime.parse(readFieldData(parser)));
        }
        if (parser.getCurrentName().equals("payload")) {
            event.setPayload(readPayload(parser));
        }
    }

    private Map<String, String> readPayload(JsonParser parser) throws IOException {
        Map<String, String> payload = new HashMap<>();
        while (true) {
            JsonToken currentToken = parser.nextToken();
            if (currentToken == JsonToken.END_OBJECT) break;
            if (currentToken == JsonToken.FIELD_NAME) {
                String fieldName = parser.getCurrentName();
                String fieldValue = readFieldData(parser);
                payload.put(fieldName, fieldValue);
            }
        }
        return payload;
    }

    private String readFieldData(JsonParser parser) throws IOException {
        parser.nextToken();
        return parser.getText();
    }

}
