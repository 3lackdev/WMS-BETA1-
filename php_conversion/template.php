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

// Get current user
$currentUser = $auth->getCurrentUser();
$currentUsername = $currentUser['username'] ?? '';

// Get workflow ID from query parameter
$workflowId = isset($_GET['WFRowId']) ? $_GET['WFRowId'] : '';

// Get workflow details
$workflow = null;
// Check if a specific workflow was requested
if (!empty($workflowId)) {
    try {
        // Get workflow by ID - Modified for the actual database structure
        $sql = "SELECT 
                WFRowId as workflow_id, 
                WFName as workflow_name, 
                UserEdit as user_edit,
                UserView as user_view 
                FROM WorkFlow WHERE WFRowId = '" . $db->escapeString($workflowId) . "'";

        $result = $db->executeQuery($sql);
        if (count($result) > 0) {
            $workflow = $result[0];
        }
    } catch (Exception $e) {
        error_log("Error getting workflow: " . $e->getMessage());
    }
}

// Check if user has permission to edit templates
$canEdit = true; // Default permission for templates page

// If a specific workflow is selected, check permissions for that workflow
if ($workflow && !empty($workflow['user_edit'])) {
    $editUsers = explode(';', $workflow['user_edit']);
    $canEdit = in_array($currentUsername, $editUsers);
}

// Get all workflow templates
$templates = [];
try {
    $sql = "SELECT t.TemplateRowId as template_id, t.TemplateName as template_name, 
                  t.TemplateContent as description, t.WFRowId as workflow_id, 
                  w.WFName as workflow_name
            FROM WorkFlowTemplate t 
            LEFT JOIN WorkFlow w ON t.WFRowId = w.WFRowId";

    // Add workflow filter if specified
    if (!empty($workflowId)) {
        $sql .= " WHERE t.WFRowId = '" . $db->escapeString($workflowId) . "'";
    }

    $sql .= " ORDER BY t.TemplateName";
    $templates = $db->executeQuery($sql);
} catch (Exception $e) {
    // Try with lowercase table names
    try {
        $sql = "SELECT t.TemplateRowId as template_id, t.TemplateName as template_name, 
                      t.TemplateContent as description, t.WFRowId as workflow_id, 
                      w.WFName as workflow_name
                FROM workflowtemplate t 
                LEFT JOIN workflow w ON t.WFRowId = w.WFRowId";

        // Add workflow filter if specified
        if (!empty($workflowId)) {
            $sql .= " WHERE t.WFRowId = '" . $db->escapeString($workflowId) . "'";
        }

        $sql .= " ORDER BY t.TemplateName";
        $templates = $db->executeQuery($sql);
    } catch (Exception $e2) {
        error_log("Error getting templates: " . $e->getMessage() . " / " . $e2->getMessage());
    }
}

// Get all workflows for dropdown
$workflows = [];
try {
    $sql = "SELECT WFRowId as workflow_id, WFName as workflow_name FROM WorkFlow ORDER BY WFName";
    $workflows = $db->executeQuery($sql);
} catch (Exception $e) {
    // Try with lowercase table name
    try {
        $sql = "SELECT WFRowId as workflow_id, WFName as workflow_name FROM workflow ORDER BY WFName";
        $workflows = $db->executeQuery($sql);
    } catch (Exception $e2) {
        error_log("Error getting workflows: " . $e->getMessage() . " / " . $e2->getMessage());
    }
}

// Handle template actions (add, edit, delete)
$successMessage = '';
$errorMessage = '';

// Add a function to generate UUIDs
function generateUUID()
{
    // Generate 16 random bytes
    $data = random_bytes(16);

    // Set version to 0100
    $data[6] = chr(ord($data[6]) & 0x0f | 0x40);
    // Set bits 6-7 to 10
    $data[8] = chr(ord($data[8]) & 0x3f | 0x80);

    // Output the 36 character UUID
    return vsprintf('%s%s-%s-%s-%s-%s%s%s', str_split(bin2hex($data), 4));
}

