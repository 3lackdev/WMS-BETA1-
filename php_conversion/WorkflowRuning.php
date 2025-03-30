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

// Mode flags
$isNew = isset($_GET['new']) && $_GET['new'] == '1';
$isEdit = isset($_GET['edit']) && $_GET['edit'] == '1';
$isView = isset($_GET['id']) && !$isEdit;
$workflowId = isset($_GET['id']) ? $_GET['id'] : 0;

// Get workflow if in edit or view mode
$workflow = null;
if ($workflowId > 0) {
    $workflow = $helper->getWorkflowById($workflowId);
    if (!$workflow) {
        // Workflow not found
        header("Location: workflowlist.php");
        exit;
    }
}

// Process form submission
$successMessage = '';
$errorMessage = '';
if ($_SERVER['REQUEST_METHOD'] === 'POST') {
    if (isset($_POST['action']) && $_POST['action'] === 'save_workflow') {
        $workflowData = [
            'workflow_name' => $_POST['workflow_name'] ?? '',
            'description' => $_POST['description'] ?? '',
            'user_edit' => $_POST['user_edit'] ?? $currentUsername,
            'user_view' => $_POST['user_view'] ?? '',
            'sql_mail_profile' => $_POST['sql_mail_profile'] ?? '',
            'application_folder' => $_POST['application_folder'] ?? '',
            'user_special_action' => $_POST['user_special_action'] ?? $currentUsername,
            'sender_display' => $_POST['sender_display'] ?? '',
            'default_domain' => $_POST['default_domain'] ?? ''
        ];
        
        try {
            if ($isNew) {
                // Create new workflow
                $workflowData['created_by'] = $currentUser['user_id'];
                $newWorkflowId = $helper->createWorkflow($workflowData);
                
                if ($newWorkflowId) {
                    $successMessage = 'Workflow created successfully.';
                    // Redirect to edit the new workflow
                    header("Location: WorkflowRuning.php?id={$newWorkflowId}&edit=1");
                    exit;
                } else {
                    $errorMessage = 'Failed to create workflow.';
                }
            } elseif ($isEdit && $workflow) {
                // Update existing workflow
                $workflowData['workflow_id'] = $workflowId;
                $workflowData['updated_by'] = $currentUser['user_id'];
                
                $success = $helper->updateWorkflow($workflowData);
                
                if ($success) {
                    $successMessage = 'Workflow updated successfully.';
                    // Refresh workflow data
                    $workflow = $helper->getWorkflowById($workflowId);
                } else {
                    $errorMessage = 'Failed to update workflow.';
                }
            }
        } catch (Exception $e) {
            $errorMessage = 'Error: ' . $e->getMessage();
        }
    }
}

// Get SQL mail profiles for dropdown
$sqlMailProfiles = [];
try {
    $sqlMailProfiles = $db->executeQuery("SELECT system, CONCAT(system, ' (Mail Profile: ', mailprofile, ')') as name FROM SystemAndMailProfile ORDER BY system");
} catch (Exception $e) {
    // Table might not exist, show empty list
}

// Set page title and header
$pageTitle = ($isNew ? "New Workflow" : ($isEdit ? "Edit Workflow" : "View Workflow")) . " - WorkFlow Engine";
$pageHeader = $isNew ? "Create New Workflow" : ($isEdit ? "Edit Workflow: " . ($workflow['workflow_name'] ?? '') : "Workflow: " . ($workflow['workflow_name'] ?? ''));
$pageNavigation = "<a href=\"workflowlist.php\" class=\"text-blue-600 hover:text-blue-800\">Workflow List</a>";

// Include additional CSS and JS
$additionalCss = [];
$additionalJs = [];

// Include header
include 'includes/header.php';
?>

