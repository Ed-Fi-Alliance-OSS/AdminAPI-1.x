# ADMINAPI-1026 - CLONE (1.x) - Fix OnRelease Workflow to Delete Previous PreRelease *Tags* as well as GH Releases

**Type**: Bug

**Status**: In QA

## Description
The OnRelease Workflow is triggered when a GitHub Release is 'promoted' to a full release and is intended to promote the associated package, and "make it official" by removing previous pre-releases.

