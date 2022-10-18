// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

import { network } from "../management/setup";
import { AdminAppPage } from "./adminAppPage";

export class EducationOrganizationsPage extends AdminAppPage {
    titleHeader = "div > h6";
    nameOnListHeader = ".panel-section h8";
    edOrgDetailsSectionCollapsedSection = 'div.content[aria-expanded="false"]';
    errorMsgSection = "div.validationSummary";
    fieldWithErrorSelector = ".row.has-error";
    activeTabSelector = "ul.nav li.active";
    addNewLeaBtn = 'button[data-target="#add-lea-modal"]';
    confirmBtn = 'button[type="submit"]';
    deleteLEABtn = "a.delete-lea-link";
    editLEABtn = ".row.heading a:has(.fa-pencil)";
    collapseBtn = 'a[data-toggle="collapse"]:has(.fa-chevron-up)';
    expandBtn = 'a[data-toggle="collapse"]:has(.fa-chevron-down)';
    dismissModalBtn = "button.close";

    get editedFormValueName(): string {
        return `${this.formValues.name} - Edited`;
    }

    get invalidFormValueId(): string {
        return `${this.formValues.ID} - Wrong`;
    }

    formSelectors = {
        ID: 'input[name="LocalEducationAgencyId"]',
        name: 'input[name="Name"]',
        address: 'input[name="StreetNumberName"]',
        city: 'input[name="City"]',
        state: 'select[name="State"]',
        zip: 'input[name="ZipCode"]',
    };

    formValues = {
        ID: "1",
        name: "Automated LEA",
        address: "123 street",
        city: "Austin",
        state: "TX",
        zip: "11208",
    };

    confirmationMessages = {
        leaAdded: "Organization Added",
        leaEdited: "Organization Updated",
        leaDeleted: "Organization Removed",
    };

    modalTitleMessages = {
        addLEA: "Add Local Education Agency",
        editLEA: "Edit Local Education Agency",
        deleteLEA: "Delete Local Education Agency",
    };

    errorMessages = {
        noData: "The highlighted fields are required to submit this form.",
        invalidID: "is not valid for Local Education Organization ID.",
        duplicatedID:
            "This 'Local Education Organization ID' is already associated with another Education Organization. Please provide a unique value.",
    };

    path(): string {
        return `${this.url}/EducationOrganizations/LocalEducationAgencies`;
    }

    async hasPageTitle(): Promise<boolean> {
        return await this.hasText({
            text: "Education Organizations",
            selector: this.titleHeader,
        });
    }

    async waitForListLoad(): Promise<void> {
        await network.waitForResponse({ url: "/EducationOrganizations/LocalEducationAgency" });
    }

    async addNewLEA(): Promise<void> {
        await this.page.locator(this.addNewLeaBtn).click();
    }

    async hasDeleteModalConfirmationMessage(): Promise<boolean> {
        return this.hasText({
            text: `Are you sure you want to permanently delete ${this.formValues.name}`,
            selector: ".modal.fade.in .modal-body",
        });
    }

    async fillLEAForm(): Promise<void> {
        await this.fillLEAId();
        await this.fillLEAName();
        await this.fillAddress();
        await this.fillCity();
        await this.selectState();
        await this.fillZipCode();
    }

    async hasTabSelected(): Promise<boolean> {
        return await this.hasText({
            text: "Education Organizations",
            selector: this.activeTabSelector,
        });
    }

    async editLEAForm(): Promise<void> {
        await this.fillLEAName(this.editedFormValueName);
    }

    async fillInvalidId(): Promise<void> {
        await this.fillLEAId(this.invalidFormValueId);
    }

    async saveLEAForm({ expectErrors = false }: { expectErrors?: boolean } = {}): Promise<void> {
        try {
            await Promise.all([
                network.waitForResponse({
                    url: "/EducationOrganizations/AddLocalEducationAgency",
                    status: expectErrors ? 400 : 200,
                }),
                this.saveForm(),
            ]);
        } catch (error) {
            throw `${error}\nErrors saving form:\n${await this.getErrorMessages()}`;
        }
    }

    async saveEditedLEAForm(): Promise<void> {
        await Promise.all([
            network.waitForResponse({ url: "/EducationOrganizations/EditLocalEducationAgency" }),
            this.saveForm(),
        ]);
    }

    async isLEAPresentOnPage(): Promise<boolean> {
        return this.hasText({ text: this.formValues.name, selector: this.nameOnListHeader });
    }

    async isEditedLEAPresentOnPage(): Promise<boolean> {
        return this.hasText({ text: this.editedFormValueName, selector: this.nameOnListHeader });
    }

    async clickCollapse() {
        await this.page.locator(this.collapseBtn).click();
    }

    async clickEdit(): Promise<void> {
        await this.page.locator(this.editLEABtn).click();
    }

    async clickDelete(): Promise<void> {
        await this.page.locator(this.deleteLEABtn).click();
    }

    async isSectionCollapsed(): Promise<boolean> {
        return (
            (await this.elementExists(this.expandBtn)) &&
            (await this.elementExists(this.edOrgDetailsSectionCollapsedSection))
        );
    }

    async deleteLEA(): Promise<void> {
        await Promise.all([
            this.clickConfirmDelete(),
            network.waitForResponse({ url: "/EducationOrganizations/DeleteLocalEducationAgency" }),
        ]);
    }

    async idFieldHasError(): Promise<boolean> {
        return (
            this.modalSelector.locator(this.fieldWithErrorSelector).locator(this.formSelectors.ID) !==
            undefined
        );
    }

    async allFieldsHaveError(): Promise<boolean> {
        let fieldsWithError = true;
        Object.values(this.formSelectors).forEach((selector) => {
            fieldsWithError =
                this.modalSelector.locator(this.fieldWithErrorSelector).locator(selector) !== undefined;
        });
        return fieldsWithError;
    }

    async dismissModal(): Promise<void> {
        await this.modalSelector.locator(this.dismissModalBtn).click();
    }

    async deleteLEAFullSteps(): Promise<void> {
        await this.clickDelete();
        await this.deleteLEA();
    }

    async addLocalEducationAgencyFullSteps(): Promise<void> {
        await this.navigate();
        await this.addNewLEA();
        await this.fillLEAForm();
        await this.saveLEAForm();
        await this.waitForListLoad();
    }

    async getErrorMessages(): Promise<string | null> {
        return await this.getText({ section: this.modalSelector, selector: this.errorMsgSection });
    }

    private async clickConfirmDelete(): Promise<void> {
        await this.modalSelector.locator(this.confirmBtn).click();
    }

    private async fillLEAId(value = this.formValues.ID): Promise<void> {
        await this.modalSelector.locator(this.formSelectors.ID).fill(value);
    }

    private async fillLEAName(value = this.formValues.name): Promise<void> {
        await this.modalSelector.locator(this.formSelectors.name).fill(value);
    }

    private async fillAddress(): Promise<void> {
        await this.modalSelector.locator(this.formSelectors.address).fill(this.formValues.address);
    }

    private async fillCity(): Promise<void> {
        await this.modalSelector.locator(this.formSelectors.city).fill(this.formValues.city);
    }

    private async selectState(): Promise<void> {
        await this.modalSelector
            .locator(this.formSelectors.state)
            .selectOption({ label: this.formValues.state });
    }

    private async fillZipCode(): Promise<void> {
        await this.modalSelector.locator(this.formSelectors.zip).fill(this.formValues.zip);
    }

    private async saveForm(): Promise<void> {
        await this.modalSelector.locator(this.confirmBtn).click();
    }
}