if ($_SERVER['REQUEST_METHOD'] === 'POST' && $canEdit) {
    if (isset($_POST['action'])) {
        switch ($_POST['action']) {
            case 'add_template':
                if (isset($_POST['workflow_id'], $_POST['template_name']) && !empty($_POST['workflow_id']) && !empty($_POST['template_name'])) {
                    try {
                        $workflowId = $db->escapeString($_POST['workflow_id']);
                        $templateName = $db->escapeString($_POST['template_name']);
                        $description = $db->escapeString($_POST['description'] ?? '');

                        // Generate a UUID for TemplateRowId
                        $templateId = generateUUID();

                        $sql = "INSERT INTO WorkFlowTemplate (
                                TemplateRowId,
                                WFRowId, 
                                TemplateName, 
                                TemplateContent, 
                                CrDate, 
                                CrBy
                                ) VALUES (
                                '$templateId',
                                '$workflowId', 
                                '$templateName', 
                                '$description',
                                NOW(),
                                '$currentUsername'
                                )";

                        if ($db->executeNonQuery($sql)) {
                            $successMessage = "Template added successfully.";
                            // Refresh templates list
                            $sql = "SELECT t.TemplateRowId as template_id, t.TemplateName as template_name, 
                                          t.TemplateContent as description, t.WFRowId as workflow_id, 
                                          w.WFName as workflow_name
                                    FROM WorkFlowTemplate t 
                                    LEFT JOIN WorkFlow w ON t.WFRowId = w.WFRowId
                                    ORDER BY t.TemplateName";
                            $templates = $db->executeQuery($sql);
                        } else {
                            $errorMessage = "Failed to add template.";
                        }
                    } catch (Exception $e) {
                        // Try with lowercase table name
                        try {
                            $workflowId = $db->escapeString($_POST['workflow_id']);
                            $templateName = $db->escapeString($_POST['template_name']);
                            $description = $db->escapeString($_POST['description'] ?? '');

                            // Generate a UUID for TemplateRowId
                            $templateId = generateUUID();

                            $sql = "INSERT INTO workflowtemplate (
                                    TemplateRowId,
                                    WFRowId, 
                                    TemplateName, 
                                    TemplateContent, 
                                    CrDate, 
                                    CrBy
                                    ) VALUES (
                                    '$templateId',
                                    '$workflowId', 
                                    '$templateName', 
                                    '$description',
                                    NOW(),
                                    '$currentUsername'
                                    )";

                            if ($db->executeNonQuery($sql)) {
                                $successMessage = "Template added successfully.";
                                // Refresh templates list
                                $sql = "SELECT t.TemplateRowId as template_id, t.TemplateName as template_name, 
                                              t.TemplateContent as description, t.WFRowId as workflow_id, 
                                              w.WFName as workflow_name
                                        FROM workflowtemplate t 
                                        LEFT JOIN workflow w ON t.WFRowId = w.WFRowId
                                        ORDER BY t.TemplateName";
                                $templates = $db->executeQuery($sql);
                            } else {
                                $errorMessage = "Failed to add template.";
                            }
                        } catch (Exception $e2) {
                            $errorMessage = "Error adding template: " . $e2->getMessage();
                            error_log("Error adding template: " . $e->getMessage() . " / " . $e2->getMessage());
                        }
                    }
                } else {
                    $errorMessage = "Workflow and template name are required.";
                }
                break;

            case 'edit_template':
                if (
                    isset($_POST['template_id'], $_POST['workflow_id'], $_POST['template_name']) &&
                    !empty($_POST['template_id']) && !empty($_POST['workflow_id']) && !empty($_POST['template_name'])
                ) {
                    try {
                        $templateId = $db->escapeString($_POST['template_id']);
                        $workflowId = $db->escapeString($_POST['workflow_id']);
                        $templateName = $db->escapeString($_POST['template_name']);
                        $description = $db->escapeString($_POST['description'] ?? '');

                        $sql = "UPDATE WorkFlowTemplate SET 
                                WFRowId = '$workflowId', 
                                TemplateName = '$templateName', 
                                TemplateContent = '$description',
                                UdDate = NOW(),
                                UdBy = '$currentUsername'
                                WHERE TemplateRowId = '$templateId'";

                        if ($db->executeNonQuery($sql)) {
                            $successMessage = "Template updated successfully.";
                            // Refresh templates list
                            $sql = "SELECT t.TemplateRowId as template_id, t.TemplateName as template_name, 
                                          t.TemplateContent as description, t.WFRowId as workflow_id, 
                                          w.WFName as workflow_name
                                    FROM WorkFlowTemplate t 
                                    LEFT JOIN WorkFlow w ON t.WFRowId = w.WFRowId
                                    ORDER BY t.TemplateName";
                            $templates = $db->executeQuery($sql);
                        } else {
                            $errorMessage = "Failed to update template.";
                        }
                    } catch (Exception $e) {
                        // Try with lowercase table name
                        try {
                            $templateId = $db->escapeString($_POST['template_id']);
                            $workflowId = $db->escapeString($_POST['workflow_id']);
                            $templateName = $db->escapeString($_POST['template_name']);
                            $description = $db->escapeString($_POST['description'] ?? '');

                            $sql = "UPDATE workflowtemplate SET 
                                    WFRowId = '$workflowId', 
                                    TemplateName = '$templateName', 
                                    TemplateContent = '$description',
                                    UdDate = NOW(),
                                    UdBy = '$currentUsername'
                                    WHERE TemplateRowId = '$templateId'";

                            if ($db->executeNonQuery($sql)) {
                                $successMessage = "Template updated successfully.";
                                // Refresh templates list
                                $sql = "SELECT t.TemplateRowId as template_id, t.TemplateName as template_name, 
                                              t.TemplateContent as description, t.WFRowId as workflow_id, 
                                              w.WFName as workflow_name
                                        FROM workflowtemplate t 
                                        LEFT JOIN workflow w ON t.WFRowId = w.WFRowId
                                        ORDER BY t.TemplateName";
                                $templates = $db->executeQuery($sql);
                            } else {
                                $errorMessage = "Failed to update template.";
                            }
                        } catch (Exception $e2) {
                            $errorMessage = "Error updating template: " . $e2->getMessage();
                            error_log("Error updating template: " . $e->getMessage() . " / " . $e2->getMessage());
                        }
                    }
                } else {
                    $errorMessage = "Template ID, workflow, and template name are required.";
                }
                break;

            case 'delete_template':
                if (isset($_POST['template_id']) && !empty($_POST['template_id'])) {
                    try {
                        $templateId = $db->escapeString($_POST['template_id']);

                        // Begin transaction
                        $db->beginTransaction();

                        try {
                            // First delete related objects
                            $sql = "DELETE FROM TemplateObject WHERE TemplateRowId = '$templateId'";
                            $db->executeNonQuery($sql);

                            // Then delete the template
                            $sql = "DELETE FROM WorkFlowTemplate WHERE TemplateRowId = '$templateId'";
                            $db->executeNonQuery($sql);

                            // Commit transaction
                            $db->commitTransaction();
                            $successMessage = "Template and all its objects deleted successfully.";

                            // Refresh templates list
                            $sql = "SELECT t.TemplateRowId as template_id, t.TemplateName as template_name, 
                                          t.TemplateContent as description, t.WFRowId as workflow_id, 
                                          w.WFName as workflow_name
                                    FROM WorkFlowTemplate t 
                                    LEFT JOIN WorkFlow w ON t.WFRowId = w.WFRowId
                                    ORDER BY t.TemplateName";
                            $templates = $db->executeQuery($sql);
                        } catch (Exception $e) {
                            // Rollback transaction
                            $db->rollbackTransaction();
                            throw $e;
                        }
                    } catch (Exception $e) {
                        // Try with lowercase table names
                        try {
                            $templateId = $db->escapeString($_POST['template_id']);

                            // Begin transaction
                            $db->beginTransaction();

                            try {
                                // First delete related objects
                                $sql = "DELETE FROM templateobject WHERE TemplateRowId = '$templateId'";
                                $db->executeNonQuery($sql);

                                // Then delete the template
                                $sql = "DELETE FROM workflowtemplate WHERE TemplateRowId = '$templateId'";
                                $db->executeNonQuery($sql);

                                // Commit transaction
                                $db->commitTransaction();
                                $successMessage = "Template and all its objects deleted successfully.";

                                // Refresh templates list
                                $sql = "SELECT t.TemplateRowId as template_id, t.TemplateName as template_name, 
                                              t.TemplateContent as description, t.WFRowId as workflow_id, 
                                              w.WFName as workflow_name
                                        FROM workflowtemplate t 
                                        LEFT JOIN workflow w ON t.WFRowId = w.WFRowId
                                        ORDER BY t.TemplateName";
                                $templates = $db->executeQuery($sql);
                            } catch (Exception $e2) {
                                // Rollback transaction
                                $db->rollbackTransaction();
                                throw $e2;
                            }
                        } catch (Exception $e2) {
                            $errorMessage = "Error deleting template: " . $e2->getMessage();
                            error_log("Error deleting template: " . $e->getMessage() . " / " . $e2->getMessage());
                        }
                    }
                } else {
                    $errorMessage = "Template ID is required.";
                }
                break;
        }
    }
}

