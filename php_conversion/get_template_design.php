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

// Initialize database
$db = new Database();

try {
    // Get the template details from the database
    $sql = "SELECT DesignData FROM WorkFlowTemplate WHERE TemplateRowId = '" . $db->escapeString($templateId) . "'";
    $result = $db->executeQuery($sql);
    
    if (count($result) > 0 && isset($result[0]['DesignData'])) {
        $designData = $result[0]['DesignData'];
        
        // Check if the design data is valid JSON
        $designObject = json_decode($designData, true);
        
        if ($designObject === null && json_last_error() !== JSON_ERROR_NONE) {
            // If not valid JSON, create a default structure
            $design = [
                'html' => $designData, // Use as raw HTML
                'elements' => []
            ];
        } else {
            $design = $designObject;
        }
        
        header('Content-Type: application/json');
        echo json_encode(['success' => true, 'design' => $design]);
    } else {
        header('Content-Type: application/json');
        echo json_encode(['success' => false, 'message' => 'Template not found or has no design data']);
    }
} catch (Exception $e) {
    header('Content-Type: application/json');
    echo json_encode(['success' => false, 'message' => $e->getMessage()]);
} 