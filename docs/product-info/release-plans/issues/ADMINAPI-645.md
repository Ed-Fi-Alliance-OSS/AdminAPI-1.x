# ADMINAPI-645 - Document practice example of creating multiple key/secret pairs from vendor listing file (1.x)

**Type**: Document

**Status**: Done

## Description
Story
-----


As an Admin API user, I would like a documented example of how to create multiple key/secret pairs using a CSV as an input and output file for the information that I need.  This example should provide example for an SEA (or large collaborative) that has multiple ODS instances and needs to automate the creation of key/secret pairs, as typically done at the beginning of a school term.


Details
-------


* Target ODS/API 6\.1 as the sample environment
* Use Admin API 1\.3 for the example
* Develop the script in a modern version of Python
* Use a CSV file with sample district IDs, vendor and application name as input values.  Rewrite to that CSV file with valid key/secrets.
* Include guidance to secure the CSV file in transit


 


Acceptance Criteria
-------------------


* TechDocs page in [https://techdocs.ed\-fi.org/display/ADMINAPI/Technical\+Information](https://techdocs.ed-fi.org/display/ADMINAPI/Technical+Information)
* Working documentation, sample file and Python scripts to illustrate key/secret creation en mass via CSV file
* Once complete, request feedback from SEA and/or other large implementations too see if it fits the field need




