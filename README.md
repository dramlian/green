## A small introduction to the solution

The solution contains two projects, the
standalone REST API solution together
with the project that tests the
given solution.

Important note:
Players themselves are handled using
a "key", a combination of the provided
name (e.g., "adam") and a generated
GUID value. These values are
retrievable through calling /getUsers.

Specifically, I did not completly 
understand two points in
the task:

1. "balance may never drop below 0"
My understanding and implementation:
If a player has 50 euros and loses 500,
then their resulting balance is 0.

2. "credit transaction to player's wallet"
My understanding and implementation:
Transfer from one player's account to
another player's account.

The project also contains codecoverage
report: 

"\coveragereport\idex.html"
----------------------------------------
