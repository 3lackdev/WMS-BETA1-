<?php
/**
 * Process Step Events
 * 
 * This file handles the form submissions from the step-events.php page.
 * It processes actions for creating, updating, and deleting workflow step events.
 */

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

// Get current user
$currentUser = $auth->getCurrentUser();
$currentUsername = $currentUser['username'] ?? '';

// Get the action from POST
$action = $_POST['action'] ?? '';
$workflowId = $_POST['WorkflowId'] ?? '';
$stepId = $_POST['WFRowId'] ?? '';
$saveType = $_POST['save_type'] ?? 'all';

// Handle different actions
switch ($action) {
    case 'create':
        createNewStep($db, $workflowId);
        break;
    case 'update':
        updateStepEvents($db, $stepId, $saveType);
        break;
    case 'delete':
        deleteStep($db, $stepId, $workflowId);
        break;
    case 'copy':
        copyStep($db, $stepId, $workflowId);
        break;
    default:
        // Redirect back to the main page if no valid action
        header("Location: step-events.php?WFRowId=$workflowId");
        exit;
}

/**
 * Create a new workflow step
 */
function createNewStep($db, $workflowId) {
    $eventName = $_POST['txt_eventname'] ?? '';
    $seq = $_POST['txt_seq'] ?? 0;
    $group = $_POST['txt_group'] ?? '';
    
    // Check to ensure workflowId and eventName are provided
    if (empty($eventName) || empty($workflowId)) {
        // Check if this is an AJAX request
        if (!empty($_SERVER['HTTP_X_REQUESTED_WITH']) && strtolower($_SERVER['HTTP_X_REQUESTED_WITH']) == 'xmlhttprequest') {
            echo json_encode(['success' => false, 'message' => 'Incomplete data. Please provide step name and Workflow ID']);
            exit;
        } else {
            header("Location: step-events.php?WFRowId=$workflowId&error=missing_data");
            exit;
        }
    }
    
    try {
        // Generate a new UUID for the step
        $uuid = generateUUID();
        
        $sql = "INSERT INTO WorkFlowStep (
                StepRowId, 
                WFRowId, 
                Seq, 
                StepName, 
                GroupName,
                CrDate
            ) VALUES (
                '$uuid',
                '" . $db->escapeString($workflowId) . "',
                '" . $db->escapeString($seq) . "',
                '" . $db->escapeString($eventName) . "',
                '" . $db->escapeString($group) . "',
                NOW()
            )";
        
        if ($db->executeNonQuery($sql)) {
            // Check if this is an AJAX request
            if (!empty($_SERVER['HTTP_X_REQUESTED_WITH']) && strtolower($_SERVER['HTTP_X_REQUESTED_WITH']) == 'xmlhttprequest') {
                echo json_encode(['success' => true, 'message' => 'Data saved successfully', 'stepId' => $uuid]);
                exit;
            } else {
                header("Location: step-events.php?WFRowId=$workflowId&success=created");
            }
        } else {
            if (!empty($_SERVER['HTTP_X_REQUESTED_WITH']) && strtolower($_SERVER['HTTP_X_REQUESTED_WITH']) == 'xmlhttprequest') {
                echo json_encode(['success' => false, 'message' => 'Failed to save data']);
                exit;
            } else {
                header("Location: step-events.php?WFRowId=$workflowId&error=create_failed");
            }
        }
    } catch (Exception $e) {
        if (!empty($_SERVER['HTTP_X_REQUESTED_WITH']) && strtolower($_SERVER['HTTP_X_REQUESTED_WITH']) == 'xmlhttprequest') {
            echo json_encode(['success' => false, 'message' => $e->getMessage()]);
            exit;
        } else {
            header("Location: step-events.php?WFRowId=$workflowId&error=" . urlencode($e->getMessage()));
        }
    }
    
    exit;
}

/**
 * Update workflow step events
 */
