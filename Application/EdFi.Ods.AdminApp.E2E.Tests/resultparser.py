# SPDX-License-Identifier: Apache-2.0
# Licensed to the Ed-Fi Alliance under one or more agreements.
# The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
# See the LICENSE and NOTICES files in the project root for more information.

import json
import xml.etree.cElementTree as ET

def readReport():
  all_tests = []
  total_time = 0

  with open("./reports/report.json") as json_file:
    jsonContent = json.load(json_file)

    for feature in jsonContent:

      test_results = []
      duration = 0
      for scenario in feature["elements"]:
        success = True
        errorMessage = None
        for step in scenario["steps"]:

          duration += step["result"]["duration"]

          if "error_message" in step["result"]:
            errorMessage = step["result"]["error_message"]

          success = step["result"]["status"] == "passed"
          # Once we find a failing step, the full test has failed
          if success == False:
            break

        current = {"name": feature["name"] +" - " + scenario["name"], "success": success}
        if errorMessage:
            current["errorMessage"] = errorMessage

        existing = next((item for item in test_results if (item["name"] == current["name"])), None)
        # If we already had the test saved and was successful but now the test is failing, we need to overwrite it
        if existing and existing["success"] is True and current["success"] is False:
          index = test_results.index(existing)
          test_results.pop(index)
          existing = None

        if not existing:
          test_results.append(current)

      total_time += duration
      all_tests.append({"feature_name": feature["name"], "tests": test_results, "time": round(duration / (10 ** 11), 5)})

    total_time = total_time / round((10 ** 11), 5)
    return {"tests": all_tests, "time": total_time}

def buildXML(all_tests, total_time):
  root = ET.Element("testsuites", {"name": "Admin App Automation", "time": str(total_time)})
  tree = ET.ElementTree(root)

  for feature in all_tests:
    testsuite = ET.SubElement(root, "testsuite", {"name": feature["feature_name"], "time": str(feature["time"])})

    for item in feature["tests"]:
      testcase = ET.SubElement(testsuite, "testcase", {"name": item["name"]})
      if not item["success"]:
        failure = ET.SubElement(testcase, "failure")
        failure.text = item["errorMessage"]

  tree.write("reports/playwright-results.xml")

result = readReport()
buildXML(result["tests"], result["time"])