// Set page title and navigation
$pageTitle = !empty($workflow) ? "Templates for " . htmlspecialchars($workflow['workflow_name']) . " - WorkFlow Engine" : "Templates - WorkFlow Engine";
$pageNavigation = '<a href="index.php">Home</a> / ' . (!empty($workflow) ? '<a href="workflowlist.php">Workflows</a> / <span>' . htmlspecialchars($workflow['workflow_name']) . '</span>' : '<span>Templates</span>');

// Include header
include 'includes/header.php';
?>

<div class="container mx-auto px-4 py-8">
    <!-- Page Header -->
    <div class="flex justify-between items-center mb-6">
        <h1 class="text-2xl font-semibold text-gray-800">Workflow Templates</h1>
        <?php if ($canEdit): ?>
            <button type="button" id="addTemplateBtn" class="bg-blue-500 hover:bg-blue-600 text-white font-medium py-2 px-4 rounded-lg flex items-center">
                <i class="fa fa-plus mr-2"></i> Add Template
            </button>
        <?php endif; ?>
    </div>

    <!-- Success/Error Messages -->
    <?php if (!empty($successMessage)): ?>
        <div class="bg-green-50 border-l-4 border-green-400 p-4 mb-4">
            <div class="flex">
                <div class="flex-shrink-0">
                    <i class="fa fa-check-circle text-green-400"></i>
                </div>
                <div class="ml-3">
                    <p class="text-sm text-green-700"><?php echo $successMessage; ?></p>
                </div>
            </div>
        </div>
    <?php endif; ?>

    <?php if (!empty($errorMessage)): ?>
        <div class="bg-red-50 border-l-4 border-red-400 p-4 mb-4">
            <div class="flex">
                <div class="flex-shrink-0">
                    <i class="fa fa-exclamation-circle text-red-400"></i>
                </div>
                <div class="ml-3">
                    <p class="text-sm text-red-700"><?php echo $errorMessage; ?></p>
                </div>
            </div>
        </div>
    <?php endif; ?>

    <!-- Templates Table -->
    <div class="bg-white shadow-md rounded-lg overflow-hidden">
        <table class="min-w-full divide-y divide-gray-200">
            <thead class="bg-gray-50">
                <tr>
                    <th scope="col" class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Template Name</th>
                    <th scope="col" class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Workflow</th>
                    <th scope="col" class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Description</th>
                    <th scope="col" class="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">Actions</th>
                </tr>
            </thead>
            <tbody class="bg-white divide-y divide-gray-200">
                <?php if (empty($templates)): ?>
                    <tr>
                        <td colspan="4" class="px-6 py-4 text-center text-sm text-gray-500">No templates found</td>
                    </tr>
                <?php else: ?>
                    <?php foreach ($templates as $template): ?>
                        <tr>
                            <td class="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900"><?php echo htmlspecialchars($template['template_name']); ?></td>
                            <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500"><?php echo htmlspecialchars($template['workflow_name'] ?? ''); ?></td>
                            <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500"><?php echo htmlspecialchars($template['description'] ?? ''); ?></td>
                            <td class="px-6 py-4 whitespace-nowrap text-sm text-right space-x-2">

                                <a href="template_design.php?TemplateRowId=<?php echo urlencode($template['template_id']); ?>" class="text-purple-600 hover:text-purple-900">
                                    <i class="fa fa-paint-brush"></i> Design
                                </a>
                                <a href="template_objects.php?TemplateRowId=<?php echo urlencode($template['template_id']); ?>" class="text-indigo-600 hover:text-indigo-900">
                                    <i class="fa fa-list"></i> Objects
                                </a>

                                <?php if ($canEdit): ?>
                                    <button type="button" class="text-blue-600 hover:text-blue-900 editTemplateBtn"
                                        data-id="<?php echo htmlspecialchars($template['template_id']); ?>"
                                        data-workflow="<?php echo htmlspecialchars($template['workflow_id']); ?>"
                                        data-name="<?php echo htmlspecialchars($template['template_name']); ?>"
                                        data-description="<?php echo htmlspecialchars($template['description'] ?? ''); ?>">
                                        <i class="fa fa-edit"></i> Edit
                                    </button>
                                    <button type="button" class="text-red-600 hover:text-red-900 deleteTemplateBtn"
                                        data-id="<?php echo htmlspecialchars($template['template_id']); ?>"
                                        data-name="<?php echo htmlspecialchars($template['template_name']); ?>">
                                        <i class="fa fa-trash"></i> Delete
                                    </button>
                                <?php endif; ?>
                            </td>
                        </tr>
                    <?php endforeach; ?>
                <?php endif; ?>
            </tbody>
        </table>
    </div>
