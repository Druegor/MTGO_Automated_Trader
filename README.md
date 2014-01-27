Magic the gathering automated trader
=====================

An automated bot to initiate and respond to trades through the MTGO client.  Initially created with AutoIt for the second version of the MTGO client I've decided to upgrade the client to C#.  It has gone through many different iterations.  The most recent has been to start the process of upgrading all the data calls away from Linq to SQL towards Dapper.  Its not quite as simple in certain circumstances but it is much faster and is a cleaner way to ensure the database transactions are happening how I want them.  

A few of the tasks I had planned for the bot before I wanted to release it
- Figure out how to do an update rather than a copy delete of the whole program as bugs are fixed.
- ServiceStack API for all the data connections so a connection string doesn't exist on the exe config.
- Figure out a good way to display popup messages to users in an unobtrusive manner that will not affect the bots execution so that a user can be individually notified of changes, updates, or downtimes.
- Find a way to get an estimate for card prices without causing excessive traffic to current online card traders.  The code maybe should broadcast the prices or allow some other way for a third party to collate all the price data for each running bot. 
- Finish the transfer functionality so that the bot can move cards between multiple accounts, this functionality was starting to flesh out but got stalled.  

Another note is that I use a framework library for logging that contains a few other pieces of functionality that I can not share in this library.  So I or someone else will most likley need to fix this before things will begin to compile. 
