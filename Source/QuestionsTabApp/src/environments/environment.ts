export const environment = {
    production: false,
    apiBaseUrl: "https://qbotapidemo.azurewebsites.net/api/Request/",
    selfUrl: "https://qbotdemo-questions-tab.azurewebsites.net",

    authConfig: {
        instance: "https://login.microsoftonline.com/",
        tenantId: "f986a5cc-1665-4eb1-a645-cc46521cfbf2",
        clientId: "f7e0fa26-6301-4fb0-bdc8-a692f2ae94ea",
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
