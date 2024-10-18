# ADMINAPI-1007 - Using Authority Setting for JWT Issuer (Admin API 1)

**Type**: Bug

**Status**: Done

## Description
Steps to Reproduce
------------------


* Set different values for the appsettings keys "Authority" and "Issuer"
* Authenticate with Admin API
* Inspect the received token, for example in <https://jwt.io>


Expected Result
---------------


The Issuer claim (iss) should contain the Issuer URL.


The Authority value is not used anywhere


Actual Result
-------------


The opposite \- uses the Authority as the Issuer, and ignores the IssuerUrl setting.



