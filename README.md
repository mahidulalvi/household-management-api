# household-management-api

Household Management API is household budget management backend application which gives us the endpoints for managing our ['Household Manager'](https://github.com/mahidulalvi/household-manager) application. It is built using C# and the ASP.NET framework's Web API template. 

The project requirements are sourced from `Coder Foundry`.


## logic

This application will let users to manage their household budget. Household owners have access to CRUD functionalities for Household, Invitations(for new members delivered via email), Categories, Bank Accounts & Transactions. Standard users can create/edit Transactions and view all other Model informations.

All users also have access to all account management features such as registration, login, setting passwords, retrieving forgotten passwords, changing passwords and logout.


## special libraries used

	* AutoMapper - AutoMapper lets us map Model information to our View Models & 
	  Binding Models and vice versa. This quickens our development speed.


# project setup

Clone the project and clean and rebuild solution. Then create a file named 'private.config' in the project directory('HouseholdManagementAPI'). In this file create these xml tags:

```
	<appsettings>
	  <add key="SmtpPassword" value="" />
	  <add key="SmtpHost" value="" />
	  <add key="SmtpPort" value="" />
	  <add key="SmtpFrom" value="" />
	  <add key="SmtpUsername" value="" />
	</appsettings>`
```

Insert your smtp configuration in the tags value attributes. The `private.config` file is
added in gitignore and should not be added to VCS.

After following the above instructions, open the project in Visual Studio and run `update-database` command to initialize the database. Make sure MSSQL Server is installed and running. Installing and using MSSQL Server Management Studio may help in managing the database.

Now the project can be run using `F5`(for debugging) or `CTRL + F5` for running without debugging.

&nbsp;

&nbsp;

&nbsp;

## project requirements

For anyone intersted, the project specific requirements can be found ['here'](./Coder Foundry - Project Requirements.docx.pdf).
