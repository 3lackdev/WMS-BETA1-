<?php
require_once 'includes/Database.php';
require_once 'includes/Authentication.php';
require_once 'includes/WorkflowHelper.php';

// Set content type to JSON
header('Content-Type: application/json');

// Initialize authentication
$auth = new Authentication();

// Check if user is authenticated
if (!$auth->isAuthenticated()) {
    echo json_encode(['success' => false, 'message' => 'Not authenticated']);
    exit;
}

// Initialize database and helper
$db = new Database();
$helper = new WorkflowHelper($db);

// Get current user
$currentUser = $auth->getCurrentUser();

// Check if this is a POST request with action=save_workflow
if ($_SERVER['REQUEST_METHOD'] === 'POST' && isset($_POST['action']) && $_POST['action'] === 'save_workflow') {
    // Validate required fields
    if (empty($_POST['workflow_name'])) {
        echo json_encode(['success' => false, 'message' => 'Workflow name is required']);
        exit;
    }
    
    // Get current timestamp for logging
    $timestamp = date('Y-m-d H:i:s');
    error_log("[$timestamp] Creating workflow with name: " . $_POST['workflow_name']);
    
    // Prepare workflow data
    $workflowData = [
        'workflow_name' => $_POST['workflow_name'] ?? '',
        'created_by' => $_POST['created_by'] ?? $currentUser['user_id'] ?? 1,
        'user_edit' => $_POST['user_edit'] ?? $currentUser['username'] ?? '',
        'user_view' => $_POST['user_view'] ?? '',
        'sql_mail_profile' => $_POST['sql_mail_profile'] ?? '',
        'application_folder' => $_POST['application_folder'] ?? '',
        'user_special_action' => $_POST['user_special_action'] ?? $currentUser['username'] ?? '',
        'sender_display' => $_POST['sender_display'] ?? '',
        'default_domain' => $_POST['default_domain'] ?? 'bst.co.th'
    ];
    
    try {
        // Create new workflow
        $workflowId = $helper->createWorkflow($workflowData);
        
        if ($workflowId) {
            error_log("[$timestamp] Workflow created successfully with ID: $workflowId");
            
            // Return success response with the new workflow ID
            echo json_encode([
                'success' => true, 
                'message' => 'Workflow created successfully',
                'workflow_id' => $workflowId
            ]);
        } else {
            error_log("[$timestamp] Failed to create workflow");
            
            echo json_encode(['success' => false, 'message' => 'Failed to create workflow']);
        }
    } catch (Exception $e) {
        error_log("[$timestamp] Error creating workflow: " . $e->getMessage());
        
        echo json_encode(['success' => false, 'message' => 'Error: ' . $e->getMessage()]);
    }
} else {
    echo json_encode(['success' => false, 'message' => 'Invalid request']);
}
?> 