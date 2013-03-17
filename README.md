#ServerRI#
========

An fully working overly engineered server side webapi solution.

The architecture for this server side solution incorporates the following patterns and technologies:

- Entity Framework 5x (Code First)
- Repository Pattern
- Unit of Work Pattern
- IoC (using AutoFac)
- A Service API using DTOs (models) and AutoMapper
- .NET WebAPI REST architecture (with REST-RPC extensions when needed).

The solution is very enterprisey with lots of layers to enable extreme abstraction and unit testing. 

It aims to demonstrate how to handle:
 - exceptions
 - logging
 - validation
 - unit testing (mocking)
 - e2e testing (end-to-end)
 

*NOTE:* If you're looking for something simpler I suggest you move along.

##Pre-Requisites:##

.Net 4.5
Visual Studio 2012
SqlServer Express 2012 

##Getting Started:##

###Database###
Attach the Data/AdventureWorksLT2012_Data.mdf to an instance of SqlServer Express 2012 and call it "AW".
Alternately, change connection strings to work with your own configuration (Core/Configuration/ConnectionStrings.config)

###Logging###
Modify Core/Configuration/Log4Net.config SMTPAppender to use your own smtp server.

Once you've completed these steps, you should be able to run the unit test projects and also fire up the webapi project and hit it.