function updateStepEvents($db, $stepId, $saveType) {
    if (empty($stepId)) {
        header("Location: step-events.php?error=missing_step_id");
        exit;
    }
    
    try {
        // Get workflow ID for redirection
        $workflowId = getWorkflowIdFromStep($db, $stepId);
        
        // Use the WorkflowHelper for setup operations
        global $helper;
        
        if ($saveType == 'basic') {
            // Only update basic fields for simple edit
            $updateData = [];
            
            if (isset($_POST['StepName'])) {
                $updateData['StepName'] = $_POST['StepName'];
            }
            
            if (isset($_POST['Seq'])) {
                $updateData['Seq'] = $_POST['Seq'];
            }
            
            if (isset($_POST['GroupName'])) {
                $updateData['GroupName'] = $_POST['GroupName'];
            }
            
            $updateData['DefaultStart'] = isset($_POST['cb_defaultstart']) ? 1 : 0;
            
            // Build update SQL
            $sql = "UPDATE WorkFlowStep SET ";
            $updates = [];
            
            foreach ($updateData as $field => $value) {
                $updates[] = "$field = '" . $db->escapeString($value) . "'";
            }
            
            $sql .= implode(", ", $updates);
            $sql .= " WHERE StepRowId = '" . $db->escapeString($stepId) . "'";
            
            $success = $db->executeNonQuery($sql);
        } elseif ($saveType == 'events_only') {
            // Use the helper to set up the step
            $success = $helper->setupStep($stepId, $_POST);
        } else {
            // Update all fields for complete edit (backwards compatibility)
            $updateData = [
                'EnableSave' => isset($_POST['cb_enablesave']) ? 1 : 0,
                'EnableApprove' => isset($_POST['cb_enableapprove']) ? 1 : 0,
                'EnableReject' => isset($_POST['cb_enablereject']) ? 1 : 0,
                'DefaultStart' => isset($_POST['cb_defaultstart']) ? 1 : 0,
                'DinamicKeyValue' => $_POST['DinamicKey'] ?? '',
                'AliasNameSave' => $_POST['AliasNameSave'] ?? '',
                'AliasNameApprove' => $_POST['AliasNameApprove'] ?? '',
                'AliasNameReject' => $_POST['AliasNameReject'] ?? '',
                'TriggerSave' => $_POST['TriggerSave'] ?? '',
                'TriggerApprove' => $_POST['TriggerApprove'] ?? '',
                'TriggerReject' => $_POST['TriggerReject'] ?? '',
                'HideButtonSave' => isset($_POST['HideButtonSave']) ? 1 : 0,
                'HideButtonApprove' => isset($_POST['HideButtonApprove']) ? 1 : 0,
                'HideButtonReject' => isset($_POST['HideButtonReject']) ? 1 : 0
            ];
            
            // Add fields from edit form if they exist
            if (isset($_POST['StepName'])) {
                $updateData['StepName'] = $_POST['StepName'];
            }
            
            if (isset($_POST['Seq'])) {
                $updateData['Seq'] = $_POST['Seq'];
            }
            
            if (isset($_POST['GroupName'])) {
                $updateData['GroupName'] = $_POST['GroupName'];
            }
            
            // Build update SQL
            $sql = "UPDATE WorkFlowStep SET ";
            $updates = [];
            
            foreach ($updateData as $field => $value) {
                $updates[] = "$field = '" . $db->escapeString($value) . "'";
            }
            
            $sql .= implode(", ", $updates);
            $sql .= " WHERE StepRowId = '" . $db->escapeString($stepId) . "'";
            
            $success = $db->executeNonQuery($sql);
        }
        
        // Handle the response based on whether the update was successful
        if ($success) {
            // Detect if this is an AJAX request
            if (!empty($_SERVER['HTTP_X_REQUESTED_WITH']) && strtolower($_SERVER['HTTP_X_REQUESTED_WITH']) == 'xmlhttprequest') {
                // Return JSON response for AJAX requests
                echo json_encode(['success' => true, 'message' => 'Data saved successfully']);
                exit;
            } else {
                // Standard redirect for non-AJAX requests
                header("Location: step-events.php?WFRowId=$workflowId&WFRowId=$stepId&action=edit&success=1");
            }
        } else {
            if (!empty($_SERVER['HTTP_X_REQUESTED_WITH']) && strtolower($_SERVER['HTTP_X_REQUESTED_WITH']) == 'xmlhttprequest') {
                echo json_encode(['success' => false, 'message' => 'Save failed']);
                exit;
            } else {
                header("Location: step-events.php?WFRowId=$workflowId&WFRowId=$stepId&action=edit&error=update_failed");
            }
        }
    } catch (Exception $e) {
        error_log("Error updating step events: " . $e->getMessage());
        
        if (!empty($_SERVER['HTTP_X_REQUESTED_WITH']) && strtolower($_SERVER['HTTP_X_REQUESTED_WITH']) == 'xmlhttprequest') {
            echo json_encode(['success' => false, 'message' => $e->getMessage()]);
            exit;
        } else {
            header("Location: step-events.php?WFRowId=$workflowId&WFRowId=$stepId&action=edit&error=" . urlencode($e->getMessage()));
        }
    }
    
    exit;
}

/**
 * Delete a workflow step
 */
