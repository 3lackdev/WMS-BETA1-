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

// View only mine setting
$viewOnlyMine = isset($_SESSION['view_only_mine']) ? $_SESSION['view_only_mine'] : true;
if (isset($_GET['toggle_view'])) {
    $viewOnlyMine = $_GET['toggle_view'] == '1';
    $_SESSION['view_only_mine'] = $viewOnlyMine;
}

// Get filter parameters
$status = isset($_GET['status']) ? $_GET['status'] : '';
$search = isset($_GET['search']) ? $_GET['search'] : '';

// Check for success or error messages
$successMessage = '';
$errorMessage = '';

if (isset($_GET['deleted']) && $_GET['deleted'] == '1') {
    $successMessage = 'Workflow has been successfully deleted.';
}

if (isset($_GET['error']) && $_GET['error'] == 'delete') {
    $errorMessage = 'An error occurred while deleting the workflow.';
}

// Build filter condition
$filter = '';
if (!empty($status)) {
    // Since we don't have status_id in our table yet, we'll skip this for now
    // $filter .= "w.status_id = '" . $db->escapeString($status) . "'";
}

if (!empty($search)) {
    if (!empty($filter)) {
        $filter .= " AND ";
    }
    $filter .= "(w.WFName LIKE '%" . $db->escapeString($search) . "%' OR w.CrBy LIKE '%" . $db->escapeString($search) . "%')";
}

// Add view only mine filter
if ($viewOnlyMine) {
    if (!empty($filter)) {
        $filter .= " AND ";
    }
    $filter .= "w.CrBy = '" . $db->escapeString($currentUser['user_id'] ?? 0) . "'";
}

// Get workflows
$workflows = $helper->getAllWorkflows($filter);

// Get statuses for filter - Match the exact case of the original table name
try {
    $statuses = $db->executeQuery("SELECT status_id, status_name FROM WorkFlowStatus ORDER BY status_name");
} catch (Exception $e) {
    // Fallback if the table name casing is different
    try {
        $statuses = $db->executeQuery("SELECT status_id, status_name FROM workflowstatus ORDER BY status_name");
    } catch (Exception $e2) {
        // If both attempts fail, provide an empty array
        $statuses = [];
    }
}

// Get SQL mail profiles for dropdown in the modal
$sqlMailProfiles = [];
try {
    $sqlMailProfiles = $db->executeQuery("SELECT system, CONCAT(system, ' (Mail Profile: ', mailprofile, ')') as name FROM SystemAndMailProfile ORDER BY system");
} catch (Exception $e) {
    // Table might not exist, show empty list
}

// Handle workflow copy action
if (isset($_GET['action']) && $_GET['action'] == 'copy' && isset($_GET['id'])) {
    $workflowId = $_GET['id'];
    
    try {
        // Call helper method to copy workflow
        $helper->copyWorkflow($workflowId, $currentUser['user_id']);
        
        // Redirect to prevent refresh issues
        header("Location: workflowlist.php");
        exit;
    } catch (Exception $e) {
        $errorMessage = "Error copying workflow: " . $e->getMessage();
    }
}

// Set page title and header
$pageTitle = "Workflow List - WorkFlow Engine";
$pageHeader = "Workflow List";
$pageNavigation = "<a href=\"index.php\" class=\"text-blue-600 hover:text-blue-800\">หน้าแรกโปรแกรม</a>";

// Include additional CSS and JS
$additionalCss = ['DatePicker/datepicker.min.css'];
$additionalJs = ['DatePicker/datepicker.min.js'];

// Include header
include 'includes/header.php';
?>

