# Microsoft Teams QBot

| [Deployment Guide](Documentation/deployment-guide.md) | [Solution Overview & Architecture](Documentation/solution-overview.md) | [Troubleshooting](Documentation/Troubleshooting.md) | [Known Limitations](Documentation/Known-limitations.md) | [Taking it further](Documentation/Taking-it-further.md) | 
| ---- | ---- | ---- | ---- | ---- | ---- |


## Overview
QBot is a solution designed for classroom teaching scenarios which allow teachers, tutors and students to intelligently answer each other's questions within the Microsoft Teams collaboration platform. It leverages the power of Azure Cognitive Services, in particular QnA Maker to achieve this.

The solution was originally developed, and is currently deployed for the School of Mechanical and Manufacturing Engineering at the University of New South Wales (UNSW) in Sydney, Australia.

Once the app is deployed to a Team, a student can ask a question on the channel by @tagging QBot. QBot will respond with the correct answer, or tag a group of demonstrators allowing them to collaborate on a response. Accepted answers are subsequently used to train Qbot for future questions.

![](Documentation/images/demo.png)
## Get Started
Begin with the [Solution Overview](Documentation/solution-overview.md) to read about what the app does and how it works.

When you're ready to try out QBot, or to use it in your own organization, follow the steps in the [Deployment Guide](Documentation/deployment-guide.md).

## Feedback
Thoughts? Questions? Ideas? Share them with us [here](https://github.com/unsw-edu-au/QBot/issues/new)!

Please report bugs and other code issues [here](https://github.com/unsw-edu-au/QBot/issues/new).

## Taking it further
If you're looking to build on top of this opensource code, [here](Documentation/Taking-it-further.md) are some ideas.

## Legal notice
Please read the license terms applicable to this [here](https://github.com/unsw-edu-au/QBot/blob/master/LICENSE). In addition to these terms, you agree to the following. You are responsible for complying with all privacy and security regulations, as well as all internal privacy and security policies of your company. You must also include your own privacy statement and terms of use for the app if you choose to deploy or share it broadly. Finally, please note that this application includes functionality to opt-in/opt-out of participation. Usage of this functionality is entirely your choice. Use and management of any personal data collected is your responsibility. Microsoft will not have any access to this data through this app.

Any Microsoft trademarks and logos included in this repository are property of Microsoft and should not be reused, redistributed, modified, repurposed, or otherwise altered or used outside of this repository.

## Contributing
This project welcomes contributions and suggestions. Most contributions require you to agree to a Contributor License Agreement (CLA) declaring that you have the right to, and actually do, grant us the rights to use your contribution. 
