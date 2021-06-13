const express = require('express')

const PORT = process.env.PORT || 5000
const app = express()

app.use(express.json())

app.get('/', function (req, res) {
    res.send('Hello World!');
});

app.get('/ping', function (req, res) {
    console.log('ping recu');
    res.send('Pong');
});

app.listen(PORT, function () {
    console.log(`Listening on ${ PORT }`);
});