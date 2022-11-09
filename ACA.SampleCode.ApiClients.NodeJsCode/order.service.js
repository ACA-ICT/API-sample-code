const https = require('https');
const urlJoin = require('url-join');
const axios = require('axios').default.create({
  httpsAgent: new https.Agent({rejectUnauthorized: false}),
});

/** ACA.API orders url part */
const ordersUrl = 'v1/orders/';

/**
 * Fetch `order` object from ACA.API
 * @param {*} orderId OrderId from request param
 * @param {*} accessToken Access token to autgorize request
 * @returns `Order` object received from ACA.API
 */
async function getOrderById(orderId, accessToken) {
  const {API_URL} = process.env;
  return axios.get(urlJoin(API_URL, ordersUrl, orderId), {
    headers: {
      Authorization: `Bearer ${accessToken}`,
    },
  });
}

module.exports = {
  getOrderById,
};
