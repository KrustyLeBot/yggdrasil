//////////////////////////////////////////////////////////////
// Includes & constants
const express = require('express');
const bodyParser = require('body-parser')

const PORT = process.env.PORT || 5000;
//////////////////////////////////////////////////////////////


//////////////////////////////////////////////////////////////
// Express app
const app = express();
app.use(express.json());
app.use(bodyParser.urlencoded({ limit: '50mb', extended: true }))
app.use(bodyParser.json({ limit: '50mb' }))
//////////////////////////////////////////////////////////////

//////////////////////////////////////////////////////////////
// Express endpoints
app.get('/*', (req, res) => {
  console.log(req.body)
  console.log(req.headers);
  res.send(req.body)
})

app.post('/*', (req, res) => {
  console.log(req.body)
  console.log(req.headers);
  res.send(req.body)
})

app.put('/*', (req, res) => {
  console.log(req.body)
  console.log(req.headers);
  res.send(req.body)
})

app.patch('/*', (req, res) => {
  console.log(req.body)
  console.log(req.headers);
  res.send(req.body)
})
//////////////////////////////////////////////////////////////

//////////////////////////////////////////////////////////////
// Express start server
app.listen(PORT, function () {
    console.log(`Listening on ${PORT}`);
});
//////////////////////////////////////////////////////////////
