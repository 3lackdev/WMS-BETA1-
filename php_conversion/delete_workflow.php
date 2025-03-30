<?php
require_once 'includes/Database.php';
require_once 'includes/Authentication.php';
require_once 'includes/WorkflowHelper.php';

// Initialize authentication
$auth = new Authentication();

// Check if user is authenticated
if (!$auth->isAuthenticated()) {
    // Redirect to login page if not authenticated
    header("Location: login.php");
    exit;
}

// Initialize database
$db = new Database();

// Get workflow ID from request
$workflowId = isset($_GET['id']) ? $_GET['id'] : 0;

// Validate workflow ID
if (empty($workflowId) || !is_numeric($workflowId)) {
    header("Location: workflowlist.php");
    exit;
}

// Get current user
$currentUser = $auth->getCurrentUser();
$currentUsername = $currentUser['username'] ?? '';

// Check if user has permission to delete
try {
    $sql = "SELECT user_edit, created_by FROM WorkFlow WHERE workflow_id = '" . $db->escapeString($workflowId) . "'";
    $result = $db->executeQuery($sql);
    
    if (empty($result)) {
        // Workflow not found
        header("Location: workflowlist.php");
        exit;
    }
    
    $userEdit = $result[0]['user_edit'] ?? '';
    $createdBy = $result[0]['created_by'] ?? '';
    
    // Check if user has permission (either admin, listed in user_edit, or is the creator)
    $hasPermission = $auth->hasRole('admin') || 
                     (strpos($userEdit, $currentUsername) !== false) || 
                     ($createdBy == $currentUser['user_id']);
    
    if (!$hasPermission) {
        // User doesn't have permission
        header("Location: workflowlist.php");
        exit;
    }
    
    // Delete workflow and related records
    // Use transaction to ensure data integrity
    $db->beginTransaction();
    
    try {
        // Delete related records first
        $db->executeNonQuery("DELETE FROM TemplateObject WHERE template_id IN (SELECT template_id FROM WorkFlowTemplate WHERE workflow_id = '" . $db->escapeString($workflowId) . "')");
        $db->executeNonQuery("DELETE FROM StepEnableObject WHERE step_id IN (SELECT step_id FROM WorkFlowStep WHERE workflow_id = '" . $db->escapeString($workflowId) . "')");
        $db->executeNonQuery("DELETE FROM WorkFlowTemplate WHERE workflow_id = '" . $db->escapeString($workflowId) . "'");
        $db->executeNonQuery("DELETE FROM WorkFlowStep WHERE workflow_id = '" . $db->escapeString($workflowId) . "'");
        $db->executeNonQuery("DELETE FROM Position WHERE workflow_id = '" . $db->escapeString($workflowId) . "'");
        
        // Finally delete the workflow itself
        $db->executeNonQuery("DELETE FROM WorkFlow WHERE workflow_id = '" . $db->escapeString($workflowId) . "'");
        
        $db->commitTransaction();
        
        // Redirect to workflow list with success message
        header("Location: workflowlist.php?deleted=1");
        exit;
    } catch (Exception $e) {
        // Rollback on error
        $db->rollbackTransaction();
        
        // Try with lowercase table names if there was an error
        try {
            $db->beginTransaction();
            
            // Delete related records first
            $db->executeNonQuery("DELETE FROM templateobject WHERE template_id IN (SELECT template_id FROM workflowtemplate WHERE workflow_id = '" . $db->escapeString($workflowId) . "')");
            $db->executeNonQuery("DELETE FROM stepenableobject WHERE step_id IN (SELECT step_id FROM workflowstep WHERE workflow_id = '" . $db->escapeString($workflowId) . "')");
            $db->executeNonQuery("DELETE FROM workflowtemplate WHERE workflow_id = '" . $db->escapeString($workflowId) . "'");
            $db->executeNonQuery("DELETE FROM workflowstep WHERE workflow_id = '" . $db->escapeString($workflowId) . "'");
            $db->executeNonQuery("DELETE FROM position WHERE workflow_id = '" . $db->escapeString($workflowId) . "'");
            
            // Finally delete the workflow itself
            $db->executeNonQuery("DELETE FROM workflow WHERE workflow_id = '" . $db->escapeString($workflowId) . "'");
            
            $db->commitTransaction();
            
            // Redirect to workflow list with success message
            header("Location: workflowlist.php?deleted=1");
            exit;
        } catch (Exception $e2) {
            // Rollback on error
            $db->rollbackTransaction();
            
            // Redirect with error message
            header("Location: workflowlist.php?error=delete");
            exit;
        }
    }
} catch (Exception $e) {
    // Error checking permission, redirect to workflow list
    header("Location: workflowlist.php");
    exit;
}
?> 