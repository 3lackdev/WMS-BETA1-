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

// Initialize database and helper
$db = new Database();
$helper = new WorkflowHelper($db);

// Get workflow ID from request
$workflowId = isset($_GET['wfrowid']) ? $_GET['wfrowid'] : 0;

// Validate workflow ID
if (empty($workflowId) || !is_numeric($workflowId)) {
    die("Invalid workflow ID.");
}

// Get workflow information
$workflow = $helper->getWorkflowById($workflowId);
if (!$workflow) {
    die("Workflow not found.");
}

// Set content type to plaintext for export
header('Content-Type: text/plain');
header('Content-Disposition: attachment; filename="workflow_' . $workflowId . '_export.sql"');

// Generate export SQL
echo "-- WorkFlow Engine Export\n";
echo "-- Workflow: " . $workflow['workflow_name'] . "\n";
echo "-- Date: " . date('Y-m-d H:i:s') . "\n";
echo "-- ------------------------------------------------\n\n";

// Export workflow definition
echo "-- Workflow Definition\n";
echo "INSERT INTO WorkFlow (workflow_name, created_by, updated_by, user_edit, user_view, sql_mail_profile, application_folder, user_special_action, sender_display, default_domain, status_id) VALUES (\n";
echo "  '" . addslashes($workflow['workflow_name']) . "',\n";
echo "  @CREATED_BY_USER_ID,\n";
echo "  @UPDATED_BY_USER_ID,\n";
echo "  '" . addslashes($workflow['user_edit'] ?? '') . "',\n";
echo "  '" . addslashes($workflow['user_view'] ?? '') . "',\n";
echo "  '" . addslashes($workflow['sql_mail_profile'] ?? '') . "',\n";
echo "  '" . addslashes($workflow['application_folder'] ?? '') . "',\n";
echo "  '" . addslashes($workflow['user_special_action'] ?? '') . "',\n";
echo "  '" . addslashes($workflow['sender_display'] ?? '') . "',\n";
echo "  '" . addslashes($workflow['default_domain'] ?? '') . "',\n";
echo "  " . ($workflow['status_id'] ?? 1) . "\n";
echo ");\n";
echo "SET @WORKFLOW_ID = LAST_INSERT_ID();\n\n";

// Export templates
$templates = $db->executeQuery("SELECT * FROM WorkFlowTemplate WHERE workflow_id = ?", [$workflowId]);
if (is_array($templates) && !empty($templates)) {
    echo "-- Templates\n";
    
    foreach ($templates as $template) {
        echo "INSERT INTO WorkFlowTemplate (workflow_id, template_name, description) VALUES (\n";
        echo "  @WORKFLOW_ID,\n";
        echo "  '" . addslashes($template['template_name']) . "',\n";
        echo "  '" . addslashes($template['description'] ?? '') . "'\n";
        echo ");\n";
        echo "SET @TEMPLATE_ID_" . $template['template_id'] . " = LAST_INSERT_ID();\n\n";
        
        // Export template objects
        $objects = $db->executeQuery("SELECT * FROM TemplateObject WHERE template_id = ?", [$template['template_id']]);
        if (is_array($objects) && !empty($objects)) {
            echo "-- Template Objects for Template " . $template['template_name'] . "\n";
            
            foreach ($objects as $object) {
                echo "INSERT INTO TemplateObject (template_id, object_name, object_type, control_type, description, default_value, validation, object_order) VALUES (\n";
                echo "  @TEMPLATE_ID_" . $template['template_id'] . ",\n";
                echo "  '" . addslashes($object['object_name']) . "',\n";
                echo "  '" . addslashes($object['object_type'] ?? '') . "',\n";
                echo "  '" . addslashes($object['control_type'] ?? '') . "',\n";
                echo "  '" . addslashes($object['description'] ?? '') . "',\n";
                echo "  '" . addslashes($object['default_value'] ?? '') . "',\n";
                echo "  '" . addslashes($object['validation'] ?? '') . "',\n";
                echo "  " . ($object['object_order'] ?? 0) . "\n";
                echo ");\n";
                echo "SET @OBJECT_ID_" . $object['object_id'] . " = LAST_INSERT_ID();\n\n";
            }
        }
    }
}

