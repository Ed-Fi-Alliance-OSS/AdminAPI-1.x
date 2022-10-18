// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

import { Given, Then, When } from "@cucumber/cucumber";
import { ok, strictEqual } from "assert";
import { models } from "../management/setup";

Given("there's an application added", async () => {
    await models.applicationsPage.navigate();
    if (await models.applicationsPage.isApplicationPresentOnPage()) {
        await models.applicationsPage.regenerateApplicationFullSteps();
        return Promise.resolve("Application is already added");
    }
    await models.applicationsPage.addApplicationFullSteps();
});

Given("applications page has loaded", async () => {
    await models.applicationsPage.waitForListLoad();
    ok(await models.applicationsPage.hasTabSelected(), "Applications tab not selected");
});

When("clicking API URL", async () => {
    await models.applicationsPage.clickURL();
});

When("adding new application", async () => {
    await models.applicationsPage.addApplication();
    await models.applicationsPage.waitForModalVisible();
});

When("filling application form", async () => {
    strictEqual(
        await models.applicationsPage.modalTitle(),
        models.applicationsPage.modalTitleMessages.addApplication,
        "Modal title not found"
    );
    await models.applicationsPage.fillApplicationForm();
});

When("modifying added application", async () => {
    strictEqual(
        await models.applicationsPage.modalTitle(),
        models.applicationsPage.modalTitleMessages.editApplication,
        "Modal title not found"
    );
    await models.applicationsPage.editApplicationForm();
});

When("clicking save application", async () => {
    await models.applicationsPage.saveApplicationForm();
});

When("clicking save edited application", async () => {
    await models.applicationsPage.saveEditedApplicationForm();
});

When("clicking save application with errors", async () => {
    await models.applicationsPage.saveApplicationForm({ expectErrors: true });
});

When("clicking edit application", async () => {
    await models.applicationsPage.clickEdit();
});

When("clicking delete application", async () => {
    await models.applicationsPage.clickDelete();
});

When("key-secret modal appears", async () => {
    strictEqual(
        await models.applicationsPage.modalTitle(),
        models.applicationsPage.modalTitleMessages.addedSecret,
        "Modal title not found"
    );

    ok(await models.applicationsPage.hasKey(), "Key not found in modal");
    ok(await models.applicationsPage.hasSecret(), "Secret not found in modal");

    await models.applicationsPage.saveKeyAndSecret();
});

When("delete application modal is open", async () => {
    strictEqual(
        await models.applicationsPage.modalTitle(),
        models.applicationsPage.modalTitleMessages.deleteApplication,
        "Delete modal title not found"
    );

    ok(
        (await models.applicationsPage.getModalConfirmationMessage())?.includes(
            models.applicationsPage.deleteApplicationConfirmationMessage
        ),
        "Validation message not correct"
    );
});

When("confirming delete application", async () => {
    await models.applicationsPage.deleteApplication();
});

When("clicking modal message", async () => {
    await models.applicationsPage.confirmKeySecretCopied();
});

When("clicking regenerate", async () => {
    models.applicationsPage.saveOldCredentials();
    await models.applicationsPage.clickRegenerate();
});

When("regenerate application modal appears", async () => {
    strictEqual(
        await models.applicationsPage.modalTitle(),
        models.applicationsPage.modalTitleMessages.regenerateApplication,
        "Regenerate modal title not found"
    );

    ok(
        (await models.applicationsPage.getModalConfirmationMessage())?.includes(
            models.applicationsPage.regenerateApplicationConfirmationMessage
        ),
        "Validation message not correct"
    );
});

When("clicking confirm regeneration", async () => {
    await models.applicationsPage.confirmRegenerate();
});

When("entering application form {string}", async (scenario: string) => {
    switch (scenario) {
        case "long app name":
            await models.applicationsPage.fillApplicationForm();
            await models.applicationsPage.enterLongApplicationName();
            break;
        case "no data":
        default:
            break;
    }
});

When("clicking collapse application", async () => {
    await models.applicationsPage.clickCollapse();
});

Then("credentials are updated", async () => {
    ok(!models.applicationsPage.keyIsUpdated(), "Key was updated");
    ok(models.applicationsPage.secretIsUpdated(), "Secret was not updated");
});

Then("copied URL message appears", async () => {
    ok(await models.applicationsPage.hasCopiedURLMessage(), "Message not found");
});

Then("copied URL is valid", async () => {
    ok(await models.applicationsPage.apiURLIsValid(), "API URL is valid");
});

Then("application appears on list", async () => {
    await models.applicationsPage.waitForListLoad();
    ok(await models.applicationsPage.isApplicationPresentOnPage(), "Application not found");
});

Then("edited application appears on list", async () => {
    await models.applicationsPage.waitForListLoad();
    ok(await models.applicationsPage.isEditedApplicationPresentOnPage(), "Application not found");
});

Then("generated key-secret is valid", async () => {
    ok(await models.applicationsPage.isKeyAndSecretValid(), "Credentials not valid");
});

Then("generated key-secret is not valid", async () => {
    ok(!(await models.applicationsPage.isKeyAndSecretValid()), "Credentials are still valid");
});

Then("application is deleted", async () => {
    strictEqual(
        await models.applicationsPage.getToastMessage(),
        models.applicationsPage.confirmationMessages.deleted,
        "Confirmation message not found"
    );
});

Then("application is edited", async () => {
    strictEqual(
        await models.applicationsPage.getToastMessage(),
        models.applicationsPage.confirmationMessages.updated,
        "Confirmation message not found"
    );
});

Then("application validation for {string} appears", async (scenario: string) => {
    const errors = await models.applicationsPage.getErrorMessages();

    switch (scenario) {
        case "long app name":
            ok(
                errors?.includes(models.applicationsPage.errorMessages.longAppName),
                `long application name error message failed. Actual message: ${errors}`
            );
            ok(await models.applicationsPage.applicationFieldHasError());
            break;
        case "no data":
            ok(
                errors?.includes(models.applicationsPage.errorMessages.noData),
                `Error message failed. Actual message: ${errors}`
            );
            ok(
                errors?.includes(models.applicationsPage.errorMessages.noOrgSelected),
                `Error message failed. Actual message: ${errors}`
            );
            ok(await models.applicationsPage.requiredFieldsHaveError());
            break;
        default:
            break;
    }
});

Then("application modal can be closed by {string}", async (scenario: string) => {
    switch (scenario) {
        case "clicking cancel":
            await models.applicationsPage.clickCancel();
            break;
        case "clicking x":
        default:
            await models.applicationsPage.closeModal();
            break;
    }
});

Then("application modal is closed", async () => {
    ok(!(await models.applicationsPage.hasModalOpen()), "Modal is still open");
});

Then("application is collapsed", async () => {
    ok(await models.applicationsPage.isCollapsed(), "Section collapsed");
});

Then("section is divided by vendor", async () => {
    ok(await models.applicationsPage.hasVendorDivision(), "Division not found");
});
