<?php
require_once 'includes/Database.php';
require_once 'includes/Authentication.php';
require_once 'includes/WorkflowHelper.php';

// Authenticate user
$auth = new Authentication();
if (!$auth->isAuthenticated()) {
    header('Location: login.php');
    exit;
}

// Get current user data
$currentUser = $auth->getCurrentUser();
$currentUsername = $currentUser['username'] ?? '';

// Check if user can edit templates (for now, assume admin role can edit)
$canEdit = isset($currentUser['role']) && $currentUser['role'] == 'admin';

// Get template ID from URL
$templateId = isset($_GET['TemplateRowId']) ? $_GET['TemplateRowId'] : null;

if (!$templateId) {
    header('Location: template.php');
    exit;
}

// Connect to database
$db = new Database();
$helper = new WorkflowHelper($db);

// Get template details
$template = null;
$workflowId = null;
try {
    $sql = "SELECT TemplateRowId as template_id, WFRowId as workflow_id, TemplateName as template_name, TemplateContent as description 
           FROM WorkFlowTemplate WHERE TemplateRowId = '" . $db->escapeString($templateId) . "'";
    $result = $db->executeQuery($sql);
    
    if (count($result) > 0) {
        $template = $result[0];
        $workflowId = $template['workflow_id'];
    }
} catch (Exception $e) {
    // Try with lowercase table name
    try {
        $sql = "SELECT TemplateRowId as template_id, WFRowId as workflow_id, TemplateName as template_name, TemplateContent as description 
               FROM workflowtemplate WHERE TemplateRowId = '" . $db->escapeString($templateId) . "'";
        $result = $db->executeQuery($sql);
        
        if (count($result) > 0) {
            $template = $result[0];
            $workflowId = $template['workflow_id'];
        }
    } catch (Exception $e2) {
        error_log("Error getting template: " . $e->getMessage() . " / " . $e2->getMessage());
    }
}

if (!$template || !$workflowId) {
    // Redirect back to workflow list if template not found
    header("Location: workflowlist.php");
    exit;
}

// Get workflow details to check permissions
$workflow = null;
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

if (!$workflow) {
    // Redirect back to workflow list if workflow not found
    header("Location: workflowlist.php");
    exit;
}

// Check if user has permission to edit this workflow
$canEdit = true; // Default permission
if (!empty($workflow['user_edit'])) {
    $editUsers = explode(';', $workflow['user_edit']);
    $canEdit = in_array($currentUsername, $editUsers);
}

// Get all template objects for this template
$objects = [];
try {
    $sql = "SELECT 
            ObjectRowId as object_id, 
            TemplateRowId as template_id, 
            ObjectName as object_name, 
            ObjectType as object_type,
            '' as control_type,
            '' as description,
            '' as default_value,
            '' as validation,
            0 as object_order 
            FROM TemplateObject 
            WHERE TemplateRowId = '" . $db->escapeString($templateId) . "' 
            ORDER BY ObjectName";
    $objects = $db->executeQuery($sql);
} catch (Exception $e) {
    // Try with lowercase table name
    try {
        $sql = "SELECT 
                ObjectRowId as object_id, 
                TemplateRowId as template_id, 
                ObjectName as object_name, 
                ObjectType as object_type,
                '' as control_type,
                '' as description,
                '' as default_value,
                '' as validation,
                0 as object_order
                FROM templateobject 
                WHERE TemplateRowId = '" . $db->escapeString($templateId) . "' 
                ORDER BY ObjectName";
        $objects = $db->executeQuery($sql);
    } catch (Exception $e2) {
        error_log("Error getting template objects: " . $e->getMessage() . " / " . $e2->getMessage());
    }
}

// Add a function to generate UUIDs
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

// Handle object actions (add, edit, delete)
$successMessage = '';
$errorMessage = '';

