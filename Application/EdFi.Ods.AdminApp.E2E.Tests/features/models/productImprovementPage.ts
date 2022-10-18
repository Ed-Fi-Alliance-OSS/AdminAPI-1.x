// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

import { AdminAppPage } from "./adminAppPage";

export class ProductImprovementPage extends AdminAppPage {
    analyticsCheckbox = 'input[type="checkbox"]';
    proceedBtn = 'button[type="submit"]';

    //Override
    get isOnPage(): boolean {
        const currentURL = this.page.url();
        const baseURL = currentURL.substring(0, currentURL.indexOf("?"));
        const URL = baseURL === "" ? currentURL : baseURL;
        return URL === this.path() || URL === this.firstTimePath();
    }

    path(): string {
        return `${this.url}/ProductImprovement/EnableProductImprovement`;
    }

    firstTimePath(): string {
        return `${this.url}/ProductImprovement/EnableProductImprovementFirstTimeSetup`;
    }

    async hasPageTitle(): Promise<boolean> {
        return await this.hasText({ text: "Enable Product Improvement", selector: "h2" });
    }

    async uncheckAnalyticsTag(): Promise<void> {
        await this.page.locator(this.analyticsCheckbox).uncheck();
    }

    async proceed(): Promise<void> {
        await Promise.all([this.clickProceed(), this.page.waitForNavigation()]);
    }

    private async clickProceed(): Promise<void> {
        await this.page.locator(this.proceedBtn).click();
    }
}
