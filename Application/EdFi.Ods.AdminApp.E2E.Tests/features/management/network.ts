// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

import { Response } from "playwright";
import { page } from "./setup";

export class Network {
    expectedURL = "";
    urlCallFound = false;

    tracker = (response: Response) => {
        if (response.url().includes(this.expectedURL)) {
            this.urlCallFound = true;
        }
    };

    startResponseTracking(url: string) {
        this.expectedURL = url;
        page.on("response", this.tracker);
    }

    stopResponseTracking() {
        page.removeListener("response", this.tracker);
        this.expectedURL = "";
    }

    async waitForResponse({
        url,
        status = 200,
        trackingResponse = false,
    }: {
        url: string;
        status?: number;
        trackingResponse?: boolean;
    }): Promise<void> {
        if (trackingResponse && this.urlCallFound && this.expectedURL === url) {
            this.stopResponseTracking();
            return Promise.resolve();
        }

        await page.waitForResponse((response) => {
            const hasURL = response.url().includes(url);
            if (hasURL && response.status() !== status) {
                this.stopResponseTracking();
                return Promise.reject(new Error(`Expected status ${status}, got ${response.status()}`));
            }
            const found = hasURL && response.status() === status;
            if (found) {
                this.stopResponseTracking();
            }
            return found;
        });
    }
}
