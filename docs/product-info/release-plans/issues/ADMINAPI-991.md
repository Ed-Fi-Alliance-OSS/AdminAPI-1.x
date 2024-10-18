# ADMINAPI-991 - Update libxml

**Type**: Task

**Status**: Done

## Description
Vulnerability : CVE\-2024\-25062  

Severity : HIGH  

Package : pkg:apk/alpine/libxml2@2\.11\.6\-r0?os\_name\=alpine\&os\_version\=3\.19  

Affected range : \<2\.12\.5\-r0  

Fixed version : 2\.12\.5\-r0


 


[https://nvd.nist.gov/vuln/detail/CVE\-2024\-25062](https://nvd.nist.gov/vuln/detail/CVE-2024-25062)


There is a very low likelihood of this being exploitable, since our application isn't using XML. Still, it will be good to patch this. If the underlying Linux image isn't updated yet, then manually install the fixed libxml using apk.


 



