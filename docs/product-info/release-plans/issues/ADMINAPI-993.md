# ADMINAPI-993 - Incorporate the OSSF scorecard in to AdminaApi repos

**Type**: Story

**Status**: Done

## Description
User Story
----------


As someone wanting to use Ed\-Fi software, I want to know about the Ed\-Fi Alliance's adherence to security best practices as defined by the Open Source Security Foundation (OSSF).


Acceptance Criteria
-------------------


1. Repository administrator (Ed\-Fi staff):
	1. Modify the code repository to use a Ruleset instead of Branch Protection
		1. Delete the existing branch protection (or at least change it to apply to a non\-existing branch, if nervous about losing the settings).
		2. Import the Ruleset created for LMS Toolkit, available in [Code Security Guidelines](https://techdocs.ed-fi.org/display/SDLC/Code+Security+Guidelines).
		3. Apply the rulest to main *and* patch\* to apply to branches created for patch releases.
	2. In the repository settings, look under "Code Security and Analysis". Enable the following
		1. Private vulnerability reporting (![](/images/icons/emoticons/warning.png) new)
		2. Dependency graph
		3. Dependabot
		4. Dependabot security updates
		5. Grouped security updates
		6. Secret Scanning
		7. Access to alerts: if you want the development team to be able to review alerts, then add the team name to the list granted access.
2. Copy [Data\-Management\-Service's scorecard.yml](https://github.com/Ed-Fi-Alliance-OSS/Data-Management-Service/blob/main/.github/workflows/scorecard.yml) into the repository.
3. After merge, look at the Security tab to see what was flagged by the scorecard. We won't try to resolve everything.
	1. Want to resolve the "Token Permission" issues, can create a new ticket for that or address immediately if time available.
	2. Want to add an appropriate CODEOWNERS file if not present.
	3. Want to add a SECURITY.md file like in [LMS Toolkit](https://github.com/Ed-Fi-Exchange-OSS/LMS-Toolkit/blob/main/SECURITY.md).
	4. For now, decision on requiring 2 reviewers is up to the product owner. Tech Team should discuss and see if we want a single answer for all repositories or continue making it optional.
	5. Not going to have fuzzing or CII\-Best\-Practices. Not going to fully fix CI\-Tests if lower score is because some PR's don't run tests. After all, why run unit tests if you're only modifying markdown files? But we might review later to see if there is a reasonable way to improve on that.
	6. Carefully review other alerts, discuss with Tech Team if in doubt about how to respond.


 


**Apply in both Admin API repositories**



