// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

import { Page, Locator } from "playwright";

export abstract class AdminAppPage {
    page: Page;

    titleHeader = ".container h2";
    bodySelector = "body";
    toastSelector = "#toast-container";
    mainSectionSelector = "div.container";
    loadingIconSelector = ".footable-loader";
    validationErrorsSection = "div#validationSummary:not(.hidden)";

    //Modal
    openModalSection = "div.modal.fade.in";
    modalTitleHeader = "h4.modal-title";

    protected get url(): string {
        if (!process.env.URL) {
            throw "URL not found. Verify that URL is set in .env file";
        }
        return process.env.URL;
    }

    get isOnPage(): boolean {
        const currentURL = this.page.url();
        const baseURL = currentURL.substring(0, currentURL.indexOf("?"));
        const URL = baseURL === "" ? currentURL : baseURL;
        return URL === this.path();
    }

    get modalSelector(): Locator {
        return this.page.locator(this.openModalSection);
    }

    constructor(page: Page) {
        this.page = page;
    }

    abstract path(): string;

    async navigate(): Promise<void> {
        if (!this.isOnPage) {
            const navigationResult = await this.page.goto(this.path(), { waitUntil: "networkidle" });
            if (!navigationResult?.ok()) {
                throw `Unable to navigate to expected page ${this.path()}. Status: ${navigationResult?.status()}`;
            }
        }
    }

    async getToastMessage(): Promise<string | null> {
        return await this.getText({ selector: this.toastSelector });
    }

    async modalTitle(): Promise<string> {
        const content = this.getText({ section: this.modalSelector, selector: this.modalTitleHeader });
        return content;
    }

    async hasModalOpen(): Promise<boolean> {
        return await this.elementExists(this.openModalSection);
    }

    async waitForModalVisible(): Promise<void> {
        await this.waitForElementToBeVisible(this.openModalSection);
    }

    async waitForElementToBeVisible(element: string): Promise<void> {
        await this.page.locator(element).waitFor({ state: "visible" });
    }

    async clickOutside(): Promise<void> {
        await this.page.locator(this.bodySelector).click();
    }

    protected async getText({
        section = this.page,
        selector,
    }: {
        section?: Locator | Page;
        selector: string;
    }): Promise<string> {
        const content = await section.locator(selector).textContent();
        if (!content) {
            throw `Text for selector ${selector} not found`;
        }
        return content;
    }

    protected async hasText({
        text,
        selector = "div",
    }: {
        text: string;
        selector?: string;
    }): Promise<boolean> {
        return await this.elementExists(`${this.mainSectionSelector} ${selector}:has-text("${text.trim()}")`);
    }

    protected async elementExists(selector: string): Promise<boolean> {
        return (await this.page.locator(selector).count()) > 0;
    }
}
