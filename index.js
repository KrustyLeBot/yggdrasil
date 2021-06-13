// Includes
const express = require('express');
const { Client } = require('pg');

// Constants
const PORT = process.env.PORT || 5000;

// Postgre client
const client = new Client({
    connectionString: process.env.DATABASE_URL,
    ssl: {
        rejectUnauthorized: false
    }
});

// Express App
const app = express();
app.use(express.json());

// Express endpoints
app.get('/', function (req, res) {
    res.send('Hello World!');
});

app.get('/ping', function (req, res) {

    client.query('SELECT * FROM test', (error, results) => {
        if (error) {
          throw error;
        }
        response.status(200).send(results.rows);
      });
});

app.listen(PORT, function () {
    console.log(`Listening on ${PORT}`);
});