// Export positions
$positions = $db->executeQuery("SELECT * FROM Position WHERE workflow_id = ?", [$workflowId]);
if (is_array($positions) && !empty($positions)) {
    echo "-- Positions\n";
    
    foreach ($positions as $position) {
        echo "INSERT INTO Position (workflow_id, position_name, position_email, description) VALUES (\n";
        echo "  @WORKFLOW_ID,\n";
        echo "  '" . addslashes($position['position_name']) . "',\n";
        echo "  '" . addslashes($position['position_email'] ?? '') . "',\n";
        echo "  '" . addslashes($position['description'] ?? '') . "'\n";
        echo ");\n";
        echo "SET @POSITION_ID_" . $position['position_id'] . " = LAST_INSERT_ID();\n\n";
    }
}

// Export workflow steps (first pass - create steps)
$steps = $db->executeQuery("SELECT * FROM WorkFlowStep WHERE workflow_id = ? ORDER BY step_order", [$workflowId]);
if (is_array($steps) && !empty($steps)) {
    echo "-- Workflow Steps (First Pass - Create Steps)\n";
    
    foreach ($steps as $step) {
        $templatePlaceholder = isset($step['template_id']) ? "@TEMPLATE_ID_" . $step['template_id'] : "NULL";
        $positionPlaceholder = isset($step['position_id']) ? "@POSITION_ID_" . $step['position_id'] : "NULL";
        
        echo "INSERT INTO WorkFlowStep (workflow_id, template_id, step_name, description, default_start, step_order, position_id) VALUES (\n";
        echo "  @WORKFLOW_ID,\n";
        echo "  " . $templatePlaceholder . ",\n";
        echo "  '" . addslashes($step['step_name']) . "',\n";
        echo "  '" . addslashes($step['description'] ?? '') . "',\n";
        echo "  '" . ($step['default_start'] ?? 'N') . "',\n";
        echo "  " . ($step['step_order'] ?? 0) . ",\n";
        echo "  " . $positionPlaceholder . "\n";
        echo ");\n";
        echo "SET @STEP_ID_" . $step['step_id'] . " = LAST_INSERT_ID();\n\n";
        
        // Export step enable objects
        if (isset($step['template_id'])) {
            $enableObjects = $db->executeQuery("SELECT * FROM StepEnableObject WHERE step_id = ? AND template_id = ?", [$step['step_id'], $step['template_id']]);
            if (is_array($enableObjects) && !empty($enableObjects)) {
                echo "-- Step Enable Objects for Step " . $step['step_name'] . "\n";
                
                foreach ($enableObjects as $enableObject) {
                    echo "INSERT INTO StepEnableObject (step_id, template_id, object_id, required_field, readonly_field) VALUES (\n";
                    echo "  @STEP_ID_" . $step['step_id'] . ",\n";
                    echo "  @TEMPLATE_ID_" . $step['template_id'] . ",\n";
                    echo "  @OBJECT_ID_" . $enableObject['object_id'] . ",\n";
                    echo "  '" . ($enableObject['required_field'] ?? 'N') . "',\n";
                    echo "  '" . ($enableObject['readonly_field'] ?? 'N') . "'\n";
                    echo ");\n\n";
                }
            }
        }
    }
    
    // Export workflow steps (second pass - update step references)
    echo "-- Workflow Steps (Second Pass - Update Step References)\n";
    foreach ($steps as $step) {
        if (!empty($step['success_step_id']) || !empty($step['fail_step_id'])) {
            $successStepPlaceholder = !empty($step['success_step_id']) ? "@STEP_ID_" . $step['success_step_id'] : "NULL";
            $failStepPlaceholder = !empty($step['fail_step_id']) ? "@STEP_ID_" . $step['fail_step_id'] : "NULL";
            
            echo "UPDATE WorkFlowStep SET\n";
            echo "  success_step_id = " . $successStepPlaceholder . ",\n";
            echo "  fail_step_id = " . $failStepPlaceholder . "\n";
            echo "WHERE step_id = @STEP_ID_" . $step['step_id'] . ";\n\n";
        }
    }
}

echo "-- End of Export\n";
?> 