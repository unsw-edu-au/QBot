import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute, Router, NavigationEnd } from '@angular/router';
import { AppService } from '../app.service';
import { Question } from '../models/Question';
import { User } from '../models/User';

//import * as $ from 'jquery';
//import 'datatables.net';
//import * as microsoftTeams from '@microsoft/teams-js';
import * as AdaptiveCards from "adaptivecards";
import * as markdownit from "markdown-it";
import * as moment from 'moment-timezone';


@Component({
    selector: 'question',
    templateUrl: './question.component.html',
    styleUrls: ['./question.component.css']
})
export class QuestionComponent implements OnInit, OnDestroy {
    appService: AppService;
    route: ActivatedRoute;
    router: Router;
    navigationSubscription;

    groupId: string;

    //context: microsoftTeams.Context;

    questions: Question[];
    unansweredQuestions: Question[];
    answeredQuestions: Question[];

    filterContentVisible: boolean = false;
    plusminus_classname = "plusminus glyphicon glyphicon-plus-sign"
    topics: string[] = ["All"];
    selectedTopic: string = "All";

    answeredContentVisible: boolean = true;
    unansweredContentVisible: boolean = true;


    constructor(appService: AppService, route: ActivatedRoute, router: Router) {
        this.appService = appService;
        this.route = route;
        this.router = router;

        this.navigationSubscription = this.router.events.subscribe((e: any) => {
            // If it is a NavigationEnd event re-initalise the component
            if (e instanceof NavigationEnd) {
                this.loadQuestions();
            }
        });
    }

    ngOnInit() {
        console.log('ngOnInit');
    }

    ngOnDestroy() {
        if (this.navigationSubscription) {
            this.navigationSubscription.unsubscribe();
        }
    }

    loadQuestions() {
        try {
            this.groupId = this.route.snapshot.paramMap.get("id");
            console.log(this.groupId);
            this.appService.getTeamChannels(this.groupId)
                .subscribe(teamChannels => {
                    let newTopics = new Array<string>();
                    newTopics.push("All");
                    teamChannels.forEach(c => {
                        newTopics.push(c.displayName);
                    });

                    this.topics = newTopics;
            });

            this.appService.getQuestionsByGroup(this.groupId)
                .subscribe(questions => {
                    var cardHolderDiv;
                    this.questions = questions;

                    this.unansweredQuestions = this.questions
                        .filter(x => x.questionStatus.trim() == "unanswered")
                        .sort((a, b) => { return new Date(b.questionSubmitted).getTime() - new Date(a.questionSubmitted).getTime(); });
                    this.answeredQuestions = this.questions
                        .filter(x => x.questionStatus.trim() == "answered")
                        .sort((a, b) => { return new Date(b.questionAnswered).getTime() - new Date(a.questionAnswered).getTime(); });

                    // create cards --- performance??

                    /*** UNANSWERED ***/
                    var unansweredFragment = document.createDocumentFragment();
                    // iterate over questions
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
                    // Remove existing fragments
                    let myUnansweredNode = document.getElementById("unansweredCardDiv");
                    while (myUnansweredNode.firstElementChild) {
                        myUnansweredNode.removeChild(myUnansweredNode.firstElementChild);
                    }
                    // And finally insert it somewhere in your page:
                    myUnansweredNode.appendChild(unansweredFragment);

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
                        cardHolderDiv.innerHTML = "<div style=\"padding: 10px;\">There are no answered questions.</div>";
                        cardHolderDiv.setAttribute("class", "cardHolder");
                        answeredFragment.appendChild(cardHolderDiv);
                    }
                    // Remove existing fragments
                    let myAnsweredNode = document.getElementById("answeredCardDiv");
                    while (myAnsweredNode.firstElementChild) {
                        myAnsweredNode.removeChild(myAnsweredNode.firstElementChild);
                    }
                    // And finally insert it somewhere in your page:
                    myAnsweredNode.appendChild(answeredFragment);
                }, error => console.error(error));
            //});
        } catch (error) {
            console.error("try-catch: " + error);
        }
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

        // Parse the card payload
        if (question.questionAnswered)
            adaptiveCard.parse(cardAnswered);
        else
            adaptiveCard.parse(cardUnanswered);


        // Render the card to an HTML element:
        var renderedCard = adaptiveCard.render();

        return renderedCard;
    }

    /*********** BUTTONS ***********/

    public showHideUnanswered(event: any) {
        this.unansweredContentVisible = !this.unansweredContentVisible;
    }

    public showHideAnswered(event: any) {
        this.answeredContentVisible = !this.answeredContentVisible;
    }

    public showHideContent(event: any) {
        this.filterContentVisible = !this.filterContentVisible;

        if (this.filterContentVisible)
            this.plusminus_classname = "plusminus glyphicon glyphicon-minus-sign";
        else
            this.plusminus_classname = "plusminus glyphicon glyphicon-plus-sign";
    }

    public applyFilters(event: any) {
        this.appService.getQuestionsByGroup(this.groupId)
            .subscribe(questions => {
                var postfilter = questions;

                if (this.selectedTopic != "All")
                    postfilter = postfilter.filter(q => q.topic.trim() == this.selectedTopic.trim());

                //if (this.selectedStatus != "All") {
                //    console.log(this.selectedStatus.toLowerCase());
                //    console.log(postfilter[0]);
                //    postfilter = postfilter.filter(q => q.questionStatus.trim() == this.selectedStatus.toLowerCase().trim());
                //}

                this.questions = postfilter;

                this.unansweredQuestions = this.questions
                    .filter(x => x.questionStatus.trim() == "unanswered")
                    .sort((a, b) => { return new Date(b.questionSubmitted).getTime() - new Date(a.questionSubmitted).getTime(); });
                this.answeredQuestions = this.questions
                    .filter(x => x.questionStatus.trim() == "answered")
                    .sort((a, b) => { return new Date(b.questionAnswered).getTime() - new Date(a.questionAnswered).getTime(); });
                // create cards --- performance??


                /*** UNANSWERED ***/
                var unansweredFragment = document.createDocumentFragment();
                var cardHolderDiv;

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
                // Remove existing fragments
                let myUnansweredNode = document.getElementById("unansweredCardDiv");
                while (myUnansweredNode.firstElementChild) {
                    myUnansweredNode.removeChild(myUnansweredNode.firstElementChild);
                }
                // And finally insert it somewhere in your page:
                myUnansweredNode.appendChild(unansweredFragment);

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
                    cardHolderDiv.innerHTML = "<div style=\"padding: 10px;\">There are no answered questions.</div>";
                    cardHolderDiv.setAttribute("class", "cardHolder");
                    answeredFragment.appendChild(cardHolderDiv);
                }
                // Remove existing fragments
                let myAnsweredNode = document.getElementById("answeredCardDiv");
                while (myAnsweredNode.firstElementChild) {
                    myAnsweredNode.removeChild(myAnsweredNode.firstElementChild);
                }
                // And finally insert it somewhere in your page:
                myAnsweredNode.appendChild(answeredFragment);
            }, error => console.error(error));

    }

    public resetFilters(event: any) {
        this.selectedTopic = "All";
        //this.selectedStatus = "All";
    }
}


