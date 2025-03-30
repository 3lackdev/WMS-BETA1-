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

// Check if template ID and name are provided
if (!isset($_GET['template_id']) || !isset($_GET['name'])) {
    header('Content-Type: application/json');
    echo json_encode(['success' => false, 'message' => 'Template ID and field name are required']);
    exit;
}

$templateId = $_GET['template_id'];
$fieldName = $_GET['name'];

// Initialize database
$db = new Database();

try {
    // Get the template design data
    $sql = "SELECT DesignData FROM WorkFlowTemplate WHERE TemplateRowId = '" . $db->escapeString($templateId) . "'";
    $result = $db->executeQuery($sql);
    
    $nameExists = false;
    
    if (count($result) > 0 && isset($result[0]['DesignData'])) {
        $designData = $result[0]['DesignData'];
        
        // Check if the design data is valid JSON
        $designObject = json_decode($designData, true);
        
        // If valid JSON and has elements array
        if ($designObject !== null && isset($designObject['elements']) && is_array($designObject['elements'])) {
            // Check if the name exists in any element
            foreach ($designObject['elements'] as $element) {
                if (isset($element['name']) && $element['name'] === $fieldName) {
                    $nameExists = true;
                    break;
                }
            }
        }
    }
    
    header('Content-Type: application/json');
    echo json_encode(['exists' => $nameExists]);
} catch (Exception $e) {
    header('Content-Type: application/json');
    echo json_encode(['success' => false, 'message' => $e->getMessage()]);
} 