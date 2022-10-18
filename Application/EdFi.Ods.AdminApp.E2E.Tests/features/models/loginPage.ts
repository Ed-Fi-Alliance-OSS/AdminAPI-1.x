// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

import { AdminAppPage } from "./adminAppPage";

export class LoginPage extends AdminAppPage {
    errorMsgSection = "div.validation-summary-errors";
    emailInput = "input#Email";
    passwordInput = "input#Password";
    passwordConfirmInput = "input#ConfirmPassword";
    submitBtn = 'button[type="submit"]';
    registerNewUserBtn = "a.btn:text('Register as a new user')";
    registerBtn = 'button[type="submit"]';

    public errorMessages = {
        missingEmail: "The Email field is required.",
        wrongEmail: "The Email field is not a valid e-mail address.",
        missingPassword: "The Password field is required.",
        invalidLogin: "Invalid login attempt.",
    };

    path(): string {
        return `${this.url}/Identity/Login`;
    }

    needsFirstTimeSetup(): boolean {
        return this.page.url().includes("FirstTimeSetup");
    }

    async fillForm(email?: string, password?: string): Promise<void> {
        await this.fillEmail(email);
        await this.fillPassword(password);
    }

    async fillEmail(email?: string): Promise<void> {
        if (!email) {
            throw "Could not find email. Verify that the variable is set in the .env file";
        }
        await this.page.locator(this.emailInput).fill(email);
    }

    async fillPassword(password?: string): Promise<void> {
        if (!password) {
            throw "Could not find password. Verify that the variable is set in the .env file";
        }
        await this.page.locator(this.passwordInput).fill(password);
    }

    async fillPasswordConfirm(password?: string): Promise<void> {
        if (!password) {
            throw "Could not find password. Verify that the variable is set in the .env file";
        }
        await this.page.locator(this.passwordConfirmInput).fill(password);
    }

    async login(): Promise<void> {
        await Promise.all([this.clickLogin(), this.page.waitForNavigation()]);
    }

    async fullLogin(email?: string, password?: string): Promise<void> {
        await this.navigate();
        await this.fillForm(email, password);
        await this.login();
    }

    async fullRegistration(email?: string, password?: string) {
        await this.startRegistration();
        await this.fillForm(email, password);
        await this.fillPasswordConfirm(password);
        await this.register();
    }

    async hasRegisterButton(): Promise<boolean> {
        return await this.hasText({ text: "Register as a new user", selector: this.registerNewUserBtn });
    }

    async startRegistration(): Promise<void> {
        await Promise.all([this.clickOnRegisterFromLogin(), this.page.waitForNavigation()]);
    }

    async register(): Promise<void> {
        await this.page.locator(this.registerBtn).click();
    }

    async getErrorMessages(): Promise<string | null> {
        return await this.getText({ selector: this.errorMsgSection });
    }

    private async clickLogin(): Promise<void> {
        await this.page.locator(this.submitBtn).click();
    }

    private async clickOnRegisterFromLogin(): Promise<void> {
        await this.page.locator(this.registerNewUserBtn).click();
    }
}
