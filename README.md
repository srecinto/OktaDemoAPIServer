# Okta Demo API Server

This project was built using .net 4.5.2

This is bare bones basic code to create a REST API that calls into Okta's API to perform basic crud operations on an org with "customerType" and "loyaltyPoints" as custom attributes.
NOTE: The code is not set up to catch edge cases nor perform robust validation and is meant to serve as a simple example of how to use the Okta API wrapped by a custom API

## Requirements
* .Net SDK 4.5.2
* Visual Studio 2015
* Okta Org URL
* Okta API Token

## Dependencies
See NuGet for libraries

## How to Run

Open up solution file (OktaDemoAPIServer.sln) into Visual Studio 2015

Edit the web.config in the OktaDemoAPIServer project

under
'
<configuration>
  <appSettings>
    <add key="okta:Org" value="myorg.oktapreview.com"/>
    <add key="okta:ApiKey" value="myApiKey"/>
  </appSettings>
  ...
</configuration>
'
set your Okta Org url to the value for key "okta:Org"
next set the API Token you configured in Okta to value for the key "okta:ApiKey"

Build your code

Run

Open your browser and navigate to the URL for the local "Okta Demo Customer Website"
i.e. http://localhost:59142/

A webpage with a username and passwourd should appear.

Login with your Okta Credentials and you should see details on your account