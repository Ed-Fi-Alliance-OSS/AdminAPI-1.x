// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

import { Given, Then, When } from "@cucumber/cucumber";
import { models } from "../management/setup";
import { ok, strictEqual } from "assert";

Given("there's a vendor added", async () => {
    await models.vendorsPage.navigate();
    if (await models.vendorsPage.isVendorPresentOnPage()) {
        return Promise.resolve("Vendor is already added");
    }
    await models.vendorsPage.addVendorFullSteps();
});

Given("vendor page has loaded", async () => {
    ok(await models.vendorsPage.hasTabSelected(), "Vendors tab not selected.");
    ok(await models.vendorsPage.hasPageTitle(), "Page title not found.");
});

Given("there are no vendors", async () => {
    ok(await models.vendorsPage.noVendorsMessageVisible(), "There are vendors on the page");
});

When("clicking add vendor", async () => {
    await models.vendorsPage.addVendor();
});

When("clicking edit vendor", async () => {
    await models.vendorsPage.clickEdit();
});

When("clicking delete vendor", async () => {
    await models.vendorsPage.clickDelete();
});

When("modifying added vendor", async () => {
    strictEqual(
        await models.vendorsPage.modalTitle(),
        models.vendorsPage.modalTitleMessages.editVendor,
        "Edit modal title not found"
    );
    await models.vendorsPage.editVendorForm();
});

When("delete vendor modal is open", async () => {
    strictEqual(
        await models.vendorsPage.modalTitle(),
        models.vendorsPage.modalTitleMessages.deleteVendor,
        "Delete modal title not found"
    );
    ok(
        (await models.vendorsPage.getDeleteVendorMessage())?.includes(
            models.vendorsPage.deleteVendorConfirmationMessage
        ),
        "Validation message not correct"
    );
});

When("confirming delete vendor", async () => {
    await models.vendorsPage.deleteVendor();
});

When("filling vendor form", async () => {
    strictEqual(
        await models.vendorsPage.modalTitle(),
        models.vendorsPage.modalTitleMessages.addVendor,
        "Modal not found."
    );

    await models.vendorsPage.fillVendorForm();
});

When("adding vendor namespace prefix", async () => {
    await models.vendorsPage.addNamespacePrefix();
    ok(await models.vendorsPage.hasPrefixAdded(), "Namespace prefix not found.");
});

When("clicking save vendor", async () => {
    await models.vendorsPage.saveVendorForm();
});

When("clicking save edited vendor", async () => {
    await models.vendorsPage.saveEditedVendorForm();
});

When("clicking save vendor with errors", async () => {
    await models.vendorsPage.saveVendorForm({ expectErrors: true });
});

When("help section is present", async () => {
    ok(await models.vendorsPage.hasHelpSection(), "Help section not found");
});

When("clicking define applications", async () => {
    await models.vendorsPage.defineApplicationsSingleInstance();
});

When("entering vendor form {string}", async (scenario: string) => {
    switch (scenario) {
        case "wrong email":
            await models.vendorsPage.fillVendorForm();
            await models.vendorsPage.fillInvalidEmail();
            break;
        case "no data":
        default:
            break;
    }
});

Then("vendor is added", async () => {
    strictEqual(
        await models.vendorsPage.getToastMessage(),
        models.vendorsPage.confirmationMessages.added,
        "Confirmation message not found"
    );
});

Then("vendor is edited", async () => {
    strictEqual(
        await models.vendorsPage.getToastMessage(),
        models.vendorsPage.confirmationMessages.edited,
        "Confirmation message not found"
    );
});

Then("vendor is deleted", async () => {
    strictEqual(
        await models.vendorsPage.getToastMessage(),
        models.vendorsPage.confirmationMessages.deleted,
        "Confirmation message not found"
    );
});

Then("added vendor appears on list", async () => {
    ok(await models.vendorsPage.isVendorPresentOnPage(), "Vendor not found in page");
});

Then("edited vendor appears on list", async () => {
    ok(await models.vendorsPage.isEditedVendorPresentOnPage(), "Vendor not found in page");
});

Then("help section can be collapsed", async () => {
    await models.vendorsPage.collapseHelpSection();
    ok(await models.vendorsPage.hasHelpSectionFlag(), "Hide help not set");
});

Then("help section can be expanded", async () => {
    await models.vendorsPage.showHelpSection();
    ok(!(await models.vendorsPage.hasHelpSectionFlag()), "Hide help set");
});

Then("it navigates to the applications page", async () => {
    ok(models.applicationsPage.isOnPage, "It did not navigate to the applications page");
});

Then("vendor validation for {string} appears", async (scenario: string) => {
    const errors = await models.vendorsPage.getErrorMessages();

    switch (scenario) {
        case "wrong email":
            ok(
                errors?.includes(models.vendorsPage.errorMessages.invalidEmail),
                `ID error message failed. Actual message: ${errors}`
            );
            ok(await models.vendorsPage.emailFieldHasError());
            break;
        case "no data":
            ok(
                errors?.includes(models.vendorsPage.errorMessages.noData),
                `Error message failed. Actual message: ${errors}`
            );
            ok(await models.vendorsPage.requiredFieldsHaveError());
            break;
        default:
            break;
    }
});

Then("vendor modal is closed", async () => {
    ok(!(await models.vendorsPage.hasModalOpen()));
});

Then("vendor modal can be closed by {string}", async (scenario: string) => {
    switch (scenario) {
        case "clicking outside":
            await models.vendorsPage.clickOutside();
            break;
        case "clicking cancel":
            await models.vendorsPage.clickCancel();
            break;
        case "clicking x":
        default:
            await models.vendorsPage.closeModal();
            break;
    }
});
