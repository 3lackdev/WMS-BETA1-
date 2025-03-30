<?php
require_once 'includes/Database.php';
require_once 'includes/Authentication.php';

// Initialize authentication
$auth = new Authentication();

// Check if user is authenticated
if (!$auth->isAuthenticated()) {
    header('Content-Type: application/json');
    echo json_encode(['success' => false, 'message' => 'Authentication required']);
    exit;
}

// Check if template ID is provided
if (!isset($_GET['template_id'])) {
    header('Content-Type: application/json');
    echo json_encode(['success' => false, 'message' => 'Template ID is required']);
    exit;
}

$templateId = $_GET['template_id'];

// Get current user
$currentUser = $auth->getCurrentUser();
$currentUsername = $currentUser['username'] ?? '';

// Initialize database
$db = new Database();

// Get JSON data from request
$input = file_get_contents('php://input');
$data = json_decode($input, true);

if (!$data) {
    header('Content-Type: application/json');
    echo json_encode(['success' => false, 'message' => 'Invalid JSON data']);
    exit;
}

// Validate required fields
if (!isset($data['designHtml'])) {
    header('Content-Type: application/json');
    echo json_encode(['success' => false, 'message' => 'Design HTML is required']);
    exit;
}

// Prepare design data for storage
$designData = [
    'html' => $data['designHtml'],
    'elements' => $data['elements'] ?? []
];

// Convert to JSON for storage
$designJson = json_encode($designData);

try {
    // Update the design data in the database
    $sql = "UPDATE WorkFlowTemplate SET 
            DesignData = '" . $db->escapeString($designJson) . "',
            UdDate = NOW(), 
            UdBy = '" . $db->escapeString($currentUsername) . "'
            WHERE TemplateRowId = '" . $db->escapeString($templateId) . "'";
    
    $result = $db->executeNonQuery($sql);
    
    if ($result) {
        header('Content-Type: application/json');
        echo json_encode(['success' => true]);
    } else {
        header('Content-Type: application/json');
        echo json_encode(['success' => false, 'message' => 'Failed to save design']);
    }
} catch (Exception $e) {
    header('Content-Type: application/json');
    echo json_encode(['success' => false, 'message' => $e->getMessage()]);
} 