# Microsoft Teams QBot

| [Demo](Documentation/demo.md) | [Deployment Guide](Documentation/armtemplate-deployment-guide.md) | [Solution Overview](Documentation/solution-overview.md) | [Troubleshooting](Documentation/Troubleshooting.md) | [Limitations](Documentation/Known-limitations.md) | [Taking it further](Documentation/Taking-it-further.md) | 
| --- | --- | --- | --- | --- | --- |


## Overview
QBot is a solution designed for classroom teaching scenarios which allow teachers, tutors and students to intelligently answer each other's questions within the Microsoft Teams collaboration platform. It leverages the power of Azure Cognitive Services, in particular QnA Maker to achieve this.

The solution was originally developed, and is currently deployed for the School of Mechanical and Manufacturing Engineering at the University of New South Wales (UNSW) in Sydney, Australia.

Once the app is deployed to a Team, a student can ask a question on the channel by @tagging QBot. QBot will respond with the correct answer, or tag a group of demonstrators allowing them to collaborate on a response. Accepted answers are subsequently used to train Qbot for future questions.

See a quick, animated demo of the various app scenarios [here](Documentation/demo.md).

![](Documentation/images/QuestionDemo.gif)

## Get Started
Begin with the [Solution Overview](Documentation/solution-overview.md) to read about what the app does and how it works.

When you're ready to try out QBot, or to use it in your own organization, follow the steps in the [Deployment Guide](Documentation/armtemplate-deployment-guide.md).

## Feedback
Thoughts? Questions? Ideas? Share them with us [here](https://github.com/unsw-edu-au/QBot/issues/new)!

Please report bugs and other code issues [here](https://github.com/unsw-edu-au/QBot/issues/new).

## Taking it further
If you're looking to build on top of this opensource code, [here](Documentation/Taking-it-further.md) are some ideas.

## Legal notice
Please read the license terms applicable to this [here](https://github.com/unsw-edu-au/QBot/blob/master/LICENSE). 

This app template is provided under the [MIT License](https://github.com/unsw-edu-au/QBot/blob/master/LICENSE) terms. In addition to these terms, by using this app template you agree to the following:

* You, not UNSW or Microsoft will license the use of your app to users or organizations. You are responsible for complying with all applicable privacy and security regulations related to use, collection and handling of any personal data by your app. This includes obtaining any necessary consents and complying with all internal privacy and security policies of your organization if your app is developed to be sideloaded internally within your organization. Any standard consent flow provided in the app template is provided as an sample implementation only and you will need to make sure the correct consent flow is implemented in the app. 
* Use and management of any personal data collected is your responsibility. Microsoft will not have any access to this data through this app.
* Where applicable, you may be responsible for data related incidents or data subject requests for data collected through your app.
* Any trademarks or registered trademarks of Microsoft in the United States and/or other countries and logos included in this repository are the property of Microsoft, and the license for this project does not grant you rights to use any Microsoft names, logos or trademarks outside of this repository.  Microsoft’s general trademark guidelines can be found here.
* You understand this app template is not intended to substitute your own regulatory due diligence or make you or your app compliant with applicable regulations including but not limited to privacy, healthcare, employment, or financial regulations. 
* You must also include your own privacy statement and terms of use for the app if you choose to deploy or share it broadly.
* Any Microsoft trademarks and logos included in this repository are property of Microsoft and should not be reused, redistributed, modified, repurposed, or otherwise altered or used outside of this repository.
 
## Contributing
This project welcomes contributions and suggestions. Most contributions require you to agree to a Contributor License Agreement (CLA) declaring that you have the right to, and actually do, grant us the rights to use your contribution. 
