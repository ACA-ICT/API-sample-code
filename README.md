# API-sample-code

API Client sample code to use the ACA API

### ACA.SampleCode.ApiClients.NetCoreConsole

This folder contains sample code about how to call the ACA API from a .NET 5 console application

### ACA.SampleCode.ApiClients.NodeJsCode

This folder contains sample code about how to call the ACA API from a NodeJs Application over **expressJs**

##### An explanation of the [.env](./ACA.SampleCode.ApiClients.NodeJsCode/.env) config file

| Variable name  |    Type     | Explanation                                    |
| -------------- | :---------: | :--------------------------------------------- |
| PORT           | **integer** | Port on which the application is listening     |
| SESSION_SECRET | **string**  | Key used for signing and/or encrypting cookies |
| API_URL        | **string**  | Target ACA.API url                             |
| AUTH_URL       | **string**  | OAuth2 authorization ACA.API endpoint          |
| TOKEN_URL      | **string**  | OAuth2 token ACA.API endpoint                  |
| CLIENT_ID      | **string**  | OAuth2 client id _(defined by ACA team)_       |
| CLIENT_SECRET  | **string**  | OAuth2 client secret _(defined by ACA team)_   |
| SCOPE          | **string**  | Scope of authorization                         |

> ##### To start application:
>
> 1. in terminal run **_npm ci_** to install required dependecies
> 2. make sure all configuration enviroments are correct (look in **[.env](./ACA.SampleCode.ApiClients.NodeJsCode/.env)** file)
> 3. to run application run **_npm run start_** in terminal
> 4. now you can open borwser on address https://localhost:**{PORT}**/**{order_id}** (for example https://localhost:3000/20) to fetch order from ACA.API
