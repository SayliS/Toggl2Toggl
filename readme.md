# Toggl2Toggl CLI

## Toggl2Toggl [options] [command]

### Options:
  `--version` -   Display version information

### Commands:
  * `show`  -  Shows all calculated entries which could be added to the workspace
  * `sync`  -  Calculates all time entries from the provided toggl workspace and performs a synchronization to the other workspace


- - -

  
#### `show`:
 ##### Shows all calculated entries which could be added to the workspace

  * Usage:
  
  	`Toggl2Toggl show [options]`

  * Options:
  * 
  `--toggle, --toggle-api-key <TOGGLE-API-KEY>` -                           Api key from your Toggl Account
  `--ado, --ado-personal-access-token <ADO-PERSONAL-ACCESS-TOKEN>` -        Personal Access Token from your Azure DevOps Account
  `--url, --ado-organization-url <ADO-ORGANIZATION-URL>` -                  URL to your organization in Azure DevOps
  `--from, --from-date <FROM-DATE>    ` -                                   Date from which to track your time
  `--to, --to-date <TO-DATE> ` -                                          Date to which to track your time
  `--workspace-from, --toggle-from-workspace <TOGGLE-FROM-WORKSPACE> ` -    Workspace from which to get the track items
  
  * Example:
  
	`Toggl2Toggl show --toggle-api-key {toggle_api_key} --ado-personal-access-token {ado-personal-access-token} --ado-organization-url https://{organization-name}.visualstudio.com --from-date "05/10/2019" --to-date "05/15/2019" --toggle-from-workspace "{workspace-name}"`
    
	`Toggl2Toggl show --toggle {toggle_api_key} --ado {ado-personal-access-token} --url https://{organization-name}.visualstudio.com --from "05/10/2019" --to "05/15/2019" --workspace-from "{workspace-name}"`
    
- - -



#### `sync`:
  ##### Calculates all time entries from the provided toggl workspace and performs a synchronization to the other workspace

* Usage: 

	`Toggl2Toggl sync [options]`

* Options:

  `--toggle, --toggle-api-key <TOGGLE-API-KEY>` -                          Api key from your Toggl Account
  `--ado, --ado-personal-access-token <ADO-PERSONAL-ACCESS-TOKEN>` -        Personal Access Token from your Azure DevOps Account
  `--url, --ado-organization-url <ADO-ORGANIZATION-URL>` -                  URL to your organization in Azure DevOps
  `--from, --from-date <FROM-DATE>  ` -                                    Date from which to track your time
  `--to, --to-date <TO-DATE> ` -                                           Date to which to track your time
  `--workspace-from, --toggle-from-workspace <TOGGLE-FROM-WORKSPACE> ` -    Workspace from which to get the track items
  `--workspace-to, --toggle-to-workspace <TOGGLE-TO-WORKSPACE>` -           Workspace to which to create the new items
  
* Example:
	`Toggl2Toggl show --toggle-api-key {toggle_api_key} --ado-personal-access-token {ado-personal-access-token} --ado-organization-url https://{organization-name}.visualstudio.com --from-date "05/10/2019" --to-date "05/15/2019" --toggle-from-workspace "{workspace-name}" --toggle-to-workspace "{workspace-name}"`
    
	`Toggl2Toggl show --toggle {toggle_api_key} --ado {ado-personal-access-token} --url https://{organization-name}.visualstudio.com --from "05/10/2019" --to "05/15/2019" --workspace-from "{workspace-name}" --workspace-to "{workspace-name}"`
  
  
 
  
  
