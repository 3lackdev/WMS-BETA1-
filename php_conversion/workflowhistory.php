<?php
require_once 'includes/Database.php';
require_once 'includes/Authentication.php';

// Initialize authentication
$auth = new Authentication();

// Check if user is authenticated
if (!$auth->isAuthenticated()) {
    // Redirect to login page if not authenticated
    header("Location: login.php");
    exit;
}

// Initialize database
$db = new Database();

// Get filter parameters
$startDate = isset($_GET['start_date']) ? $_GET['start_date'] : date('Y-m-d', strtotime('-30 days'));
$endDate = isset($_GET['end_date']) ? $_GET['end_date'] : date('Y-m-d');
$search = isset($_GET['search']) ? $_GET['search'] : '';

// Build filter condition
$filter = "created_date BETWEEN '" . $db->escapeString($startDate) . " 00:00:00' AND '" . $db->escapeString($endDate) . " 23:59:59'";

if (!empty($search)) {
    $filter .= " AND (workflow_name LIKE '%" . $db->escapeString($search) . "%' OR created_by_name LIKE '%" . $db->escapeString($search) . "%')";
}

// Get workflow history - match original table case from ASP.NET
try {
    $query = "SELECT wh.*, w.workflow_name, u.username as created_by_name, s.status_name 
            FROM WorkFlowHistory wh 
            JOIN WorkFlow w ON wh.workflow_id = w.workflow_id 
            JOIN users u ON wh.created_by = u.user_id 
            JOIN WorkFlowStatus s ON wh.status_id = s.status_id 
            WHERE $filter 
            ORDER BY wh.created_date DESC";
    
    $history = $db->executeQuery($query);
} catch (Exception $e) {
    // Try alternative table cases if the first query fails
    try {
        $query = "SELECT wh.*, w.workflow_name, u.username as created_by_name, s.status_name 
                FROM workflowhistory wh 
                JOIN workflow w ON wh.workflow_id = w.workflow_id 
                JOIN users u ON wh.created_by = u.user_id 
                JOIN workflowstatus s ON wh.status_id = s.status_id 
                WHERE $filter 
                ORDER BY wh.created_date DESC";
        
        $history = $db->executeQuery($query);
    } catch (Exception $e2) {
        // If both attempts fail, provide an empty array
        $history = [];
        // Log the error for debugging
        error_log("Database error in workflowhistory.php: " . $e->getMessage() . " / " . $e2->getMessage());
    }
}

// Set page title and header
$pageTitle = "Workflow History - WorkFlow Engine";
$pageHeader = "Workflow Run History";
$pageNavigation = "<a href=\"index.php\" class=\"text-blue-600 hover:text-blue-800\">หน้าแรกโปรแกรม</a>";

// Additional CSS and JS
$additionalCss = ['DatePicker/datepicker.min.css'];
$additionalJs = ['DatePicker/datepicker.min.js'];

// Include header
include 'includes/header.php';
?>

<div class="w-full max-w-7xl mx-auto">
    <div class="bg-white shadow-md rounded-lg overflow-hidden mb-6">
        <div class="px-6 py-4 bg-gray-50 border-b border-gray-200">
            <h5 class="text-xl font-semibold text-gray-800">Workflow Run History</h5>
        </div>
        <div class="p-6">
            <!-- Filter Form -->
            <form method="get" action="workflowhistory.php" class="mb-6">
                <div class="flex flex-col md:flex-row space-y-4 md:space-y-0 md:space-x-4">
                    <div class="md:w-1/4">
                        <label class="block text-sm font-medium text-gray-700 mb-1">Start Date</label>
                        <input type="date" class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500" 
                               name="start_date" value="<?php echo htmlspecialchars($startDate); ?>">
                    </div>
                    <div class="md:w-1/4">
                        <label class="block text-sm font-medium text-gray-700 mb-1">End Date</label>
                        <input type="date" class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500" 
                               name="end_date" value="<?php echo htmlspecialchars($endDate); ?>">
                    </div>
                    <div class="md:w-1/3">
                        <label class="block text-sm font-medium text-gray-700 mb-1">Search</label>
                        <input type="text" class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500" 
                               name="search" value="<?php echo htmlspecialchars($search); ?>" 
                               placeholder="Search workflow name or creator">
                    </div>
                    <div class="md:w-1/6 flex items-end">
                        <button type="submit" class="w-full bg-blue-500 hover:bg-blue-600 text-white px-4 py-2 rounded-lg transition-colors duration-200 flex items-center justify-center">
                            <i class="fa fa-search mr-2"></i> Filter
                        </button>
                    </div>
                </div>
            </form>
            
            <!-- History Table -->
            <?php if (is_array($history) && count($history) > 0): ?>
            <div class="overflow-x-auto">
                <table class="w-full table-auto">
                    <thead class="bg-gray-50 border-b border-gray-200">
                        <tr>
                            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Workflow Name</th>
                            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Run Date</th>
                            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Created By</th>
                            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Status</th>
                            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Actions</th>
                        </tr>
                    </thead>
                    <tbody class="bg-white divide-y divide-gray-200">
                        <?php foreach ($history as $item): ?>
                        <tr class="hover:bg-gray-50">
                            <td class="px-6 py-4 whitespace-nowrap"><?php echo htmlspecialchars($item['workflow_name'] ?? 'N/A'); ?></td>
                            <td class="px-6 py-4 whitespace-nowrap"><?php echo htmlspecialchars($item['created_date'] ?? 'N/A'); ?></td>
                            <td class="px-6 py-4 whitespace-nowrap"><?php echo htmlspecialchars($item['created_by_name'] ?? 'N/A'); ?></td>
                            <td class="px-6 py-4 whitespace-nowrap">
                                <span class="px-2 py-1 text-xs font-semibold rounded-full <?php echo getTailwindStatusClass($item['status_name'] ?? ''); ?>">
                                    <?php echo htmlspecialchars($item['status_name'] ?? 'N/A'); ?>
                                </span>
                            </td>
                            <td class="px-6 py-4 whitespace-nowrap">
                                <div class="flex space-x-2">
                                    <a href="ViewHistoryDetail.php?id=<?php echo $item['history_id'] ?? 0; ?>" 
                                       class="bg-blue-500 hover:bg-blue-600 text-white p-2 rounded-md transition-colors duration-200" 
                                       data-toggle="tooltip" title="View History Details">
                                        <i class="fa fa-eye"></i>
                                    </a>
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
                        <p class="text-sm text-blue-700">No workflow history found for the selected period.</p>
                    </div>
                </div>
            </div>
            <?php endif; ?>
        </div>
    </div>
</div>

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

// Add datepicker initialization
$pageScript = <<<JS
$(document).ready(function() {
    // Initialize datepickers
    $('.datepicker').datepicker({
        format: 'yyyy-mm-dd',
        autoclose: true,
        todayHighlight: true
    });
});
JS;

// Include footer
include 'includes/footer.php';
?> 