<?php
/**
 * WorkflowHelper Class for WorkFlowEngine
 * Handles workflow operations and data retrieval
 */

class WorkflowHelper {
    private $db;
    
    /**
     * Constructor - initializes database connection
     * 
     * @param Database $db Database connection
     */
    public function __construct($db) {
        $this->db = $db;
    }
    
    /**
     * Get workflows for a specific user
     * 
     * @param int $userId User ID
     * @return array List of workflows
     */
    public function getWorkflowsForUser($userId) {
        try {
            $sql = "SELECT w.workflow_id, w.workflow_name, w.created_date, s.status_name as status 
                    FROM WorkFlow w 
                    JOIN WorkFlowStatus s ON w.status_id = s.status_id 
                    WHERE w.created_by = '" . $this->db->escapeString($userId) . "' 
                    OR w.workflow_id IN (
                        SELECT workflow_id FROM WorkFlowStep 
                        WHERE assigned_to = '" . $this->db->escapeString($userId) . "'
                    )
                    ORDER BY w.created_date DESC";
            
            return $this->db->executeQuery($sql);
        } catch (Exception $e) {
            // Try alternative table case if the first query fails
            try {
                $sql = "SELECT w.workflow_id, w.workflow_name, w.created_date, s.status_name as status 
                        FROM workflow w 
                        JOIN workflowstatus s ON w.status_id = s.status_id 
                        WHERE w.created_by = '" . $this->db->escapeString($userId) . "' 
                        OR w.workflow_id IN (
                            SELECT workflow_id FROM workflowstep 
                            WHERE assigned_to = '" . $this->db->escapeString($userId) . "'
                        )
                        ORDER BY w.created_date DESC";
                
                return $this->db->executeQuery($sql);
            } catch (Exception $e2) {
                error_log("Error getting workflows for user: " . $e->getMessage() . " / " . $e2->getMessage());
                return [];
            }
        }
    }
    
    /**
     * Get all workflows
     * 
     * @param string $filter Optional filter condition
     * @param string $orderBy Optional order by clause
     * @return array List of all workflows
     */
    public function getAllWorkflows($filter = '', $orderBy = 'CrDate DESC') {
        try {
            // Modify the query to match the actual database structure
            $sql = "SELECT 
                    w.WFRowId as workflow_id, 
                    w.WFName as workflow_name, 
                    w.CrDate as created_date, 
                    w.CrBy as created_by_id,
                    w.CrBy as created_by,  
                    w.UserEdit as user_edit,
                    w.UserView as user_view
                    FROM WorkFlow w";
            
            if (!empty($filter)) {
                $sql .= " WHERE " . $filter;
            }
            
            if (!empty($orderBy)) {
                $sql .= " ORDER BY " . $orderBy;
            }
            
            $workflows = $this->db->executeQuery($sql);
            
            // Post-process results to add status information if needed
            foreach ($workflows as &$workflow) {
                // Set a default status
                $workflow['status'] = 'Draft';
            }
            
            return $workflows;
        } catch (Exception $e) {
            error_log("Error getting all workflows: " . $e->getMessage());
            return [];
        }
    }
    
    /**
     * Get workflow details by ID
     * 
     * @param int $workflowId Workflow ID
     * @return array|null Workflow details or null if not found
     */
    public function getWorkflowById($workflowId) {
        try {
            $sql = "SELECT w.*, u.username as created_by_name, s.status_name 
                    FROM WorkFlow w 
                    JOIN users u ON w.created_by = u.user_id 
                    JOIN WorkFlowStatus s ON w.status_id = s.status_id 
                    WHERE w.workflow_id = '" . $this->db->escapeString($workflowId) . "'";
            
            $result = $this->db->executeQuery($sql);
            
            if (count($result) > 0) {
                return $result[0];
            }
            
            return null;
        } catch (Exception $e) {
            // Try alternative table case if the first query fails
            try {
                $sql = "SELECT w.*, u.username as created_by_name, s.status_name 
                        FROM workflow w 
                        JOIN users u ON w.created_by = u.user_id 
                        JOIN workflowstatus s ON w.status_id = s.status_id 
                        WHERE w.workflow_id = '" . $this->db->escapeString($workflowId) . "'";
                
                $result = $this->db->executeQuery($sql);
                
                if (count($result) > 0) {
                    return $result[0];
                }
            } catch (Exception $e2) {
                error_log("Error getting workflow by ID: " . $e->getMessage() . " / " . $e2->getMessage());
            }
            
            return null;
        }
    }
    
