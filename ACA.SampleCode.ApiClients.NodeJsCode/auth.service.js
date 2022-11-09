const https = require('https');
const urlJoin = require('url-join');
const axios = require('axios').default.create({
  httpsAgent: new https.Agent({rejectUnauthorized: false}),
});

/**
 * Starting autgorization process (redirecting to OAuth2 provider)
 */
async function startAuthorization(req, res) {
  const {CLIENT_ID, AUTH_URL, SCOPE} = process.env; // Destruct `process.env` (values from .env file) object to get neccesery config values
  const {protocol, originalUrl, headers} = req;
  const orgUrl = `${protocol}://${headers.host}${originalUrl}`;

  // Redirect current request to OAuth2 provider
  res.redirect(
    `${AUTH_URL}?response_type=code&client_id=${CLIENT_ID}&redirect_uri=${urlJoin(
      `${protocol}://${headers.host}`,
      'oauth',
      'redirect',
      `?redirectTo=${orgUrl}`
    )}&scope=${SCOPE}`
  );
}

/**
 * Fetching access tokent from `ACA.API`
 */
async function getAccessToken(req) {
  const {CLIENT_ID, CLIENT_SECRET, TOKEN_URL} = process.env; // Destruct `process.env` object to get neccesery config values
  const {code, redirectTo} = req.query;
  const {protocol, headers} = req;
  const origin = `${protocol}://${headers.host}`;

  return axios.post(
    TOKEN_URL,
    new URLSearchParams({
      code,
      grant_type: 'authorization_code',
      redirect_uri: urlJoin(origin, 'oauth', 'redirect', `?redirectTo=${redirectTo}`),
      client_id: CLIENT_ID,
    }),
    {
      headers: {
        Accept: 'application/json',
        Authorization: `Basic ${Buffer.from(`${CLIENT_ID}:${CLIENT_SECRET}`).toString('base64')}`,
      },
    }
  );
}

/**
 * Authorization middleware check if current request is authorized. If not starting authorization process
 */
async function authMiddleware(req, res, next) {
  if (!req.session?.authorization) {
    await startAuthorization(req, res);
    return;
  }
  req.authorization = req.session.authorization;
  next();
}

module.exports = {
  getAccessToken,
  authMiddleware,
};