</div>

<!-- Add Template Modal -->
<div id="addTemplateModal" class="fixed inset-0 bg-gray-600 bg-opacity-50 flex items-center justify-center hidden z-50">
    <div class="bg-white rounded-lg shadow-lg w-full max-w-md">
        <div class="px-6 py-4 border-b">
            <h3 class="text-lg font-semibold text-gray-800">Add Template</h3>
        </div>
        <form method="post" action="">
            <input type="hidden" name="action" value="add_template">
            <div class="px-6 py-4 space-y-4">
                <div>
                    <label for="workflow_id" class="block text-sm font-medium text-gray-700">Workflow</label>
                    <select id="workflow_id" name="workflow_id" class="mt-1 block w-full pl-3 pr-10 py-2 text-base border-gray-300 focus:outline-none focus:ring-blue-500 focus:border-blue-500 sm:text-sm rounded-md">
                        <option value="">Select a workflow</option>
                        <?php foreach ($workflows as $workflow): ?>
                            <option value="<?php echo htmlspecialchars($workflow['workflow_id']); ?>"><?php echo htmlspecialchars($workflow['workflow_name']); ?></option>
                        <?php endforeach; ?>
                    </select>
                </div>
                <div>
                    <label for="template_name" class="block text-sm font-medium text-gray-700">Template Name</label>
                    <input type="text" id="template_name" name="template_name" class="mt-1 focus:ring-blue-500 focus:border-blue-500 block w-full shadow-sm sm:text-sm border-gray-300 rounded-md">
                </div>
                <div>
                    <label for="description" class="block text-sm font-medium text-gray-700">Description</label>
                    <textarea id="description" name="description" rows="3" class="mt-1 focus:ring-blue-500 focus:border-blue-500 block w-full shadow-sm sm:text-sm border-gray-300 rounded-md"></textarea>
                </div>
            </div>
            <div class="px-6 py-4 border-t bg-gray-50 flex justify-end space-x-3">
                <button type="button" id="cancelAddTemplate" class="bg-white py-2 px-4 border border-gray-300 rounded-md shadow-sm text-sm font-medium text-gray-700 hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500">
                    Cancel
                </button>
                <button type="submit" class="bg-blue-500 py-2 px-4 border border-transparent rounded-md shadow-sm text-sm font-medium text-white hover:bg-blue-600 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500">
                    Save
                </button>
            </div>
        </form>
    </div>
