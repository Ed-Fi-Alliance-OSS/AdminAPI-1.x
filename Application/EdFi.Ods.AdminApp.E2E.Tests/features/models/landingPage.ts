// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

import { AdminAppPage } from "./adminAppPage";

export class LandingPage extends AdminAppPage {
    path(): string {
        return this.url;
    }

    hasPageTitle(): Promise<boolean> {
        return this.hasText({ text: "Ed-Fi ODS Admin App for Suite 3" });
    }
}
