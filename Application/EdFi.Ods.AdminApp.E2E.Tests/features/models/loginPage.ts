import { AdminAppPage } from "./adminAppPage";

export class LoginPage extends AdminAppPage {
    emailInput = "input#Email";
    passwordInput = "input#Password";
    submitBtn = 'button[type="submit"]';

    path(): string {
        return `${this.url}/Identity/Login`;
    }

    async fillForm(username?: string, password?: string): Promise<void> {
        if (username && password) {
            await this.page.fill(this.emailInput, username);
            await this.page.fill(this.passwordInput, password);
        } else {
            throw "Could not find email or password. Verify that variables are set in the .env file";
        }
    }

    async login(): Promise<void> {
        await this.page.click(this.submitBtn);
    }

    async fullLogin(username?: string, password?: string): Promise<void> {
        await this.navigate();
        await this.fillForm(username, password);
        await this.login();
    }
}
