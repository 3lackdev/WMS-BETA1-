# WorkFlowEngine PHP Conversion

This directory contains PHP conversions of the original ASP.NET WorkFlowEngine application.

## Structure

- `/api` - API endpoints converted from ASP.NET to PHP
  - `/api/getdata.php` - Endpoint for retrieving workflow data
  - `/api/line/default.php` - Endpoint for LINE integration

- `/includes` - Shared classes and utilities
  - `/includes/Database.php` - Database connection and query handling
  - `/includes/Authentication.php` - User authentication and session management
  - `/includes/WorkflowHelper.php` - Workflow operations and data retrieval
  - `/includes/header.php` - Common header template
  - `/includes/footer.php` - Common footer template

- Core application files
  - `index.php` - Dashboard/home page (converted from Default.aspx)
  - `login.php` - Login page
  - `logout.php` - Logout script
  - `workflowlist.php` - List of workflows (converted from workflowlist.aspx)
  - Additional files to be converted as needed

## Requirements

- PHP 7.4 or higher
- MySQL 5.7 or higher
- MySQLi extension for PHP
- Session support enabled in PHP

## Configuration

1. Update database connection details in `includes/Database.php`:
   - host
   - username
   - password
   - database name

2. Make sure the MySQL schema has been properly imported into your database

3. Configure your web server (Apache, Nginx, etc.) to serve the PHP files

4. Set appropriate file permissions:
   - Read/write access to directories that need to store uploaded files
   - Read/execute access to PHP files

## Installation

1. Clone or copy this repository to your web server's document root or subdirectory
2. Import the MySQL schema into your database server
3. Update the database configuration in `includes/Database.php`
4. Access the application through your web browser

## Usage

1. Navigate to the login page and sign in with valid credentials
2. The dashboard displays workflows assigned to or created by the current user
3. View all workflows in the workflow list page
4. Create, view, and manage workflows based on user permissions

## Directory Structure Mapping (ASP.NET to PHP)

| ASP.NET | PHP |
|---------|-----|
| Default.aspx | index.php |
| login.aspx | login.php |
| workflowlist.aspx | workflowlist.php |
| WorkflowRuning.aspx | WorkflowRuning.php |
| ViewRuningHistory.aspx | ViewRuningHistory.php |
| api/getdata.aspx | api/getdata.php |
| api/line/Default.aspx | api/line/default.php |

## Notes

- This is a partial conversion of the original ASP.NET application
- Additional files will be converted as needed
- Some functionality may require adjustments for PHP environment
- The application structure has been reorganized to follow PHP best practices
- Authentication mechanism has been simplified compared to ASP.NET version 