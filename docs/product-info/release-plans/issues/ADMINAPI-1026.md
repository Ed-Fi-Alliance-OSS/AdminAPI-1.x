# ADMINAPI-1026 - CLONE (1.x) - Fix OnRelease Workflow to Delete Previous PreRelease *Tags* as well as GH Releases

**Type**: Bug

**Status**: Done

## Description
The OnRelease Workflow is triggered when a GitHub Release is 'promoted' to a full release and is intended to promote the associated package, and "make it official" by removing previous pre\-releases.


Currently, the Workflow successfully removes those Pre\-Releases from GitHub, but leaves behind all the git tags. We need to fix this so that once a version is officially released, the previous tags are cleaned up (leaving only release version tags for posterity)


**Acceptance Criteria**


* OnRelease Workflow removes prior Pre\-Release **version tags** from git
* OnRelease Workflow continues to remove prior Pre\-Release GH Releases
* OnRelease Workflow does not remove "release" version tags *or* GH Releases