<div class="w-full max-w-7xl mx-auto">
    <div class="bg-white shadow-md rounded-lg overflow-hidden mb-6">
        <div class="px-6 py-4 bg-gray-50 border-b border-gray-200 flex justify-between items-center">
            <h5 class="text-xl font-semibold text-gray-800"><?php echo $pageHeader; ?></h5>
            <div class="flex space-x-2">
                <a href="workflowlist.php" class="bg-gray-500 hover:bg-gray-600 text-white px-4 py-2 rounded-lg transition-colors duration-200 flex items-center">
                    <i class="fa fa-arrow-left mr-2"></i> Back to List
                </a>
            </div>
        </div>
        <div class="p-6">
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
            
            <?php if ($isNew || $isEdit): ?>
            <!-- Workflow Form -->
            <form method="post" action="">
                <input type="hidden" name="action" value="save_workflow">
                
                <div class="grid grid-cols-1 md:grid-cols-2 gap-6 mb-6">
                    <div class="space-y-4">
                        <div>
                            <label for="workflow_name" class="block text-sm font-medium text-gray-700 mb-1">Workflow Name <span class="text-red-500">*</span></label>
                            <input type="text" id="workflow_name" name="workflow_name" 
                                   class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500" 
                                   value="<?php echo htmlspecialchars($workflow['workflow_name'] ?? ''); ?>" required>
                        </div>
                        
                        <div>
                            <label for="sql_mail_profile" class="block text-sm font-medium text-gray-700 mb-1">SQL Mail Profile</label>
                            <select id="sql_mail_profile" name="sql_mail_profile" 
                                    class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500">
                                <option value="">-- Select Mail Profile --</option>
                                <?php if (is_array($sqlMailProfiles)): ?>
                                    <?php foreach ($sqlMailProfiles as $profile): ?>
                                    <option value="<?php echo htmlspecialchars($profile['system'] ?? ''); ?>" 
                                            <?php echo ($workflow['sql_mail_profile'] ?? '') == ($profile['system'] ?? '') ? 'selected' : ''; ?>>
                                        <?php echo htmlspecialchars($profile['name'] ?? $profile['system'] ?? ''); ?>
                                    </option>
                                    <?php endforeach; ?>
                                <?php endif; ?>
                                <?php if (empty($sqlMailProfiles)): ?>
                                    <!-- Default option when no profiles available -->
                                    <option value="WEBWF" <?php echo ($workflow['sql_mail_profile'] ?? '') == 'WEBWF' ? 'selected' : ''; ?>>WEBWF</option>
                                <?php endif; ?>
                            </select>
                            <p class="text-sm text-gray-500 mt-1">รายการมาจากระบบ Notification Service table : BST_NOTIFICATION.SystemAndMailProfile หากต้องการ Create profile ใหม่กรุณาติดต่อ CKP</p>
                        </div>
                        
                        <div>
                            <label for="sender_display" class="block text-sm font-medium text-gray-700 mb-1">Sender Email Display</label>
                            <input type="text" id="sender_display" name="sender_display" 
                                   class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500" 
                                   value="<?php echo htmlspecialchars($workflow['sender_display'] ?? ''); ?>" 
                                   placeholder="BST Workflow <no-reply_webwf@bst.co.th>">
                            <p class="text-sm text-gray-500 mt-1">ตัวอย่าง : BST Workflow &lt;no-reply_webwf@bst.co.th&gt;</p>
                        </div>
                        
                        <div>
                            <label for="application_folder" class="block text-sm font-medium text-gray-700 mb-1">Customize Application Folder</label>
                            <input type="text" id="application_folder" name="application_folder" 
                                   class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500" 
                                   value="<?php echo htmlspecialchars($workflow['application_folder'] ?? ''); ?>">
                            <p class="text-sm text-gray-500 mt-1">ระบุ customize application folder หากมีการ deverlop นอกเหนือ standard หากไม่มีไม่ต้องระบุ</p>
                        </div>
                    </div>
                    
                    <div class="space-y-4">
                        <div>
                            <label for="user_view" class="block text-sm font-medium text-gray-700 mb-1">User View</label>
                            <textarea id="user_view" name="user_view" 
                                      class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500" 
                                      rows="3"><?php echo htmlspecialchars($workflow['user_view'] ?? ''); ?></textarea>
                            <p class="text-sm text-gray-500 mt-1"><span class="text-red-500">ไม่ระบุคือ ทุกคนสามารถ View</span>&nbsp;&nbsp; ตัวอย่างการระบุ Chakrapan_a;doltawat_s</p>
                        </div>
                        
                        <div>
                            <label for="user_edit" class="block text-sm font-medium text-gray-700 mb-1">User Edit</label>
                            <textarea id="user_edit" name="user_edit" 
                                      class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500" 
                                      rows="3"><?php echo htmlspecialchars($workflow['user_edit'] ?? $currentUsername); ?></textarea>
                            <p class="text-sm text-gray-500 mt-1"><span class="text-red-500">ไม่ระบุคือ ทุกคนสามารถ Edit</span>&nbsp;&nbsp; ตัวอย่างการระบุ Chakrapan_a;doltawat_s</p>
                        </div>
                        
                        <div>
                            <label for="user_special_action" class="block text-sm font-medium text-gray-700 mb-1">User Can Use Special Action History</label>
                            <textarea id="user_special_action" name="user_special_action" 
                                      class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500" 
                                      rows="3"><?php echo htmlspecialchars($workflow['user_special_action'] ?? $currentUsername); ?></textarea>
                            <p class="text-sm text-gray-500 mt-1"><span class="text-red-500">ไม่ระบุคือ ทุกคนสามารถ</span>&nbsp;&nbsp; ตัวอย่างการระบุ Chakrapan_a;doltawat_s</p>
                        </div>
                        
                        <div>
                            <label for="default_domain" class="block text-sm font-medium text-gray-700 mb-1">Default Domain</label>
                            <input type="text" id="default_domain" name="default_domain" 
                                   class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500" 
                                   value="<?php echo htmlspecialchars($workflow['default_domain'] ?? ''); ?>">
                        </div>
                    </div>
                </div>
                
                <div class="flex justify-between mt-8">
                    <div>
                        <button type="submit" class="bg-green-500 hover:bg-green-600 text-white px-6 py-2 rounded-lg transition-colors duration-200">
                            <i class="fa fa-save mr-2"></i> <?php echo $isNew ? 'Create Workflow' : 'Save Changes'; ?>
                        </button>
                        <a href="workflowlist.php" class="bg-gray-500 hover:bg-gray-600 text-white px-6 py-2 rounded-lg transition-colors duration-200 ml-2">
                            <i class="fa fa-times mr-2"></i> Cancel
                        </a>
                    </div>
                    
                    <?php if ($isEdit && $workflow): ?>
                    <button type="button" onclick="confirmDelete(<?php echo $workflow['workflow_id']; ?>)" class="bg-red-500 hover:bg-red-600 text-white px-6 py-2 rounded-lg transition-colors duration-200">
                        <i class="fa fa-trash mr-2"></i> Delete
                    </button>
                    <?php endif; ?>
                </div>
            </form>
            <?php else: ?>
            <!-- View Mode -->
            <div class="grid grid-cols-1 md:grid-cols-2 gap-6 mb-6">
                <div class="space-y-4">
                    <div>
                        <h3 class="text-sm font-medium text-gray-500">Workflow Name</h3>
                        <p class="text-lg"><?php echo htmlspecialchars($workflow['workflow_name'] ?? 'N/A'); ?></p>
                    </div>
                    
                    <div>
                        <h3 class="text-sm font-medium text-gray-500">Created By</h3>
                        <p><?php echo htmlspecialchars($workflow['created_by_name'] ?? 'N/A'); ?></p>
                    </div>
                    
                    <div>
                        <h3 class="text-sm font-medium text-gray-500">Created Date</h3>
                        <p><?php echo htmlspecialchars($workflow['created_date'] ?? 'N/A'); ?></p>
                    </div>
                    
                    <div>
                        <h3 class="text-sm font-medium text-gray-500">Status</h3>
                        <span class="px-2 py-1 text-xs font-semibold rounded-full <?php echo getTailwindStatusClass($workflow['status_name'] ?? ''); ?>">
                            <?php echo htmlspecialchars($workflow['status_name'] ?? 'N/A'); ?>
                        </span>
                    </div>
                </div>
                
                <div class="space-y-4">
                    <div>
                        <h3 class="text-sm font-medium text-gray-500">SQL Mail Profile</h3>
                        <p><?php echo htmlspecialchars($workflow['sql_mail_profile'] ?? 'N/A'); ?></p>
                    </div>
                    
                    <div>
                        <h3 class="text-sm font-medium text-gray-500">Application Folder</h3>
                        <p><?php echo htmlspecialchars($workflow['application_folder'] ?? 'N/A'); ?></p>
                    </div>
                    
                    <div>
                        <h3 class="text-sm font-medium text-gray-500">Sender Display</h3>
                        <p><?php echo htmlspecialchars($workflow['sender_display'] ?? 'N/A'); ?></p>
                    </div>
                    
                    <div>
                        <a href="WorkflowRuning.php?id=<?php echo $workflow['workflow_id']; ?>&edit=1" class="bg-yellow-500 hover:bg-yellow-600 text-white px-4 py-2 rounded-lg transition-colors duration-200 inline-block mt-4">
                            <i class="fa fa-edit mr-2"></i> Edit Workflow
                        </a>
                    </div>
                </div>
            </div>
            <?php endif; ?>
        </div>
    </div>
    
    <?php if (!$isNew && $workflow): ?>
    <!-- Workflow Tabs (when viewing or editing existing workflow) -->
    <div class="bg-white shadow-md rounded-lg overflow-hidden mb-6">
        <div class="px-6 py-4 bg-gray-50 border-b border-gray-200">
            <h5 class="text-xl font-semibold text-gray-800">Workflow Configuration</h5>
        </div>
        <div class="p-6">
            <div class="flex flex-wrap space-x-2 space-y-2 md:space-y-0">
                <a href="template.php?WFRowId=<?php echo $workflow['workflow_id']; ?>" class="bg-blue-500 hover:bg-blue-600 text-white px-4 py-2 rounded-lg transition-colors duration-200">
                    <i class="fa fa-file-alt mr-2"></i> Templates
                </a>
                <a href="Position.php?rowid=<?php echo $workflow['workflow_id']; ?>" class="bg-purple-500 hover:bg-purple-600 text-white px-4 py-2 rounded-lg transition-colors duration-200">
                    <i class="fa fa-users mr-2"></i> Positions & Email
                </a>
                <a href="step-events.php?rowid=<?php echo $workflow['workflow_id']; ?>" class="bg-green-500 hover:bg-green-600 text-white px-4 py-2 rounded-lg transition-colors duration-200">
                    <i class="fa fa-tasks mr-2"></i> Steps & Events
                </a>
                <a href="ViewRuningHistory.php?id=<?php echo $workflow['workflow_id']; ?>" class="bg-gray-500 hover:bg-gray-600 text-white px-4 py-2 rounded-lg transition-colors duration-200">
                    <i class="fa fa-history mr-2"></i> View History
                </a>
                <a href="#" onclick="showApiView('<?php echo $workflow['workflow_id']; ?>')" class="bg-red-500 hover:bg-red-600 text-white px-4 py-2 rounded-lg transition-colors duration-200">
                    <i class="fa fa-code mr-2"></i> View API
                </a>
            </div>
        </div>
    </div>
    <?php endif; ?>
