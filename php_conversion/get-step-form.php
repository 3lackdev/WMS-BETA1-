<?php
/**
 * Get Step Form
 * 
 * This file handles AJAX requests to load step forms for editing or setup
 */

require_once 'includes/Database.php';
require_once 'includes/Authentication.php';
require_once 'includes/WorkflowHelper.php';

// Initialize authentication
$auth = new Authentication();

// Check if user is authenticated
if (!$auth->isAuthenticated()) {
    echo json_encode(['error' => 'Not authenticated']);
    exit;
}

// Initialize database and helper
$db = new Database();
$helper = new WorkflowHelper($db);

// Get parameters
$action = $_GET['action'] ?? '';
$stepId = $_GET['stepId'] ?? '';
$workflowId = $_GET['workflowId'] ?? '';

// Validate required parameters
if (empty($stepId) || empty($action)) {
    echo json_encode(['error' => 'Missing parameters']);
    exit;
}

// Get step details
try {
    $sql = "SELECT * FROM WorkFlowStep WHERE StepRowId = '" . $db->escapeString($stepId) . "'";
    $result = $db->executeQuery($sql);
    
    if (count($result) === 0) {
        echo json_encode(['error' => 'Step not found']);
        exit;
    }
    
    $stepDetails = $result[0];
} catch (Exception $e) {
    echo json_encode(['error' => $e->getMessage()]);
    exit;
}

