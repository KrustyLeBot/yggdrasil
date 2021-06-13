// Includes
const express = require('express');
const redis = require("redis");
const fs = require("fs");

// Constants
const PORT = process.env.PORT || 5000;

// Redis client
const client = redis.createClient(process.env.REDIS_URL);

client.on('connect', () => {
    console.log('##########################################################');
    console.log('#####            REDIS STORE CONNECTED               #####');
    console.log('##########################################################\n');
});

client.on("error", function (err) {
    console.log("Error " + err);
});

// Express App
const app = express();
app.use(express.json());

// Express endpoints
app.get('/', function (req, res) {
    res.send('Hello World!');
});

app.get('/ping', function (req, res) {
    console.log('ping recu');
    res.send('Pong');
});

app.get('/insert', async function (req, res) {
    client.set("foo", "bar", function () { });
    res.send("Ok");
});

app.get('/retrieve', async function (req, res) {
    const key = "foo";

    client.get(key, async (err, value) => {
        if (value) {
            return res.status(200).send({
                error: false,
                message: `Value for ${key} from the cache`,
                data: JSON.parse(value)
            });
        }
        else {
            return res.status(200).send({
                error: true,
                message: `Key not found`,
                data: JSON.parse(value)
            });
        }
    })
});

app.listen(PORT, function () {
    console.log(`Listening on ${PORT}`);
});