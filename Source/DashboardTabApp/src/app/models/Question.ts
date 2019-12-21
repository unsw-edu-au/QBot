import { User } from "./User";

export class Question {
    id: number = 0;
    teamId: string = "";
    teamName: string = "";
    conversationId: string = "";
    messageId: string = "";
    topic: string = "";
    questionStatus: string = "";
    questionText: string = "";
    originalPoster: User = new User();
    questionSubmitted: Date | undefined;
    answerText: string = "";
    answerPoster: User = new User();
    questionAnswered: Date | undefined;
    link: string = "";
}