function deleteStep($db, $stepId, $workflowId) {
    if (empty($stepId) || empty($workflowId)) {
        header("Location: step-events.php?WFRowId=$workflowId&error=missing_data");
        exit;
    }
    
    try {
        // First check if this is a valid step for this workflow
        $sql = "SELECT COUNT(*) as count FROM WorkFlowStep 
                WHERE StepRowId = '" . $db->escapeString($stepId) . "' 
                AND WFRowId = '" . $db->escapeString($workflowId) . "'";
        
        $result = $db->executeQuery($sql);
        
        if ($result[0]['count'] == 0) {
            header("Location: step-events.php?WFRowId=$workflowId&error=invalid_step");
            exit;
        }
        
        // Delete the step
        $sql = "DELETE FROM WorkFlowStep WHERE StepRowId = '" . $db->escapeString($stepId) . "'";
        
        if ($db->executeNonQuery($sql)) {
            header("Location: step-events.php?WFRowId=$workflowId&success=deleted");
        } else {
            header("Location: step-events.php?WFRowId=$workflowId&error=delete_failed");
        }
    } catch (Exception $e) {
        header("Location: step-events.php?WFRowId=$workflowId&error=" . urlencode($e->getMessage()));
    }
    
    exit;
}

/**
 * Copy a workflow step
 */
function copyStep($db, $stepId, $workflowId) {
    if (empty($stepId) || empty($workflowId)) {
        header("Location: step-events.php?WFRowId=$workflowId&error=missing_data");
        exit;
    }
    
    try {
        // Get the step details
        $sql = "SELECT * FROM WorkFlowStep WHERE StepRowId = '" . $db->escapeString($stepId) . "'";
        $result = $db->executeQuery($sql);
        
        if (count($result) == 0) {
            header("Location: step-events.php?WFRowId=$workflowId&error=step_not_found");
            exit;
        }
        
        $step = $result[0];
        
        // Generate a new UUID for the step
        $newUuid = generateUUID();
        
        // Insert the new step
        $sql = "INSERT INTO WorkFlowStep (
                StepRowId, 
                WFRowId, 
                Seq, 
                StepName, 
                GroupName,
                DefaultStart,
                DinamicKeyValue,
                EnableSave,
                EnableApprove,
                EnableReject,
                TriggerSave,
                TriggerApprove,
                TriggerReject,
                EventBeforeSaveCallJavascriptFunction,
                EventBeforeApproveCallJavascriptFunction,
                EventBeforeRejectCallJavascriptFunction,
                EventAfterSaveGotoStep,
                EventAfterApproveGotoStep,
                EventAfterRejectGotoStep,
                CrDate
            ) VALUES (
                '$newUuid',
                '" . $db->escapeString($workflowId) . "',
                '" . $db->escapeString($step['Seq']) . "',
                '" . $db->escapeString($step['StepName']) . " (Copy)',
                '" . $db->escapeString($step['GroupName']) . "',
                '" . $db->escapeString($step['DefaultStart']) . "',
                '" . $db->escapeString($step['DinamicKeyValue']) . "',
                '" . $db->escapeString($step['EnableSave']) . "',
                '" . $db->escapeString($step['EnableApprove']) . "',
                '" . $db->escapeString($step['EnableReject']) . "',
                '" . $db->escapeString($step['TriggerSave']) . "',
                '" . $db->escapeString($step['TriggerApprove']) . "',
                '" . $db->escapeString($step['TriggerReject']) . "',
                '" . $db->escapeString($step['EventBeforeSaveCallJavascriptFunction']) . "',
                '" . $db->escapeString($step['EventBeforeApproveCallJavascriptFunction']) . "',
                '" . $db->escapeString($step['EventBeforeRejectCallJavascriptFunction']) . "',
                '" . $db->escapeString($step['EventAfterSaveGotoStep']) . "',
                '" . $db->escapeString($step['EventAfterApproveGotoStep']) . "',
                '" . $db->escapeString($step['EventAfterRejectGotoStep']) . "',
                NOW()
            )";
        
        if ($db->executeNonQuery($sql)) {
            header("Location: step-events.php?WFRowId=$workflowId&success=copied");
        } else {
            header("Location: step-events.php?WFRowId=$workflowId&error=copy_failed");
        }
    } catch (Exception $e) {
        header("Location: step-events.php?WFRowId=$workflowId&error=" . urlencode($e->getMessage()));
    }
    
    exit;
}

/**
 * Get the workflow ID for a step
 */
function getWorkflowIdFromStep($db, $stepId) {
    try {
        $sql = "SELECT WFRowId FROM WorkFlowStep WHERE StepRowId = '" . $db->escapeString($stepId) . "'";
        $result = $db->executeQuery($sql);
        
        if (count($result) > 0) {
            return $result[0]['WFRowId'];
        }
    } catch (Exception $e) {
        // Log error
    }
    
    return '';
}

/**
 * Generate a UUID v4
 */
function generateUUID() {
    // Generate 16 random bytes
    $data = random_bytes(16);
    
    // Set version to 0100
    $data[6] = chr(ord($data[6]) & 0x0f | 0x40);
    // Set bits 6-7 to 10
    $data[8] = chr(ord($data[8]) & 0x3f | 0x80);
    
    // Output the 36 character UUID
    return vsprintf('%s%s-%s-%s-%s-%s%s%s', str_split(bin2hex($data), 4));
} 