</div>

<!-- Edit Template Modal -->
<div id="editTemplateModal" class="fixed inset-0 bg-gray-600 bg-opacity-50 flex items-center justify-center hidden z-50">
    <div class="bg-white rounded-lg shadow-lg w-full max-w-md">
        <div class="px-6 py-4 border-b">
            <h3 class="text-lg font-semibold text-gray-800">Edit Template</h3>
        </div>
        <form method="post" action="">
            <input type="hidden" name="action" value="edit_template">
            <input type="hidden" name="template_id" id="edit_template_id">
            <div class="px-6 py-4 space-y-4">
                <div>
                    <label for="edit_workflow_id" class="block text-sm font-medium text-gray-700">Workflow</label>
                    <select id="edit_workflow_id" name="workflow_id" class="mt-1 block w-full pl-3 pr-10 py-2 text-base border-gray-300 focus:outline-none focus:ring-blue-500 focus:border-blue-500 sm:text-sm rounded-md">
                        <option value="">Select a workflow</option>
                        <?php foreach ($workflows as $workflow): ?>
                            <option value="<?php echo htmlspecialchars($workflow['workflow_id']); ?>"><?php echo htmlspecialchars($workflow['workflow_name']); ?></option>
                        <?php endforeach; ?>
                    </select>
                </div>
                <div>
                    <label for="edit_template_name" class="block text-sm font-medium text-gray-700">Template Name</label>
                    <input type="text" id="edit_template_name" name="template_name" class="mt-1 focus:ring-blue-500 focus:border-blue-500 block w-full shadow-sm sm:text-sm border-gray-300 rounded-md">
                </div>
                <div>
                    <label for="edit_description" class="block text-sm font-medium text-gray-700">Description</label>
                    <textarea id="edit_description" name="description" rows="3" class="mt-1 focus:ring-blue-500 focus:border-blue-500 block w-full shadow-sm sm:text-sm border-gray-300 rounded-md"></textarea>
                </div>
            </div>
            <div class="px-6 py-4 border-t bg-gray-50 flex justify-end space-x-3">
                <button type="button" id="cancelEditTemplate" class="bg-white py-2 px-4 border border-gray-300 rounded-md shadow-sm text-sm font-medium text-gray-700 hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500">
                    Cancel
                </button>
                <button type="submit" class="bg-blue-500 py-2 px-4 border border-transparent rounded-md shadow-sm text-sm font-medium text-white hover:bg-blue-600 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500">
                    Update
                </button>
            </div>
        </form>
    </div>
