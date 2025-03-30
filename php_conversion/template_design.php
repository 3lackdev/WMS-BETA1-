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

// Check if ID parameter exists
if (!isset($_GET['TemplateRowId'])) {
    header("Location: template.php");
    exit;
}

$templateId = $_GET['TemplateRowId'];
$title = "Template Design";
$successMessage = "";
$errorMessage = "";

// Initialize database and helper
$db = new Database();
$helper = new WorkflowHelper($db);

// Get current user
$currentUser = $auth->getCurrentUser();
$currentUsername = $currentUser['username'] ?? '';

// Function to generate UUID
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

function custom_bin2hex($string) {
    $hex = '';
    for ($i = 0; $i < strlen($string); $i++) {
        $ord = ord($string[$i]);
        $hexCode = dechex($ord);
        $hex .= substr('0'.$hexCode, -2);
    }
    return $hex;
}

// Handle save design action via AJAX
if ($_SERVER['REQUEST_METHOD'] === 'POST' && isset($_POST['action']) && $_POST['action'] === 'save_design') {
    // Process the AJAX request to save design
    header('Content-Type: application/json');
    
    // Validate and sanitize input
    $designData = isset($_POST['design_data']) ? $_POST['design_data'] : '';
    
    if (empty($designData)) {
        echo json_encode(['success' => false, 'error' => 'No design data provided']);
        exit;
    }
    
    // Save design data to the database
    try {
        $updateSql = "UPDATE WorkFlowTemplate SET DesignData = '" . $db->escapeString($designData) . "' WHERE TemplateRowId = '" . $db->escapeString($templateId) . "'";
        $result = $db->executeNonQuery($updateSql);
        
        if ($result) {
            echo json_encode(['success' => true]);
        } else {
            echo json_encode(['success' => false, 'error' => 'Failed to save design']);
        }
    } catch (Exception $e) {
        echo json_encode(['success' => false, 'error' => $e->getMessage()]);
    }
    
    exit;
}

// Handle get design data via AJAX
if ($_SERVER['REQUEST_METHOD'] === 'GET' && isset($_GET['action']) && $_GET['action'] === 'get_design') {
    // Set content type to JSON for AJAX response
    header('Content-Type: application/json');
    
    try {
        // Get the template details from the database
        $sql = "SELECT DesignData FROM WorkFlowTemplate WHERE TemplateRowId = '" . $db->escapeString($templateId) . "'";
        $result = $db->executeQuery($sql);
        
        if (count($result) > 0 && isset($result[0]['DesignData'])) {
            $designData = $result[0]['DesignData'];
            echo json_encode(['success' => true, 'designData' => $designData]);
        } else {
            echo json_encode(['success' => false, 'error' => 'Template not found or has no design data']);
        }
        exit;
    } catch (Exception $e) {
        echo json_encode(['success' => false, 'error' => $e->getMessage()]);
        exit;
    }
}

// Get template details
try {
    $sql = "SELECT * FROM WorkFlowTemplate WHERE TemplateRowId = '" . $db->escapeString($templateId) . "'";
    $result = $db->executeQuery($sql);
    
    if (count($result) > 0) {
        $template = $result[0];
        $title = "Design Template: " . htmlspecialchars($template['TemplateName']);
    } else {
        $errorMessage = "Template not found";
    }
} catch (Exception $e) {
    $errorMessage = "Database error: " . $e->getMessage();
}

// Get template objects
try {
    $sql = "SELECT * FROM TemplateObject WHERE TemplateRowId = '" . $db->escapeString($templateId) . "'";
    $objects = $db->executeQuery($sql);
} catch (Exception $e) {
    $errorMessage = "Error loading objects: " . $e->getMessage();
    $objects = [];
}

include 'includes/header.php';
?>

<!-- Breadcrumb navigation -->
<nav class="flex mb-4" aria-label="Breadcrumb">
    <ol class="inline-flex items-center space-x-1 md:space-x-3">
        <li class="inline-flex items-center">
            <a href="index.php" class="text-gray-700 hover:text-blue-600">
                <svg class="w-5 h-5 mr-2.5" fill="currentColor" viewBox="0 0 20 20" xmlns="http://www.w3.org/2000/svg"><path d="M10.707 2.293a1 1 0 00-1.414 0l-7 7a1 1 0 001.414 1.414L4 10.414V17a1 1 0 001 1h2a1 1 0 001-1v-2a1 1 0 011-1h2a1 1 0 011 1v2a1 1 0 001 1h2a1 1 0 001-1v-6.586l.293.293a1 1 0 001.414-1.414l-7-7z"></path></svg>
                Home
            </a>
        </li>
        <li>
            <div class="flex items-center">
                <svg class="w-6 h-6 text-gray-400" fill="currentColor" viewBox="0 0 20 20" xmlns="http://www.w3.org/2000/svg"><path fill-rule="evenodd" d="M7.293 14.707a1 1 0 010-1.414L10.586 10 7.293 6.707a1 1 0 011.414-1.414l4 4a1 1 0 010 1.414l-4 4a1 1 0 01-1.414 0z" clip-rule="evenodd"></path></svg>
                <a href="template.php" class="ml-1 text-gray-700 hover:text-blue-600 md:ml-2">Templates</a>
            </div>
        </li>
        <li>
            <div class="flex items-center">
                <svg class="w-6 h-6 text-gray-400" fill="currentColor" viewBox="0 0 20 20" xmlns="http://www.w3.org/2000/svg"><path fill-rule="evenodd" d="M7.293 14.707a1 1 0 010-1.414L10.586 10 7.293 6.707a1 1 0 011.414-1.414l4 4a1 1 0 010 1.414l-4 4a1 1 0 01-1.414 0z" clip-rule="evenodd"></path></svg>
                <a href="template_objects.php?TemplateRowId=<?php echo $templateId; ?>" class="ml-1 text-gray-700 hover:text-blue-600 md:ml-2">Objects</a>
            </div>
        </li>
        <li aria-current="page">
            <div class="flex items-center">
                <svg class="w-6 h-6 text-gray-400" fill="currentColor" viewBox="0 0 20 20" xmlns="http://www.w3.org/2000/svg"><path fill-rule="evenodd" d="M7.293 14.707a1 1 0 010-1.414L10.586 10 7.293 6.707a1 1 0 011.414-1.414l4 4a1 1 0 010 1.414l-4 4a1 1 0 01-1.414 0z" clip-rule="evenodd"></path></svg>
                <span class="ml-1 text-gray-500 md:ml-2 font-medium">Design</span>
            </div>
        </li>
    </ol>
</nav>