<div class="w-full max-w-7xl mx-auto">
    <div class="bg-white shadow-md rounded-lg overflow-hidden mb-6">
        <div class="px-6 py-4 bg-gray-50 border-b border-gray-200 flex flex-wrap justify-between items-center">
            <h5 class="text-xl font-semibold text-gray-800">Workflows</h5>
            <div class="flex space-x-2">
                <button type="button" onclick="openNewWorkflowModal()" class="bg-green-500 hover:bg-green-600 text-white px-4 py-2 rounded-lg transition-colors duration-200 flex items-center">
                    <i class="fa fa-plus mr-2"></i> New Workflow
                </button>
                <a href="index.php" class="bg-gray-500 hover:bg-gray-600 text-white px-4 py-2 rounded-lg transition-colors duration-200 flex items-center">
                    <i class="fa fa-arrow-left mr-2"></i> Back
                </a>
            </div>
        </div>
        <div class="p-6">
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
            
            <!-- Toggle View Only Mine -->
            <div class="mb-4 flex justify-between items-center">
                <div>
                    <a href="?toggle_view=<?php echo $viewOnlyMine ? '0' : '1'; ?>" class="flex items-center">
                        <div class="relative inline-block w-10 mr-2 align-middle select-none">
                            <input type="checkbox" <?php echo $viewOnlyMine ? 'checked' : ''; ?> class="toggle-checkbox absolute block w-6 h-6 rounded-full bg-white border-4 appearance-none cursor-pointer" disabled />
                            <label class="toggle-label block overflow-hidden h-6 rounded-full bg-gray-300 cursor-pointer"></label>
                        </div>
                        <span class="text-sm text-gray-700"><?php echo $viewOnlyMine ? 'ดูเฉพาะของฉัน' : 'ดูทั้งหมด'; ?></span>
                    </a>
                </div>
                <div>
                    <a href="#" class="text-blue-500 hover:text-blue-700 transition-colors duration-200">
                        <i class="fa fa-upload mr-1"></i> Import Workflow
                    </a>
                </div>
            </div>
            
            <!-- Filter Form -->
            <form method="get" action="workflowlist.php" class="mb-6">
                <div class="flex flex-col md:flex-row space-y-4 md:space-y-0 md:space-x-4">
                    <div class="md:w-2/5">
                        <input type="text" class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500" 
                               name="search" value="<?php echo htmlspecialchars($search); ?>" 
                               placeholder="Search workflow name or creator">
                    </div>
                    <div class="md:w-2/5">
                        <select class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500" name="status">
                            <option value="">-- All Statuses --</option>
                            <?php if (is_array($statuses)): ?>
                                <?php foreach ($statuses as $statusItem): ?>
                                <option value="<?php echo $statusItem['status_id'] ?? ''; ?>" <?php echo ($status == ($statusItem['status_id'] ?? '')) ? 'selected' : ''; ?>>
                                    <?php echo htmlspecialchars($statusItem['status_name'] ?? 'Unknown'); ?>
                                </option>
                                <?php endforeach; ?>
                            <?php endif; ?>
                        </select>
                    </div>
                    <div class="md:w-1/5">
                        <button type="submit" class="w-full bg-blue-500 hover:bg-blue-600 text-white px-4 py-2 rounded-lg transition-colors duration-200 flex items-center justify-center">
                            <i class="fa fa-search mr-2"></i> Filter
                        </button>
                    </div>
                </div>
                
                <!-- Keep the toggle value when filtering -->
                <input type="hidden" name="toggle_view" value="<?php echo $viewOnlyMine ? '1' : '0'; ?>">
            </form>
            
            <!-- Workflows Table -->
            <?php if (is_array($workflows) && count($workflows) > 0): ?>
            <div class="overflow-x-auto">
                <table class="w-full table-auto">
                    <thead class="bg-indigo-800 text-white">
                        <tr>
                            <th class="px-6 py-3 text-left text-xs font-medium uppercase tracking-wider">Workflow ID</th>
                            <th class="px-6 py-3 text-left text-xs font-medium uppercase tracking-wider">Workflow Name</th>
                            <th class="px-6 py-3 text-left text-xs font-medium uppercase tracking-wider">Settings</th>
                        </tr>
                    </thead>
                    <tbody class="bg-white divide-y divide-gray-200">
                        <?php foreach ($workflows as $workflow): ?>
                        <tr class="hover:bg-gray-50">
                            <td class="px-6 py-4 whitespace-nowrap"><?php echo htmlspecialchars($workflow['workflow_id'] ?? 'N/A'); ?></td>
                            <td class="px-6 py-4">
                                <div>
                                    <span class="font-bold"><?php echo htmlspecialchars($workflow['workflow_name'] ?? 'N/A'); ?></span>
                                    <div class="mt-2 flex flex-wrap space-x-2">
                                        <?php 
                                        $userCanEdit = $auth->hasRole('admin') || 
                                            (isset($currentUser['user_id'], $workflow['created_by_id']) && 
                                            $currentUser['user_id'] == $workflow['created_by_id']);
                                        
                                        if ($userCanEdit): 
                                        ?>
                                        <a href="WorkflowRuning.php?id=<?php echo $workflow['workflow_id'] ?? 0; ?>&edit=1" 
                                           class="text-yellow-600 hover:text-yellow-800">
                                            Edit
                                        </a>
                                        <?php endif; ?>
                                        <a href="ViewRuningHistory.php?id=<?php echo $workflow['workflow_id'] ?? 0; ?>" 
                                           class="text-gray-600 hover:text-gray-800">
                                            History
                                        </a>
                                        <a href="workflowlist.php?action=copy&id=<?php echo $workflow['workflow_id'] ?? 0; ?>" 
                                           class="text-blue-600 hover:text-blue-800"
                                           onclick="return confirm('Do you want to copy this workflow?')">
                                            Copy
                                        </a>
                                        <a href="exportscript.php?wfrowid=<?php echo $workflow['workflow_id'] ?? 0; ?>" 
                                           class="text-green-600 hover:text-green-800" target="_blank">
                                            Export WF
                                        </a>
                                        <a href="#" onclick="showApiView('<?php echo $workflow['workflow_id'] ?? 0; ?>')" 
                                           class="text-red-600 hover:text-red-800">
                                            View API
                                        </a>
                                    </div>
                                </div>
                            </td>
                            <td class="px-6 py-4">
                                <div class="flex flex-col space-y-2">
                                    <a href="WorkflowRuning.php?WFRowId=<?php echo $workflow['workflow_id'] ?? 0; ?>" 
                                       class="text-blue-700 font-bold hover:text-blue-900" target="_blank">
                                        Run
                                    </a>
                                    <div class="flex flex-wrap space-x-2">
                                        <a href="template.php?WFRowId=<?php echo $workflow['workflow_id'] ?? 0; ?>" 
                                           class="text-gray-700 hover:text-gray-900">
                                            Template
                                        </a>
                                        <span class="text-gray-500">|</span>
                                        <a href="Position.php?WFRowId=<?php echo $workflow['workflow_id'] ?? 0; ?>" 
                                           class="text-gray-700 hover:text-gray-900">
                                            Position & Email
                                        </a>
                                        <span class="text-gray-500">|</span>
                                        <a href="step-events.php?WFRowId=<?php echo $workflow['workflow_id'] ?? 0; ?>" 
                                           class="text-gray-700 hover:text-gray-900">
                                            Step & Events
                                        </a>
                                    </div>
                                </div>
                            </td>
                        </tr>
                        <?php endforeach; ?>
                    </tbody>
                </table>
            </div>
            <?php else: ?>
            <div class="bg-blue-50 border-l-4 border-blue-400 p-4 mb-4">
                <div class="flex">
                    <div class="flex-shrink-0">
                        <i class="fa fa-info-circle text-blue-400"></i>
                    </div>
                    <div class="ml-3">
                        <p class="text-sm text-blue-700">No workflows found.</p>
                    </div>
                </div>
            </div>
            <?php endif; ?>
        </div>
    </div>
