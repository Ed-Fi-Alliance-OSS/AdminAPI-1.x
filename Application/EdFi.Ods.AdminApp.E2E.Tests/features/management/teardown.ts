// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

import { After, AfterAll } from "@cucumber/cucumber";
import { TestStepResultStatus } from "@cucumber/messages";
import { saveTrace, takeScreenshot } from "./functions";
import { page, browser, models, currentTest } from "./setup";

After(async (scenario) => {
    scenario.result?.status === TestStepResultStatus.PASSED
        ? await takeScreenshot("SUCCESS")
        : await takeScreenshot("FAIL");

    const steps = scenario.pickle.steps.map((step) => step.text);
    await cleanup(steps);

    await saveTrace();
});

AfterAll(() => {
    if (!page?.isClosed()) {
        browser.close();
    }
});

async function cleanup(steps: string[]): Promise<void> {
    try {
        if (
            currentTest.Scenario.match(".*Add local education agency$") ||
            (steps.includes("there's a local education agency added") &&
                !currentTest.Scenario.match(".*Delete local education agency$"))
        ) {
            await models.edOrgsPage.navigate();
            await models.edOrgsPage.deleteLEAFullSteps();
        }

        if (
            currentTest.Scenario.match(".*Add vendor$") ||
            (steps.includes("there's a vendor added") && !currentTest.Scenario.match(".*Delete vendor$"))
        ) {
            await models.vendorsPage.navigate();
            await models.vendorsPage.deleteVendorFullSteps();
        }
    } catch (error) {
        console.info(`Item to delete for scenario ${currentTest.Scenario} not found\n${error}`);
    }
}
