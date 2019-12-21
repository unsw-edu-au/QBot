// do not populate the following:
// - token
// - upn
// - tid
export const environment = {
    production: true,
    token: "",
    upn: "",
    tid: "",
    authConfig: {
        instance: "https://login.microsoftonline.com/",
        tenantId: "",
        clientId: "",
        redirectUri: "/app-silent-end",
        cacheLocation: "localStorage",
        navigateToLoginRequestUrl: false,
        extraQueryParameters: "",
        popUp: true,
        popUpUri: "/app-silent-start",
        popUpWidth: 600,
        popUpHeight: 535
    }
};
