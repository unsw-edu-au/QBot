export const environment = {
    production: false,
    apiBaseUrl: "https://qbottesting15.azurewebsites.net/api/Request/",
    selfUrl: "https://qbottesting15-questions.azurewebsites.net",

    authConfig: {
        instance: "https://login.microsoftonline.com/",
        tenantId: "34b955cb-5ad9-4daf-b94d-9a234739b382",
        clientId: "11535dc3-2360-40f6-bb46-bac309dd2ee2",
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
