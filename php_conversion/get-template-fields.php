<?php
/**
 * Get Template Fields
 * 
 * This file handles AJAX requests to load template-specific fields and settings
 */

require_once 'includes/Database.php';
require_once 'includes/Authentication.php';

// Initialize authentication
$auth = new Authentication();

// Check if user is authenticated
if (!$auth->isAuthenticated()) {
    echo json_encode(['error' => 'Not authenticated']);
    exit;
}

// Initialize database
$db = new Database();

// Get parameters
$templateId = $_GET['template_id'] ?? '';
$stepId = $_GET['step_id'] ?? '';

// Validate required parameters
if (empty($templateId)) {
    echo '<div class="alert alert-warning">No template selected</div>';
    exit;
}

// Get template details
try {
    $sql = "SELECT * FROM WorkFlowTemplate WHERE TemplateRowId = '" . $db->escapeString($templateId) . "'";
    $result = $db->executeQuery($sql);
    
    if (count($result) === 0) {
        echo '<div class="alert alert-warning">Template not found</div>';
        exit;
    }
    
    $template = $result[0];
    
    // Get template fields
    $fieldsSql = "SELECT * FROM WorkFlowTemplateField WHERE TemplateRowId = '" . $db->escapeString($templateId) . "' ORDER BY FieldOrder";
    $fieldsResult = $db->executeQuery($fieldsSql);
    
    // Get field settings if step ID is provided
    $fieldSettings = [];
    if (!empty($stepId)) {
        $settingsSql = "SELECT * FROM WorkFlowStepFieldConfig WHERE StepRowId = '" . $db->escapeString($stepId) . "'";
        $settingsResult = $db->executeQuery($settingsSql);
        
        foreach ($settingsResult as $setting) {
            $fieldSettings[$setting['FieldName']] = $setting;
        }
    }
    
    // Display template info
    ?>
    <div class="card mb-3">
        <div class="card-header">
            <strong><?php echo htmlspecialchars($template['TemplateName']); ?></strong>
        </div>
        <div class="card-body">
            <div class="mb-3">
                <small class="text-muted"><?php echo htmlspecialchars($template['Description'] ?? 'No description'); ?></small>
            </div>
            
            <?php if (count($fieldsResult) > 0): ?>
                <h6>Template Fields</h6>
                <div class="table-responsive">
                    <table class="table table-sm table-bordered">
                        <thead>
                            <tr>
                                <th>Field Name</th>
                                <th>Type</th>
                                <th>Enable</th>
                                <th>Required</th>
                                <th>Default Value</th>
                            </tr>
                        </thead>
                        <tbody>
                            <?php foreach ($fieldsResult as $field): ?>
                                <tr>
                                    <td>
                                        <?php echo htmlspecialchars($field['FieldName']); ?>
                                        <?php if (!empty($field['FieldDescription'])): ?>
                                            <i class="bi bi-info-circle" title="<?php echo htmlspecialchars($field['FieldDescription']); ?>"></i>
                                        <?php endif; ?>
                                    </td>
                                    <td><?php echo htmlspecialchars($field['FieldType'] ?? 'Text'); ?></td>
                                    <td>
                                        <div class="form-check">
                                            <input type="checkbox" class="form-check-input" 
                                                id="field_enabled_<?php echo htmlspecialchars($field['FieldName']); ?>" 
                                                name="template_field[<?php echo htmlspecialchars($field['FieldName']); ?>][enabled]" 
                                                value="1" 
                                                <?php echo (isset($fieldSettings[$field['FieldName']]) && 
                                                          ($fieldSettings[$field['FieldName']]['IsEnabled'] == 1 || 
                                                           $fieldSettings[$field['FieldName']]['IsEnabled'] == 'Y')) ? 'checked' : ''; ?>>
                                        </div>
                                    </td>
                                    <td>
                                        <div class="form-check">
                                            <input type="checkbox" class="form-check-input" 
                                                id="field_required_<?php echo htmlspecialchars($field['FieldName']); ?>" 
                                                name="template_field[<?php echo htmlspecialchars($field['FieldName']); ?>][required]" 
                                                value="1" 
                                                <?php echo (isset($fieldSettings[$field['FieldName']]) && 
                                                          ($fieldSettings[$field['FieldName']]['IsRequired'] == 1 || 
                                                           $fieldSettings[$field['FieldName']]['IsRequired'] == 'Y')) ? 'checked' : ''; ?>>
                                        </div>
                                    </td>
                                    <td>
                                        <input type="text" class="form-control form-control-sm" 
                                            id="field_default_<?php echo htmlspecialchars($field['FieldName']); ?>" 
                                            name="template_field[<?php echo htmlspecialchars($field['FieldName']); ?>][default]" 
                                            value="<?php echo htmlspecialchars($fieldSettings[$field['FieldName']]['DefaultValue'] ?? ''); ?>">
                                    </td>
                                </tr>
                            <?php endforeach; ?>
                        </tbody>
                    </table>
                </div>
            <?php else: ?>
                <div class="alert alert-info">No fields defined for this template</div>
            <?php endif; ?>
            
            <!-- Additional template configurations -->
            <h6 class="mt-3">Additional Settings</h6>
            <div class="form-check mb-2">
                <input type="checkbox" class="form-check-input" id="EnableRichTextEditor" name="EnableRichTextEditor" value="1" 
                    <?php echo ($template['EnableRichTextEditor'] == 1 || $template['EnableRichTextEditor'] == 'Y') ? 'checked' : ''; ?>>
                <label class="form-check-label" for="EnableRichTextEditor">Enable Rich Text Editor</label>
            </div>
        </div>
    </div>
    
    <script>
    // Add any additional JavaScript for template field handling here
    </script>
    <?php
    
} catch (Exception $e) {
    echo '<div class="alert alert-danger">Error loading template: ' . htmlspecialchars($e->getMessage()) . '</div>';
}
?> 