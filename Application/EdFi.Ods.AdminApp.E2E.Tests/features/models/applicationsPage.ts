// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

import { Credentials } from "../interfaces";
import { getAccessToken, getRandomString, isTokenValid, testURL } from "../management/functions";
import { models, network } from "../management/setup";
import { AdminAppPage } from "./adminAppPage";

export class ApplicationsPage extends AdminAppPage {
    activeTabSelector = "ul.nav li.active";
    apiURLSelector = "a.copy-to-clipboard";
    errorMsgSection = "div.validationSummary";
    fieldWithErrorSelector = ".row.has-error";
    modalConfirmationBtn = 'button[type="submit"]';
    addApplicationBtn = this.modalConfirmationBtn;
    confirmRegenerateBtn = this.modalConfirmationBtn;
    confirmDeleteBtn = this.modalConfirmationBtn;
    addNewApplicationBtn = "button.loads-ajax-modal";
    applicationOnListSelector = ".vendor-application h8";
    keySecretCopiedBtn = "#key-and-secret-dismiss-button";
    editApplicationBtn = "a.edit-table";
    deleteApplicationBtn = "a.delete-application-link";
    confirmSelector = "div.alert:not(.hidden)";
    regenerateBtn = "a.regenerate-application-secret-link";
    closeModalBtn = 'button[aria-label="Close"]';
    cancelBtn = 'button.btn-default[data-dismiss="modal"]';
    collapseBtn = 'a[data-toggle="collapse"]:has(.fa-chevron-up)';
    expandBtn = 'a[data-toggle="collapse"]:has(.fa-chevron-down)';
    loadingIconSelector = "div h6";
    vendorHeadingSelector = ".vendor-heading";

    get keySelector(): string {
        return this.credentialsSelector("Key");
    }

    get secretSelector(): string {
        return this.credentialsSelector("Secret");
    }

    get odsURLSelector(): string {
        return this.credentialsSelector("API URL");
    }

    applicationListURL = "/Application/ApplicationList";

    credentials!: Credentials;
    oldCredentials!: Credentials;

    get editedFormValueName(): string {
        return `${this.formValues.name} - Edited`;
    }

    get deleteApplicationConfirmationMessage(): string {
        return `Are you sure you want to permanently delete ${this.formValues.name}?`;
    }

    get regenerateApplicationConfirmationMessage(): string {
        return `Are you sure you want to regenerate the Key/Secret for ${this.formValues.name}?`;
    }

    formSelectors = {
        name: 'input[name="ApplicationName"]',
        lea: 'span label:text("Local Education Agency")',
        leaSelectBtn: 'div[data-edorg-type="1"] button.dropdown-toggle',
        leaSelect: 'div[data-edorg-type="1"] select',
        claimSetSelect: 'select[name="ClaimSetName"]',
    };

    formValues = {
        name: "Automated Application",
        lea: "Automated LEA",
    };

    confirmationMessages = {
        deleted: "Application deleted successfully",
        updated: "Application updated successfully",
    };

    errorMessages = {
        noData: "The highlighted fields are required to submit this form.",
        noOrgSelected: "You must choose at least one Education Organization",
        longAppName:
            "would be too long for Admin App to set up necessary Application records. Consider shortening the name by 1 character(s).",
    };

    modalTitleMessages = {
        addApplication: "Add Application to Vendor",
        addedSecret: "Add Application",
        deleteApplication: "Delete Application",
        editApplication: "Edit Application",
        regenerateApplication: "Regenerate Application Key/Secret",
    };

    path(): string {
        return `${this.url}/Application`;
    }

    async waitForListLoad(): Promise<void> {
        if (await this.hasLoadingSpinner()) {
            await network.waitForResponse({ url: this.applicationListURL, trackingResponse: true });
        }
    }

    async hasLoadingSpinner(): Promise<boolean> {
        return await this.hasText({
            text: "Warming up Production API....",
            selector: this.loadingIconSelector,
        });
    }