if ($_SERVER['REQUEST_METHOD'] === 'POST' && $canEdit) {
    if (isset($_POST['action'])) {
        switch ($_POST['action']) {
            case 'add_object':
                if (isset($_POST['object_name']) && !empty($_POST['object_name'])) {
                    try {
                        $objectName = $db->escapeString($_POST['object_name']);
                        $objectType = $db->escapeString($_POST['object_type'] ?? '');
                        
                        // Generate a UUID for ObjectRowId
                        $objectId = generateUUID();
                        
                        $sql = "INSERT INTO TemplateObject (
                                ObjectRowId,
                                TemplateRowId, 
                                ObjectName, 
                                ObjectType,
                                CrDate,
                                CrBy
                                ) VALUES (
                                '$objectId',
                                '$templateId', 
                                '$objectName', 
                                '$objectType',
                                NOW(),
                                '$currentUsername'
                                )";
                        
                        if ($db->executeNonQuery($sql)) {
                            $successMessage = "Object added successfully.";
                            // Refresh objects list
                            $sql = "SELECT 
                                    ObjectRowId as object_id, 
                                    TemplateRowId as template_id, 
                                    ObjectName as object_name, 
                                    ObjectType as object_type,
                                    '' as control_type,
                                    '' as description,
                                    '' as default_value,
                                    '' as validation,
                                    0 as object_order
                                    FROM TemplateObject 
                                    WHERE TemplateRowId = '$templateId' 
                                    ORDER BY ObjectName";
                            $objects = $db->executeQuery($sql);
                        } else {
                            $errorMessage = "Failed to add object.";
                        }
                    } catch (Exception $e) {
                        // Try with lowercase table name
                        try {
                            $objectName = $db->escapeString($_POST['object_name']);
                            $objectType = $db->escapeString($_POST['object_type'] ?? '');
                            
                            // Generate a UUID for ObjectRowId
                            $objectId = generateUUID();
                            
                            $sql = "INSERT INTO templateobject (
                                    ObjectRowId,
                                    TemplateRowId, 
                                    ObjectName, 
                                    ObjectType,
                                    CrDate,
                                    CrBy
                                    ) VALUES (
                                    '$objectId',
                                    '$templateId', 
                                    '$objectName', 
                                    '$objectType',
                                    NOW(),
                                    '$currentUsername'
                                    )";
                            
                            if ($db->executeNonQuery($sql)) {
                                $successMessage = "Object added successfully.";
                                // Refresh objects list
                                $sql = "SELECT 
                                        ObjectRowId as object_id, 
                                        TemplateRowId as template_id, 
                                        ObjectName as object_name, 
                                        ObjectType as object_type,
                                        '' as control_type,
                                        '' as description,
                                        '' as default_value,
                                        '' as validation,
                                        0 as object_order
                                        FROM templateobject 
                                        WHERE TemplateRowId = '$templateId' 
                                        ORDER BY ObjectName";
                                $objects = $db->executeQuery($sql);
                            } else {
                                $errorMessage = "Failed to add object.";
                            }
                        } catch (Exception $e2) {
                            $errorMessage = "Error adding object: " . $e2->getMessage();
                            error_log("Error adding object: " . $e->getMessage() . " / " . $e2->getMessage());
                        }
                    }
                } else {
                    $errorMessage = "Object name is required.";
                }
                break;
                
            case 'edit_object':
                if (isset($_POST['object_id'], $_POST['object_name']) && !empty($_POST['object_id']) && !empty($_POST['object_name'])) {
                    try {
                        $objectId = $db->escapeString($_POST['object_id']);
                        $objectName = $db->escapeString($_POST['object_name']);
                        $objectType = $db->escapeString($_POST['object_type'] ?? '');
                        
                        $sql = "UPDATE TemplateObject SET 
                                ObjectName = '$objectName', 
                                ObjectType = '$objectType',
                                UdDate = NOW(),
                                UdBy = '$currentUsername'
                                WHERE ObjectRowId = '$objectId' AND TemplateRowId = '$templateId'";
                        
                        if ($db->executeNonQuery($sql)) {
                            $successMessage = "Object updated successfully.";
                            // Refresh objects list
                            $sql = "SELECT 
                                    ObjectRowId as object_id, 
                                    TemplateRowId as template_id, 
                                    ObjectName as object_name, 
                                    ObjectType as object_type,
                                    '' as control_type,
                                    '' as description,
                                    '' as default_value,
                                    '' as validation,
                                    0 as object_order
                                    FROM TemplateObject 
                                    WHERE TemplateRowId = '$templateId' 
                                    ORDER BY ObjectName";
                            $objects = $db->executeQuery($sql);
                        } else {
                            $errorMessage = "Failed to update object.";
                        }
                    } catch (Exception $e) {
                        // Try with lowercase table name
                        try {
                            $objectId = $db->escapeString($_POST['object_id']);
                            $objectName = $db->escapeString($_POST['object_name']);
                            $objectType = $db->escapeString($_POST['object_type'] ?? '');
                            
                            $sql = "UPDATE templateobject SET 
                                    ObjectName = '$objectName', 
                                    ObjectType = '$objectType',
                                    UdDate = NOW(),
                                    UdBy = '$currentUsername'
                                    WHERE ObjectRowId = '$objectId' AND TemplateRowId = '$templateId'";
                            
                            if ($db->executeNonQuery($sql)) {
                                $successMessage = "Object updated successfully.";
                                // Refresh objects list
                                $sql = "SELECT 
                                        ObjectRowId as object_id, 
                                        TemplateRowId as template_id, 
                                        ObjectName as object_name, 
                                        ObjectType as object_type,
                                        '' as control_type,
                                        '' as description,
                                        '' as default_value,
                                        '' as validation,
                                        0 as object_order
                                        FROM templateobject 
                                        WHERE TemplateRowId = '$templateId' 
                                        ORDER BY ObjectName";
                                $objects = $db->executeQuery($sql);
                            } else {
                                $errorMessage = "Failed to update object.";
                            }
                        } catch (Exception $e2) {
                            $errorMessage = "Error updating object: " . $e2->getMessage();
                            error_log("Error updating object: " . $e->getMessage() . " / " . $e2->getMessage());
                        }
                    }
                } else {
                    $errorMessage = "Object ID and name are required.";
                }
                break;
                
            case 'delete_object':
                if (isset($_POST['object_id']) && !empty($_POST['object_id'])) {
                    try {
                        $objectId = $db->escapeString($_POST['object_id']);
                        
                        $sql = "DELETE FROM TemplateObject WHERE ObjectRowId = '$objectId' AND TemplateRowId = '$templateId'";
                        
                        if ($db->executeNonQuery($sql)) {
                            $successMessage = "Object deleted successfully.";
                            // Refresh objects list
                            $sql = "SELECT 
                                    ObjectRowId as object_id, 
                                    TemplateRowId as template_id, 
                                    ObjectName as object_name, 
                                    ObjectType as object_type,
                                    '' as control_type,
                                    '' as description,
                                    '' as default_value,
                                    '' as validation,
                                    0 as object_order
                                    FROM TemplateObject 
                                    WHERE TemplateRowId = '$templateId' 
                                    ORDER BY ObjectName";
                            $objects = $db->executeQuery($sql);
                        } else {
                            $errorMessage = "Failed to delete object.";
                        }
                    } catch (Exception $e) {
                        // Try with lowercase table name
                        try {
                            $objectId = $db->escapeString($_POST['object_id']);
                            
                            $sql = "DELETE FROM templateobject WHERE ObjectRowId = '$objectId' AND TemplateRowId = '$templateId'";
                            
                            if ($db->executeNonQuery($sql)) {
                                $successMessage = "Object deleted successfully.";
                                // Refresh objects list
                                $sql = "SELECT 
                                        ObjectRowId as object_id, 
                                        TemplateRowId as template_id, 
                                        ObjectName as object_name, 
                                        ObjectType as object_type,
                                        '' as control_type,
                                        '' as description,
                                        '' as default_value,
                                        '' as validation,
                                        0 as object_order
                                        FROM templateobject 
                                        WHERE TemplateRowId = '$templateId' 
                                        ORDER BY ObjectName";
                                $objects = $db->executeQuery($sql);
                            } else {
                                $errorMessage = "Failed to delete object.";
                            }
                        } catch (Exception $e2) {
                            $errorMessage = "Error deleting object: " . $e2->getMessage();
                            error_log("Error deleting object: " . $e->getMessage() . " / " . $e2->getMessage());
                        }
                    }
                } else {
                    $errorMessage = "Object ID is required.";
                }
                break;
        }
    }
}

