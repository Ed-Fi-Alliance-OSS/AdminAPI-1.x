import { Page } from "playwright";
import { AdminAppPage } from "./adminAppPage";
import { LoginPage } from "./loginPage";
import { HomePage } from "./homePage";

export class ModelResolver {
    aaPages: Array<AdminAppPage> = [];

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

    constructor(public page: Page) {}

    getModel<T extends AdminAppPage>(name: string): T {
        const model = this.aaPages.find((p) => p.constructor.name === name) as T;
        return model;
    }
}