<div class="bg-white p-4 rounded-lg shadow">
    <h1 class="text-2xl font-bold text-gray-800 mb-6"><?php echo $title; ?></h1>
    
    <!-- Error/Success Messages -->
    <?php if (!empty($errorMessage)): ?>
        <div class="bg-red-100 border-l-4 border-red-500 text-red-700 p-4 mb-4" role="alert">
            <p><?php echo $errorMessage; ?></p>
        </div>
    <?php endif; ?>
    
    <?php if (!empty($successMessage)): ?>
        <div class="bg-green-100 border-l-4 border-green-500 text-green-700 p-4 mb-4" role="alert">
            <p><?php echo $successMessage; ?></p>
        </div>
    <?php endif; ?>
</div>

<!-- Notification -->
<div id="notification" class="fixed top-4 right-4 px-4 py-2 rounded-md text-white hidden transition-all duration-300 z-50"></div>

<!-- Properties Modal -->
<div id="propertiesModal" class="fixed inset-0 bg-gray-600 bg-opacity-50 flex items-center justify-center hidden z-50">
    <div class="bg-white rounded-lg shadow-lg w-full max-w-md">
        <div class="px-6 py-4 border-b">
            <h3 class="text-lg font-semibold text-gray-800">Object Properties</h3>
        </div>
        <div class="px-6 py-4 space-y-4">
            <div>
                <label class="block text-sm font-medium text-gray-700">Object Name</label>
                <p id="modalObjectName" class="mt-1 text-sm text-gray-900"></p>
            </div>
            <div>
                <label for="modalObjectLabel" class="block text-sm font-medium text-gray-700">Label</label>
                <input type="text" id="modalObjectLabel" class="mt-1 focus:ring-blue-500 focus:border-blue-500 block w-full shadow-sm sm:text-sm border-gray-300 rounded-md">
            </div>
            <div id="textStyleOptions" class="space-y-4 hidden">
                <div>
                    <label for="modalTextSize" class="block text-sm font-medium text-gray-700">Text Size</label>
                    <select id="modalTextSize" class="mt-1 block w-full pl-3 pr-10 py-2 text-base border-gray-300 focus:outline-none focus:ring-blue-500 focus:border-blue-500 sm:text-sm rounded-md">
                        <option value="xs">Extra Small</option>
                        <option value="sm">Small</option>
                        <option value="base" selected>Normal</option>
                        <option value="lg">Large</option>
                        <option value="xl">Extra Large</option>
                        <option value="2xl">2X Large</option>
                        <option value="3xl">3X Large</option>
                    </select>
                </div>
                <div>
                    <label for="modalTextStyle" class="block text-sm font-medium text-gray-700">Text Style</label>
                    <select id="modalTextStyle" class="mt-1 block w-full pl-3 pr-10 py-2 text-base border-gray-300 focus:outline-none focus:ring-blue-500 focus:border-blue-500 sm:text-sm rounded-md">
                        <option value="normal">Normal</option>
                        <option value="bold">Bold</option>
                        <option value="italic">Italic</option>
                        <option value="boldItalic">Bold Italic</option>
                        <option value="underline">Underlined</option>
                    </select>
                </div>
                <div>
                    <label for="modalTextAlign" class="block text-sm font-medium text-gray-700">Alignment</label>
                    <select id="modalTextAlign" class="mt-1 block w-full pl-3 pr-10 py-2 text-base border-gray-300 focus:outline-none focus:ring-blue-500 focus:border-blue-500 sm:text-sm rounded-md">
                        <option value="left">Left</option>
                        <option value="center">Center</option>
                        <option value="right">Right</option>
                    </select>
                </div>
                <div>
                    <label for="modalTextColor" class="block text-sm font-medium text-gray-700">Text Color</label>
                    <select id="modalTextColor" class="mt-1 block w-full pl-3 pr-10 py-2 text-base border-gray-300 focus:outline-none focus:ring-blue-500 focus:border-blue-500 sm:text-sm rounded-md">
                        <option value="gray-900">Black</option>
                        <option value="gray-600">Dark Gray</option>
                        <option value="gray-500">Gray</option>
                        <option value="blue-600">Blue</option>
                        <option value="green-600">Green</option>
                        <option value="red-600">Red</option>
                        <option value="purple-600">Purple</option>
                    </select>
                </div>
            </div>
            <div>
                <label for="modalObjectWidth" class="block text-sm font-medium text-gray-700">Width</label>
                <select id="modalObjectWidth" class="mt-1 block w-full pl-3 pr-10 py-2 text-base border-gray-300 focus:outline-none focus:ring-blue-500 focus:border-blue-500 sm:text-sm rounded-md">
                    <option value="1">1/2 Column</option>
                    <option value="2">Full Width</option>
                </select>
            </div>
            <div>
                <label for="modalObjectRequired" class="flex items-center">
                    <input type="checkbox" id="modalObjectRequired" class="focus:ring-blue-500 h-4 w-4 text-blue-600 border-gray-300 rounded">
                    <span class="ml-2 text-sm text-gray-700">Required</span>
                </label>
            </div>
        </div>
        <div class="px-6 py-4 border-t bg-gray-50 flex justify-end space-x-3">
            <button type="button" id="removeObjectBtn" class="bg-red-500 py-2 px-4 border border-transparent rounded-md shadow-sm text-sm font-medium text-white hover:bg-red-600 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-red-500">
                Remove
            </button>
            <button type="button" id="cancelPropertiesBtn" class="bg-white py-2 px-4 border border-gray-300 rounded-md shadow-sm text-sm font-medium text-gray-700 hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500">
                Cancel
            </button>
            <button type="button" id="savePropertiesBtn" class="bg-blue-500 py-2 px-4 border border-transparent rounded-md shadow-sm text-sm font-medium text-white hover:bg-blue-600 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500">
                Apply
            </button>
        </div>
    </div>
</div>

