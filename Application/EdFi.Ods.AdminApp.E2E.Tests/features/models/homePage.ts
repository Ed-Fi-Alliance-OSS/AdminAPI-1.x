// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

import { AdminAppPage } from "./adminAppPage";

export class HomePage extends AdminAppPage {
    logoutBtn = "li a:text('Log Out')";

    path(): string {
        return `${this.url}/`;
    }

    async hasSettingsOption(): Promise<boolean> {
        return await this.hasText({ text: "Settings" });
    }

    async hasGlobalOption(): Promise<boolean> {
        return await this.hasText({ text: "Global" });
    }

    async logout(): Promise<void> {
        await Promise.all([this.clickLogout(), this.page.waitForNavigation()]);
    }

    private async clickLogout(): Promise<void> {
        await this.page.locator(this.logoutBtn).click();
    }
}
