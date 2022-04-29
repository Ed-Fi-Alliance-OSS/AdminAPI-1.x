// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

import { Given, Then, When } from "@cucumber/cucumber";
import { strictEqual, ok } from "assert";
import { models } from "../management/setup";

Given("there's a local education agency added", async () => {
    await models.edOrgsPage.navigate();
    if (await models.edOrgsPage.isLEAPresentOnPage()) {
        return Promise.resolve("LEA is already added");
    }
    await models.edOrgsPage.addLocalEducationAgencyFullSteps();
});

Given("education organization list has loaded", async () => {
    ok(await models.edOrgsPage.hasTabSelected(), "Education Organization tab not selected.");
    ok(await models.edOrgsPage.hasPageTitle(), "Page title not found");
});

When("adding new local education agency", async () => {
    await models.edOrgsPage.addNewLEA();
});

When("filling local education agency form", async () => {
    strictEqual(
        await models.edOrgsPage.modalTitle(),
        models.edOrgsPage.modalTitleMessages.addLEA,
        "Add modal title not found"
    );
    await models.edOrgsPage.fillLEAForm();
});

When("modifying added local education agency", async () => {
    strictEqual(
        await models.edOrgsPage.modalTitle(),
        models.edOrgsPage.modalTitleMessages.editLEA,
        "Edit modal title not found"
    );
    await models.edOrgsPage.editLEAForm();
});

When("clicking save local education agency", async () => {
    await models.edOrgsPage.saveLEAForm();
});

When("clicking save local education agency with errors", async () => {
    await models.edOrgsPage.saveLEAForm({ expectErrors: true });
});

When("clicking save edited local education agency", async () => {
    await models.edOrgsPage.saveEditedLEAForm();
});

When("clicking edit local education agency", async () => {
    await models.edOrgsPage.clickEdit();
});

When("clicking delete local education agency", async () => {
    await models.edOrgsPage.clickDelete();
});

When("delete local education agency modal is open", async () => {
    strictEqual(
        await models.edOrgsPage.modalTitle(),
        models.edOrgsPage.modalTitleMessages.deleteLEA,
        "Delete modal title not found"
    );
    await models.edOrgsPage.hasDeleteModalConfirmationMessage();
});

When("confirming delete local education agency", async () => {
    await models.edOrgsPage.deleteLEA();
});

When("clicking collapse local education agency section", async () => {
    await models.edOrgsPage.clickCollapse();
});

When("entering local education agency form {string}", async (scenario: string) => {
    switch (scenario) {
        case "wrong id":
            await models.edOrgsPage.fillLEAForm();
            await models.edOrgsPage.fillInvalidId();
            break;
        case "no data":
        default:
            break;
    }
});

Then("local education agency section is collapsed", async () => {
    ok(await models.edOrgsPage.isSectionCollapsed(), "Section is not collapsed");
});

Then("local education agency is added", async () => {
    strictEqual(
        await models.edOrgsPage.getToastMessage(),
        models.edOrgsPage.confirmationMessages.leaAdded,
        "Confirmation message not found"
    );
});

Then("local education agency is edited", async () => {
    strictEqual(
        await models.edOrgsPage.getToastMessage(),
        models.edOrgsPage.confirmationMessages.leaEdited,
        "Confirmation message not found"
    );
});

Then("added local education agency appears on list", async () => {
    await models.edOrgsPage.waitForListLoad();
    ok(await models.edOrgsPage.isLEAPresentOnPage(), "local education agency not found in page");
});

Then("edited local education agency appears on list", async () => {
    await models.edOrgsPage.waitForListLoad();
    ok(await models.edOrgsPage.isEditedLEAPresentOnPage(), "local education agency not found in page");
});

Then("local education agency is deleted", async () => {
    strictEqual(
        await models.edOrgsPage.getToastMessage(),
        models.edOrgsPage.confirmationMessages.leaDeleted,
        "Confirmation message not found"
    );
});

Then("local education agency validation for {string} appears", async (scenario: string) => {
    const errors = await models.edOrgsPage.getErrorMessages();

    switch (scenario) {
        case "no data":
            ok(
                errors?.includes(models.edOrgsPage.errorMessages.noData),
                `Error message failed. Actual message: ${errors}`
            );
            ok(await models.edOrgsPage.allFieldsHaveError());
            break;
        case "wrong id":
            ok(
                errors?.includes(models.edOrgsPage.errorMessages.invalidID),
                `ID error message failed. Actual message: ${errors}`
            );
            ok(await models.edOrgsPage.idFieldHasError());
            break;
        case "duplicated id":
            ok(
                errors?.includes(models.edOrgsPage.errorMessages.duplicatedID),
                `Error message failed. Actual message: ${errors}`
            );
            ok(await models.edOrgsPage.idFieldHasError());
            break;
        default:
            break;
    }
});

Then("modal is dismissed", async () => {
    await models.edOrgsPage.dismissModal();
});
