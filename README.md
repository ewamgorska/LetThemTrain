# Note
 - There is no registration feature for admins. Each admin is added directly to a database in sqlite with a hashed password generated by an external program.
- Login interface is common for both users and admins - function first checks if credentials belong to an Admin, if not, it checks if they exist in the User table 
# Project setup
  In a project if database does not exist it's being created with one admin account and basic exercise library added to enable testing all implemented features such as "create a workout plan" or "admin log in"

#  Credentials for admin: 
  ```
  email: admin@gmail.com
  password: Admin1234@
  ```
  #  Credentials for test user: 
  ```
  email: user1@gmail.com
  password: User1234@
  ```
