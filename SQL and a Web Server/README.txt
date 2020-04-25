Author:     Aaron Morgan and Xavier Davis
Partner:    Xavier Davis and Aaron Morgan
Date:       4/17/2020
Course:     CS 3500, University of Utah, School of Computing
Assignment: Assignment #9 - SQL and a Web Server
Copyright:  CS 3500, Xavier Davis and Aaron Morgan - This work may not be copied for use in Academic Coursework.
Github Repository: https://github.com/uofu-cs3500-spring20/assignment-9-web-server-and-sql-quaranteens
Commit #:  

1. Comments to Evaluators:
    
    A) User Secrets

    We're not sure how other students completed their assignment, but we decided to make a new project to handle the communication with the SQL database. 
    We're hoping that the creation of this extra project isn't going to conflict with anything (particularly the User Secrets), but we weren't having any
    problems with it on our end.

    B) Instrumenting the Agario Client code

    We ran into some issues with referencing our Database project when we tried to run the Client_and_GUI from our Agario assignment. For some reason, it seemed
    to work fine when Xavier ran it, but when Aaron pulled the code from Git, the code didn't recognize one of the methods from AgarioDatabase. What is even more odd 
    about it, is that the referencing seemed to be working correctly; the Client_and_GUI code did recognize the AgarioDatabase class, but it was only a single 
    method (Insert_Player_Data) that wasn't being recognized, which of course, was the one we needed to use.

    We tried to solve the problem by figuring out if there was anything wrong with the references, but it all seemed to be set up correctly. The "Quick Action"
    option was also incredibly unhelpful; instead of suggesting that we implement the method, which is what we thought it would suggest, it actually suggested to store 
    the call into a variable (which didn't do anything). We searched the internet numerous times to see if anyone else had a similar problem, but couldn't find any 
    information about it. We considered making a post on Piazza, but then we realized that we're coming up on Finals week, and we didn't feel confident that 
    we would get a response in time.

    Instead, we simply added the Database project to our Agario solution, so we hope that this doesn't affect running the Client_and_GUI if it's still being
    graded from Assignment 8.

2. Assignment Specific Topics

    A) Time Tracking 

            Expected Time to Complete: 18 hours.

                Time spent on Analysis: 5.5
                Time spent on Implementation: 8.5
                Time spent Debugging: 2
                Time spent Testing: 5

            Total Time: 21 hours.

3. Database Table Summary:

    We chose to create three distinct tables to store what we believed we needed to display. The tables we created are:

        1) The HighScores table - This table includes all of the high scores gathered from the various players of the Agario game. For any given player,
        this table holds a compilation of their highest scores, including: Mass, Rank, and Longest Time Alive.

        2) The TimeInFirst table = This table keeps track of all things related to the player in first place. The columns include: The Date/Time they entered
        first place, how long the player stayed in first place, and how many other players they absorbed.

        3) The indiviual player table - This is a created table, but we do have one in our database for display purposes (that contains made-up data for VC Jim).
        This table is created whenever a new player is input into the url scheme, or whenever a new player is added via the Agario game. The columns for this
        table are identical to the HighScores table, but with one extra column, the Game Session, which tracks the game ID.

    We considered creating other tables, but we realized it wasn't really necessary. One idea was to create a table to store more information about the Game
    Session, but then realized the Game Session is tied to each individual player. This means that any information about that Game Session can be simply added to
    the individual player's table instead.

    We ended up hardcoding the information found in the TimeInFirst table, mainly because we felt it was a little strange to change the code in the Client_and_GUI class
    to accomodate for information that we (currently) didn't have. In the Agario game code, there were no other real players being generated, so by consequence there
    are no real ranks being generated. We did hardcode a set number for the rank, but it didn't make much sense to alter the code further just for a timestamp that
    would ultimately not have any practical use.

4. Extent of work:

    1) Our code does run the web server correctly, and displays a main web page when the user tries to connect to localhost:11000. The main page is fairly
    simple, with a title and a couple of links to the other important pages.

    2) localhost:11000/highscores correctly links to the respective HighScores table, and displays the data correctly in an html table.

    3) localhost:11000/scores/name/highmass/highrank/starttime/endtime correctly inserts the data into the player's respective table, and if the player does
    not have a table created, it will create one. Instead of assuming that the "highmass" and "highrank" values given are in fact higher scores (than previous entries),
    we check the player's table to verify first, then insert it into the HighScore table if appropriate. After the information has been inserted into the
    appropriate tables, a confirmation page is displayed to show that the insertion was successful.

    4) localhost:11000/timeinfirst displays the data from the aforementioned TimeInFirst table, in an html table. We decided to format the page with its own
    colored background, as well as color the table's borders.

    5) We thought it would be best software practices to separate the handling of the SQL connection and its commands into a different project. We created
    a Database project and an AgarioDatabase class in order to accomplish this.

5. Partnership:

    All of the code was completed via Pair Programming, and neither of us worked alone for the duration of the assignment.

6. Testing:

    We didn't use any Unit Tests to test our code, but we used Whitebox, Integration, and Incremental testing instead. Every time we
    implemented a new method, we ran the WebServer and tried to connect to one of the url pages to verify our method's functionality.

    We used SSMS as well to test SQL commands before attempting to write the commands in C#. We used numerous different queries to view the information
    found in each table after attempting to confirm it was modified correctly.

    When we instrumented the Agario code, we tested to verify that the Client_and_GUI was interacting with the AgarioDatabase, and then we tested to
    verify that the data gathered from the Client_and_GUI was being displayed on the web page.

7. Consulted Peers:

    None.

8. Branching:
    
    Branching was not used in this assignment.

9. References:

    1) w3schools.com: HTML Tables: https://www.w3schools.com/html/html_tables.asp
    2) Stack Overflow: Insert multiple rows WITHOUT repeating the “INSERT INTO …” part of the statement?: https://stackoverflow.com/questions/2624713/insert-multiple-rows-without-repeating-the-insert-into-part-of-the-stateme
    3) Stack Overflow: Iterate through DataSet: https://stackoverflow.com/questions/10822304/iterate-through-dataset
    4) DeveloperFusion: ADO.NET: Populate a DataSet from a Database: http://quickstart.developerfusion.co.uk/quickstart/howto/doc/adoplus/GetDataFromDB.aspx
    5) w3schools.com: HTML <table> border Attribute: https://www.w3schools.com/tags/att_table_border.asp
    6) w3schools.com: HTML <table> cellpadding Attribute: https://www.w3schools.com/tags/att_table_cellpadding.asp
    7) Stack Overflow: What's wrong with my IF/ELSE? “ELSE: Incorrect syntax near 'ELSE'.”: https://stackoverflow.com/questions/27696510/whats-wrong-with-my-if-else-else-incorrect-syntax-near-else
    8) w3schools.com: SQL ALTER TABLE Statement: https://www.w3schools.com/sql/sql_alter.asp
    9) Stack Overflow: Convert milliseconds to human readable time lapse: https://stackoverflow.com/questions/9993883/convert-milliseconds-to-human-readable-time-lapse
    10) Stack Overflow: Check if table exists and if it doesn't exist, create it in SQL Server 2008: https://stackoverflow.com/questions/5952006/check-if-table-exists-and-if-it-doesnt-exist-create-it-in-sql-server-2008
    11) w3schools.com: SQL UPDATE Statement: https://www.w3schools.com/sql/sql_update.asp
    12) Microsoft: Variables (Transact-SQL): https://docs.microsoft.com/en-us/sql/t-sql/language-elements/variables-transact-sql?view=sql-server-ver15


