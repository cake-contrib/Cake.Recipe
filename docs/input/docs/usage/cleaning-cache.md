---
Order: 25
Title: How to clean AppVeyor cache
---

This document describes how to clear the AppVeyor build cache using the `Cake.Recipe` script.

1. Make sure that you have the following environment variables set in your local development environment:
   - [APPVEYOR_API_TOKEN](../fundamentals/environment-variables#appveyor_api_token)
2. Run:
   - On Windows: `.\build.ps1 --target=clearcache`
   - On MacOS/Linux: `./build.sh --target=clearcache`