<!-- Form Element Settings Modal -->
<div id="formElementModal" class="fixed inset-0 bg-gray-600 bg-opacity-50 flex items-center justify-center hidden z-50">
    <div class="bg-white rounded-lg shadow-lg w-full max-w-md">
        <div class="px-6 py-4 border-b">
            <h3 id="formElementModalTitle" class="text-lg font-semibold text-gray-800">เพิ่ม Form Element</h3>
        </div>
        <div class="px-6 py-4 space-y-4">
            <input type="hidden" id="formElementType">
            
            <div>
                <label for="elementName" class="block text-sm font-medium text-gray-700">Name</label>
                <input type="text" id="elementName" class="mt-1 focus:ring-blue-500 focus:border-blue-500 block w-full shadow-sm sm:text-sm border-gray-300 rounded-md">
                <p id="nameError" class="text-red-500 text-xs mt-1 hidden">Name นี้ถูกใช้งานแล้ว กรุณาเลือกชื่ออื่น</p>
            </div>
            
            <div>
                <label for="elementLabel" class="block text-sm font-medium text-gray-700">Label</label>
                <input type="text" id="elementLabel" class="mt-1 focus:ring-blue-500 focus:border-blue-500 block w-full shadow-sm sm:text-sm border-gray-300 rounded-md">
            </div>
            
            <div id="placeholderContainer">
                <label for="elementPlaceholder" class="block text-sm font-medium text-gray-700">Placeholder</label>
                <input type="text" id="elementPlaceholder" class="mt-1 focus:ring-blue-500 focus:border-blue-500 block w-full shadow-sm sm:text-sm border-gray-300 rounded-md">
            </div>
        </div>
        <div class="px-6 py-4 border-t bg-gray-50 flex justify-end space-x-3">
            <button type="button" id="cancelElementBtn" class="bg-white py-2 px-4 border border-gray-300 rounded-md shadow-sm text-sm font-medium text-gray-700 hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500">
                ยกเลิก
            </button>
            <button type="button" id="addElementBtn" class="bg-blue-500 py-2 px-4 border border-transparent rounded-md shadow-sm text-sm font-medium text-white hover:bg-blue-600 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500">
                เพิ่ม
            </button>
        </div>
    </div>
</div>

