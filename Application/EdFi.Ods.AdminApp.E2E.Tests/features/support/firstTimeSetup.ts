// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

import { Then, When } from "@cucumber/cucumber";
import { ok } from "assert";
import { models } from "../management/setup";

When("clicking Continue", async () => {
    await models.firstTimeSetupPage.continue(models.productImprovementPage.firstTimePath());
});

Then("first time setup is successful", async () => {
    ok(
        models.productImprovementPage.isOnPage,
        `Page not expected. Current page is ${models.homePage.page.url()}`
    );
    ok(await models.productImprovementPage.hasPageTitle(), "Title not found");
});
