import { After, AfterAll } from "@cucumber/cucumber";
import { saveLog, takeScreenshot } from "./functions";
import { page, browser } from "./setup";

After(async (scenario) => {
    if (scenario.result?.status.toString() === "FAILED") {
        await takeScreenshot("FAIL");
        if (scenario.result?.message) {
            saveLog(scenario.result.message);
        }
    }
});

AfterAll(() => {
    if (!page?.isClosed()) {
        browser.close();
    }
});