</div>

<!-- API View Modal -->
<div id="apiViewModal" class="fixed inset-0 bg-gray-600 bg-opacity-50 hidden flex items-center justify-center z-50">
    <div class="bg-white rounded-lg shadow-lg w-11/12 md:w-3/4 lg:w-2/3 max-h-screen overflow-y-auto">
        <div class="px-6 py-4 bg-gray-50 border-b border-gray-200 flex justify-between items-center">
            <h5 class="text-xl font-semibold text-gray-800">API Information</h5>
            <button onclick="closeApiModal()" class="text-gray-500 hover:text-gray-800">
                <i class="fa fa-times"></i>
            </button>
        </div>
        <div class="p-6">
            <h3 class="font-bold text-lg mb-4 underline">Embedded Code สามารถเอาไปใช้ใน Guru ได้</h3>
            
            <h4 class="font-bold mt-4 mb-2">Form</h4>
            <textarea id="embedForm" class="w-full h-20 p-2 border border-gray-300 rounded-lg mb-2"></textarea>
            <a id="formLink" href="#" target="_blank" class="text-blue-600 hover:text-blue-800">Open Link</a>
            
            <h4 class="font-bold mt-4 mb-2">History >>> Option URL get parameter: distinct=fieldname, where fieldname=value</h4>
            <textarea id="embedHistory" class="w-full h-20 p-2 border border-gray-300 rounded-lg mb-2"></textarea>
            <a id="historyLink" href="#" target="_blank" class="text-blue-600 hover:text-blue-800">Open Link</a>
            
            <h4 class="font-bold mt-4 mb-2">Update email wording / Email position</h4>
            <textarea id="embedEmail" class="w-full h-20 p-2 border border-gray-300 rounded-lg mb-2"></textarea>
            <a id="emailLink" href="#" target="_blank" class="text-blue-600 hover:text-blue-800">Open Link</a>
            
            <h4 class="font-bold mt-4 mb-2">Auto Start Flow by URL Template For GET Method</h4>
            <textarea id="embedStart" class="w-full h-20 p-2 border border-gray-300 rounded-lg mb-2"></textarea>
            
            <h4 class="font-bold mt-4 mb-2">For POST Method</h4>
            <div id="postMethodInfo" class="p-4 bg-gray-100 rounded-lg mb-4"></div>
            
            <h4 class="font-bold mt-4 mb-2">For API JSON</h4>
            <div id="apiJsonInfo" class="p-4 bg-gray-100 rounded-lg"></div>
            
            <div class="mt-6">
                <button onclick="closeApiModal()" class="bg-gray-500 hover:bg-gray-600 text-white px-4 py-2 rounded-lg transition-colors duration-200">
                    Back
                </button>
            </div>
        </div>
    </div>
