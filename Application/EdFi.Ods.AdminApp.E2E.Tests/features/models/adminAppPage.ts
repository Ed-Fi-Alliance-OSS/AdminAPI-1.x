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
            await this.page.goto(this.path(), { waitUntil: "networkidle" });
        }
    }

    async waitForResponse({ url, status = 200 }: { url: string; status?: number }): Promise<void> {
        await this.page.waitForResponse((response) => {
            if (response.url().includes(url) && response.status() !== status) {
                return Promise.reject(new Error(`Expected status ${status}, got ${response.status()}`));
            }
            return response.url().includes(url) && response.status() === status;
        });
    }

    async getToastMessage(): Promise<string | null> {
        return await this.getText(this.toastSelector);
    }

    async modalTitle(): Promise<string> {
        const content = await this.modalSelector.locator(this.modalTitleHeader).textContent();
        return content ? content : "";
    }

    async hasModalOpen(): Promise<boolean> {
        return this.elementExists(this.openModalSection);
    }

    async clickOutside(): Promise<void> {
        await this.page.locator(this.bodySelector).click();
    }

    protected async getText(text: string): Promise<string | null> {
        return this.page.textContent(text);
    }

    protected async hasText({
        text,
        selector = "div",
    }: {
        text: string;
        selector?: string;
    }): Promise<boolean> {
        return await this.elementExists(`${this.mainSectionSelector} ${selector}:has-text("${text}")`);
    }

    protected async elementExists(selector: string): Promise<boolean> {
        return (await this.page.locator(selector).count()) > 0;
    }
}
