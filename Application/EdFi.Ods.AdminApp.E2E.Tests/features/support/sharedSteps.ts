// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

import { Given } from "@cucumber/cucumber";
import { models, context } from "../management/setup";
import { validatePath } from "../management/validators";

Given("user is registered", async () => {
    await models.loginPage.navigate();
    if (!(await models.loginPage.hasRegisterButton())) {
        return;
    }
    await models.loginPage.fullRegistration(process.env.email, process.env.password);
});

Given("user is logged in", async () => {
    await models.loginPage.navigate();
    await models.loginPage.fullLogin(process.env.email, process.env.password);
    await context.storageState({ path: "./state/login-state.json" });
});

Given("setup is complete", async () => {
    if (models.firstTimeSetupPage.isOnPage) {
        await models.firstTimeSetupPage.continue(models.productImprovementPage.firstTimePath());
    }

    if (models.productImprovementPage.isOnPage) {
        await models.productImprovementPage.uncheckAnalyticsTag();
        await models.productImprovementPage.proceed();
    }
});

Given("it's on the {string} page", async (pageName: string) => {
    let currentPage;

    switch (pageName) {
        case "Activity":
            break;
        case "Log in":
            currentPage = models.loginPage;
            break;
        case "First Time Setup":
            currentPage = models.firstTimeSetupPage;
            break;
        case "Education Organizations":
            currentPage = models.edOrgsPage;
            break;
        case "Vendors":
            currentPage = models.vendorsPage;
            break;
        case "Applications":
            currentPage = models.applicationsPage;
            break;
        default:
            break;
    }

    if (currentPage) {
        await currentPage.navigate();
        validatePath(currentPage.path());
    }
});