<style>
    /* Design Element Styles */
    .design-element {
        cursor: move;
        position: absolute;
        min-width: 100px;
        min-height: 50px;
        background: white;
        border: 1px solid #e5e7eb;
        border-radius: 0.25rem;
        box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
        transition: box-shadow 0.2s, transform 0.1s;
        overflow: hidden;
        display: flex;
        flex-direction: column;
    }
    
    .design-element:hover {
        box-shadow: 0 0 0 2px rgba(59, 130, 246, 0.5);
        z-index: 10;
    }
    
    .design-element.dragging {
        opacity: 0.8;
        box-shadow: 0 4px 8px rgba(0, 0, 0, 0.2);
        z-index: 100;
    }
    
    .element-header {
        background: #f3f4f6;
        color: #1f2937;
        padding: 0.25rem 0.5rem;
        font-size: 0.75rem;
        border-bottom: 1px solid #e5e7eb;
        display: flex;
        align-items: center;
        cursor: move;
    }
    
    .element-name {
        font-weight: 500;
        margin-right: 0.5rem;
        white-space: nowrap;
        overflow: hidden;
        text-overflow: ellipsis;
        flex: 1;
    }
    
    .element-type {
        color: #6b7280;
        font-size: 0.6875rem;
        background: #e5e7eb;
        padding: 0.125rem 0.25rem;
        border-radius: 0.125rem;
        margin-right: 0.5rem;
    }
    
    .element-actions {
        display: flex;
        gap: 0.25rem;
    }
    
    .element-actions button {
        background: none;
        border: none;
        color: #6b7280;
        cursor: pointer;
        padding: 0.125rem;
        border-radius: 0.125rem;
    }
    
    .element-actions button:hover {
        color: #1f2937;
        background: #e5e7eb;
    }
    
    .element-content {
        padding: 0.5rem;
        flex: 1;
    }
    
    /* Design Area Styles */
    #designArea {
        overflow: hidden;
        min-height: 600px;
        border: 2px dashed #d1d5db;
        border-radius: 0.375rem;
        background-color: #f9fafb;
        padding: 1rem;
        position: relative;
        transition: border-color 0.3s, box-shadow 0.3s, background-color 0.3s;
    }
    
    #designArea.drop-target-active {
        border-color: #93c5fd;
        box-shadow: 0 0 0 3px rgba(147, 197, 253, 0.5);
    }
    
    #designArea.drop-target-hover {
        border-color: #3b82f6;
        border-style: solid;
        box-shadow: 0 0 0 3px rgba(59, 130, 246, 0.5);
        background-color: #eff6ff;
    }
    
    /* Drop indicator styles */
    .drop-indicator {
        position: absolute;
        background-color: #3b82f6;
        color: white;
        padding: 0.5rem 1rem;
        border-radius: 9999px;
        font-size: 0.875rem;
        font-weight: 500;
        pointer-events: none;
        z-index: 50;
        transform: translateY(-100%);
        box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
        animation: pulse 1.5s infinite;
    }
    
    .drop-indicator::after {
        content: '';
        position: absolute;
        bottom: -6px;
        left: 50%;
        transform: translateX(-50%);
        width: 0;
        height: 0;
        border-left: 6px solid transparent;
        border-right: 6px solid transparent;
        border-top: 6px solid #3b82f6;
    }
    
    .hidden {
        display: none;
    }
    
    /* Resize Handle */
    .resize-handle {
        position: absolute;
        width: 8px;
        height: 8px;
        background-color: #3b82f6;
        border: 1px solid white;
        border-radius: 4px;
        z-index: 20;
    }
    
    .resize-n { top: -4px; left: 50%; transform: translateX(-50%); cursor: ns-resize; }
    .resize-e { top: 50%; right: -4px; transform: translateY(-50%); cursor: ew-resize; }
    .resize-s { bottom: -4px; left: 50%; transform: translateX(-50%); cursor: ns-resize; }
    .resize-w { top: 50%; left: -4px; transform: translateY(-50%); cursor: ew-resize; }
    .resize-ne { top: -4px; right: -4px; cursor: nesw-resize; }
    .resize-se { bottom: -4px; right: -4px; cursor: nwse-resize; }
    .resize-sw { bottom: -4px; left: -4px; cursor: nesw-resize; }
    .resize-nw { top: -4px; left: -4px; cursor: nwse-resize; }
    
    /* Object Item Styles */
    .object-item {
        cursor: grab;
        transition: transform 0.2s, box-shadow 0.2s, opacity 0.2s;
        will-change: transform;
    }
    
    .object-item:hover {
        box-shadow: 0 2px 6px rgba(0,0,0,0.15);
        transform: translateY(-2px);
    }
    
    .object-item:active {
        cursor: grabbing;
        transform: translateY(0) scale(0.98);
    }
    
    .object-item[draggable="true"] {
        -webkit-user-select: none;
        user-select: none;
    }
    
    .object-item-dragging {
        opacity: 0.6;
        transform: scale(0.95);
        box-shadow: 0 3px 8px rgba(0,0,0,0.2);
    }
    
    /* Context Menu Styles */
    .context-menu {
        position: absolute;
        background: white;
        border: 1px solid #e5e7eb;
        border-radius: 0.25rem;
        box-shadow: 0 4px 6px rgba(0,0,0,0.1), 0 1px 3px rgba(0,0,0,0.08);
        min-width: 160px;
        z-index: 1000;
        overflow: hidden;
    }
    
    .context-menu-item {
        padding: 0.5rem 0.75rem;
        font-size: 0.875rem;
        color: #1f2937;
        cursor: pointer;
        display: flex;
        align-items: center;
    }
    
    .context-menu-item:hover {
        background-color: #f3f4f6;
    }
    
    .context-menu-item i {
        margin-right: 0.5rem;
        width: 1rem;
        text-align: center;
    }
    
    .context-menu-divider {
        height: 1px;
        background-color: #e5e7eb;
        margin: 0.25rem 0;
    }
    
    /* Animation for drag and drop */
    @keyframes pulse {
        0% { box-shadow: 0 0 0 0 rgba(59, 130, 246, 0.3); }
        70% { box-shadow: 0 0 0 8px rgba(59, 130, 246, 0); }
        100% { box-shadow: 0 0 0 0 rgba(59, 130, 246, 0); }
    }
    
    /* Empty design message */
    #emptyDesignMessage {
        position: absolute;
        top: 50%;
        left: 50%;
        transform: translate(-50%, -50%);
        width: 100%;
    }
    
    /* Form elements styling */
    .input-label {
        font-size: 0.875rem;
        font-weight: 500;
        color: #374151;
        margin-bottom: 0.25rem;
        display: block;
    }
    
    .form-input, .form-textarea, .form-select {
        width: 100%;
        padding: 0.5rem;
        border: 1px solid #d1d5db;
        border-radius: 0.25rem;
        background-color: white;
        font-size: 0.875rem;
    }
    
    .form-textarea {
        min-height: 4rem;
        resize: none;
    }
    
    /* Clone created during drag */
    .dragging-clone {
        position: fixed;
        pointer-events: none;
        z-index: 9999;
        opacity: 0.7;
        transform-origin: top left;
        box-shadow: 0 5px 10px rgba(0,0,0,0.15);
    }
    
    .form-text {
        margin-top: 0.25rem;
        font-size: 0.875em;
        color: #6c757d;
    }
    
    /* Rating stars styling */
    .rating-stars {
        display: inline-flex;
        font-size: 24px;
        cursor: pointer;
    }
    
    .rating-stars .star {
        color: #ccc;
        transition: color 0.2s ease;
        padding: 0 5px;
    }
    
    .rating-stars .star:hover,
    .rating-stars .star:hover ~ .star {
        color: #ffc107;
    }
    
    .rating-stars .star.selected {
        color: #ffc107;
    }
    
    /* Signature pad styling */
    .signature-pad {
        border: 1px dashed #ced4da;
        border-radius: 0.25rem;
        cursor: crosshair;
        transition: background-color 0.2s ease;
    }
    
    .signature-pad:hover {
        background-color: #f0f0f0 !important;
    }
    
    /* Layout utilities */
    .row {
        display: flex;
        flex-wrap: wrap;
        margin-right: -0.5rem;
        margin-left: -0.5rem;
    }
    
    .col-12, .col-md-6, .col-md-4, .col-md-2 {
        position: relative;
        padding-right: 0.5rem;
        padding-left: 0.5rem;
        width: 100%;
    }
    
    @media (min-width: 768px) {
        .col-md-6 { width: 50%; }
        .col-md-4 { width: 33.33333%; }
        .col-md-2 { width: 16.66667%; }
    }
    
    .g-3 > * {
        margin-bottom: 1rem;
    }
    
    /* Form section styling */
    .form-section-heading {
        font-size: 1.25rem;
        font-weight: 500;
        color: #212529;
        margin-bottom: 0.5rem;
    }
    
    /* Button styling */
    .btn {
        display: inline-block;
        font-weight: 400;
        text-align: center;
        white-space: nowrap;
        vertical-align: middle;
        user-select: none;
        border: 1px solid transparent;
        padding: 0.375rem 0.75rem;
        font-size: 1rem;
        line-height: 1.5;
        border-radius: 0.25rem;
        transition: all 0.15s ease-in-out;
    }
    
    .btn-sm {
        padding: 0.25rem 0.5rem;
        font-size: 0.875rem;
        line-height: 1.5;
        border-radius: 0.2rem;
    }
    
    .btn-outline-secondary {
        color: #6c757d;
        background-color: transparent;
        border-color: #6c757d;
    }
    
    .btn-outline-secondary:hover {
        color: #fff;
        background-color: #6c757d;
        border-color: #6c757d;
    }
    
    /* Form Builder Custom Style */
    .form-builder-toolbar {
        margin-bottom: 15px;
    }
    
    .form-element-preview {
        border: 1px dashed #ccc;
        padding: 10px;
        margin-bottom: 10px;
        border-radius: 4px;
        background: #f8f9fa;
    }
    
    .form-element-handler {
        cursor: move;
        padding: 5px;
        margin-bottom: 5px;
        background: #f0f0f0;
        border-radius: 3px;
        display: flex;
        justify-content: space-between;
        align-items: center;
    }
    
    .form-element-delete {
        cursor: pointer;
        color: #dc3545;
    }
    
    /* Summernote customizations */
    .note-editor {
        border: 1px solid #ced4da;
        border-radius: 0.25rem;
    }
    
    .note-editor .note-toolbar {
        background-color: #f8f9fa;
        border-bottom: 1px solid #ced4da;
        padding: 5px;
    }
    
    .note-editor .dropdown-toggle::after {
        display: none;
    }
    
    .note-editor .note-btn-group {
        margin: 2px;
    }
    
    .note-editor .note-btn {
        border-color: #ced4da;
        background-color: #fff;
        padding: 5px 10px;
    }
    
    .note-editor .note-btn:hover {
        background-color: #e9ecef;
    }
    
    /* Form element buttons styling */
    .note-btn-form-text i,
    .note-btn-form-email i {
        color: #007bff;
    }
    
    .note-btn-form-textarea i {
        color: #6610f2;
    }
    
    .note-btn-form-select i,
    .note-btn-form-radio i,
    .note-btn-form-checkbox i {
        color: #fd7e14;
    }
    
    .note-btn-form-date i {
        color: #e83e8c;
    }
    
    .note-btn-form-file i {
        color: #20c997;
    }
    
    /* Add a separator for form elements group */
    .note-toolbar .note-btn-group.note-formElements {
        border-left: 1px solid #ced4da;
        padding-left: 5px;
        margin-left: 5px;
    }
    
    /* Tooltip styling for form buttons */
    .note-tooltip {
        position: absolute;
        z-index: 1070;
        display: block;
        font-size: 13px;
        transition: opacity 0.3s;
    }
    
    /* Form elements styling */
    .form-control, .form-select {
        width: 100%;
        padding: 0.375rem 0.75rem;
        font-size: 1rem;
        line-height: 1.5;
        color: #212529;
        background-color: #fff;
        border: 1px solid #ced4da;
        border-radius: 0.25rem;
        display: block !important;
    }
    
    /* Specific fix for textarea in Summernote editor */
    .note-editable textarea.form-control {
        min-height: 100px;
        display: block !important;
        width: 100% !important;
        border: 1px solid #ced4da !important;
        margin-top: 4px;
    }
    
    /* Make sure input boxes and selects are properly displayed in editor */
    .note-editable input.form-control,
    .note-editable select.form-select {
        display: block !important;
        width: 100% !important;
        border: 1px solid #ced4da !important;
        margin-top: 4px;
        min-height: 38px;
    }
    
    .note-editable .form-group {
        margin-bottom: 1rem;
    }
    
    .note-editable .form-label {
        display: block;
        margin-bottom: 0.5rem;
    }
    
    .form-check-input {
        width: 1em;
        height: 1em;
        margin-top: 0.25em;
        vertical-align: top;
        background-color: #fff;
        border: 1px solid rgba(0,0,0,.25);
        appearance: none;
    }
    
    .form-text {
        margin-top: 0.25rem;
        font-size: 0.875em;
        color: #6c757d;
    }
    
    /* Table styles for forms */
    table.form-table {
        width: 100%;
        border-collapse: collapse;
        margin-bottom: 1rem;
    }
    
    table.form-table, table.form-table th, table.form-table td {
        border: 1px solid #dee2e6;
    }
    
    table.form-table th, table.form-table td {
        padding: 0.75rem;
        vertical-align: top;
    }
    
    table.form-table thead th {
        vertical-align: bottom;
        background-color: #f8f9fa;
        border-bottom: 2px solid #dee2e6;
    }
    
    /* Additional styling for specific form elements */
    .custom-select-wrapper {
        position: relative;
    }
    
    .custom-select-wrapper::after {
        content: "\f0d7";
        font-family: "Font Awesome 5 Free";
        font-weight: 900;
        position: absolute;
        right: 10px;
        top: 50%;
        transform: translateY(-50%);
        pointer-events: none;
    }
