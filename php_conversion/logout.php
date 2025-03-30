<?php
/**
 * Logout script for WorkFlowEngine
 * Destroys the session and redirects to login page
 */

// Include authentication class
require_once 'includes/Authentication.php';

// Initialize authentication
$auth = new Authentication();

// Logout user
$auth->logout();

// Redirect to login page
header("Location: login.php");
exit;
?> 