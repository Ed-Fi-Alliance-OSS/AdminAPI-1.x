import { Then, When } from "@cucumber/cucumber";
import { ok } from "assert";
import { takeScreenshot } from "../management/functions";
import { models } from "../management/setup";
import { validatePath } from "../management/validators";

When("user enters valid username and password", async () => {
    await models.loginPage.fillForm(process.env.email, process.env.password);
});

When("clicks Log in", async () => {
    await models.loginPage.login();
});

Then("login is successful", async () => {
    validatePath(models.homePage.path(), true);
    ok(await models.homePage.hasGlobalOption());
    ok(await models.homePage.hasSettingsOption());
    await takeScreenshot("login successful");
});
