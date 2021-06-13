// Includes
const express = require('express');
const redis = require("redis");
const { promisify } = require("util");

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

const getAsync = promisify(client.get).bind(client);
const setAsync = promisify(client.set).bind(client);

// Express endpoints
app.get('/', function (req, res) {
    res.send('Hello World!');
});

app.get('/ping', function (req, res) {
    console.log('ping recu');
    res.send('Pong');
});

app.get('/insert', async function (req, res) {
    const ip = req.headers['x-forwarded-for'] || req.connection.remoteAddress;
    console.log('ping recu de ' + ip);

    const value = await getAsync(ip, "toto");
    console.log(value);

    res.end();
});

app.get('/retrieve', async function (req, res) {
    const ip = req.headers['x-forwarded-for'] || req.connection.remoteAddress;
    const value = await getAsync(ip);
    console.log(value);
    res.send(value);
});

app.listen(PORT, function () {
    console.log(`Listening on ${PORT}`);
});