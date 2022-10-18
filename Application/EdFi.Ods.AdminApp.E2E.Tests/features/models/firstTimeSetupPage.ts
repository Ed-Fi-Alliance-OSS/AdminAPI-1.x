// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

import { AdminAppPage } from "./adminAppPage";

export class FirstTimeSetupPage extends AdminAppPage {
    continueBtn = "a#finish-setup-link";

    path(): string {
        return `${this.url}/Setup/FirstTimeSetup`;
    }

    hasTitle(): Promise<boolean> {
        return this.hasText({ text: "Additional Setup Required" });
    }

    async continue(url: string): Promise<void> {
        await Promise.all([this.clickContinue(), this.page.waitForNavigation({ url })]);
    }

    private async clickContinue(): Promise<void> {
        await this.page.locator(this.continueBtn).click();
    }
}
