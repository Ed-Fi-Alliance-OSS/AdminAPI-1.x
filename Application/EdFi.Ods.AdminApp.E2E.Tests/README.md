# Admin App End To End Tests

![e2e tests results](https://github.com/Ed-Fi-Alliance-OSS/Ed-Fi-ODS-AdminApp/actions/workflows/e2e.yml/badge.svg)

The E2E tests are UI Automation tests written in [CucumberJS](https://cucumber.io/) to be executed using [Playwright](https://playwright.dev/) with Typescript

# Execution

## Getting Started

- Install [Node](https://nodejs.org/en/download/) version 18.
- Have a running instance of ODS/API in Shared Instance Mode and Admin App.
    - This can be accomplished using [ODS Docker](https://github.com/Ed-Fi-Alliance-OSS/Ed-Fi-ODS-Docker)

## Setup

### Install

Navigate to /Application/EdFi.Ods.AdminApp.E2E.Tests/

Run `npm install` to get all dependencies

After the installation is successful, Playwright must be ready to execute the tests.

### Environment

Setup a .env file. This should be based on the .env.example file, with details such as:

- Admin App URL
- Username
- Password

Additionally, there are flags  such as:
- Headless: Run in Headed or Headless mode (default is Headless)
- Trace: Save the execution trace of the tests, read more about it here: [Trace Viewer](https://playwright.dev/docs/trace-viewer)
- Video: Record a video of the execution.

> **Note**
> The video recordings are ignored from the source code, but those are never automatically deleted from your folder, therefore, be in the lookup for any large amount of videos saved.
> Additionally, the video feature of playwright does not have an option to set file name, so, all videos will be saved in the folder and you'll need to find them by creation date.
> When possible, prefer traces over videos since the traces are organized by test and have more information than the video.


## Run

There are multiple commands to run the tests, that can be found in the script section of [package.json](package.json)

Some of the commands are:

`npm test`: Run all tests, except for the ones that have specific preconditions required, such as registering and first time setup.

`npm run sanity-test`: Run the sanity tests (labeled with @Sanity in the .feature files)

`npm run test-{x}`: Run only the test for a specified feature. For example: `npm run test-vendors` executes the vendor tests only.

`npm run html-report`: Run the tests and generates a report in HTML.

# Debug

The preferred method for debug is the integrated playwright inspector.

```
$env:PWDEBUG=1
npm run test
```

## Trace Viewer

To see the generated traces, locate the zip files located in /Application/EdFi.Ods.AdminApp.E2E.Tests/traces/. These tests are sorted by Feature and specific test. After finding the **trace.zip** file corresponding to the desired test, you can open it by running:

```
  npm run trace-viewer -- {trace.zip path}
```

Additionally, you can browse to https://trace.playwright.dev/ and select the trace.zip file to view it in the browser.

More info on debug: https://playwright.dev/docs/debug

# How to Add a new Test Case

1. Add the test definition to the .feature file.
2. Create the step definition in the /support folder.
3. Create a new Page Object in the /models folder, that inherits from adminAppPage.
4. Register the Page Object into the modelResolver.
5. Add calls to the Page Object methods from the step definition class.
6. To execute the added test only, add tag @WIP before the scenario definition, and run by executing `npm run test-wip`.

```
@WIP
Scenario: WIP Demo Scenario
```

# More Information

- [Architecture](./ARCHITECTURE.md)
- [Send results to Zephyr](./reports/reports.md)
