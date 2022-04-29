// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

import { Given, Then, When } from "@cucumber/cucumber";
import { ok } from "assert";
import { models } from "../management/setup";
import { validatePath } from "../management/validators";

Given("there are no users registered", async () => {
    ok(await models.loginPage.hasRegisterButton(), "There is a user already registered");
});

When("clicking on register as a new user", async () => {
    await models.loginPage.startRegistration();
});

When("user enters valid email and password", async () => {
    await models.loginPage.fillForm(process.env.email, process.env.password);
});

When("password confirmation", async () => {
    await models.loginPage.fillPasswordConfirm(process.env.password);
});

When("clicks Log in", async () => {
    await models.loginPage.login();
});

When("clicks sign out", async () => {
    await models.homePage.logout();
});

When("clicks Register", async () => {
    await models.loginPage.register();
});

When("user enters {string} for Log in", async (scenario: string) => {
    switch (scenario) {
        case "email only":
            await models.loginPage.fillEmail(process.env.email);
            break;
        case "wrong email":
            await models.loginPage.fillEmail("not-an-email");
            await models.loginPage.fillPassword(process.env.password);
            break;
        case "email not registered":
            await models.loginPage.fillEmail("wrong-email@test-ed-fi.org");
            await models.loginPage.fillPassword(process.env.password);
            break;
        case "password only":
            await models.loginPage.fillPassword(process.env.password);
            break;
        case "wrong password":
            await models.loginPage.fillEmail(process.env.email);
            await models.loginPage.fillPassword(process.env.password + "x");
            break;
        case "no data":
        default:
            break;
    }
});

Then("login is successful", async () => {
    if (models.homePage.isOnPage) {
        ok(await models.homePage.hasGlobalOption(), "Global option not found");
        ok(await models.homePage.hasSettingsOption(), "Settings option not found");
    } else if (models.firstTimeSetupPage.isOnPage) {
        ok(await models.firstTimeSetupPage.hasTitle(), "Page Title not found");
    } else {
        throw "Login failed, current page is: " + models.homePage.page.url();
    }
});

Then("logout is successful", async () => {
    ok(models.landingPage.isOnPage || models.loginPage.isOnPage, "Page not expected");
    if (models.landingPage.isOnPage) {
        ok(await models.landingPage.hasPageTitle(), "Page title not found");
    }
});

Then("registration is successful", async () => {
    validatePath(models.firstTimeSetupPage.path(), true);
    ok(await models.firstTimeSetupPage.hasTitle());
});

Then("validation errors for Log in scenario: {string} appears", async (scenario: string) => {
    const errors = await models.loginPage.getErrorMessages();

    switch (scenario) {
        case "email only":
            ok(
                errors?.includes(models.loginPage.errorMessages.missingPassword),
                `Password error message failed. Actual message: ${errors}`
            );
            break;
        case "wrong email":
            ok(
                errors?.includes(models.loginPage.errorMessages.wrongEmail),
                `Email error message failed. Actual message: ${errors}`
            );
            break;
        case "email not registered":
            ok(
                errors?.includes(models.loginPage.errorMessages.invalidLogin),
                `Email error message failed. Actual message: ${errors}`
            );
            break;
        case "password only":
            ok(
                errors?.includes(models.loginPage.errorMessages.missingEmail),
                `Email error message failed. Actual message: ${errors}`
            );
            break;
        case "wrong password":
            ok(
                errors?.includes(models.loginPage.errorMessages.invalidLogin),
                `Password error message failed. Actual message: ${errors}`
            );
            break;
        case "no data":
            ok(
                errors?.includes(models.loginPage.errorMessages.missingEmail),
                `Email error message failed. Actual message: ${errors}`
            );
            ok(
                errors?.includes(models.loginPage.errorMessages.missingPassword),
                `Password error message failed. Actual message: ${errors}`
            );
            break;
        default:
            break;
    }
});