    async addApplication(): Promise<void> {
        await this.page.locator(this.addNewApplicationBtn).click();
    }

    async fillApplicationForm(): Promise<void> {
        await this.fillApplicationName();
        await this.selectLEA();
        await this.selectOrganizationId();
        await this.selectClaimSet("Ed-Fi Sandbox");
    }

    async editApplicationForm(): Promise<void> {
        await this.fillApplicationName(this.editedFormValueName);
    }

    async enterLongApplicationName(): Promise<void> {
        await this.fillApplicationName(getRandomString(51));
    }

    async saveApplicationForm({ expectErrors = false }: { expectErrors?: boolean } = {}): Promise<void> {
        try {
            await Promise.all([
                network.waitForResponse({
                    url: "/Application/Add",
                    status: expectErrors ? 400 : 200,
                }),
                this.saveForm(),
            ]);
            network.startResponseTracking(this.applicationListURL);
        } catch (error) {
            throw `${error}\nErrors saving form:\n${await this.getErrorMessages()}`;
        }
    }

    async saveEditedApplicationForm(): Promise<void> {
        try {
            await Promise.all([network.waitForResponse({ url: "/Application/Edit" }), this.saveForm()]);
            network.startResponseTracking(this.applicationListURL);
        } catch (error) {
            throw `${error}\nErrors saving form:\n${await this.getErrorMessages()}`;
        }
    }

    async isApplicationPresentOnPage(): Promise<boolean> {
        return await this.hasText({
            text: this.formValues.name,
            selector: this.applicationOnListSelector,
        });
    }

    async isEditedApplicationPresentOnPage(): Promise<boolean> {
        return await this.hasText({
            text: this.editedFormValueName,
            selector: this.applicationOnListSelector,
        });
    }

    async hasTabSelected(): Promise<boolean> {
        return await this.hasText({
            text: "Applications",
            selector: this.activeTabSelector,
        });
    }

    async clickURL(): Promise<void> {
        await this.page.locator(this.apiURLSelector).click();
    }

    async hasCopiedURLMessage(): Promise<boolean> {
        return await this.elementExists(`${this.apiURLSelector} span:has-text("${"Copied to clipboard!"}")`);
    }

    async apiURLIsValid(): Promise<boolean> {
        const apiURL = await this.page.locator(this.apiURLSelector).getAttribute("data-clipboard-text");
        if (!apiURL) {
            return false;
        }
        return await testURL(apiURL);
    }

    async getErrorMessages(): Promise<string | null> {
        return await this.getText({ section: this.modalSelector, selector: this.errorMsgSection });
    }

    async applicationFieldHasError(): Promise<boolean> {
        return (
            this.modalSelector.locator(this.fieldWithErrorSelector).locator(this.formSelectors.name) !==
            undefined
        );
    }

    async requiredFieldsHaveError(): Promise<boolean> {
        return (
            this.modalSelector.locator(this.fieldWithErrorSelector).locator(this.formSelectors.name) !==
            undefined
        );
    }

    async hasKey(): Promise<boolean> {
        return await this.elementExists(this.keySelector);
    }

    async hasSecret(): Promise<boolean> {
        return await this.elementExists(this.secretSelector);
    }

    async saveKeyAndSecret(): Promise<void> {
        this.credentials = {
            Key: await this.getText({ section: this.modalSelector, selector: this.keySelector }),
            Secret: await this.getText({ section: this.modalSelector, selector: this.secretSelector }),
            URL: await this.getText({ section: this.modalSelector, selector: this.odsURLSelector }),
        };
    }

    async isKeyAndSecretValid(): Promise<boolean> {
        const token = await getAccessToken(this.credentials);

        if (!token) {
            return false;
        }

        return isTokenValid({ token, api: this.credentials.URL });
    }

    async confirmKeySecretCopied() {
        await Promise.all([
            network.waitForResponse({ url: "/Application" }),
            this.clickKeySecretCopiedButton(),
        ]);
    }

    async clickEdit(): Promise<void> {
        await this.page.locator(this.editApplicationBtn).click();
    }

