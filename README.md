# CourseHub

## Overview:
CourseHub is a small ASP.NET Core MVC web application that demonstrates a role-based course platform using ASP.NET Identity.

The app allows users to:

* Register and log in

* Enroll in courses as students

* Create, edit, and delete courses as instructors

* Track enrolled and completed courses

* Filter, sort, enroll, unenroll, and mark courses as completed

The project is intentionally not a real learning platform (no video streaming or actual content). Its main purpose is to demonstrate authentication, authorization and Identity integration.

## Features:

Authentication & Authorization:

 * ASP.NET Core Identity with Entity Framework Core

 * Roles: Student, Instructor

 * Automatic redirect to login for protected actions

 * Return URL handling after login

Courses:

 * View all available courses

 * Create, edit, and delete courses (Instructor only)

 * Enroll and unenroll from courses

 * Mark courses as completed

 * Prevent instructors from enrolling in their own courses

Filtering & Sorting Capabilities:

 * Dynamic search bar

 * Filter by completion state

 * Sort by newest/oldest or recently completed

 * Clear Filters button

## Technologies Used
* ASP.NET Core MVC

* ASP.NET Core Identity

* EF Core

* SQL Server

## What I Learned
This project helped me understand:

* How ASP.NET Identity actually works under the hood

* How Identity stores users, roles, and claims in the database

* Proper handling of return URLs and login redirects

* How to avoid redirect loops and 405 errors

## Challenges I Faced
Some of the main challenges I struggled with during development:

* Understanding Identity routing

* Login redirects overriding controller logic

* ReturnUrl forming incorrectly and causing infinite redirects

* Handling enrollment logic correctly when users weren't logged in

* Making filters work together without polluting URL

* Bootstrap modals

* UX flows

## Configuration Instructions
* In the project folder, open the example appsettings file and replace the placeholder value with your desired connection string.
* Remove the .example extension
* You're good to go!




