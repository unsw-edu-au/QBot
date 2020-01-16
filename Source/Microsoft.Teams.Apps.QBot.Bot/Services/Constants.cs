namespace Microsoft.Teams.Apps.QBot.Bot.Services
{
    public static class Constants
    {
        public const string STUDENT_ROLE = "Student";
        public const string LECTURER_ROLE = "Lecturer";
        public const string DEMONSTRATOR_ROLE = "Demonstrator";

        public const string QUESTION_STATUS_ANSWERED = "answered";
        public const string QUESTION_STATUS_UNANSWERED = "unanswered";

        public const string ACTIVITY_SELECT_ANSWER = "SelectAnswer";
        public const string ACTIVITY_MARKED_ANSWERED = "MarkedAnswered";
        public const string ACTIVITY_BOT_HELPFUL = "BotHelpful";
        public const string ACTIVITY_BOT_NOT_HELPFUL = "BotUnhelpful";

        public const string QNA_HOST_KEY = "QnAServiceHostName";
        public const string QNA_ID_KEY = "QnAknowledgeBaseId";
        public const string QNA_ENDPOINT_KEY = "QnAEndpointKey";
        public const string BOT_ID_KEY = "BotId";
        public const string BASE_URL_KEY = "BaseUrl";
        public const string BOT_APPID_KEY = "MicrosoftAppId";
        public const string BOT_APPPASSWORD_KEY = "MicrosoftAppPassword";
        public const string AAD_AUTHORITY_KEY = "AADAuthority";
        public const string AAD_GRAPHRESOURCE_KEY = "AADGraphResource";
        public const string SERVICE_ACCOUNT_KEY = "AADServiceName";
        public const string SERVICE_ACCOUNT_PASSWORD_KEY = "AADServicePassword";
        public const string AAD_GRAPHROOT_KEY = "AADGraphRoot";
        public const string AAD_CLIENTID_KEY = "AADClientId";
        public const string AAD_CLIENTSECRET_KEY = "AADClientSecret";
        public const string AAD_PERMISSIONTYPE_KEY = "AADPermissionType";
    }
}