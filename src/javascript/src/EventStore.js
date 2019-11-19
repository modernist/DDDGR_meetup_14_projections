const StreamArray = require('stream-json/streamers/StreamArray');
const fs = require('fs');

function EventStore(...projections) {
  const mapTimestamp = event => ({...event, timestamp: new Date(event.timestamp)});

  const project = event => projections.forEach(projection => projection(event));

  const replay = (filePath, callback) => {
    const jsonStream = StreamArray.withParser()
      .on('data', data => {
        project(mapTimestamp(data.value));
      })
      .on('end', () => {
        callback();
      });

    console.log(`reading events from ${filePath} ...`);
    fs.createReadStream(filePath).pipe(jsonStream.input);
  };
  return {replay}
}

module.exports = EventStore;
