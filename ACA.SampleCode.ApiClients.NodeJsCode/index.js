const fs = require('fs');
const https = require('https');

const express = require('express');
const bodyParser = require('body-parser');
const cors = require('cors');
const session = require('express-session');

const urlJoin = require('url-join');
const dotenv = require('dotenv');

const authService = require('./auth.service');
const ordersService = require('./order.service');

// Loading `.env` file to curent process
dotenv.config({path: '.env'});

/** INITIALIZE APPLICATION WITH `cors` and `session` (using `express.js`) */
async function initServer() {
  const app = express();
  app.use(bodyParser.json());
  app.use(cors());
  app.use(
    session({
      secret: process.env.SESSION_SECRET,
      resave: true,
      saveUninitialized: true,
      cookie: {secure: true},
    })
  );

  /** APP ROUTING REFINITION */
  app.get('/oauth/redirect', async (req, res) => {
    try {
      const {redirectTo} = req.query;

      const resp = await authService.getAccessToken(req);
      const {data} = resp;

      req.session.authorization = data;

      //redirect to previous request or to home if not specified
      res.redirect(redirectTo || '/');
    } catch (err) {
      res.sendStatus(500);
    }
  });

  app.get('/:id', authService.authMiddleware, async (req, res) => {
    try {
      const {id} = req.params; // destruct `request.query` object to get requested `orderId`

      if (!id) {
        res.sendStatus(404);
      }

      const {data} = await ordersService.getOrderById(id, req.authorization.access_token);

      res.status(200).json(data);
    } catch (err) {
      res.status(500).json({err: 'internal server error'});
    }
  });

  /** CONFUGURE HTTPS SERVER WITH VALID SSL CERTIFICATES */
  https
    .createServer(
      {
        key: fs.readFileSync(urlJoin(__dirname, 'certs', 'localhost.key')),
        cert: fs.readFileSync(urlJoin(__dirname, 'certs', 'localhost.crt')),
      },
      app
    )
    .listen(process.env.PORT || 3000, function () {
      console.log(`⚡️[server]: Server is running at https://localhost:${process.env.PORT || 3000}`);
    });
}

initServer();
