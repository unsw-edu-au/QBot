import { Tutorial } from "./Tutorial";

export class User {
    id: number = 0;
    studentId: string = "";
    firstName: string = "";
    lastName: string = "";
    userName: string = "";
    email: string = "";
    roleName: string = "";
    tutorialGroupsString: string = "";
    tutorialGroups: Tutorial[] = new Array<Tutorial>();
    fullName: string = "";
    isAdmin: boolean = false;
}
