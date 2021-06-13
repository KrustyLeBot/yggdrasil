// Includes
const express = require('express');
const redis = require("redis");

// Constants
const PORT = process.env.PORT || 5000;

// Express App
const app = express();
app.use(express.json());

// Redis client
const client = redis.createClient(process.env.REDIS_URL, {
    tls: {
        rejectUnauthorized: false
    }
});

// Express endpoints
app.get('/', function (req, res) {
    res.send('Hello World!');
});

app.get('/ping', function (req, res) {
    console.log('ping recu');
    res.send('Pong');
});

app.get('/insert', async function (req, res) {
    client.set("foo","bar",function(){});
    res.send(JSON.stringify({result : "ok"}, null, 4));
});

app.get('/retrieve', async function (req, res) {

    client.get("foo", function (err, value) {
        res.send(JSON.stringify({ 'key': "foo", 'value': value }, null, 4));
    });
});

app.listen(PORT, function () {
    console.log(`Listening on ${PORT}`);
});