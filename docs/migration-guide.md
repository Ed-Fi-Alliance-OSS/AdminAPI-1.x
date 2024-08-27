# Migrate from previous versions

To migrate from a previous version (2.2.0) to a new version (2.2.1) you need to replace the old version binaries with the new ones.

The steps to migrate to the new version are shown for both an installation in a docker container and an installation in IIS.

## Building the new version

### Step 1: Download the latest version

First, download the latest version and extract the contents of the sources directory to a directory to build the new version binaries.

### Step 2: Build and publish the new version binaries

Go to the root directory and execute the build and publish command to generate the new binaries.

```bash
.\build.ps1 BuildAndPublish       
```

### Step 3: Update the binaries for your installation

You can find the steps below to update the binaries for a Docker Container or a IIS installation

- [Docker Container](./migration-guide-docker.md)
- [IIS Installation](./migration-guide-iis.md)
