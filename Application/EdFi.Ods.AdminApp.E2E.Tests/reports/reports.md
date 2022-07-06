These reports are generated when running the command

`npm run report`

Additionally, you can generate reports in other formats as specified here: [Cucumber-JS Formatters](https://github.com/cucumber/cucumber-js/blob/main/docs/formatters.md)

## Send results to Zephyr

To send the results to Zephyr, execute the script [send-test-results.ps1](../../../eng/send-test-results.ps1) specifiying the required parameters

Parameters:

- cycleName: Get the id for the given test cycle, if the name is not found, it will create a new cycle with the given name. Not required if cycleId is specified.
- cycleId: Test Cycle ID.
- taskName: Name of the task to be included in Zephyr.
- folderName: Name of the folder to group the tests.
- PersonalAccessToken: Token to send the results to zephyr.
- ProjectId
- AdminAppVersion: This must be an existing fix version on the Admin App Jira project. If version does not exist, will use value -1 (Unscheduled)
- ResultsFilePath: Location of results with format JUnit XML.

More information: [How to get IDs](https://support.smartbear.com/zephyr-squad-server/docs/api/how-to/get-ids.html)

Example:

```powershell
$parameters = @{
  cycleName = 'Automation Cycle'
  taskName = "Playwright Automation Task"
  folderName = "Playwright Automation Run"
}

.\send-test-results.ps1 -PersonalAccessToken PAT -ProjectId 1 -AdminAppVersion "2.4" -ResultsFilePath "..\playwright-results.xml" -ConfigParams $parameters	
```

When running the E2E Tests from main, the GitHub Action executes this script automatically.
