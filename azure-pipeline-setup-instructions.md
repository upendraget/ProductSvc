This project includes a GitHub Actions workflow to deploy to Azure App Service using ZIP deploy:

1. `azure-webapp-deploy.yml` - Zip deploy for code (non-container) App Service.

Required GitHub secrets (add in repository Settings -> Secrets -> Actions):

- `AZURE_WEBAPP_NAME` - Name of your App Service (e.g., my-app-service)
- `AZURE_WEBAPP_PUBLISH_PROFILE` - The publish profile XML from Azure Portal (App Service -> Get publish profile)

Azure setup steps (high-level):

1. Create a resource group, an App Service Plan, and an App Service (Windows or Linux) in the Azure Portal.
2. In the App Service page, select "Get publish profile" and download the publish profile XML.
3. Add the publish profile XML as the `AZURE_WEBAPP_PUBLISH_PROFILE` secret in your GitHub repo (Settings → Secrets → Actions).
4. Add your App Service name as the `AZURE_WEBAPP_NAME` secret.

Trigger: push to `main` branch will run the workflow and ZIP-deploy the repository contents to the App Service.