    async clickDelete(): Promise<void> {
        await this.page.locator(this.deleteApplicationBtn).click();
    }

    async clickRegenerate() {
        await this.page.locator(this.regenerateBtn).click();
    }

    async confirmRegenerate() {
        await Promise.all([
            network.waitForResponse({ url: "/RegenerateSecret" }),
            this.clickConfirmRegenerate(),
        ]);
    }

    saveOldCredentials() {
        this.oldCredentials = this.credentials;
    }

    keyIsUpdated(): boolean {
        return this.oldCredentials && this.oldCredentials.Key !== this.credentials.Key;
    }

    secretIsUpdated(): boolean {
        return this.oldCredentials && this.oldCredentials.Secret !== this.credentials.Secret;
    }

    async deleteApplication(): Promise<void> {
        await Promise.all([
            this.clickConfirmDelete(),
            network.waitForResponse({ url: "Application/Delete" }),
        ]);
    }

    async getModalConfirmationMessage(): Promise<string | null> {
        return await this.getText({ section: this.modalSelector, selector: this.confirmSelector });
    }

    async closeModal(): Promise<void> {
        await this.modalSelector.locator(this.closeModalBtn).click();
    }

    async clickCancel(): Promise<void> {
        await this.modalSelector.locator(this.cancelBtn).click();
    }

    async clickCollapse(): Promise<void> {
        await this.page.locator(this.collapseBtn).click();
    }

    async isCollapsed(): Promise<boolean> {
        return (await this.elementExists(this.expandBtn)) && (await this.isApplicationPresentOnPage());
    }

    async hasVendorDivision(): Promise<boolean> {
        return await this.hasText({
            text: `Vendor: ${models.vendorsPage.formValues.name}`,
            selector: this.vendorHeadingSelector,
        });
    }

    async addApplicationFullSteps(): Promise<void> {
        await this.navigate();
        await this.addApplication();
        await this.fillApplicationForm();
        await this.saveApplicationForm();
        await this.saveKeyAndSecret();
        await this.confirmKeySecretCopied();
        if (!(await this.isKeyAndSecretValid())) {
            throw "Key and secret not valid";
        }
        await this.isApplicationPresentOnPage();
    }

    async regenerateApplicationFullSteps() {
        await this.clickRegenerate();
        await this.confirmRegenerate();
        await this.saveKeyAndSecret();
        await this.confirmKeySecretCopied();
        if (!(await this.isKeyAndSecretValid())) {
            throw "Key and secret not valid";
        }
    }

    async deleteApplicationFullSteps() {
        await this.clickDelete();
        await this.deleteApplication();
    }

    private async clickKeySecretCopiedButton(): Promise<void> {
        await this.modalSelector.locator(this.keySecretCopiedBtn).click();
    }

    private async clickConfirmRegenerate() {
        await this.modalSelector.locator(this.confirmRegenerateBtn).click();
    }

    private async fillApplicationName(name = this.formValues.name): Promise<void> {
        await this.modalSelector.locator(this.formSelectors.name).fill(name);
    }

    private async selectLEA(): Promise<void> {
        await this.modalSelector.locator(this.formSelectors.lea).click();
    }

    private async selectOrganizationId(): Promise<void> {
        await this.modalSelector.locator(this.formSelectors.leaSelectBtn).click();
        await this.modalSelector
            .locator(this.formSelectors.leaSelect)
            .selectOption({ label: this.formValues.lea });
    }

    private async selectClaimSet(claimSetName: string): Promise<void> {
        await this.modalSelector
            .locator(this.formSelectors.claimSetSelect)
            .selectOption({ label: claimSetName });
    }

    private async saveForm(): Promise<void> {
        await this.modalSelector.locator(this.addApplicationBtn).click();
    }

    private async clickConfirmDelete(): Promise<void> {
        await this.modalSelector.locator(this.confirmDeleteBtn).click();
    }

    private credentialsSelector(text: string): string {
        return `.key-text div:has-text('${text}') .key-generated`;
    }
}