// Set page title and navigation
$pageTitle = "Template Objects - " . ($template['template_name'] ?? 'Unknown Template');
$pageNavigation = '<a href="index.php">Home</a> / <a href="template.php">Templates</a> / <span>' . htmlspecialchars($template['template_name'] ?? 'Template Objects') . '</span>';

require_once 'includes/header.php';
?>

<div class="container mx-auto px-4 py-8">
    <!-- Page Header -->
    <div class="flex justify-between items-center mb-6">
        <h1 class="text-2xl font-semibold text-gray-800">
            Template Objects: <?php echo htmlspecialchars($template['template_name'] ?? 'Unknown Template'); ?>
        </h1>
        <div class="space-x-2">
            <?php if ($canEdit): ?>
            <a href="template_design.php?TemplateRowId=<?php echo htmlspecialchars($templateId); ?>" class="bg-purple-500 hover:bg-purple-600 text-white font-medium py-2 px-4 rounded-lg flex items-center">
                <i class="fa fa-paint-brush mr-2"></i> Design Template
            </a>
            <?php endif; ?>
            <a href="template.php" class="bg-gray-500 hover:bg-gray-600 text-white font-medium py-2 px-4 rounded-lg flex items-center">
                <i class="fa fa-arrow-left mr-2"></i> Back to Templates
            </a>
        </div>
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
    
    <!-- Objects Table -->
    <div class="bg-white shadow-md rounded-lg overflow-hidden">
        <table class="min-w-full divide-y divide-gray-200">
            <thead class="bg-gray-50">
                <tr>
                    <th scope="col" class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Object Name</th>
                    <th scope="col" class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Object Type</th>
                </tr>
            </thead>
            <tbody class="bg-white divide-y divide-gray-200">
                <?php if (empty($objects)): ?>
                <tr>
                    <td colspan="2" class="px-6 py-4 text-center text-sm text-gray-500">No objects found for this template</td>
                </tr>
                <?php else: ?>
                <?php foreach ($objects as $object): ?>
                <tr>
                    <td class="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900"><?php echo htmlspecialchars($object['object_name']); ?></td>
                    <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500"><?php echo htmlspecialchars($object['object_type']); ?></td>
                </tr>
                <?php endforeach; ?>
                <?php endif; ?>
            </tbody>
        </table>
    </div>
</div>

<script>
document.addEventListener('DOMContentLoaded', function() {
    // No more modals or buttons needed after cleanup
});
</script>

<?php
// Include footer
include 'includes/footer.php';
?> 