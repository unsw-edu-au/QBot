## QBot
QBot is a custom Teams app that was designed for an educational or classroom context. A student asks a question on the channel and and @tags QBot, either directly or contextually: for example, a student may write “I have a @question regarding the units used to measure force?”, or “I don’t know how to differentiate the function for bending moment for @question 2.14 of the tutorial problems”

QBot interprets the message, and checks QnA Maker knowledge base for the answer. If an answer is found, QBot will notify the student and respond to the original conversation thread with with an adaptive card.

Otherwise, QBot will record the question as "unanswered", whilst @tagging a tutor within the course. The tutor visits a custom "Questions" tab deployed to the Teams Channel, and inputs their answer. Note that other students can also collaborate and try to answer this question from their fellow classmates, however only the original poster, tutor or lecturer can "accept" the answer.

Once "accepted", QBot takes the answer and trains the QnA Maker knowledge base for future reference. This aims to progressively build a knowledge database of high quality successfully answered questions.


* [Solution overview](Solution-Overview)
  * [Known limitations](Known-limitations.md)
* Deploying the app
  * [Deployment guide](deployment-guide.md)
  * [Troubleshooting](Troubleshooting.md)
* [Extending QBot](Taking-it-further.md)