</div>

<!-- Delete Template Modal -->
<div id="deleteTemplateModal" class="fixed inset-0 bg-gray-600 bg-opacity-50 flex items-center justify-center hidden z-50">
    <div class="bg-white rounded-lg shadow-lg w-full max-w-md">
        <div class="px-6 py-4 border-b">
            <h3 class="text-lg font-semibold text-gray-800">Delete Template</h3>
        </div>
        <form method="post" action="">
            <input type="hidden" name="action" value="delete_template">
            <input type="hidden" name="template_id" id="delete_template_id">
            <div class="px-6 py-4">
                <p class="text-gray-700">Are you sure you want to delete the template "<span id="delete_template_name" class="font-semibold"></span>"?</p>
                <p class="text-red-600 mt-2">This will also delete all template objects associated with this template.</p>
            </div>
            <div class="px-6 py-4 border-t bg-gray-50 flex justify-end space-x-3">
                <button type="button" id="cancelDeleteTemplate" class="bg-white py-2 px-4 border border-gray-300 rounded-md shadow-sm text-sm font-medium text-gray-700 hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500">
                    Cancel
                </button>
                <button type="submit" class="bg-red-500 py-2 px-4 border border-transparent rounded-md shadow-sm text-sm font-medium text-white hover:bg-red-600 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-red-500">
                    Delete
                </button>
            </div>
        </form>
    </div>