</style>


<script>
    // Globals for design data
    let designData = {
        elements: [],
        html: ''
    };
    // ใช้เก็บ names และ ids ที่มีอยู่แล้ว
    let existingNames = new Set();
    let existingIds = new Set();
    // ตัวแปรเก็บตำแหน่ง cursor ล่าสุดใน editor
    let lastCursorPosition = null;

    document.addEventListener('DOMContentLoaded', function() {
        // Load existing design data
        fetch(`template_design.php?action=get_design&TemplateRowId=<?php echo $templateId; ?>`)
            .then(response => response.json())
            .then(data => {
                if (data.success && data.designData) {
                    try {
                        designData = JSON.parse(data.designData);
                        // Initial Summernote with design data
                        initializeSummernote();
                        if (designData.html) {
                            $('#designArea').summernote('code', designData.html);
                        }
                    } catch (e) {
                        console.error('Error parsing design data:', e);
                        designData = {};
                        initializeSummernote();
                    }
                } else {
                    initializeSummernote();
                }
            })
            .catch(error => {
                console.error('Error loading design:', error);
                initializeSummernote();
            });
        
        // เรียกใช้ฟังก์ชัน loadDesign เพื่อโหลดข้อมูลการออกแบบ
        loadDesign();
        
        // Initialize save button
        document.getElementById('saveDesignBtn').addEventListener('click', saveDesign);

        // Initialize form element modal
        const formElementModal = document.getElementById('formElementModal');
        const cancelElementBtn = document.getElementById('cancelElementBtn');
        const addElementBtn = document.getElementById('addElementBtn');
        
        cancelElementBtn.addEventListener('click', function() {
            formElementModal.classList.add('hidden');
        });
        
        addElementBtn.addEventListener('click', async function() {
            const type = document.getElementById('formElementType').value;
            const name = document.getElementById('elementName').value;
            const label = document.getElementById('elementLabel').value;
            const placeholder = document.getElementById('elementPlaceholder').value;
            const modal = document.getElementById('formElementModal');
            
            // ตรวจสอบว่า name ไม่ซ้ำกัน
            if (existingNames.has(name)) {
                document.getElementById('nameError').classList.remove('hidden');
                return;
            }
            
            // Check with server as well
            const isAvailable = await checkNameAvailability(name);
            if (!isAvailable) {
                document.getElementById('nameError').classList.remove('hidden');
                return;
            }
            
            // เพิ่ม name และ id ลงในชุด existing
            existingNames.add(name);
            existingIds.add(name); // ID เหมือนกับ name
            
            // คืนค่าตำแหน่ง cursor ก่อนเพิ่ม element
            $('#designArea').summernote('editor.restoreRange');
            $('#designArea').summernote('editor.focus');
            
            // Insert form element
            insertFormElement(type, name, label, placeholder);
            
            // ปิด modal
            modal.classList.add('hidden');
        });
        
        // รีเซ็ต error message เมื่อผู้ใช้แก้ไข name
        document.getElementById('elementName').addEventListener('input', function() {
            document.getElementById('nameError').classList.add('hidden');
        });
    });
    
    // Initialize Summernote editor
    function initializeSummernote() {
        // Set up Summernote with custom toolbar
        $('#designArea').summernote({
            height: 600,
            focus: true,
            placeholder: 'ออกแบบแบบฟอร์มที่นี่...',
            toolbar: [
                ['style', ['style']],
                ['font', ['bold', 'underline', 'clear']],
                ['color', ['color']],
                ['para', ['ul', 'ol', 'paragraph']],
                ['table', ['table']],
                ['insert', ['link']],
                ['view', ['fullscreen', 'codeview']],
                ['formElements', ['formText', 'formTextarea', 'formSelect', 'formCheckbox', 'formRadio', 'formDate', 'formEmail', 'formFile']]
            ],
            callbacks: {
                onChange: function(contents, $editable) {
                    // Store content to designData
                    designData.html = contents;
                    
                    // Hide the empty message if there's content
                    if (contents && contents.trim() !== '') {
                        document.getElementById('emptyDesignMessage').style.display = 'none';
                    } else {
                        document.getElementById('emptyDesignMessage').style.display = 'block';
                    }
                }
            },
            buttons: {
                formText: function(context) {
                    return createFormButton(context, 'Text Field', 'text', 'fa-font');
                },
                formTextarea: function(context) {
                    return createFormButton(context, 'Textarea', 'textarea', 'fa-align-left');
                },
                formSelect: function(context) {
                    return createFormButton(context, 'Select', 'select', 'fa-caret-down');
                },
                formCheckbox: function(context) {
                    return createFormButton(context, 'Checkbox', 'checkbox', 'fa-check-square');
                },
                formRadio: function(context) {
                    return createFormButton(context, 'Radio', 'radio', 'fa-dot-circle');
                },
                formDate: function(context) {
                    return createFormButton(context, 'Date', 'date', 'fa-calendar-alt');
                },
                formEmail: function(context) {
                    return createFormButton(context, 'Email', 'email', 'fa-envelope');
                },
                formFile: function(context) {
                    return createFormButton(context, 'File Upload', 'file', 'fa-file-upload');
                }
            }
        });
    }
    
    // Helper function to create form buttons
    function createFormButton(context, title, type, icon) {
        const ui = $.summernote.ui;
        
        const button = ui.button({
            className: 'note-btn-form-' + type,
            contents: '<i class="fas ' + icon + '"></i>',
            tooltip: title,
            click: function() {
                // บันทึกตำแหน่ง cursor ปัจจุบัน
                context.invoke('editor.saveRange');
                
                // Show modal instead of inserting directly
                showFormElementModal(type, title, context);
            }
        });
        
        return button.render();
    }
    
    // Function to show the form element modal
    function showFormElementModal(type, title, context) {
        const modal = document.getElementById('formElementModal');
        const modalTitle = document.getElementById('formElementModalTitle');
        const formElementType = document.getElementById('formElementType');
        const elementName = document.getElementById('elementName');
        const elementLabel = document.getElementById('elementLabel');
        const elementPlaceholder = document.getElementById('elementPlaceholder');
        const placeholderContainer = document.getElementById('placeholderContainer');
        
        // Store context for later use
        modal.dataset.context = context ? 'summernote' : '';
        
        // Set modal title and type
        modalTitle.textContent = 'เพิ่ม ' + title;
        formElementType.value = type;
        
        // Generate default values
        const defaultName = type + '_' + Date.now();
        let defaultLabel = '';
        let defaultPlaceholder = '';
        
        switch(type) {
            case 'text':
                defaultLabel = 'Text Field Label';
                defaultPlaceholder = 'Enter text';
                placeholderContainer.classList.remove('hidden');
                break;
            case 'textarea':
                defaultLabel = 'Textarea Label';
                defaultPlaceholder = 'Enter text here';
                placeholderContainer.classList.remove('hidden');
                break;
            case 'select':
                defaultLabel = 'Select Label';
                defaultPlaceholder = 'Choose an option';
                placeholderContainer.classList.remove('hidden');
                break;
            case 'checkbox':
                defaultLabel = 'Checkbox label';
                placeholderContainer.classList.add('hidden');
                break;
            case 'radio':
                defaultLabel = 'Radio Button Group';
                placeholderContainer.classList.add('hidden');
                break;
            case 'date':
                defaultLabel = 'Date Field';
                placeholderContainer.classList.add('hidden');
                break;
            case 'email':
                defaultLabel = 'Email Address';
                defaultPlaceholder = 'name@example.com';
                placeholderContainer.classList.remove('hidden');
                break;
            case 'file':
                defaultLabel = 'File Upload';
                placeholderContainer.classList.add('hidden');
                break;
        }
        
        // Check name availability from server
        checkNameAvailability(defaultName).then(isAvailable => {
            if (!isAvailable) {
                // Append random digits to make it unique
                elementName.value = defaultName + '_' + Math.floor(Math.random() * 1000);
            } else {
                elementName.value = defaultName;
            }
        });
        
        // Set default values
        elementLabel.value = defaultLabel;
        elementPlaceholder.value = defaultPlaceholder;
        
        // Reset error message
        document.getElementById('nameError').classList.add('hidden');
        
        // Show modal
        modal.classList.remove('hidden');
    }
    
    // Function to insert different form elements
    function insertFormElement(type, name, label, placeholder) {
        // ไม่ต้องใช้ lastCursorPosition แล้ว เพราะได้ทำการ restoreRange ไปแล้ว
        
        switch (type) {
            case 'text':
                // สร้าง DOM elements ของ Text Input
                const textContainer = document.createElement('div');
                textContainer.className = 'form-group mb-3';
                
                const textLabel = document.createElement('label');
                textLabel.setAttribute('for', name);
                textLabel.className = 'form-label';
                textLabel.textContent = label;
                
                const textInput = document.createElement('input');
                textInput.type = 'text';
                textInput.className = 'form-control';
                textInput.id = name;
                textInput.name = name;
                textInput.placeholder = placeholder;
                
                textContainer.appendChild(textLabel);
                textContainer.appendChild(textInput);
                
                $('#designArea').summernote('insertNode', textContainer);
                break;
                
            case 'textarea':
                // สร้าง DOM elements ของ Textarea
                const textareaContainer = document.createElement('div');
                textareaContainer.className = 'form-group mb-3';
                
                const textareaLabel = document.createElement('label');
                textareaLabel.setAttribute('for', name);
                textareaLabel.className = 'form-label';
                textareaLabel.textContent = label;
                
                const textarea = document.createElement('textarea');
                textarea.className = 'form-control';
                textarea.id = name;
                textarea.name = name;
                textarea.rows = 3;
                textarea.placeholder = placeholder;
                textarea.style.width = '100%';
                textarea.style.minHeight = '100px';
                textarea.style.border = '1px solid #ced4da';
                textarea.style.borderRadius = '0.25rem';
                textarea.style.padding = '0.375rem 0.75rem';
                textarea.style.display = 'block';
                
                textareaContainer.appendChild(textareaLabel);
                textareaContainer.appendChild(textarea);
                
                $('#designArea').summernote('insertNode', textareaContainer);
                break;
                
            case 'select':
                // สร้าง DOM elements ของ Select
                const selectContainer = document.createElement('div');
                selectContainer.className = 'form-group mb-3';
                
                const selectLabel = document.createElement('label');
                selectLabel.setAttribute('for', name);
                selectLabel.className = 'form-label';
                selectLabel.textContent = label;
                
                const select = document.createElement('select');
                select.className = 'form-select';
                select.id = name;
                select.name = name;
                
                const option0 = document.createElement('option');
                option0.selected = true;
                option0.textContent = placeholder || 'Choose an option';
                
                const option1 = document.createElement('option');
                option1.value = '1';
                option1.textContent = 'Option 1';
                
                const option2 = document.createElement('option');
                option2.value = '2';
                option2.textContent = 'Option 2';
                
                const option3 = document.createElement('option');
                option3.value = '3';
                option3.textContent = 'Option 3';
                
                select.appendChild(option0);
                select.appendChild(option1);
                select.appendChild(option2);
                select.appendChild(option3);
                
                selectContainer.appendChild(selectLabel);
                selectContainer.appendChild(select);
                
                $('#designArea').summernote('insertNode', selectContainer);
                break;
                
            case 'checkbox':
                // สร้าง DOM elements ของ Checkbox
                const checkContainer = document.createElement('div');
                checkContainer.className = 'form-check mb-3';
                
                const checkbox = document.createElement('input');
                checkbox.type = 'checkbox';
                checkbox.className = 'form-check-input';
                checkbox.id = name;
                checkbox.name = name;
                
                const checkLabel = document.createElement('label');
                checkLabel.className = 'form-check-label';
                checkLabel.setAttribute('for', name);
                checkLabel.textContent = label;
                
                checkContainer.appendChild(checkbox);
                checkContainer.appendChild(checkLabel);
                
                $('#designArea').summernote('insertNode', checkContainer);
                break;
                
            case 'radio':
                // สร้าง DOM elements ของ Radio
                const id2 = name + '_2';
                const radioGroupName = name;
                
                const radioContainer = document.createElement('div');
                radioContainer.className = 'mb-3';
                
                const radioGroupLabel = document.createElement('label');
                radioGroupLabel.className = 'form-label d-block';
                radioGroupLabel.textContent = label;
                
                const radio1Container = document.createElement('div');
                radio1Container.className = 'form-check';
                
                const radio1 = document.createElement('input');
                radio1.className = 'form-check-input';
                radio1.type = 'radio';
                radio1.name = radioGroupName;
                radio1.id = name;
                radio1.checked = true;
                
                const radio1Label = document.createElement('label');
                radio1Label.className = 'form-check-label';
                radio1Label.setAttribute('for', name);
                radio1Label.textContent = 'Option One';
                
                radio1Container.appendChild(radio1);
                radio1Container.appendChild(radio1Label);
                
                const radio2Container = document.createElement('div');
                radio2Container.className = 'form-check';
                
                const radio2 = document.createElement('input');
                radio2.className = 'form-check-input';
                radio2.type = 'radio';
                radio2.name = radioGroupName;
                radio2.id = id2;
                
                const radio2Label = document.createElement('label');
                radio2Label.className = 'form-check-label';
                radio2Label.setAttribute('for', id2);
                radio2Label.textContent = 'Option Two';
                
                radio2Container.appendChild(radio2);
                radio2Container.appendChild(radio2Label);
                
                radioContainer.appendChild(radioGroupLabel);
                radioContainer.appendChild(radio1Container);
                radioContainer.appendChild(radio2Container);
                
                $('#designArea').summernote('insertNode', radioContainer);
                break;
                
            case 'date':
                // สร้าง DOM elements ของ Date Input
                const dateContainer = document.createElement('div');
                dateContainer.className = 'form-group mb-3';
                
                const dateLabel = document.createElement('label');
                dateLabel.setAttribute('for', name);
                dateLabel.className = 'form-label';
                dateLabel.textContent = label;
                
                const dateInput = document.createElement('input');
                dateInput.type = 'date';
                dateInput.className = 'form-control';
                dateInput.id = name;
                dateInput.name = name;
                
                dateContainer.appendChild(dateLabel);
                dateContainer.appendChild(dateInput);
                
                $('#designArea').summernote('insertNode', dateContainer);
                break;
                
            case 'email':
                // สร้าง DOM elements ของ Email Input
                const emailContainer = document.createElement('div');
                emailContainer.className = 'form-group mb-3';
                
                const emailLabel = document.createElement('label');
                emailLabel.setAttribute('for', name);
                emailLabel.className = 'form-label';
                emailLabel.textContent = label;
                
                const emailInput = document.createElement('input');
                emailInput.type = 'email';
                emailInput.className = 'form-control';
                emailInput.id = name;
                emailInput.name = name;
                emailInput.placeholder = placeholder;
                
                emailContainer.appendChild(emailLabel);
                emailContainer.appendChild(emailInput);
                
                $('#designArea').summernote('insertNode', emailContainer);
                break;
                
            case 'file':
                // สร้าง DOM elements ของ File Upload
                const fileContainer = document.createElement('div');
                fileContainer.className = 'form-group mb-3';
                
                const fileLabel = document.createElement('label');
                fileLabel.setAttribute('for', name);
                fileLabel.className = 'form-label';
                fileLabel.textContent = label;
                
                const fileInput = document.createElement('input');
                fileInput.type = 'file';
                fileInput.className = 'form-control';
                fileInput.id = name;
                fileInput.name = name;
                
                fileContainer.appendChild(fileLabel);
                fileContainer.appendChild(fileInput);
                
                $('#designArea').summernote('insertNode', fileContainer);
                break;
        }
    }
    
    // Function to save design
    function saveDesign() {
        const designHtml = $('#designArea').summernote('code');
        
        // Create a temporary div to parse the HTML
        const tempDiv = document.createElement('div');
        tempDiv.innerHTML = designHtml;
        
        // Collect all form element names and IDs
        const elements = [];
        const formElements = tempDiv.querySelectorAll('input, textarea, select');
        
        console.log('Found form elements:', formElements.length);
        
        formElements.forEach(function(el) {
            if (el.id && el.name) {
                elements.push({
                    id: el.id,
                    name: el.name,
                    type: el.tagName.toLowerCase() + (el.type ? '_' + el.type : '')
                });
                console.log('Added element:', el.name, el.tagName, el.type);
            } else {
                console.log('Skipped element (missing id or name):', el.tagName, el.type);
            }
        });
        
        console.log('Collected elements:', elements);
        
        const data = {
            designHtml: designHtml,
            elements: elements
        };
        
        // Show loading state
        document.getElementById('saveDesignBtn').textContent = 'กำลังบันทึก...';
        document.getElementById('saveDesignBtn').disabled = true;
        
        // Save via AJAX
        fetch('save_template_design.php?template_id=<?php echo $templateId; ?>', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(data)
        })
        .then(response => {
            console.log('Save design response status:', response.status);
            return response.json();
        })
        .then(data => {
            console.log('Save design response data:', data);
            
            if (data.success) {
                // Update design data
                designData.html = designHtml;
                designData.elements = elements;
                
                // Create template objects automatically
                createTemplateObjects(elements);
            } else {
                // Show error message
                const notification = document.getElementById('notification');
                notification.textContent = 'เกิดข้อผิดพลาด: ' + data.message;
                notification.classList.remove('hidden');
                notification.classList.add('bg-red-500');
                
                setTimeout(function() {
                    notification.classList.add('hidden');
                }, 3000);
                
                // Reset button
                document.getElementById('saveDesignBtn').textContent = 'บันทึกการออกแบบ';
                document.getElementById('saveDesignBtn').disabled = false;
            }
        })
        .catch(error => {
            console.error('Error saving design:', error);
            
            // Show error message
            const notification = document.getElementById('notification');
            notification.textContent = 'เกิดข้อผิดพลาดในการเชื่อมต่อ';
            notification.classList.remove('hidden');
            notification.classList.add('bg-red-500');
            
            setTimeout(function() {
                notification.classList.add('hidden');
            }, 3000);
            
            // Reset button
            document.getElementById('saveDesignBtn').textContent = 'บันทึกการออกแบบ';
            document.getElementById('saveDesignBtn').disabled = false;
        });
    }
    
    // Function to create template objects automatically
    function createTemplateObjects(elements) {
        console.log('Creating template objects with elements:', elements);
        
        if (!elements || elements.length === 0) {
            console.log('No elements to create objects from');
            // ถ้าไม่มี elements ก็ไม่ต้องสร้าง objects
            
            // Show success message for design save
            const notification = document.getElementById('notification');
            notification.textContent = 'บันทึกการออกแบบแล้ว';
            notification.classList.remove('hidden');
            notification.classList.add('bg-green-500');
            
            setTimeout(function() {
                notification.classList.add('hidden');
            }, 3000);
            
            // Reset button
            document.getElementById('saveDesignBtn').textContent = 'บันทึกการออกแบบ';
            document.getElementById('saveDesignBtn').disabled = false;
            
            return;
        }
        
        // ส่ง elements ไปยัง API เพื่อสร้าง objects
        const url = 'create_template_objects.php?template_id=<?php echo $templateId; ?>';
        console.log('Sending request to:', url);
        
        fetch(url, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({ elements: elements })
        })
        .then(response => {
            console.log('Response status:', response.status);
            return response.json();
        })
        .then(data => {
            console.log('Response data:', data);
            // Show success message
            const notification = document.getElementById('notification');
            if (data.success) {
                // แสดงข้อความสำเร็จพร้อมสถิติการซิงค์
                const stats = data.stats || {};
                const created = stats.created || 0;
                const deleted = stats.deleted || 0;
                const kept = stats.kept || 0;
                
                if (created > 0 || deleted > 0) {
                    notification.textContent = `บันทึกการออกแบบและซิงค์ Objects แล้ว (สร้าง ${created}, ลบ ${deleted}, คงไว้ ${kept})`;
                } else {
                    notification.textContent = 'บันทึกการออกแบบแล้ว (ไม่มีการเปลี่ยนแปลง Objects)';
                }
            } else {
                notification.textContent = 'บันทึกการออกแบบแล้ว แต่มีปัญหาในการซิงค์ Objects: ' + data.message;
            }
            notification.classList.remove('hidden');
            notification.classList.add('bg-green-500');
            
            setTimeout(function() {
                notification.classList.add('hidden');
            }, 3000);
            
            // Reset button
            document.getElementById('saveDesignBtn').textContent = 'บันทึกการออกแบบ';
            document.getElementById('saveDesignBtn').disabled = false;
        })
        .catch(error => {
            console.error('Error creating objects:', error);
            
            // Show error message
            const notification = document.getElementById('notification');
            notification.textContent = 'บันทึกการออกแบบแล้ว แต่มีปัญหาในการซิงค์ Objects';
            notification.classList.remove('hidden');
            notification.classList.add('bg-green-500');
            
            setTimeout(function() {
                notification.classList.add('hidden');
            }, 3000);
            
            // Reset button
            document.getElementById('saveDesignBtn').textContent = 'บันทึกการออกแบบ';
            document.getElementById('saveDesignBtn').disabled = false;
        });
    }
    
    // Function to load design
    function loadDesign() {
        fetch('get_template_design.php?template_id=<?php echo $templateId; ?>')
        .then(response => response.json())
        .then(data => {
            if (data.success && data.design) {
                // Update the design area with the saved HTML
                $('#designArea').summernote('code', data.design.html);
                
                // Hide empty message if design is not empty
                if (data.design.html && data.design.html.trim() !== '') {
                    document.getElementById('emptyDesignMessage').style.display = 'none';
                }
                
                // Store the design data
                designData.html = data.design.html;
                
                // Store the element data
                if (data.design.elements && Array.isArray(data.design.elements)) {
                    designData.elements = data.design.elements;
                    
                    // Add element names to the existingNames set to prevent duplicates
                    data.design.elements.forEach(function(element) {
                        if (element.name) {
                            existingNames.add(element.name);
                        }
                        if (element.id) {
                            existingIds.add(element.id);
                        }
                    });
                }
            }
        })
        .catch(error => {
            console.error('Error loading design:', error);
        });
    }
    
    // Prevent closing dropdown when clicking inside it
    $(document).on('click', '.note-form-elements-menu', function(e) {
        e.stopPropagation();
    });
    
    // Initialize form element event handlers
    $(document).on('click', '.form-check-input, .form-control, .form-select, .btn', function(e) {
        // Prevent editing in preview mode
        if ($(this).closest('.note-editable').length) {
            e.preventDefault();
            e.stopPropagation();
        }
    });

    // Function to check if name is available
    function checkNameAvailability(name) {
        return fetch('check_field_name.php?template_id=<?php echo $templateId; ?>&name=' + encodeURIComponent(name))
            .then(response => response.json())
            .then(data => {
                return !data.exists;
            })
            .catch(error => {
                console.error('Error checking name availability:', error);
                return false; // Assume name is not available on error (safer approach)
            });
    }
