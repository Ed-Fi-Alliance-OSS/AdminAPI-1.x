// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

import { mkdir } from "fs/promises";
import { APIRequestContext, request } from "playwright";
import { Credentials } from "../interfaces";
import { context, currentTest, page } from "./setup";

export async function saveTrace(): Promise<void> {
    if (process.env.TRACE) {
        const traceFolder = "./traces";
        mkdir(traceFolder).catch(() => {});
        const path = `${traceFolder}/${currentTest.Feature}/${currentTest.Scenario}/trace.zip`;

        await context.tracing.stop({ path });
    }
}

export async function takeScreenshot(name: string): Promise<void> {
    await page.screenshot({
        path: `./screenshots/${currentTest.Feature}/${currentTest.Scenario}/${name}.png`,
    });
}

export function getRandomString(len: number): string {
    let randomWord = "";
    for (let i = 0; i < len; ++i) {
        randomWord += "a";
    }
    return randomWord;
}

export async function getApiContext(extraHTTPHeaders?: {
    Authorization: string;
    "Content-Type"?: string;
}): Promise<APIRequestContext> {
    return await request.newContext({
        ignoreHTTPSErrors: true,
        extraHTTPHeaders,
    });
}

export async function testURL(url: string): Promise<boolean> {
    const apiContext = await getApiContext();
    const getAPI = await apiContext.get(url);

    if (!getAPI.ok()) {
        console.error(`Unable to verify URL. Response: ${getAPI.status()}`);
    }

    return getAPI.ok();
}

export async function getAccessToken(credentials: Credentials): Promise<string | null> {
    //May need to refactor for year specific mode
    const apiURL = credentials.URL.substring(0, credentials.URL.indexOf("data") - 1);
    const tokenURL = `${apiURL}/oauth/token`;
    const encryptedCredentials = Buffer.from(credentials.Key + ":" + credentials.Secret).toString("base64");

    const apiContext = await getApiContext({
        Authorization: `Basic ${encryptedCredentials}`,
        "Content-Type": "application/x-www-form-urlencoded",
    });

    const response = await apiContext.post(tokenURL, { data: "grant_type=client_credentials" });

    let token = null;
    if (response.ok()) {
        const jsonResponse = await response.json();
        token = jsonResponse.access_token;
    }
    return token;
}

export async function isTokenValid({ api, token }: { api: string; token: string }): Promise<boolean> {
    const apiContext = await getApiContext({
        Authorization: `Bearer ${token}`,
    });

    const response = await apiContext.get(`${api}/ed-fi/academicWeeks`);
    return response.status() !== 401 && response.status() !== 404;
}
