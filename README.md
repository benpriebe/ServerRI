ServerRI
========

An fully working overly engineered server side webapi solution

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