    /**
     * Get workflow steps for a workflow
     * 
     * @param int $workflowId Workflow ID
     * @return array List of workflow steps
     */
    public function getWorkflowSteps($workflowId) {
        try {
            $sql = "SELECT s.*, u.username as assigned_to_name 
                    FROM WorkFlowStep s 
                    LEFT JOIN users u ON s.assigned_to = u.user_id 
                    WHERE s.workflow_id = '" . $this->db->escapeString($workflowId) . "' 
                    ORDER BY s.step_order";
            
            return $this->db->executeQuery($sql);
        } catch (Exception $e) {
            // Try alternative table case if the first query fails
            try {
                $sql = "SELECT s.*, u.username as assigned_to_name 
                        FROM workflowstep s 
                        LEFT JOIN users u ON s.assigned_to = u.user_id 
                        WHERE s.workflow_id = '" . $this->db->escapeString($workflowId) . "' 
                        ORDER BY s.step_order";
                
                return $this->db->executeQuery($sql);
            } catch (Exception $e2) {
                error_log("Error getting workflow steps: " . $e->getMessage() . " / " . $e2->getMessage());
                return [];
            }
        }
    }
    
    /**
     * Create a new workflow
     * 
     * @param array $data Workflow data
     * @return int|bool New workflow ID if successful, false otherwise
     */
    public function createWorkflow($data) {
        try {
            $workflowName = $this->db->escapeString($data['workflow_name']);
            $createdBy = $this->db->escapeString($data['created_by']);
            $userView = $this->db->escapeString($data['user_view'] ?? '');
            $userEdit = $this->db->escapeString($data['user_edit'] ?? '');
            $userSpecialAction = $this->db->escapeString($data['user_special_action'] ?? '');
            $sqlMailProfile = $this->db->escapeString($data['sql_mail_profile'] ?? '');
            $applicationFolder = $this->db->escapeString($data['application_folder'] ?? '');
            $senderDisplay = $this->db->escapeString($data['sender_display'] ?? '');
            $defaultDomain = $this->db->escapeString($data['default_domain'] ?? 'bst.co.th');

            // Generate a UUID for WFRowId
            $uuid = $this->generateUUID();
            
            $sql = "INSERT INTO WorkFlow (
                    WFRowId, 
                    WFName, 
                    UserView, 
                    UserEdit, 
                    UserSpecialAction, 
                    SQLMailProfile, 
                    ApplicationFolder, 
                    SenderDisplay, 
                    DefaultDomain, 
                    CrDate, 
                    CrBy
                ) VALUES (
                    '$uuid', 
                    '$workflowName', 
                    '$userView', 
                    '$userEdit', 
                    '$userSpecialAction', 
                    '$sqlMailProfile', 
                    '$applicationFolder', 
                    '$senderDisplay', 
                    '$defaultDomain', 
                    NOW(), 
                    '$createdBy'
                )";
            
            if ($this->db->executeNonQuery($sql)) {
                // Return the UUID as the workflow ID
                return $uuid;
            }
            
            return false;
        } catch (Exception $e) {
            error_log("Error creating workflow: " . $e->getMessage());
            return false;
        }
    }
    
    /**
     * Generate a UUID v4
     * 
     * @return string UUID
     */
    private function generateUUID() {
        // Generate 16 random bytes
        $data = random_bytes(16);
        
        // Set version to 0100
        $data[6] = chr(ord($data[6]) & 0x0f | 0x40);
        // Set bits 6-7 to 10
        $data[8] = chr(ord($data[8]) & 0x3f | 0x80);
        
        // Output the 36 character UUID
        return vsprintf('%s%s-%s-%s-%s-%s%s%s', str_split(bin2hex($data), 4));
    }
    
    /**
     * Update workflow status
     * 
     * @param int $workflowId Workflow ID
     * @param int $statusId New status ID
     * @return bool True if successful, false otherwise
     */
    public function updateWorkflowStatus($workflowId, $statusId) {
        try {
            $sql = "UPDATE WorkFlow SET status_id = '" . $this->db->escapeString($statusId) . "' 
                    WHERE workflow_id = '" . $this->db->escapeString($workflowId) . "'";
            
            return $this->db->executeNonQuery($sql);
        } catch (Exception $e) {
            // Try alternative table case if the first query fails
            try {
                $sql = "UPDATE workflow SET status_id = '" . $this->db->escapeString($statusId) . "' 
                        WHERE workflow_id = '" . $this->db->escapeString($workflowId) . "'";
                
                return $this->db->executeNonQuery($sql);
            } catch (Exception $e2) {
                error_log("Error updating workflow status: " . $e->getMessage() . " / " . $e2->getMessage());
                return false;
            }
        }
    }

    /**
     * Copy a workflow and all its related components
     * 
     * @param int $workflowId The ID of the workflow to copy
     * @param int $userId The ID of the user who is copying the workflow
     * @return int The ID of the newly created workflow copy
     * @throws Exception if an error occurs during the copying process
     */
    public function copyWorkflow($workflowId, $userId) {
        try {
            // Start transaction
            $this->db->beginTransaction();
            
            // Get the original workflow
            $workflow = $this->getWorkflowById($workflowId);
            if (!$workflow) {
                throw new Exception("Workflow not found");
            }
            
            // Create a copy of the workflow with a new name
            $originalName = $workflow['workflow_name'];
            $newName = $originalName . " (Copy)";
            $username = $this->getUsernameById($userId);
            
            // Insert the new workflow
            $query = "INSERT INTO WorkFlow 
                      (workflow_name, created_by, updated_by, user_edit, user_view, sql_mail_profile, 
                      application_folder, user_special_action, sender_display, default_domain, status_id) 
                      VALUES 
                      (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)";
            
            $params = [
                $newName,
                $userId,
                $userId,
                $username, // User edit permissions
                $workflow['user_view'] ?? '', 
                $workflow['sql_mail_profile'] ?? '',
                $workflow['application_folder'] ?? '',
                $workflow['user_special_action'] ?? $username,
                $workflow['sender_display'] ?? '',
                $workflow['default_domain'] ?? '',
                $workflow['status_id'] ?? 1
            ];
            
            $newWorkflowId = $this->db->executeInsert($query, $params);
            
            // Copy templates
            $this->copyWorkflowTemplates($workflowId, $newWorkflowId);
            
            // Copy workflow steps
            $this->copyWorkflowSteps($workflowId, $newWorkflowId);
            
            // Copy positions
            $this->copyWorkflowPositions($workflowId, $newWorkflowId);
            
            // Commit transaction
            $this->db->commitTransaction();
            
            return $newWorkflowId;
        } catch (Exception $e) {
            // Rollback transaction on error
            $this->db->rollbackTransaction();
            throw $e;
        }
    }

    /**
     * Get username by user ID
     * 
     * @param int $userId User ID
     * @return string Username
     */
    private function getUsernameById($userId) {
        $query = "SELECT username FROM users WHERE user_id = ?";
        $result = $this->db->executeQuery($query, [$userId]);
        
        if (is_array($result) && !empty($result) && isset($result[0]['username'])) {
            return $result[0]['username'];
        }
        
        return '';
    }

    /**
     * Copy workflow templates from original to new workflow
     * 
     * @param int $sourceWorkflowId Original workflow ID
     * @param int $targetWorkflowId New workflow ID
     */
    private function copyWorkflowTemplates($sourceWorkflowId, $targetWorkflowId) {
        // Get templates from original workflow
        $query = "SELECT * FROM WorkFlowTemplate WHERE workflow_id = ?";
        $templates = $this->db->executeQuery($query, [$sourceWorkflowId]);
        
        if (!is_array($templates) || empty($templates)) {
            return;
        }
        
        // Template ID mapping (old ID => new ID) for later reference
        $templateIdMap = [];
        
        foreach ($templates as $template) {
            // Insert new template
            $insertQuery = "INSERT INTO WorkFlowTemplate 
                             (workflow_id, template_name, description) 
                             VALUES (?, ?, ?)";
            $params = [
                $targetWorkflowId,
                $template['template_name'],
                $template['description'] ?? ''
            ];
            
            $newTemplateId = $this->db->executeInsert($insertQuery, $params);
            $templateIdMap[$template['template_id']] = $newTemplateId;
            
            // Copy template objects
            $this->copyTemplateObjects($template['template_id'], $newTemplateId);
        }
        
        return $templateIdMap;
    }

    /**
     * Copy template objects from original to new template
     * 
     * @param int $sourceTemplateId Original template ID
     * @param int $targetTemplateId New template ID
     */
    private function copyTemplateObjects($sourceTemplateId, $targetTemplateId) {
        // Get objects from original template
        $query = "SELECT * FROM TemplateObject WHERE template_id = ?";
        $objects = $this->db->executeQuery($query, [$sourceTemplateId]);
        
        if (!is_array($objects) || empty($objects)) {
            return;
        }
        
        foreach ($objects as $object) {
            // Insert new object
            $insertQuery = "INSERT INTO TemplateObject 
                             (template_id, object_name, object_type, control_type, description, 
                             default_value, validation, object_order) 
                             VALUES (?, ?, ?, ?, ?, ?, ?, ?)";
            $params = [
                $targetTemplateId,
                $object['object_name'],
                $object['object_type'] ?? '',
                $object['control_type'] ?? '',
                $object['description'] ?? '',
                $object['default_value'] ?? '',
                $object['validation'] ?? '',
                $object['object_order'] ?? 0
            ];
            
            $this->db->executeInsert($insertQuery, $params);
        }
    }

    /**
     * Copy workflow steps from original to new workflow
     * 
     * @param int $sourceWorkflowId Original workflow ID
     * @param int $targetWorkflowId New workflow ID
     */
    private function copyWorkflowSteps($sourceWorkflowId, $targetWorkflowId) {
        // Get steps from original workflow
        $query = "SELECT * FROM WorkFlowStep WHERE workflow_id = ?";
        $steps = $this->db->executeQuery($query, [$sourceWorkflowId]);
        
        if (!is_array($steps) || empty($steps)) {
            return;
        }
        
        // Step ID mapping (old ID => new ID) for later reference
        $stepIdMap = [];
        
        foreach ($steps as $step) {
            // Insert new step
            $insertQuery = "INSERT INTO WorkFlowStep 
                             (workflow_id, template_id, step_name, description, default_start, 
                             step_order, success_step_id, fail_step_id, position_id) 
                             VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?)";
            $params = [
                $targetWorkflowId,
                $step['template_id'],
                $step['step_name'],
                $step['description'] ?? '',
                $step['default_start'] ?? 'N',
                $step['step_order'] ?? 0,
                $step['success_step_id'] ?? null,
                $step['fail_step_id'] ?? null,
                $step['position_id'] ?? null
            ];
            
            $newStepId = $this->db->executeInsert($insertQuery, $params);
            $stepIdMap[$step['step_id']] = $newStepId;
            
            // Copy step enable objects
            $this->copyStepEnableObjects($step['step_id'], $newStepId, $step['template_id']);
        }
        
        // Update step references (success_step_id, fail_step_id)
        foreach ($stepIdMap as $oldStepId => $newStepId) {
            $step = $this->getStepById($oldStepId);
            
            $successStepId = null;
            $failStepId = null;
            
            if (!empty($step['success_step_id']) && isset($stepIdMap[$step['success_step_id']])) {
                $successStepId = $stepIdMap[$step['success_step_id']];
            }
            
            if (!empty($step['fail_step_id']) && isset($stepIdMap[$step['fail_step_id']])) {
                $failStepId = $stepIdMap[$step['fail_step_id']];
            }
            
            $updateQuery = "UPDATE WorkFlowStep SET success_step_id = ?, fail_step_id = ? WHERE step_id = ?";
            $this->db->executeUpdate($updateQuery, [$successStepId, $failStepId, $newStepId]);
        }
    }

    /**
     * Get a step by ID
     * 
     * @param int $stepId Step ID
     * @return array|null Step data or null if not found
     */
    private function getStepById($stepId) {
        $query = "SELECT * FROM WorkFlowStep WHERE step_id = ?";
        $result = $this->db->executeQuery($query, [$stepId]);
        
        if (is_array($result) && !empty($result)) {
            return $result[0];
        }
        
        return null;
    }

    /**
     * Copy step enable objects from original to new step
     * 
     * @param int $sourceStepId Original step ID
     * @param int $targetStepId New step ID
     * @param int $templateId Template ID
     */
    private function copyStepEnableObjects($sourceStepId, $targetStepId, $templateId) {
        // Get enable objects from original step
        $query = "SELECT * FROM StepEnableObject WHERE step_id = ?";
        $enableObjects = $this->db->executeQuery($query, [$sourceStepId]);
        
        if (!is_array($enableObjects) || empty($enableObjects)) {
            return;
        }
        
        foreach ($enableObjects as $object) {
            // Insert new enable object
            $insertQuery = "INSERT INTO StepEnableObject 
                             (step_id, template_id, object_id, required_field, readonly_field) 
                             VALUES (?, ?, ?, ?, ?)";
            $params = [
                $targetStepId,
                $templateId,
                $object['object_id'],
                $object['required_field'] ?? 'N',
                $object['readonly_field'] ?? 'N'
            ];
            
            $this->db->executeInsert($insertQuery, $params);
        }
    }

    /**
     * Copy workflow positions from original to new workflow
     * 
     * @param int $sourceWorkflowId Original workflow ID
     * @param int $targetWorkflowId New workflow ID
     */
    private function copyWorkflowPositions($sourceWorkflowId, $targetWorkflowId) {
        // Get positions from original workflow
        $query = "SELECT * FROM Position WHERE workflow_id = ?";
        $positions = $this->db->executeQuery($query, [$sourceWorkflowId]);
        
        if (!is_array($positions) || empty($positions)) {
            return;
        }
        
        foreach ($positions as $position) {
            // Insert new position
            $insertQuery = "INSERT INTO Position 
                             (workflow_id, position_name, position_email, description) 
                             VALUES (?, ?, ?, ?)";
            $params = [
                $targetWorkflowId,
                $position['position_name'],
                $position['position_email'] ?? '',
                $position['description'] ?? ''
            ];
            
            $this->db->executeInsert($insertQuery, $params);
        }
    }

    /**
     * Update an existing workflow
     * 
     * @param array $data Workflow data including workflow_id
     * @return bool True if successful, false otherwise
     */
    public function updateWorkflow($data) {
        if (empty($data['workflow_id'])) {
            return false;
        }
        
        try {
            $workflowId = $this->db->escapeString($data['workflow_id']);
            $workflowName = $this->db->escapeString($data['workflow_name']);
            $description = $this->db->escapeString($data['description'] ?? '');
            $updatedBy = $this->db->escapeString($data['updated_by']);
            $userEdit = $this->db->escapeString($data['user_edit'] ?? '');
            $userView = $this->db->escapeString($data['user_view'] ?? '');
            $sqlMailProfile = $this->db->escapeString($data['sql_mail_profile'] ?? '');
            $applicationFolder = $this->db->escapeString($data['application_folder'] ?? '');
            $userSpecialAction = $this->db->escapeString($data['user_special_action'] ?? '');
            $senderDisplay = $this->db->escapeString($data['sender_display'] ?? '');
            $defaultDomain = $this->db->escapeString($data['default_domain'] ?? '');
            
            $sql = "UPDATE WorkFlow SET 
                    workflow_name = '$workflowName', 
                    description = '$description', 
                    updated_by = '$updatedBy', 
                    updated_date = NOW(), 
                    user_edit = '$userEdit', 
                    user_view = '$userView', 
                    sql_mail_profile = '$sqlMailProfile', 
                    application_folder = '$applicationFolder', 
                    user_special_action = '$userSpecialAction', 
                    sender_display = '$senderDisplay', 
                    default_domain = '$defaultDomain' 
                    WHERE workflow_id = '$workflowId'";
            
            return $this->db->executeNonQuery($sql);
        } catch (Exception $e) {
            // Try alternative table case if the first query fails
            try {
                $workflowId = $this->db->escapeString($data['workflow_id']);
                $workflowName = $this->db->escapeString($data['workflow_name']);
                $description = $this->db->escapeString($data['description'] ?? '');
                $updatedBy = $this->db->escapeString($data['updated_by']);
                $userEdit = $this->db->escapeString($data['user_edit'] ?? '');
                $userView = $this->db->escapeString($data['user_view'] ?? '');
                $sqlMailProfile = $this->db->escapeString($data['sql_mail_profile'] ?? '');
                $applicationFolder = $this->db->escapeString($data['application_folder'] ?? '');
                $userSpecialAction = $this->db->escapeString($data['user_special_action'] ?? '');
                $senderDisplay = $this->db->escapeString($data['sender_display'] ?? '');
                $defaultDomain = $this->db->escapeString($data['default_domain'] ?? '');
                
                $sql = "UPDATE workflow SET 
                        workflow_name = '$workflowName', 
                        description = '$description', 
                        updated_by = '$updatedBy', 
                        updated_date = NOW(), 
                        user_edit = '$userEdit', 
                        user_view = '$userView', 
                        sql_mail_profile = '$sqlMailProfile', 
                        application_folder = '$applicationFolder', 
                        user_special_action = '$userSpecialAction', 
                        sender_display = '$senderDisplay', 
                        default_domain = '$defaultDomain' 
                        WHERE workflow_id = '$workflowId'";
                
                return $this->db->executeNonQuery($sql);
            } catch (Exception $e2) {
                error_log("Error updating workflow: " . $e->getMessage() . " / " . $e2->getMessage());
                return false;
            }
        }
    }

    /**
     * Set up a workflow step with event handlers and configurations
     * 
     * @param string $stepId The step ID to set up
     * @param array $data Form data
     * @return bool True if setup was successful
     */
    public function setupStep($stepId, $data) {
        try {
            // Extract event settings from the data
            $enableSave = isset($data['cb_enablesave']) ? 1 : 0;
            $enableApprove = isset($data['cb_enableapprove']) ? 1 : 0;
            $enableReject = isset($data['cb_enablereject']) ? 1 : 0;
            $defaultStart = isset($data['cb_defaultstart']) ? 1 : 0;
            
            // Button aliases/text
            $aliasSave = $data['AliasNameSave'] ?? '';
            $aliasApprove = $data['AliasNameApprove'] ?? '';
            $aliasReject = $data['AliasNameReject'] ?? '';
            
            // Trigger conditions
            $triggerSave = $data['TriggerSave'] ?? '';
            $triggerApprove = $data['TriggerApprove'] ?? '';
            $triggerReject = $data['TriggerReject'] ?? '';
            
            // Hide button flags
            $hideButtonSave = isset($data['HideButtonSave']) ? 1 : 0;
            $hideButtonApprove = isset($data['HideButtonApprove']) ? 1 : 0;
            $hideButtonReject = isset($data['HideButtonReject']) ? 1 : 0;
            
            // Dynamic key value
            $dinamicKeyValue = $data['DinamicKey'] ?? '';
            
            // Template-related settings
            $templateId = $data['template_id'] ?? '';
            $allowUseUploadMgr = isset($data['AllowUseUploadMgr']) ? 1 : 0;
            $allowMoreUploadFile = isset($data['AllowMoreUploadFile']) ? 1 : 0;
            $enableRichTextEditor = isset($data['EnableRichTextEditor']) ? 1 : 0;
            
            // If this is a default start step, set all other steps to not be default
            if ($defaultStart) {
                // Get the workflow ID
                $sql = "SELECT WFRowId FROM WorkFlowStep WHERE StepRowId = '" . $this->db->escapeString($stepId) . "'";
                $result = $this->db->executeQuery($sql);
                if (!empty($result)) {
                    $workflowId = $result[0]['WFRowId'];
                    
                    // Set all steps in this workflow to not be default
                    $sql = "UPDATE WorkFlowStep SET DefaultStart = 0 WHERE WFRowId = '" . 
                           $this->db->escapeString($workflowId) . "'";
                    $this->db->executeNonQuery($sql);
                }
            }
            
            // Build update SQL
            $sql = "UPDATE WorkFlowStep SET 
                    EnableSave = '" . $this->db->escapeString($enableSave) . "',
                    EnableApprove = '" . $this->db->escapeString($enableApprove) . "',
                    EnableReject = '" . $this->db->escapeString($enableReject) . "',
                    DefaultStart = '" . $this->db->escapeString($defaultStart) . "',
                    AliasNameSave = '" . $this->db->escapeString($aliasSave) . "',
                    AliasNameApprove = '" . $this->db->escapeString($aliasApprove) . "',
                    AliasNameReject = '" . $this->db->escapeString($aliasReject) . "',
                    TriggerSave = '" . $this->db->escapeString($triggerSave) . "',
                    TriggerApprove = '" . $this->db->escapeString($triggerApprove) . "',
                    TriggerReject = '" . $this->db->escapeString($triggerReject) . "',
                    HideButtonSave = '" . $this->db->escapeString($hideButtonSave) . "',
                    HideButtonApprove = '" . $this->db->escapeString($hideButtonApprove) . "',
                    HideButtonReject = '" . $this->db->escapeString($hideButtonReject) . "',
                    DinamicKeyValue = '" . $this->db->escapeString($dinamicKeyValue) . "'";
            
            // Add template-related fields if template is selected
            if (!empty($templateId)) {
                $sql .= ",
                    TemplateRowId = '" . $this->db->escapeString($templateId) . "',
                    AllowUseUploadMgr = '" . $this->db->escapeString($allowUseUploadMgr) . "',
                    AllowMoreUploadFile = '" . $this->db->escapeString($allowMoreUploadFile) . "',
                    EnableRichTextEditor = '" . $this->db->escapeString($enableRichTextEditor) . "'";
            }
            
            $sql .= ", UdDate = NOW() 
                    WHERE StepRowId = '" . $this->db->escapeString($stepId) . "'";
            
            if ($this->db->executeNonQuery($sql)) {
                // Process field configurations from the standard field config
                if (isset($data['field_config']) && is_array($data['field_config'])) {
                    $this->saveFieldConfigurations($stepId, $data['field_config']);
                }
                
                // Process template-specific field configurations if present
                if (isset($data['template_field']) && is_array($data['template_field'])) {
                    $this->saveTemplateFieldConfigurations($stepId, $data['template_field']);
                }
                
                return true;
            }
            
            return false;
        } catch (Exception $e) {
            error_log("Error in setupStep: " . $e->getMessage());
            return false;
        }
    }

    /**
     * Save field configurations for a workflow step
     * 
     * @param string $stepId The step ID to save configurations for
     * @param array $fieldConfig The field configurations
     * @return bool True if save was successful
     */
    private function saveFieldConfigurations($stepId, $fieldConfig) {
        // First delete existing configurations
        $sql = "DELETE FROM WorkFlowStepFieldConfig WHERE StepRowId = '" . $this->db->escapeString($stepId) . "'";
        $this->db->executeNonQuery($sql);
        
        // Insert new configurations
        foreach ($fieldConfig as $field => $config) {
            $enabled = isset($config['enabled']) ? 1 : 0;
            $required = isset($config['required']) ? 1 : 0;
            $defaultValue = $config['default'] ?? '';
            
            $sql = "INSERT INTO WorkFlowStepFieldConfig (
                    StepRowId,
                    FieldName,
                    IsEnabled,
                    IsRequired,
                    DefaultValue,
                    CrDate
                ) VALUES (
                    '" . $this->db->escapeString($stepId) . "',
                    '" . $this->db->escapeString($field) . "',
                    '" . $this->db->escapeString($enabled) . "',
                    '" . $this->db->escapeString($required) . "',
                    '" . $this->db->escapeString($defaultValue) . "',
                    NOW()
                )";
                
            $this->db->executeNonQuery($sql);
        }
        
        return true;
    }
    
    /**
     * Save template field configurations for a workflow step
     * 
     * @param string $stepId The step ID to save configurations for
     * @param array $fieldConfig The template field configurations
     * @return bool True if save was successful
     */
    private function saveTemplateFieldConfigurations($stepId, $fieldConfig) {
        // Get existing configurations to update or insert as needed
        $sql = "SELECT FieldName FROM WorkFlowStepFieldConfig WHERE StepRowId = '" . $this->db->escapeString($stepId) . "'";
        $result = $this->db->executeQuery($sql);
        
        $existingFields = [];
        foreach ($result as $row) {
            $existingFields[$row['FieldName']] = true;
        }
        
        // Process each template field
        foreach ($fieldConfig as $field => $config) {
            $enabled = isset($config['enabled']) ? 1 : 0;
            $required = isset($config['required']) ? 1 : 0;
            $defaultValue = $config['default'] ?? '';
            
            // Check if this field already exists in the configuration
            if (isset($existingFields[$field])) {
                // Update existing field
                $sql = "UPDATE WorkFlowStepFieldConfig SET 
                        IsEnabled = '" . $this->db->escapeString($enabled) . "',
                        IsRequired = '" . $this->db->escapeString($required) . "',
                        DefaultValue = '" . $this->db->escapeString($defaultValue) . "',
                        UdDate = NOW()
                        WHERE StepRowId = '" . $this->db->escapeString($stepId) . "'
                        AND FieldName = '" . $this->db->escapeString($field) . "'";
            } else {
                // Insert new field
                $sql = "INSERT INTO WorkFlowStepFieldConfig (
                        StepRowId,
                        FieldName,
                        IsEnabled,
                        IsRequired,
                        DefaultValue,
                        CrDate
                    ) VALUES (
                        '" . $this->db->escapeString($stepId) . "',
                        '" . $this->db->escapeString($field) . "',
                        '" . $this->db->escapeString($enabled) . "',
                        '" . $this->db->escapeString($required) . "',
                        '" . $this->db->escapeString($defaultValue) . "',
                        NOW()
                    )";
            }
                
            $this->db->executeNonQuery($sql);
        }
        
        return true;
    }
}
?> 