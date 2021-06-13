//////////////////////////////////////////////////////////////
// Includes & constants
const express = require('express');
const pg = require('pg');
const PORT = process.env.PORT || 5000;
//////////////////////////////////////////////////////////////

//////////////////////////////////////////////////////////////
// Postgresql => Create a pool of 20 connections max
// Replace xxxxx with URI for local tests
const conString = process.env.DATABASE_URL || 'xxxxx';

const pool = new pg.Pool({
    connectionString: conString,
    ssl: {
        rejectUnauthorized: false
    },
    max: 20,
    idleTimeoutMillis: 30000,
    connectionTimeoutMillis: 2000,
});
//////////////////////////////////////////////////////////////

//////////////////////////////////////////////////////////////
// Postgresql method
const databaseExecute = async function (req) {
    const client = await pool.connect()
    const result = await client.query(req);
    await client.end();
    return result;
};
//////////////////////////////////////////////////////////////

//////////////////////////////////////////////////////////////
// Login restriction
const checkLogin = function (login) {
    return conString.includes(login);
}
//////////////////////////////////////////////////////////////

//////////////////////////////////////////////////////////////
// Express app
const app = express();
app.use(express.json());
//////////////////////////////////////////////////////////////

//////////////////////////////////////////////////////////////
// Express endpoints
app.get('/', function (req, res) {
    res.send('Hello World!');
});

app.get('/ping', function (req, res) {
    res.send('Pong');
});

app.get('/:login/test', async function (req, res) {
    // Dumb check to restrict users
    if (!checkLogin(req.params.login)) {
        return res.status(500).send('Unauthorized!');
    }

    const sql = "SELECT * FROM test";
    const result = await databaseExecute(sql);
    res.json(result.rows);
});
//////////////////////////////////////////////////////////////

//////////////////////////////////////////////////////////////
// Express start server
app.listen(PORT, function () {
    console.log(`Listening on ${PORT}`);
});
//////////////////////////////////////////////////////////////