// Handle different form types
if ($action === 'edit') {
    // Return the basic edit form with only the specified fields
    ?>
    <form id="editForm" action="process-step-events.php" method="POST">
        <input type="hidden" name="action" value="update">
        <input type="hidden" name="WFRowId" value="<?php echo htmlspecialchars($stepId); ?>">
        <input type="hidden" name="WorkflowId" value="<?php echo htmlspecialchars($workflowId); ?>">
        <input type="hidden" name="save_type" value="basic">
        
        <div class="mb-3">
            <label for="StepName" class="form-label">Step Name</label>
            <input type="text" class="form-control" id="StepName" name="StepName" value="<?php echo htmlspecialchars($stepDetails['StepName'] ?? ''); ?>" required>
        </div>
        
        <div class="mb-3">
            <label for="Seq" class="form-label">Sequence</label>
            <input type="number" class="form-control" id="Seq" name="Seq" value="<?php echo htmlspecialchars($stepDetails['Seq'] ?? ''); ?>" required>
        </div>
        
        <div class="mb-3">
            <label for="GroupName" class="form-label">Group Name</label>
            <input type="text" class="form-control" id="GroupName" name="GroupName" value="<?php echo htmlspecialchars($stepDetails['GroupName'] ?? ''); ?>">
        </div>
        
        <div class="modal-footer">
            <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Close</button>
            <button type="submit" class="btn btn-primary">Save Changes</button>
        </div>
    </form>
    <?php
} elseif ($action === 'setup') {
    // Get field configurations for this step
    $fieldConfigs = [];
    try {
        $sql = "SELECT * FROM WorkFlowStepFieldConfig WHERE StepRowId = '" . $db->escapeString($stepId) . "'";
        $result = $db->executeQuery($sql);
        
        foreach ($result as $row) {
            $fieldConfigs[$row['FieldName']] = $row;
        }
    } catch (Exception $e) {
        // Silently handle error
    }
    
    // Get available fields for this workflow
    $availableFields = [];
    try {
        // This is a simplification - in a real application, you would get the fields
        // from the database or an API based on the workflow's configuration
        $workflowSql = "SELECT WFRowId FROM WorkFlowStep WHERE StepRowId = '" . $db->escapeString($stepId) . "'";
        $workflowResult = $db->executeQuery($workflowSql);
        
        if (!empty($workflowResult)) {
            $workflowId = $workflowResult[0]['WFRowId'];
            
            // Get fields from a related table (this is a placeholder - adjust to your schema)
            $fieldsSql = "SELECT DISTINCT FieldName FROM WorkFlowTemplateField WHERE WFRowId = '" . $db->escapeString($workflowId) . "'";
            $fieldsResult = $db->executeQuery($fieldsSql);
            
            foreach ($fieldsResult as $field) {
                $availableFields[] = $field['FieldName'];
            }
        }
    } catch (Exception $e) {
        // If no fields found, provide some defaults
        $availableFields = ['Field1', 'Field2', 'Field3', 'Field4', 'Field5'];
    }
    
    // Return the setup form with event configurations
    ?>
    <form id="setupEventsForm" action="process-step-events.php" method="POST">
        <input type="hidden" name="action" value="update">
        <input type="hidden" name="WFRowId" value="<?php echo htmlspecialchars($stepId); ?>">
        <input type="hidden" name="WorkflowId" value="<?php echo htmlspecialchars($workflowId); ?>">
        <input type="hidden" name="save_type" value="events_only">
        
        <h5>Event Configuration for: <?php echo htmlspecialchars($stepDetails['StepName']); ?></h5>
        
        <div class="mb-3 form-check">
            <input type="checkbox" class="form-check-input" id="DefaultStart" name="cb_defaultstart" value="1" <?php echo ($stepDetails['DefaultStart'] == 1) ? 'checked' : ''; ?>>
            <label class="form-check-label" for="DefaultStart">Default Start Step</label>
        </div>
        
        <div class="mb-3">
            <label for="template_id" class="form-label">Form Template</label>
            <select class="form-select" id="template_id" name="template_id">
                <option value="">-- Select Template --</option>
                <?php
                // Get available templates
                try {
                    $templateSql = "SELECT TemplateRowId, TemplateName FROM WorkFlowTemplate ORDER BY TemplateName";
                    $templates = $db->executeQuery($templateSql);
                    
                    foreach ($templates as $template) {
                        $selected = ($stepDetails['TemplateRowId'] == $template['TemplateRowId']) ? 'selected' : '';
                        echo "<option value=\"" . htmlspecialchars($template['TemplateRowId']) . "\" $selected>" . 
                             htmlspecialchars($template['TemplateName']) . "</option>";
                    }
                } catch (Exception $e) {
                    // Silently handle error
                }
                ?>
            </select>
        </div>
        
        <div id="template_settings" class="mb-3" style="display: <?php echo empty($stepDetails['TemplateRowId']) ? 'none' : 'block'; ?>">
            <h6>Template Settings</h6>
            <div class="form-check mb-2">
                <input type="checkbox" class="form-check-input" id="AllowUseUploadMgr" name="AllowUseUploadMgr" value="1" 
                    <?php echo ($stepDetails['AllowUseUploadMgr'] == 1 || $stepDetails['AllowUseUploadMgr'] == 'Y') ? 'checked' : ''; ?>>
                <label class="form-check-label" for="AllowUseUploadMgr">Allow File Upload</label>
            </div>
            
            <div class="form-check mb-2">
                <input type="checkbox" class="form-check-input" id="AllowMoreUploadFile" name="AllowMoreUploadFile" value="1" 
                    <?php echo ($stepDetails['AllowMoreUploadFile'] == 1 || $stepDetails['AllowMoreUploadFile'] == 'Y') ? 'checked' : ''; ?>>
                <label class="form-check-label" for="AllowMoreUploadFile">Allow Multiple File Upload</label>
            </div>
            
            <div id="template_fields_container" class="mt-3">
                <!-- Template-specific fields will be loaded here via AJAX -->
                <?php if (!empty($stepDetails['TemplateRowId'])): ?>
                <div class="spinner-border spinner-border-sm" role="status">
                    <span class="visually-hidden">Loading template fields...</span>
                </div>
                <?php endif; ?>
            </div>
        </div>
        
        <div class="row mb-3">
            <div class="col-md-4">
                <div class="form-check">
                    <input type="checkbox" class="form-check-input" id="cb_enablesave" name="cb_enablesave" value="1" <?php echo ($stepDetails['EnableSave'] == 1) ? 'checked' : ''; ?>>
                    <label class="form-check-label" for="cb_enablesave">Enable Save</label>
                </div>
                
                <div class="mt-2">
                    <label for="AliasNameSave" class="form-label">Save Button Text</label>
                    <input type="text" class="form-control" id="AliasNameSave" name="AliasNameSave" value="<?php echo htmlspecialchars($stepDetails['AliasNameSave'] ?? ''); ?>">
                </div>
                
                <div class="mt-2">
                    <label for="TriggerSave" class="form-label">Save Trigger Condition</label>
                    <input type="text" class="form-control" id="TriggerSave" name="TriggerSave" value="<?php echo htmlspecialchars($stepDetails['TriggerSave'] ?? ''); ?>">
                </div>
                
                <div class="form-check mt-2">
                    <input type="checkbox" class="form-check-input" id="HideButtonSave" name="HideButtonSave" value="1" <?php echo ($stepDetails['HideButtonSave'] == 1) ? 'checked' : ''; ?>>
                    <label class="form-check-label" for="HideButtonSave">Hide Save Button</label>
                </div>
            </div>
            
            <div class="col-md-4">
                <div class="form-check">
                    <input type="checkbox" class="form-check-input" id="cb_enableapprove" name="cb_enableapprove" value="1" <?php echo ($stepDetails['EnableApprove'] == 1) ? 'checked' : ''; ?>>
                    <label class="form-check-label" for="cb_enableapprove">Enable Approve</label>
                </div>
                
                <div class="mt-2">
                    <label for="AliasNameApprove" class="form-label">Approve Button Text</label>
                    <input type="text" class="form-control" id="AliasNameApprove" name="AliasNameApprove" value="<?php echo htmlspecialchars($stepDetails['AliasNameApprove'] ?? ''); ?>">
                </div>
                
                <div class="mt-2">
                    <label for="TriggerApprove" class="form-label">Approve Trigger Condition</label>
                    <input type="text" class="form-control" id="TriggerApprove" name="TriggerApprove" value="<?php echo htmlspecialchars($stepDetails['TriggerApprove'] ?? ''); ?>">
                </div>
                
                <div class="form-check mt-2">
                    <input type="checkbox" class="form-check-input" id="HideButtonApprove" name="HideButtonApprove" value="1" <?php echo ($stepDetails['HideButtonApprove'] == 1) ? 'checked' : ''; ?>>
                    <label class="form-check-label" for="HideButtonApprove">Hide Approve Button</label>
                </div>
            </div>
            
            <div class="col-md-4">
                <div class="form-check">
                    <input type="checkbox" class="form-check-input" id="cb_enablereject" name="cb_enablereject" value="1" <?php echo ($stepDetails['EnableReject'] == 1) ? 'checked' : ''; ?>>
                    <label class="form-check-label" for="cb_enablereject">Enable Reject</label>
                </div>
                
                <div class="mt-2">
                    <label for="AliasNameReject" class="form-label">Reject Button Text</label>
                    <input type="text" class="form-control" id="AliasNameReject" name="AliasNameReject" value="<?php echo htmlspecialchars($stepDetails['AliasNameReject'] ?? ''); ?>">
                </div>
                
                <div class="mt-2">
                    <label for="TriggerReject" class="form-label">Reject Trigger Condition</label>
                    <input type="text" class="form-control" id="TriggerReject" name="TriggerReject" value="<?php echo htmlspecialchars($stepDetails['TriggerReject'] ?? ''); ?>">
                </div>
                
                <div class="form-check mt-2">
                    <input type="checkbox" class="form-check-input" id="HideButtonReject" name="HideButtonReject" value="1" <?php echo ($stepDetails['HideButtonReject'] == 1) ? 'checked' : ''; ?>>
                    <label class="form-check-label" for="HideButtonReject">Hide Reject Button</label>
                </div>
            </div>
        </div>
        
        <div class="mb-3">
            <label for="DinamicKey" class="form-label">Dynamic Key Value</label>
            <input type="text" class="form-control" id="DinamicKey" name="DinamicKey" value="<?php echo htmlspecialchars($stepDetails['DinamicKeyValue'] ?? ''); ?>">
        </div>
        
        <hr>
        
        <h5>Field Configurations</h5>
        <div class="mb-2">
            <input type="text" id="field_search" class="form-control form-control-sm" placeholder="Search fields...">
            <button type="button" class="btn btn-sm btn-outline-primary mt-1" onclick="searchFields()">Search</button>
        </div>
        
        <div class="table-responsive">
            <table class="table table-sm table-bordered">
                <thead>
                    <tr>
                        <th>Field</th>
                        <th>Enable</th>
                        <th>Required</th>
                        <th>Default Value</th>
                    </tr>
                </thead>
                <tbody id="field_list">
                    <?php foreach ($availableFields as $field): ?>
                        <tr class="field-row">
                            <td><?php echo htmlspecialchars($field); ?></td>
                            <td>
                                <div class="form-check">
                                    <input type="checkbox" class="form-check-input" 
                                        id="field_enabled_<?php echo htmlspecialchars($field); ?>" 
                                        name="field_config[<?php echo htmlspecialchars($field); ?>][enabled]" 
                                        value="1" 
                                        <?php echo (isset($fieldConfigs[$field]) && $fieldConfigs[$field]['IsEnabled'] == 1) ? 'checked' : ''; ?>>
                                </div>
                            </td>
                            <td>
                                <div class="form-check">
                                    <input type="checkbox" class="form-check-input" 
                                        id="field_required_<?php echo htmlspecialchars($field); ?>" 
                                        name="field_config[<?php echo htmlspecialchars($field); ?>][required]" 
                                        value="1" 
                                        <?php echo (isset($fieldConfigs[$field]) && $fieldConfigs[$field]['IsRequired'] == 1) ? 'checked' : ''; ?>>
                                </div>
                            </td>
                            <td>
                                <input type="text" class="form-control form-control-sm" 
                                    id="field_default_<?php echo htmlspecialchars($field); ?>" 
                                    name="field_config[<?php echo htmlspecialchars($field); ?>][default]" 
                                    value="<?php echo htmlspecialchars($fieldConfigs[$field]['DefaultValue'] ?? ''); ?>">
                            </td>
                        </tr>
                    <?php endforeach; ?>
                </tbody>
            </table>
        </div>
        
        <div class="modal-footer">
            <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Close</button>
            <button type="submit" class="btn btn-primary">Save changes</button>
        </div>
    </form>
    
    <script>
    function searchFields() {
        const searchText = document.getElementById('field_search').value.toLowerCase();
        const rows = document.querySelectorAll('.field-row');
        
        rows.forEach(row => {
            const fieldName = row.querySelector('td:first-child').textContent.toLowerCase();
            if (fieldName.includes(searchText)) {
                row.style.display = '';
            } else {
                row.style.display = 'none';
            }
        });
    }
    
    // Add event listener for checkboxes to update UI state
    document.querySelectorAll('.form-check-input').forEach(checkbox => {
        checkbox.addEventListener('change', function() {
            const fieldId = this.id;
            if (fieldId.startsWith('cb_enable')) {
                const buttonType = fieldId.replace('cb_enable', '');
                const aliasField = document.getElementById('AliasName' + buttonType);
                const triggerField = document.getElementById('Trigger' + buttonType);
                const hideButton = document.getElementById('HideButton' + buttonType);
                
                aliasField.disabled = !this.checked;
                triggerField.disabled = !this.checked;
                hideButton.disabled = !this.checked;
            }
        });
    });
    
    // Initialize UI state
    document.getElementById('cb_enablesave').dispatchEvent(new Event('change'));
    document.getElementById('cb_enableapprove').dispatchEvent(new Event('change'));
    document.getElementById('cb_enablereject').dispatchEvent(new Event('change'));
    
    // Template selection handling
    document.getElementById('template_id').addEventListener('change', function() {
        const templateId = this.value;
        const templateSettings = document.getElementById('template_settings');
        
        if (templateId) {
            templateSettings.style.display = 'block';
            loadTemplateFields(templateId);
        } else {
            templateSettings.style.display = 'none';
        }
    });
    
    function loadTemplateFields(templateId) {
        const container = document.getElementById('template_fields_container');
        container.innerHTML = '<div class="spinner-border spinner-border-sm" role="status"><span class="visually-hidden">Loading...</span></div>';
        
        // Load template-specific fields via AJAX
        fetch('get-template-fields.php?template_id=' + templateId + '&step_id=<?php echo htmlspecialchars($stepId); ?>')
            .then(response => response.text())
            .then(data => {
                container.innerHTML = data;
            })
            .catch(error => {
                console.error('Error loading template fields:', error);
                container.innerHTML = '<div class="alert alert-danger">Error loading template fields</div>';
            });
    }
    
    // Load template fields on page load if template is selected
    <?php if (!empty($stepDetails['TemplateRowId'])): ?>
    document.addEventListener('DOMContentLoaded', function() {
        loadTemplateFields('<?php echo htmlspecialchars($stepDetails['TemplateRowId']); ?>');
    });
    <?php endif; ?>
    </script>
    <?php
} else {
    echo json_encode(['error' => 'Invalid action']);
}
?> 