</div>

<!-- New Workflow Modal -->
<div id="newWorkflowModal" class="fixed inset-0 bg-gray-600 bg-opacity-50 hidden flex items-center justify-center z-50">
    <div class="bg-white rounded-lg shadow-lg w-11/12 md:w-3/4 lg:w-2/3 max-h-screen overflow-y-auto">
        <div class="px-6 py-4 bg-gray-50 border-b border-gray-200 flex justify-between items-center">
            <h5 class="text-xl font-semibold text-gray-800">Create New Workflow</h5>
            <button onclick="closeNewWorkflowModal()" class="text-gray-500 hover:text-gray-800">
                <i class="fa fa-times"></i>
            </button>
        </div>
        <div class="p-6">
            <form id="newWorkflowForm" method="post">
                <input type="hidden" name="action" value="save_workflow">
                
                <div class="grid grid-cols-1 md:grid-cols-2 gap-6 mb-6">
                    <div class="space-y-4">
                        <div>
                            <label for="workflow_name" class="block text-sm font-medium text-gray-700 mb-1">Workflow Name <span class="text-red-500">*</span></label>
                            <input type="text" id="workflow_name" name="workflow_name" 
                                   class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500" 
                                   required>
                        </div>
                        
                        <div>
                            <label for="sql_mail_profile" class="block text-sm font-medium text-gray-700 mb-1">SQL Mail Profile</label>
                            <select id="sql_mail_profile" name="sql_mail_profile" 
                                    class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500">
                                <option value="">-- Select Mail Profile --</option>
                                <?php if (is_array($sqlMailProfiles)): ?>
                                    <?php foreach ($sqlMailProfiles as $profile): ?>
                                    <option value="<?php echo htmlspecialchars($profile['system'] ?? ''); ?>">
                                        <?php echo htmlspecialchars($profile['name'] ?? $profile['system'] ?? ''); ?>
                                    </option>
                                    <?php endforeach; ?>
                                <?php endif; ?>
                                <?php if (empty($sqlMailProfiles)): ?>
                                    <!-- Default option when no profiles available -->
                                    <option value="WEBWF">WEBWF</option>
                                <?php endif; ?>
                            </select>
                            <p class="text-sm text-gray-500 mt-1">รายการมาจากระบบ Notification Service table : BST_NOTIFICATION.SystemAndMailProfile หากต้องการ Create profile ใหม่กรุณาติดต่อ CKP</p>
                        </div>
                        
                        <div>
                            <label for="sender_display" class="block text-sm font-medium text-gray-700 mb-1">Sender Email Display</label>
                            <input type="text" id="sender_display" name="sender_display" 
                                   class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500" 
                                   placeholder="BST Workflow <no-reply_webwf@bst.co.th>">
                            <p class="text-sm text-gray-500 mt-1">ตัวอย่าง : BST Workflow &lt;no-reply_webwf@bst.co.th&gt;</p>
                        </div>
                        
                        <div>
                            <label for="application_folder" class="block text-sm font-medium text-gray-700 mb-1">Customize Application Folder</label>
                            <input type="text" id="application_folder" name="application_folder" 
                                   class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500">
                            <p class="text-sm text-gray-500 mt-1">ระบุ customize application folder หากมีการ deverlop นอกเหนือ standard หากไม่มีไม่ต้องระบุ</p>
                        </div>
                    </div>
                    
                    <div class="space-y-4">
                        <div>
                            <label for="user_view" class="block text-sm font-medium text-gray-700 mb-1">User View</label>
                            <textarea id="user_view" name="user_view" 
                                      class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500" 
                                      rows="3"></textarea>
                            <p class="text-sm text-gray-500 mt-1"><span class="text-red-500">ไม่ระบุคือ ทุกคนสามารถ View</span>&nbsp;&nbsp; ตัวอย่างการระบุ Chakrapan_a;doltawat_s</p>
                        </div>
                        
                        <div>
                            <label for="user_edit" class="block text-sm font-medium text-gray-700 mb-1">User Edit</label>
                            <textarea id="user_edit" name="user_edit" 
                                      class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500" 
                                      rows="3"><?php echo htmlspecialchars($currentUsername); ?></textarea>
                            <p class="text-sm text-gray-500 mt-1"><span class="text-red-500">ไม่ระบุคือ ทุกคนสามารถ Edit</span>&nbsp;&nbsp; ตัวอย่างการระบุ Chakrapan_a;doltawat_s</p>
                        </div>
                        
                        <div>
                            <label for="user_special_action" class="block text-sm font-medium text-gray-700 mb-1">User Can Use Special Action History</label>
                            <textarea id="user_special_action" name="user_special_action" 
                                      class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500" 
                                      rows="3"><?php echo htmlspecialchars($currentUsername); ?></textarea>
                            <p class="text-sm text-gray-500 mt-1"><span class="text-red-500">ไม่ระบุคือ ทุกคนสามารถ</span>&nbsp;&nbsp; ตัวอย่างการระบุ Chakrapan_a;doltawat_s</p>
                        </div>
                        
                        <div>
                            <label for="default_domain" class="block text-sm font-medium text-gray-700 mb-1">Default Domain</label>
                            <input type="text" id="default_domain" name="default_domain" 
                                   class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500">
                        </div>
                    </div>
                </div>
                
                <div id="workflowFormError" class="bg-red-50 border-l-4 border-red-400 p-4 mb-4 hidden">
                    <div class="flex">
                        <div class="flex-shrink-0">
                            <i class="fa fa-exclamation-circle text-red-400"></i>
                        </div>
                        <div class="ml-3">
                            <p class="text-sm text-red-700" id="errorMessage"></p>
                        </div>
                    </div>
                </div>
                
                <div class="flex justify-between mt-8">
                    <div>
                        <button type="submit" class="bg-green-500 hover:bg-green-600 text-white px-6 py-2 rounded-lg transition-colors duration-200">
                            <i class="fa fa-save mr-2"></i> Create Workflow
                        </button>
                        <button type="button" onclick="closeNewWorkflowModal()" class="bg-gray-500 hover:bg-gray-600 text-white px-6 py-2 rounded-lg transition-colors duration-200 ml-2">
                            <i class="fa fa-times mr-2"></i> Cancel
                        </button>
                    </div>
                </div>
            </form>
        </div>
    </div>
