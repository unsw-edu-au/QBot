import { Component, OnInit, Inject } from '@angular/core';
import { Http, RequestOptions, Headers } from '@angular/http';

import { Question } from '../models/Question';
import { User } from '../models/User';

import * as microsoftTeams from '@microsoft/teams-js'
import * as AdaptiveCards from "adaptivecards";
import * as markdownit from "markdown-it";
import * as moment from 'moment-timezone';
import { AppService } from '../app.service';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from '../auth.service';
import { environment } from 'src/environments/environment';


@Component({
    selector: 'home',
    templateUrl: './home.component.html'
})
export class HomeComponent implements OnInit {
    appService: AppService;
    route: ActivatedRoute;

    questions: Question[];
    unansweredQuestions: Question[];
    answeredQuestions: Question[];

    currentUser: User;
    currentTopic: string;

    answeredContentVisible: boolean = true;
    unansweredContentVisible: boolean = true;

    isIframe: boolean;

    constructor(appService: AppService, route: ActivatedRoute, public authService: AuthService) {
        this.authService.onAuthUpdate(this.populateQuestions.bind(this));

        this.appService = appService;
        this.route = route;
        this.isIframe = window !== window.parent && !window.opener;

        var upnParam = this.route.snapshot.queryParamMap.get("upn");
        if (upnParam)
            environment.upn = upnParam
        //console.log("upn: " + upn);
        var tidParam = this.route.snapshot.queryParamMap.get("tid");
        if (tidParam)
            environment.tid = tidParam
        //console.log("tid: " + tid);
        var gidParam = this.route.snapshot.queryParamMap.get("gid");
        if (gidParam)
            environment.gid = gidParam
        //console.log("gid: " + gid);
        var cnameParam = this.route.snapshot.queryParamMap.get("cname");
        if (cnameParam)
            environment.cname = cnameParam
        //console.log("cname: " + cname);

        authService.getToken();
    }

    ngOnInit(): void {
        

        

        // get teams context
        // initialize teams


    }

    populateQuestions() {
        //alert("loggedIn: " + this.authService.loggedIn);

        this.appService.setHeaders();

        this.appService.getUserByUpn(environment.upn).subscribe(result => {
            this.currentUser = result;
        }, error => console.error(error));

        // get questions
        this.appService.getQuestionsByGroup(environment.gid).subscribe(result => {
            this.questions = result;

            if (environment.cname.toLowerCase() == "general") {
                this.currentTopic = "All";
            } else {
                this.currentTopic = environment.cname;
                this.questions = result.filter(x => x.topic.trim().toLowerCase() == this.currentTopic.toLowerCase());
            }

            this.unansweredQuestions = this.questions
                .filter(x => x.questionStatus.trim() == "unanswered")
                .sort((a, b) => { return new Date(b.questionSubmitted).getTime() - new Date(a.questionSubmitted).getTime(); });
            this.answeredQuestions = this.questions
                .filter(x => x.questionStatus.trim() == "answered")
                .sort((a, b) => { return new Date(b.questionAnswered).getTime() - new Date(a.questionAnswered).getTime(); });

            // create cards --- performance??
            var cardHolderDiv;

            /*** UNANSWERED ***/
            var unansweredFragment = document.createDocumentFragment();

            if (this.unansweredQuestions.length > 0) {
                // iterate over questions
                this.unansweredQuestions.forEach(q => {
                    cardHolderDiv = document.createElement("div");
                    cardHolderDiv.setAttribute("id", "unansweredCardHolder");
                    cardHolderDiv.setAttribute("class", "cardHolder");
                    cardHolderDiv.appendChild(this.createCard(q));
                    unansweredFragment.appendChild(cardHolderDiv);
                });
            } else {
                cardHolderDiv = document.createElement("div");
                cardHolderDiv.setAttribute("class", "cardHolder");
                cardHolderDiv.innerHTML = "<div style=\"padding: 10px;\">There are no unanswered questions.</div>";
                unansweredFragment.appendChild(cardHolderDiv);
            }

            // And finally insert it somewhere in your page:
            document.getElementById("unansweredCardDiv").appendChild(unansweredFragment);

            /*** ANSWERED ***/
            var answeredFragment = document.createDocumentFragment();

            if (this.answeredQuestions.length > 0) {
                // iterate over questions
                this.answeredQuestions.forEach(q => {
                    cardHolderDiv = document.createElement("div");
                    cardHolderDiv.setAttribute("id", "answeredCardHolder")
                    cardHolderDiv.setAttribute("class", "cardHolder")
                    cardHolderDiv.appendChild(this.createCard(q));
                    answeredFragment.appendChild(cardHolderDiv);
                });
            } else {
                cardHolderDiv = document.createElement("div");
                cardHolderDiv.setAttribute("class", "cardHolder");
                cardHolderDiv.innerHTML = "<div style=\"padding: 10px;\">There are no answered questions.</div>";
                answeredFragment.appendChild(cardHolderDiv);
            }
            // And finally insert it somewhere in your page:
            document.getElementById("answeredCardDiv").appendChild(answeredFragment);


        }, error => console.error(error));
    }

