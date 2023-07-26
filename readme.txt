THUNDERDOME SAMPLE


INTRODUCTION:
---------------------------------
This program allows data to be pushed to Vault clients.  It also allows a user to back up their various client settings.
This project is published as open source; a sample application installer can be found in the Releases section. Respect the disclaimer of the installable sample.

REQUIREMENTS:
---------------------------------
- Vault Workgroup/Professional 2022


TO CREATE and MANAGE DEPLOYMENTS (ADMIN):
---------------------------------
1. Run the install and log-in Vault Explorer as a user with role "Configuration Administrator".
2. Create a new folder for the purposes of storing your deployment(s).
3. Set the security on the folder so that you have full access, but everyone else has read-only access.
4. Under the Tools menu, select "Tools->Thunderdome->Manage Deployments"
5. In the Create Deployment dialog, provide a name and select your deployment folder. 
6. Select the contents of the deployment package; the tree offers all standard content detected on your machine. You can select additional folders and its content to be copied to the deployment.
7. Click OK.
8. Now, all other Vault users who have Thunderdome will be prompted to receive updates the next time they log in to Vault Explorer.


TO USE (NON-ADMIN):
---------------------------------
Run the install and start Vault Explorer. 
If the administrator has set up a deployment, you may be prompted to receive updates.  If you agree, you will need to exit Vault Explorer for the update to complete.

If you want to backup your Vault settings "Thunderdome->Backup Vault Settings" command from the tools menu. The command zips up all your client Vault settings into a single file.
If you want to restore your settings, extract the zip to the Autodesk folder of your user application settings, e.g., "C:\Users\MyUsername\AppData\Roaming\Autodesk"


NOTES:
---------------------------------
- For security reasons, files related to login are not part of the backup nor can they be added to deployments.
- You can configure and manage multiple deployments.
- Deploying "Configuration Files" will overwrite user's existing settings. For example, deploying "Shortcuts" will overwrite user's shortcuts with yours.
- Plug-ins will only be deployed if the user does not already have the plug-in. Thunderdome will not attempt to update or patch existing plug-ins.
- If somebody selects the "No, and never ask me again" option when prompted then later changes their mind, the dialog can be re-activated by deleting the "settings.xml" file in the %MyDocuments%\Thunderdome 20xx folder.
- Not all plug-ins can be part of a deployment.  Only plug-ins from justonesandzeros.typepad.com and plug-ins signed with Thunderdome.snk will be included.  This behavior is to avoid conflicts with the Vault App Store.  If you want your plug-in to be deployable, you can find Thunderdome.snk by downloading the source code.
- In the Create Deployment dialog.  The first level of checkboxes is for multi-select only.  The second-tier checkboxes is what determines the contents of the package.
- When deploying DECO files, you will probably want to include the "DECO settings" entry.  That's the file that determines which XAML files are hooked to which custom objects.


CREATING DERIVATIVES:
---------------------------------
Thunderdome is open source. You are expected to build your individual application instead of using the sample application installer (Releases) in production environments. Use the code as a starting point and:
- rename the application and assemblies
- remove Copy Rights of Autodesk
- replace all icons, the included icons are limited for redistribution by Autodesk.
- include the Thunderdome Project link and License in your Third-Party-Notices


RELEASE NOTES:
---------------------------------
2023.1.1: Added filtering for _V folders in Folder deployments; fixed search group deployment
2023.0: updated for 2023
2022.1: Support for Saved Searches Groups added; Minor updates on the Tools->Thunderdome Pullout-Menu

2022.0.0 - Next Generation of Thunderdome:
Autodesk Consulting contributed many customer project driven enhancements, like Client language support de-XX, managing multiple deployments and user defined addition of folders to be deployed to Vault Workspace locations or any other target. This version is available as open source and installable sample application.

For 2020 and older releases review the readme: https://github.com/koechlm/Vault-Thunderdome.

Project Thunderdome 2019, 2020, 2021 sample applications had been published by AMC Bridge LCC on Autodesk App Store.


Sincerely,
Markus Koechl