</div>

<script>
    document.addEventListener('DOMContentLoaded', function() {
        // Add Template Modal
        const addTemplateBtn = document.getElementById('addTemplateBtn');
        const addTemplateModal = document.getElementById('addTemplateModal');
        const cancelAddTemplate = document.getElementById('cancelAddTemplate');

        if (addTemplateBtn) {
            addTemplateBtn.addEventListener('click', function() {
                addTemplateModal.classList.remove('hidden');
            });
        }

        if (cancelAddTemplate) {
            cancelAddTemplate.addEventListener('click', function() {
                addTemplateModal.classList.add('hidden');
            });
        }

        // Edit Template Modal
        const editTemplateBtns = document.querySelectorAll('.editTemplateBtn');
        const editTemplateModal = document.getElementById('editTemplateModal');
        const cancelEditTemplate = document.getElementById('cancelEditTemplate');
        const editTemplateId = document.getElementById('edit_template_id');
        const editWorkflowId = document.getElementById('edit_workflow_id');
        const editTemplateName = document.getElementById('edit_template_name');
        const editDescription = document.getElementById('edit_description');

        editTemplateBtns.forEach(function(btn) {
            btn.addEventListener('click', function() {
                const id = this.getAttribute('data-id');
                const workflow = this.getAttribute('data-workflow');
                const name = this.getAttribute('data-name');
                const description = this.getAttribute('data-description');

                editTemplateId.value = id;
                editWorkflowId.value = workflow;
                editTemplateName.value = name;
                editDescription.value = description;

                editTemplateModal.classList.remove('hidden');
            });
        });

        if (cancelEditTemplate) {
            cancelEditTemplate.addEventListener('click', function() {
                editTemplateModal.classList.add('hidden');
            });
        }

        // Delete Template Modal
        const deleteTemplateBtns = document.querySelectorAll('.deleteTemplateBtn');
        const deleteTemplateModal = document.getElementById('deleteTemplateModal');
        const cancelDeleteTemplate = document.getElementById('cancelDeleteTemplate');
        const deleteTemplateId = document.getElementById('delete_template_id');
        const deleteTemplateName = document.getElementById('delete_template_name');

        deleteTemplateBtns.forEach(function(btn) {
            btn.addEventListener('click', function() {
                const id = this.getAttribute('data-id');
                const name = this.getAttribute('data-name');

                deleteTemplateId.value = id;
                deleteTemplateName.textContent = name;

                deleteTemplateModal.classList.remove('hidden');
            });
        });

        if (cancelDeleteTemplate) {
            cancelDeleteTemplate.addEventListener('click', function() {
                deleteTemplateModal.classList.add('hidden');
            });
        }
    });
</script>

<?php
// Include footer
include 'includes/footer.php';
?>