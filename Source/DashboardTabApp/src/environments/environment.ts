export const environment = {
    production: false,
    apiBaseUrl: "https://ngaelayqbot.azurewebsites.net/api/Request/",

    authConfig: {
        instance: "https://login.microsoftonline.com/",
        tenantId: "6b022901-4576-4f52-bac4-71fbe4e58816",
        clientId: "792b9694-3214-4360-969b-f435fd8eeae9",
        redirectUri: "/app-silent-end",
        cacheLocation: "localStorage",
        navigateToLoginRequestUrl: false,
        extraQueryParameters: "",
        popUp: true,
        popUpUri: "/app-silent-start",
        popUpWidth: 600,
        popUpHeight: 535
    },

    // do not populate the following:
    upn: "",
    tid: "",
};
