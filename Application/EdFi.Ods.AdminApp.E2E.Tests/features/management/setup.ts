import { Before, BeforeAll, setDefaultTimeout } from "@cucumber/cucumber";
import { chromium, Browser, BrowserContext, Page } from "playwright";
import { ModelResolver } from "../models/modelResolver";

import dotenv = require("dotenv");

dotenv.config();
setDefaultTimeout(60 * 1000);

export let browser: Browser;
export let page: Page;
export let context: BrowserContext;
export let models: ModelResolver;
export let currentScenarioName: string;

Before(async (scenario) => {
    context = process.env.RECORD
        ? await browser.newContext({
              recordVideo: { dir: "./videos" },
              acceptDownloads: true,
              ignoreHTTPSErrors: true,
          })
        : await browser.newContext({ acceptDownloads: true, ignoreHTTPSErrors: true });
    page = await context.newPage();
    models = new ModelResolver(page);
    setScenarioName(scenario.pickle.name);
});

BeforeAll(async () => {
    browser =
        process.env.GITHUB_ACTIONS || process.env.HEADLESS
            ? await chromium.launch()
            : await chromium.launch({ headless: false });
});

function setScenarioName(scenarioName: string) {
    currentScenarioName =
        scenarioName.indexOf("Outline") > -1
            ? scenarioName.substr(scenarioName.indexOf(":") + 1, scenarioName.length)
            : scenarioName;
}
