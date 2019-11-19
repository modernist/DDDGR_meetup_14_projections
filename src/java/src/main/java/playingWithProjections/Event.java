package playingWithProjections;

import java.time.ZonedDateTime;
import java.util.Map;

public class Event {
    private String type;
    public String getType() { return type; }
    public void setType(String type) { this.type = type; }

    private ZonedDateTime timestamp;
    public ZonedDateTime getTimestamp() { return timestamp; }
    public void setTimestamp(ZonedDateTime timestamp) { this.timestamp = timestamp; }

    private String id;
    public String getId() { return id; }
    public void setId(String id) { this.id = id; }

    private Map<String, String> payload;
    public Map<String, String> getPayload() { return payload; }
    public void setPayload(Map<String, String> payload) { this.payload = payload; }
}
