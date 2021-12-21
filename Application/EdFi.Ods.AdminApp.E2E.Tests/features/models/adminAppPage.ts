import { Page } from "playwright";

export abstract class AdminAppPage {
    page: Page;

    title = ".container h2";
    toast = "#toast-container";
    loadingSelector = ".footable-loader";
    validationErrors = "div#validationSummary:not(.hidden)";

    get url(): string {
        if (!process.env.URL) {
            throw "URL not found. Verify that URL is set in .env file";
        }
        return process.env.URL;
    }

    constructor(page: Page) {
        this.page = page;
    }

    abstract path(): string;

    async navigate(): Promise<void> {
        if (this.page.url() !== this.path()) {
            await this.page.goto(this.path());
        }
    }

    async waitForResponse(url: string, status = 200): Promise<void> {
        await this.page.waitForResponse(
            (response) => response.url().includes(url) && response.status() === status
        );
    }

    protected async getText(text: string): Promise<string | null> {
        return this.page.textContent(text);
    }

    protected async hasText(text: string, selector = "div"): Promise<boolean> {
        return await this.elementExists(`div.container ${selector}:has-text("${text}")`);
    }

    protected async elementExists(selector: string): Promise<boolean> {
        return (await this.page.locator(selector).count()) > 0;
    }
}
