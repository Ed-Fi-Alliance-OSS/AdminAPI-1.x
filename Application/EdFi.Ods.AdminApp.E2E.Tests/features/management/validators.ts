import { page } from "./setup";
import { ok, strictEqual } from "assert";

export function validatePath(path: string, full = false): void {
    const url = page.url();
    if (full) {
        strictEqual(url, path, "Path does not match");
    } else {
        ok(url.includes(path), `Path ${url} does not include ${path}`);
    }
}
