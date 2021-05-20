# Maraki1982

The application is divided into three parts:  
- The first one is the Core part, where all the database code first and the communication with the services providers exists.
- The second one is the Web part, where the redirect API endpoint exists, as well as all the functionality.
- The third one is the Token Refresher app, which is a console app that refreshes the service providers aquired tokens.

## Core Part

- Contains all the database models
- Contains all the database scheme
- Contains all the Microsoft's API communication
- Contains all the Google's API communication

## Web Part

- Provides a malicious URL
- Provides service provider user management
- Provides refresh token functionality
- Provides email folders (get + display)
- Provides emails (get + display for every folder)
- Provides drives (get + display)
- Provides drives folders + subfolders (get + display)
- Provides folder files (get + display + public download link)
- All the results are pageable

## Token Refresher Part

- Provides a way to automatically run tokens refresh
- You can create a scheduled job to run it

## Web Settings

In order to run the web application there are a couple of things that need set up.  
All these settings are at the appsettings.json file as presented below:  

``` json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "Authentication": {
    "Microsoft": {
      "ClientId": "",
      "ClientSecret": "",
      "BaseUri": "https://graph.microsoft.com/v1.0",
      "TokenUri": "https://login.microsoftonline.com/common/oauth2/v2.0/token",
      "RedirectUri": "https://localhost:5001/office/token",
      "FileToDownload": "Super Important Document.docx",
      "MaliciousUrlCraft": {
        "AuthorizationUrl": "https://login.microsoftonline.com/common/oauth2/v2.0/authorize",
        "ResponseType": "id_token code",
        "Scope": "offline_access Directory.Read.All Directory.AccessAsUser.All files.read.all user.read mail.read openid profile",
        "State": "12345",
        "Nonce": "678910",
        "ResponseMode": "form_post"
      },
      "ExceptionRedirectUrl": "/Home/Index/?errorMessage=Error while using the Microsoft Malicious URL"
    },
    "Google": {
      "ClientId": "",
      "ClientSecret": "",
      "BaseUri": "https://www.googleapis.com",
      "TokenUri": "https://oauth2.googleapis.com/token",
      "RedirectUri": "https://localhost:5001/google/token",
      "FileToDownload": "Super Important Document.docx",
      "MaliciousUrlCraft": {
        "AuthorizationUrl": "https://accounts.google.com/o/oauth2/v2/auth",
        "Scope": "https://mail.google.com/ https://www.googleapis.com/auth/userinfo.email https://www.googleapis.com/auth/userinfo.profile https://www.googleapis.com/auth/drive",
        "AccessType": "offline",
        "IncludeGrantedScopes": "true",
        "ResponseType": "code",
        "State": "12345",
        "Prompt": "consent"
      },
      "ExceptionRedirectUrl": "/Home/Index/?errorMessage=Error while using the Google Malicious URL"
    }
  },
  "AllowedHosts": "*",
  "General": {
    "DbType": "SQLite", //Can be SQLite, SQLServer, PostgreSQL
    "PagedResultsSize": 10,
    "GlobalExceptionRedirectUrl": "/Home/Index/?errorMessage=StatusCodeException error"
  },
  "ConnectionStrings": {
    "OAuthServerContextConnection": "Data Source=../Database/Maraki1982.db;"
  }
}
```

Settings Explained:  
  
Authentication:  
- Authentication:Microsoft:ClientId = The client id from the Azure Portal  
- Authentication:Microsoft:ClientSecret = The client secret from the Azure Portal  
- Authentication:Microsoft:BaseUri = The Microsoft's graph api base URL (should not be changed)  
- Authentication:Microsoft:TokenUri = The Microsoft's token api from the OAuth2 URL  
- Authentication:Microsoft:RedirectUri = The server redirect URL found at RedirectHandlerController.cs  
- Authentication:Microsoft:FileToDownload = The arbitrary file to be downloaded  
- Authentication:Microsoft:MaliciousUrlCraft = The settings for the malicious URL  
  
<b>Note:</b> the same settings should be provided for the Authentication:Google.

General:  
- General:DbType = The database type. The code first approach was followed to allow multiple database type with the same codebase  
- General:PagedResultsSize = The number of results displayed in pages  
- General:GlobalExceptionRedirectUrl = The global exception redirect url.  
  
Connection Strings:  
- ConnectionStrings:OAuthServerContextConnection = The database connection string  
  
## How to run the application  
  
- Make the appropriate changes at the appsettings.json  
- Change the default email and password at Maraki1982.Core\DAL\Configuration\UserModelConfiguration.cs  
- Open the project with Visual Studio and select the Maraki1982.Web project  
- Open the Package Manager Console and run the following commands:  
```
Add-Migration InitializeSchema
Update-Database  
```
- Builde the solution  
- Hit the start button  
  
## Add Database Migrations  
  
- Open the Package Manager Console and run the following commands:  
```
Add-Migration MIGRATION_NAME  
Update-Database  
```