</div>

<!-- Delete Confirmation Modal -->
<div id="deleteModal" class="fixed inset-0 bg-gray-600 bg-opacity-50 hidden flex items-center justify-center z-50">
    <div class="bg-white rounded-lg shadow-lg w-11/12 md:w-1/3 p-6">
        <h3 class="text-xl font-semibold text-gray-800 mb-4">Confirm Deletion</h3>
        <p class="mb-6">ต้องการลบ Workflow นี้ใช่หรือไม่? การลบจะไม่สามารถกู้คืนได้</p>
        <div class="flex justify-end space-x-2">
            <button onclick="closeDeleteModal()" class="bg-gray-500 hover:bg-gray-600 text-white px-4 py-2 rounded-lg transition-colors duration-200">
                Cancel
            </button>
            <a href="#" id="confirmDeleteButton" class="bg-red-500 hover:bg-red-600 text-white px-4 py-2 rounded-lg transition-colors duration-200">
                Delete
            </a>
        </div>
    </div>
</div>

<script>
    function confirmDelete(workflowId) {
        document.getElementById('confirmDeleteButton').href = 'delete_workflow.php?id=' + workflowId;
        document.getElementById('deleteModal').classList.remove('hidden');
    }
    
    function closeDeleteModal() {
        document.getElementById('deleteModal').classList.add('hidden');
    }
</script>

<?php
// Helper function to get Tailwind status classes
function getTailwindStatusClass($status) {
    switch (strtolower($status)) {
        case 'completed':
            return 'bg-green-100 text-green-800';
        case 'in progress':
            return 'bg-blue-100 text-blue-800';
        case 'pending':
            return 'bg-yellow-100 text-yellow-800';
        case 'cancelled':
            return 'bg-red-100 text-red-800';
        default:
            return 'bg-gray-100 text-gray-800';
    }
}

// Include footer
include 'includes/footer.php';
?> 