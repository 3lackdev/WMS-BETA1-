<?php
require_once 'includes/Database.php';
require_once 'includes/Authentication.php';
require_once 'includes/WorkflowHelper.php';

// Initialize authentication
$auth = new Authentication();

// Set page title and navigation
$pageTitle = "หน้าแรก - WorkFlow Engine";
$pageHeader = "Dashboard";

// Get navigation text based on user status
if ($auth->isAuthenticated()) {
    $pageNavigation = "<a href=\"#\" class=\"text-blue-600 hover:text-blue-800\">หน้าแรกโปรแกรม</a>";
} else {
    $pageNavigation = "<a href=\"Login.php\" class=\"text-blue-600 hover:text-blue-800\">เข้าสู่ระบบ</a>";
}

// Include header
include 'includes/header.php';
?>

<div class="w-full max-w-7xl mx-auto">
    <!-- Transaction Menu Section -->
    <div class="mb-8">
        <div class="bg-white shadow-md rounded-lg overflow-hidden">
            <div class="px-6 py-4 bg-gray-50 border-b border-gray-200">
                <h3 class="text-xl font-semibold text-gray-800">Transaction Menu</h3>
            </div>
            <div class="p-6">
                <div class="flex flex-wrap justify-center">
                    <!-- Workflow History -->
                    <a href="workflowhistory.php" class="dashboard-module transition-transform hover:scale-105">
                        <div class="w-16 h-16 mx-auto mb-2 flex items-center justify-center bg-blue-100 text-blue-500 rounded-full">
                            <i class="fa fa-history fa-2x"></i>
                        </div>
                        <span class="text-gray-700">Workflow Run History</span>
                    </a>

                    <!-- Workflow List -->
                    <a href="workflowlist.php" class="dashboard-module transition-transform hover:scale-105">
                        <div class="w-16 h-16 mx-auto mb-2 flex items-center justify-center bg-green-100 text-green-500 rounded-full">
                            <i class="fa fa-list fa-2x"></i>
                        </div>
                        <span class="text-gray-700">Workflow Creation</span>
                    </a>
                </div>
            </div>
        </div>
    </div>

    <!-- Master Data Menu Section -->
    <div class="mb-8">
        <div class="bg-white shadow-md rounded-lg overflow-hidden">
            <div class="px-6 py-4 bg-gray-50 border-b border-gray-200">
                <h3 class="text-xl font-semibold text-gray-800">Master Data</h3>
            </div>
            <div class="p-6">
                <div class="flex flex-wrap justify-center">
                    <!-- Data Binding -->
                    <a href="Databind.php" class="dashboard-module transition-transform hover:scale-105">
                        <div class="w-16 h-16 mx-auto mb-2 flex items-center justify-center bg-purple-100 text-purple-500 rounded-full">
                            <i class="fa fa-database fa-2x"></i>
                        </div>
                        <span class="text-gray-700">Data Binding</span>
                    </a>
                </div>
            </div>
        </div>
    </div>
</div>

<?php
// Include footer
include 'includes/footer.php';
?> 