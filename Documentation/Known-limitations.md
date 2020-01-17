## Azure Search Service Limits
Because each course requires a new QnA knowledge base, the number of Teams/courses you want to provision will be based on the chosen Azure Cognitive Seach tier. More information can be found at: https://docs.microsoft.com/en-us/azure/cognitive-services/qnamaker/limits


## Graceful handling when cannot find suitable users to tag
QBot works best in a collaborative environment, so it assumes that your course has lecturers and/or tutorial group demonstrators set up. If there are no suitable people to tag for unanswered questions, we can improve the message that the bot sends back
