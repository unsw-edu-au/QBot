export const environment = {
    production: false,
    apiBaseUrl: "https://myqbotakl.azurewebsites.net/api/Request/",

    authConfig: {
        instance: "https://login.microsoftonline.com/",
        tenantId: "f57ea879-3b6b-4408-bca3-b03becd4d4d1",
        clientId: "a4075f66-c061-48db-a05c-6779ef43f0c3",
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