</script>

<!-- Design Area Section -->
<div class="mt-8">
    <div class="bg-white shadow overflow-hidden sm:rounded-lg">
        <div class="px-4 py-5 sm:px-6 flex justify-between items-center">
            <h3 class="text-lg leading-6 font-medium text-gray-900">พื้นที่ออกแบบ</h3>
            <button id="saveDesignBtn" type="button" class="inline-flex items-center px-4 py-2 border border-transparent text-sm font-medium rounded-md shadow-sm text-white bg-blue-600 hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500">
                บันทึกการออกแบบ
            </button>
        </div>
        <div class="border-t border-gray-200">
            <!-- Full Editor View -->
            <div class="p-4">
                <!-- Text editor toolbar will be generated by Summernote -->
                <div id="designArea" class="bg-white border border-gray-300 rounded-lg min-h-[600px] relative">
                    <div id="emptyDesignMessage" class="absolute top-1/2 left-1/2 transform -translate-x-1/2 -translate-y-1/2 text-center w-full text-gray-500">
                        <i class="fas fa-edit text-4xl mb-2"></i>
                        <h3 class="text-xl font-semibold">ออกแบบแบบฟอร์มของคุณ</h3>
                        <p>เพิ่มเอลิเมนต์ฟอร์มจากแถบเครื่องมือด้านบน</p>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>

<?php
// Include footer
include 'includes/footer.php';
?> 