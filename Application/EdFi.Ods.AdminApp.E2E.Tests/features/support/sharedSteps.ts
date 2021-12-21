import { Given } from "@cucumber/cucumber";
import { models, context } from "../management/setup";
import { validatePath } from "../management/validators";

Given("user is logged in", async () => {
    await models.loginPage.fullLogin(process.env.email, process.env.password);
    await context.storageState({ path: "./state/login-state.json" });
});

Given("it's on the {string} page", async (pageName: string) => {
    let currentPage;

    switch (pageName) {
        case "Activity":
            break;
        case "Log in":
            currentPage = models.loginPage;
            break;
        default:
            break;
    }

    if (currentPage) {
        await currentPage.navigate();
        validatePath(currentPage.path());
    }
});
