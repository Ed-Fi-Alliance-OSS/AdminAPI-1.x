import { mkdir, writeFile } from "fs/promises";
import { currentScenarioName, page } from "./setup";

export function saveLog(error: string): void {
    const logFolder = "./logs";
    mkdir(logFolder).catch(() => {});
    const content = `${new Date().toISOString()}\n${error}`;
    writeFile(`${logFolder}/ERROR ${currentScenarioName}.txt`, content).catch();
}

export async function takeScreenshot(name: string): Promise<void> {
    await page.screenshot({
        path: `./screenshots/${currentScenarioName} - ${name}.png`,
    });
}
