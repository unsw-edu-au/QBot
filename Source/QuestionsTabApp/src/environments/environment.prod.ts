export const environment = {
    production: false,
    apiBaseUrl: "https://qbotdemotesting.azurewebsites.net/api/Request/",
    selfUrl: "https://qbotdemotesting-questions.azurewebsites.net",

    authConfig: {
        instance: "https://login.microsoftonline.com/",
        tenantId: "7520da05-6887-4d9d-9876-45bbf886ac78",
        clientId: "62a0eea7-3c79-450e-8083-ab31364d45a3",
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
    gid: "",
    cname: ""
};