</div>

<style>
    .toggle-checkbox:checked {
        right: 0;
        border-color: #68D391;
    }
    .toggle-checkbox:checked + .toggle-label {
        background-color: #68D391;
    }
</style>

<script>
    function showApiView(workflowId) {
        // Set the content of the modal
        const baseUrl = window.location.origin + '/WorkFlowEngineCore'; // Replace with your actual base URL
        
        document.getElementById('embedForm').value = `<iframe frameborder="no" height="1400" id="workflowiframe" name="workflowiframe" src="${baseUrl}/WorkflowRuning.php?id=${workflowId}" width="100%"></iframe>`;
        document.getElementById('formLink').href = `${baseUrl}/WorkflowRuning.php?id=${workflowId}`;
        
        document.getElementById('embedHistory').value = `<iframe frameborder="no" height="800" id="workflowiframe" name="workflowiframe" src="${baseUrl}/ViewRuningHistory.php?id=${workflowId}" width="100%"></iframe>`;
        document.getElementById('historyLink').href = `${baseUrl}/ViewRuningHistory.php?id=${workflowId}`;
        
        document.getElementById('embedEmail').value = `<iframe frameborder="no" height="800" id="workflowiframe" name="workflowiframe" src="${baseUrl}/updateemailwording.php?id=${workflowId}" width="100%"></iframe>`;
        document.getElementById('emailLink').href = `${baseUrl}/updateemailwording.php?id=${workflowId}`;
        
        document.getElementById('embedStart').value = `${baseUrl}/WorkflowRuning.php?id=${workflowId}&btnaction={?}&{objtemplatename}={?}`;
        
        document.getElementById('postMethodInfo').innerHTML = `
            <b>Action:</b> ${baseUrl}/WorkflowRuning.php?id=${workflowId}&btnaction={?}<br><br>
            <b>btnaction</b><br>
            Save<br>
            Approve<br>
            Reject<br><br>
            <b>objtemplatename</b><br>
            (Template names will be loaded from the database)
        `;
        
        document.getElementById('apiJsonInfo').innerHTML = `
            <b>For API JSON</b><br>
            ${baseUrl}/api/${workflowId}/{column}/{where}<br>
            ${baseUrl}/api/${workflowId}/{column}/{where}/{orderby}<br><br>
            {workflowid} Required<br>
            {column} Optional<br>
            {where} Optional<br>
            {orderby} Optional<br><br>
            
            <b>Example:</b><br>
            ${baseUrl}/api/${workflowId}
        `;
        
        // Show the modal
        document.getElementById('apiViewModal').classList.remove('hidden');
    }
    
    function closeApiModal() {
        document.getElementById('apiViewModal').classList.add('hidden');
    }
    
    function openNewWorkflowModal() {
        document.getElementById('newWorkflowModal').classList.remove('hidden');
    }
    
    function closeNewWorkflowModal() {
        document.getElementById('newWorkflowModal').classList.add('hidden');
        document.getElementById('newWorkflowForm').reset();
        document.getElementById('workflowFormError').classList.add('hidden');
    }
    
    document.getElementById('newWorkflowForm').addEventListener('submit', function(e) {
        e.preventDefault();
        
        const formData = new FormData(this);
        
        formData.append('created_by', '<?php echo $currentUser['user_id'] ?? 0; ?>');
        
        fetch('create_workflow.php', {
            method: 'POST',
            body: formData
        })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                window.location.href = 'WorkflowRuning.php?id=' + data.workflow_id + '&edit=1';
            } else {
                document.getElementById('errorMessage').textContent = data.message || 'Failed to create workflow';
                document.getElementById('workflowFormError').classList.remove('hidden');
            }
        })
        .catch(error => {
            console.error('Error:', error);
            document.getElementById('errorMessage').textContent = 'An error occurred while creating the workflow';
            document.getElementById('workflowFormError').classList.remove('hidden');
        });
    });
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