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

if (!$data || !isset($data['elements']) || !is_array($data['elements'])) {
    header('Content-Type: application/json');
    echo json_encode(['success' => false, 'message' => 'Invalid or missing elements data']);
    exit;
}

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

// Get existing objects to sync with current elements
$existingObjectsQuery = "SELECT ObjectRowId, ObjectName FROM TemplateObject WHERE TemplateRowId = '" . $db->escapeString($templateId) . "'";
$existingObjects = $db->executeQuery($existingObjectsQuery);

// Create associative arrays for easier lookup
$existingObjectsByName = [];
$currentElementNames = [];

foreach ($existingObjects as $obj) {
    $existingObjectsByName[$obj['ObjectName']] = $obj['ObjectRowId'];
}

$elements = $data['elements'];
foreach ($elements as $element) {
    if (isset($element['name']) && !empty($element['name'])) {
        $currentElementNames[] = $element['name'];
    }
}

// Stats for reporting
$createdCount = 0;
$deletedCount = 0;
$keptCount = 0;
$errors = [];

try {
    // First, find and delete objects that are no longer in the design
    $objectsToDelete = array_diff(array_keys($existingObjectsByName), $currentElementNames);
    
    foreach ($objectsToDelete as $objectName) {
        $objectId = $existingObjectsByName[$objectName];
        $deleteSql = "DELETE FROM TemplateObject WHERE ObjectRowId = '" . $db->escapeString($objectId) . "'";
        $result = $db->executeNonQuery($deleteSql);
        
        if ($result) {
            $deletedCount++;
        } else {
            $errors[] = "Failed to delete object: " . $objectName;
        }
    }
    
    // Process each element and create an object if it doesn't exist
    foreach ($elements as $element) {
        // Skip if name is missing
        if (!isset($element['name']) || empty($element['name'])) {
            continue;
        }
        
        $objectName = $element['name'];
        
        // Skip if this object already exists (but count it as kept)
        if (isset($existingObjectsByName[$objectName])) {
            $keptCount++;
            continue;
        }
        
        // Determine object type based on element type
        $objectType = 'TEXT'; // Default type
        
        if (isset($element['type'])) {
            $type = strtolower($element['type']);
            
            if (strpos($type, 'textarea') !== false) {
                $objectType = 'TEXTAREA';
            } elseif (strpos($type, 'select') !== false) {
                $objectType = 'SELECT';
            } elseif (strpos($type, 'checkbox') !== false) {
                $objectType = 'CHECKBOX';
            } elseif (strpos($type, 'radio') !== false) {
                $objectType = 'RADIO';
            } elseif (strpos($type, 'date') !== false) {
                $objectType = 'DATE';
            } elseif (strpos($type, 'email') !== false) {
                $objectType = 'EMAIL';
            } elseif (strpos($type, 'file') !== false) {
                $objectType = 'FILE';
            } elseif (strpos($type, 'text') !== false) {
                $objectType = 'TEXT';
            }
        }
        
        // Generate a UUID for the new object
        $objectId = generateUUID();
        
        // Insert the new object
        $insertSql = "INSERT INTO TemplateObject (
                ObjectRowId,
                TemplateRowId,
                ObjectName,
                ObjectType,
                CrDate,
                CrBy
            ) VALUES (
                '" . $db->escapeString($objectId) . "',
                '" . $db->escapeString($templateId) . "',
                '" . $db->escapeString($objectName) . "',
                '" . $db->escapeString($objectType) . "',
                NOW(),
                '" . $db->escapeString($currentUsername) . "'
            )";
        
        $result = $db->executeNonQuery($insertSql);
        
        if ($result) {
            $createdCount++;
        } else {
            $errors[] = "Failed to create object: " . $objectName;
        }
    }
    
    // Return success or partial success based on results
    header('Content-Type: application/json');
    if (empty($errors)) {
        echo json_encode([
            'success' => true, 
            'message' => 'Synchronized objects: Created ' . $createdCount . ', Deleted ' . $deletedCount . ', Kept ' . $keptCount,
            'stats' => [
                'created' => $createdCount,
                'deleted' => $deletedCount,
                'kept' => $keptCount,
                'total' => count($elements)
            ]
        ]);
    } else {
        echo json_encode([
            'success' => ($createdCount > 0 || $deletedCount > 0 || $keptCount > 0), 
            'message' => 'Synchronized with errors: Created ' . $createdCount . ', Deleted ' . $deletedCount . ', Kept ' . $keptCount . ', Errors: ' . count($errors),
            'errors' => $errors,
            'stats' => [
                'created' => $createdCount,
                'deleted' => $deletedCount,
                'kept' => $keptCount,
                'total' => count($elements),
                'errors' => count($errors)
            ]
        ]);
    }
} catch (Exception $e) {
    header('Content-Type: application/json');
    echo json_encode(['success' => false, 'message' => $e->getMessage()]);
} 