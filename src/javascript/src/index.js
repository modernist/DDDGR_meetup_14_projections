const EventStore = require('./EventStore');

const CountEvents = function () {
  let counter = 0;
  return {
    projection: _ => counter++,
    result: () => { return counter }
  }
};

let fileName = process.argv.slice(2)[0];
let projector = new CountEvents();

new EventStore(projector.projection).replay(fileName, () => {
  console.log('number of events:', projector.result());
});