    createCard(question: Question): any {
        let answerPosterName = "";
        if (question.answerPoster != null)
            answerPosterName = question.answerPoster.fullName;

        var cardUnanswered = {
            "$schema": "https://adaptivecards.io/schemas/adaptive-card.json",
            "type": "AdaptiveCard",
            "version": "1.0",
            "style": "emphasis",
            "body": [
                {
                    "type": "Container",
                    "items": [
                        {
                            "type": "ColumnSet",
                            "columns": [
                                {
                                    "type": "Column",
                                    "width": "stretch",
                                    "items": [
                                        {
                                            "type": "TextBlock",
                                            "text": question.topic,
                                            "size": "medium"
                                        },
                                        {
                                            "type": "TextBlock",
                                            "text": question.questionText,
                                            "weight": "bolder",
                                            "wrap": true,
                                            "size": "medium"
                                        },
                                        {
                                            "type": "TextBlock",
                                            "text": question.answerText,
                                            "weight": "bolder",
                                            "wrap": true,
                                            "size": "medium",
                                            "color": "good"
                                        }
                                    ]
                                },
                                {
                                    "type": "Column",
                                    "width": "auto",
                                    "items": [
                                        {
                                            "type": "ColumnSet",
                                            "columns": [
                                                {
                                                    "type": "Column",
                                                    "width": "auto",
                                                    "items": [
                                                        {
                                                            "type": "TextBlock",
                                                            "text": "Submitted: ",
                                                            "weight": "bolder",
                                                            "wrap": true
                                                        }
                                                    ]
                                                },
                                                {
                                                    "type": "Column",
                                                    "width": "auto",
                                                    "items": [
                                                        {
                                                            "type": "TextBlock",
                                                            "text": question.originalPoster.fullName,
                                                            "weight": "bolder",
                                                            "wrap": true
                                                        },
                                                        {
                                                            "type": "TextBlock",
                                                            "spacing": "none",
                                                            "text": moment.utc(question.questionSubmitted).local().format("DD MMM YYYY, hh:mm a"),
                                                            "isSubtle": true,
                                                            "wrap": true
                                                        }
                                                    ]
                                                }
                                            ]
                                        }
                                    ]
                                }
                            ]
                        }
                    ]
                }
            ],
            "actions": [
                {
                    "type": "Action.OpenUrl",
                    "title": "View Conversation",
                    "url": question.link
                }
            ]
        }

        var cardAnswered = {
            "$schema": "https://adaptivecards.io/schemas/adaptive-card.json",
            "type": "AdaptiveCard",
            "version": "1.0",
            "style": "emphasis",
            "body": [
                {
                    "type": "Container",
                    "items": [
                        {
                            "type": "ColumnSet",
                            "columns": [
                                {
                                    "type": "Column",
                                    "width": "stretch",
                                    "items": [
                                        {
                                            "type": "TextBlock",
                                            "text": question.topic,
                                            "size": "medium"
                                        },
                                        {
                                            "type": "TextBlock",
                                            "text": question.questionText,
                                            "weight": "bolder",
                                            "wrap": true,
                                            "size": "medium"
                                        },
                                        {
                                            "type": "TextBlock",
                                            "text": question.answerText,
                                            "weight": "bolder",
                                            "wrap": true,
                                            "size": "medium",
                                            "color": "good"
                                        }
                                    ]
                                },
                                {
                                    "type": "Column",
                                    "width": "auto",
                                    "items": [
                                        {
                                            "type": "ColumnSet",
                                            "columns": [
                                                {
                                                    "type": "Column",
                                                    "width": "auto",
                                                    "items": [
                                                        {
                                                            "type": "TextBlock",
                                                            "text": "Submitted: ",
                                                            "weight": "bolder",
                                                            "wrap": true
                                                        }
                                                    ]
                                                },
                                                {
                                                    "type": "Column",
                                                    "width": "auto",
                                                    "items": [
                                                        {
                                                            "type": "TextBlock",
                                                            "text": question.originalPoster.fullName,
                                                            "weight": "bolder",
                                                            "wrap": true
                                                        },
                                                        {
                                                            "type": "TextBlock",
                                                            "spacing": "none",
                                                            "text": moment.utc(question.questionSubmitted).local().format("DD MMM YYYY, hh:mm a"),
                                                            "isSubtle": true,
                                                            "wrap": true
                                                        }
                                                    ]
                                                }
                                            ]
                                        },
                                        {
                                            "type": "ColumnSet",
                                            "columns": [
                                                {
                                                    "type": "Column",
                                                    "width": "auto",
                                                    "items": [
                                                        {
                                                            "type": "TextBlock",
                                                            "text": "Answered:  ",
                                                            "weight": "bolder",
                                                            "wrap": true
                                                        }
                                                    ]
                                                },
                                                {
                                                    "type": "Column",
                                                    "width": "auto",
                                                    "items": [
                                                        {
                                                            "type": "TextBlock",
                                                            "text": answerPosterName,
                                                            "weight": "bolder",
                                                            "wrap": true
                                                        },
                                                        {
                                                            "type": "TextBlock",
                                                            "spacing": "none",
                                                            "text": moment.utc(question.questionAnswered).local().format("DD MMM YYYY, hh:mm a"),
                                                            "isSubtle": true,
                                                            "wrap": true
                                                        }
                                                    ]
                                                }
                                            ]
                                        }
                                    ]
                                }
                            ]
                        }
                    ]
                }
            ],
            "actions": [
                {
                    "type": "Action.OpenUrl",
                    "title": "View Conversation",
                    "url": question.link
                }
            ]
        }

        var cardUnansweredMobile = {
            "$schema": "https://adaptivecards.io/schemas/adaptive-card.json",
            "type": "AdaptiveCard",
            "version": "1.0",
            "style": "emphasis",
            "body": [
                {
                    "type": "Container",
                    "items": [
                        {
                            "type": "TextBlock",
                            "text": question.questionText,
                            "weight": "bolder",
                            "wrap": true,
                            "size": "small"
                        },
                        {
                            "type": "TextBlock",
                            "text": question.answerText,
                            "wrap": true,
                            "size": "small",
                            "color": "good"
                        },
                        {
                            "type": "ColumnSet",
                            "columns": [
                                {
                                    "type": "Column",
                                    "width": "auto",
                                    "items": [
                                        {
                                            "type": "TextBlock",
                                            "text": "Submitted: ",
                                            "weight": "bolder",
                                            "size": "small",
                                            "wrap": true
                                        }
                                    ]
                                },
                                {
                                    "type": "Column",
                                    "width": "auto",
                                    "items": [
                                        {
                                            "type": "TextBlock",
                                            "text": question.originalPoster.fullName,
                                            "size": "small",
                                            "wrap": true
                                        }
                                    ]
                                },
                                {
                                    "type": "Column",
                                    "width": "auto",
                                    "items": [
                                        {
                                            "type": "TextBlock",
                                            "spacing": "none",
                                            "size": "small",
                                            "text": moment.utc(question.questionSubmitted).local().format("DD MMM YYYY, hh:mm a"),
                                            "isSubtle": true,
                                            "wrap": true
                                        }
                                    ]
                                }
                            ]
                        }
                    ]
                }
            ],
            "actions": [
                {
                    "type": "Action.OpenUrl",
                    "title": "View Conversation",
                    "url": question.link
                }
            ]
        }

        var cardAnsweredMobile = {
            "$schema": "https://adaptivecards.io/schemas/adaptive-card.json",
            "type": "AdaptiveCard",
            "version": "1.0",
            "style": "emphasis",
            "body": [
                {
                    "type": "TextBlock",
                    "text": question.questionText,
                    "weight": "bolder",
                    "wrap": true,
                    "size": "small"
                },
                {
                    "type": "TextBlock",
                    "text": question.answerText,
                    "wrap": true,
                    "size": "small",
                    "color": "good"
                },
                {

                    "type": "ColumnSet",
                    "columns": [
                        {
                            "type": "Column",
                            "width": "auto",
                            "items": [
                                {
                                    "type": "TextBlock",
                                    "text": "Submitted: ",
                                    "weight": "bolder",
                                    "size": "small",
                                    "wrap": true
                                }
                            ]
                        },
                        {
                            "type": "Column",
                            "width": "auto",
                            "items": [
                                {
                                    "type": "TextBlock",
                                    "text": question.originalPoster.fullName,
                                    "size": "small",
                                    "wrap": true
                                }
                            ]
                        },
                        {
                            "type": "Column",
                            "width": "auto",
                            "items": [
                                {
                                    "type": "TextBlock",
                                    "spacing": "none",
                                    "size": "small",
                                    "text": moment.utc(question.questionSubmitted).local().format("DD MMM YYYY, hh:mm a"),
                                    "isSubtle": true,
                                    "wrap": true
                                }
                            ]
                        }
                    ]
                },
                {
                    "type": "ColumnSet",
                    "columns": [
                        {
                            "type": "Column",
                            "width": "auto",
                            "items": [
                                {
                                    "type": "TextBlock",
                                    "text": "Answered:  ",
                                    "weight": "bolder",
                                    "size": "small",
                                    "wrap": true
                                }
                            ]
                        },
                        {
                            "type": "Column",
                            "width": "auto",
                            "items": [
                                {
                                    "type": "TextBlock",
                                    "text": answerPosterName,
                                    "size": "small",
                                    "wrap": true
                                }
                            ]
                        },
                        {
                            "type": "Column",
                            "width": "auto",
                            "items": [
                                {
                                    "type": "TextBlock",
                                    "spacing": "none",
                                    "size": "small",
                                    "text": moment.utc(question.questionAnswered).local().format("DD MMM YYYY, hh:mm a"),
                                    "isSubtle": true,
                                    "wrap": true
                                }
                            ]
                        }
                    ]
                }
            ],
            "actions": [
                {
                    "type": "Action.OpenUrl",
                    "title": "View Conversation",
                    "url": question.link
                }
            ]
        }

        // Create an AdaptiveCard instance
        var adaptiveCard = new AdaptiveCards.AdaptiveCard();

        // Set its hostConfig property unless you want to use the default Host Config
        // Host Config defines the style and behavior of a card
        adaptiveCard.hostConfig = new AdaptiveCards.HostConfig({
            "choiceSetInputValueSeparator": ",",
            "supportsInteractivity": true,
            "fontFamily": "Segoe UI",
            "spacing": {
                "small": 3,
                "default": 8,
                "medium": 20,
                "large": 30,
                "extraLarge": 40,
                "padding": 20
            },
            "separator": {
                "lineThickness": 1,
                "lineColor": "#EEEEEE"
            },
            "fontSizes": {
                "small": 12,
                "default": 14,
                "medium": 17,
                "large": 21,
                "extraLarge": 26
            },
            "fontWeights": {
                "lighter": 200,
                "default": 400,
                "bolder": 600
            },
            "imageSizes": {
                "small": 40,
                "medium": 80,
                "large": 160
            },
            "containerStyles": {
                "default": {
                    "foregroundColors": {
                        "default": {
                            "default": "#333333",
                            "subtle": "#EE333333"
                        },
                        "dark": {
                            "default": "#000000",
                            "subtle": "#66000000"
                        },
                        "light": {
                            "default": "#FFFFFF",
                            "subtle": "#33000000"
                        },
                        "accent": {
                            "default": "#2E89FC",
                            "subtle": "#882E89FC"
                        },
                        "good": {
                            "default": "#54a254",
                            "subtle": "#DD54a254"
                        },
                        "warning": {
                            "default": "#e69500",
                            "subtle": "#DDe69500"
                        },
                        "attention": {
                            "default": "#cc3300",
                            "subtle": "#DDcc3300"
                        }
                    },
                    "backgroundColor": "#00000000"
                },
                "emphasis": {
                    "foregroundColors": {
                        "default": {
                            "default": "#333333",
                            "subtle": "#EE333333"
                        },
                        "dark": {
                            "default": "#000000",
                            "subtle": "#66000000"
                        },
                        "light": {
                            "default": "#FFFFFF",
                            "subtle": "#33000000"
                        },
                        "accent": {
                            "default": "#2E89FC",
                            "subtle": "#882E89FC"
                        },
                        "good": {
                            "default": "#54a254",
                            "subtle": "#DD54a254"
                        },
                        "warning": {
                            "default": "#e69500",
                            "subtle": "#DDe69500"
                        },
                        "attention": {
                            "default": "#cc3300",
                            "subtle": "#DDcc3300"
                        }
                    },
                    "backgroundColor": "#08000000"
                }
            },
            "actions": {
                "maxActions": 5,
                "spacing": "Default",
                "buttonSpacing": 10,
                "showCard": {
                    "actionMode": "Inline",
                    "inlineTopMargin": 16,
                    "style": "emphasis"
                },
                "preExpandSingleShowCardAction": false,
                "actionsOrientation": "Horizontal",
                "actionAlignment": "Right"
            },
            "adaptiveCard": {
                "allowCustomStyle": false
            },
            "imageSet": {
                "imageSize": "Medium",
                "maxImageHeight": 100
            },
            "factSet": {
                "title": {
                    "size": "Default",
                    "color": "Default",
                    "isSubtle": false,
                    "weight": "Bolder",
                    "warp": true
                },
                "value": {
                    "size": "Default",
                    "color": "Default",
                    "isSubtle": false,
                    "weight": "Default",
                    "warp": true
                },
                "spacing": 10
            }            // More host config options
        });

        // Set the adaptive card's event handlers. onExecuteAction is invoked
        // whenever an action is clicked in the card
        adaptiveCard.onExecuteAction = function (action: AdaptiveCards.OpenUrlAction) {
            window.open(action.url, "_blank");
        }

        // For markdown support
        AdaptiveCards.AdaptiveCard.onProcessMarkdown = function (text) { return markdownit().render(text); }


        // get screen size
        let width = (window.screen.width);
        if (width < 750) {
            // Parse the card payload
            if (question.questionAnswered)
                adaptiveCard.parse(cardAnsweredMobile);
            else
                adaptiveCard.parse(cardUnansweredMobile);
        } else {
            // Parse the card payload
            if (question.questionAnswered)
                adaptiveCard.parse(cardAnswered);
            else
                adaptiveCard.parse(cardUnanswered);
        }
         


       


        // Render the card to an HTML element:
        var renderedCard = adaptiveCard.render();

        return renderedCard;
    }

    showHideUnanswered(event: any) {
        this.unansweredContentVisible = !this.unansweredContentVisible;
    }

    showHideAnswered(event: any) {
        this.answeredContentVisible = !this.answeredContentVisible;
    }


    public SignIn(event: any) {
        this.authService.signIn();
    }

    public SignOut(event: any) {
        this.authService.signOut();
    }

}
