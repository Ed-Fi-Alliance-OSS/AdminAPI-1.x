// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

import { Page } from "playwright";
import { AdminAppPage } from "./adminAppPage";
import { LoginPage } from "./loginPage";
import { HomePage } from "./homePage";
import { LandingPage } from "./landingPage";
import { FirstTimeSetupPage } from "./firstTimeSetupPage";
import { ProductImprovementPage } from "./productImprovementPage";
import { EducationOrganizationsPage } from "./educationOrganizationsPage";
import { VendorsPage } from "./vendorsPage";
import { ApplicationsPage } from "./applicationsPage";

export class ModelResolver {
    aaPages: Array<AdminAppPage> = [];

    public get landingPage(): LandingPage {
        let model = this.getModel<LandingPage>(LandingPage.name);
        if (!model) {
            model = new LandingPage(this.page);
            this.aaPages.push(model);
        }
        return model;
    }

    public get loginPage(): LoginPage {
        let model = this.getModel<LoginPage>(LoginPage.name);
        if (!model) {
            model = new LoginPage(this.page);
            this.aaPages.push(model);
        }
        return model;
    }

    public get homePage(): HomePage {
        let model = this.getModel<HomePage>(HomePage.name);
        if (!model) {
            model = new HomePage(this.page);
            this.aaPages.push(model);
        }
        return model;
    }

    public get firstTimeSetupPage(): FirstTimeSetupPage {
        let model = this.getModel<FirstTimeSetupPage>(FirstTimeSetupPage.name);
        if (!model) {
            model = new FirstTimeSetupPage(this.page);
            this.aaPages.push(model);
        }
        return model;
    }

    public get productImprovementPage(): ProductImprovementPage {
        let model = this.getModel<ProductImprovementPage>(ProductImprovementPage.name);
        if (!model) {
            model = new ProductImprovementPage(this.page);
            this.aaPages.push(model);
        }
        return model;
    }

    public get applicationsPage(): ApplicationsPage {
        let model = this.getModel<ApplicationsPage>(ApplicationsPage.name);
        if (!model) {
            model = new ApplicationsPage(this.page);
            this.aaPages.push(model);
        }
        return model;
    }

    public get edOrgsPage(): EducationOrganizationsPage {
        let model = this.getModel<EducationOrganizationsPage>(EducationOrganizationsPage.name);
        if (!model) {
            model = new EducationOrganizationsPage(this.page);
            this.aaPages.push(model);
        }
        return model;
    }

    public get vendorsPage(): VendorsPage {
        let model = this.getModel<VendorsPage>(VendorsPage.name);
        if (!model) {
            model = new VendorsPage(this.page);
            this.aaPages.push(model);
        }
        return model;
    }

    constructor(public page: Page) {}

    getModel<T extends AdminAppPage>(name: string): T {
        const model = this.aaPages.find((p) => p.constructor.name === name) as T;
        return model;
    }
}
