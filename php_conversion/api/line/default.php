<?php
/**
 * API Endpoint for WorkFlowEngine Line Integration
 * Handles incoming LINE messages and responds with appropriate data
 */

// Include database connection class
require_once '../../includes/Database.php';

class LineAPI {
    private $db;
    private $testFilePath;
    
    /**
     * Constructor - initializes database connection and file path
     */
    public function __construct() {
        $this->db = new Database();
        $this->testFilePath = __DIR__ . '/test.txt';
    }
    
    /**
     * Process the request
     */
    public function processRequest() {
        header('Content-Type: application/json');
        
        // Check if test file exists and create it if needed
        $this->checkAndCreateTestFile();
        
        // Get input data
        $inputData = $this->getRequestData();
        
        // Process the data if any
        if (!empty($inputData)) {
            $this->processInputData($inputData);
        } else {
            echo json_encode(['error' => 'No input data received']);
        }
    }
    
    /**
     * Check if test file exists and create it if needed
     */
    private function checkAndCreateTestFile() {
        if (!file_exists($this->testFilePath)) {
            file_put_contents($this->testFilePath, "sss");
        }
    }
    
    /**
     * Get data from request body
     * 
     * @return string JSON string from request body
     */
    private function getRequestData() {
        return file_get_contents('php://input');
    }
    
    /**
     * Process the input data from request
     * 
     * @param string $inputData JSON string from request
     */
    private function processInputData($inputData) {
        // Append the input data to test file for logging
        file_put_contents($this->testFilePath, $inputData, FILE_APPEND);
        
        // Try to parse JSON data
        $data = json_decode($inputData, true);
        
        if ($data && !empty($data)) {
            // Extract the query text from JSON data
            $queryText = $data['queryResult']['queryText'] ?? '';
            
            if (!empty($queryText)) {
                // Query the database for the node name
                $result = $this->getNodeName($queryText);
                
                // Prepare response based on database result
                $response = $this->prepareResponse($data, $result);
                
                // Log the response
                file_put_contents($this->testFilePath, $response, FILE_APPEND);
                
                // Output the response
                echo $response;
            } else {
                echo json_encode(['error' => 'No query text found in input data']);
            }
        } else {
            echo json_encode(['error' => 'Invalid JSON input']);
        }
    }
    
    /**
     * Get node name from database
     * 
     * @param string $nodeId Node ID to search for
     * @return string|null Node name if found, null otherwise
     */
    private function getNodeName($nodeId) {
        try {
            $sql = "SELECT node_name FROM node WHERE node_id = '" . $this->db->escapeString($nodeId) . "' LIMIT 1";
            $result = $this->db->executeQuery($sql);
            
            if (count($result) > 0) {
                return $result[0]['node_name'];
            }
            
            return null;
        } catch (Exception $e) {
            return null;
        }
    }
    
    /**
     * Prepare response JSON
     * 
     * @param array $data Input data array
     * @param string|null $nodeName Node name from database
     * @return string JSON response
     */
    private function prepareResponse($data, $nodeName) {
        $responseId = $data['responseId'] ?? '';
        
        $response = [
            'source' => $responseId,
            'fulfillmentText' => ($nodeName !== null) ? $nodeName : 'ไม่พบข้อมูล'
        ];
        
        return json_encode($response);
    }
}

// Initialize and process the request
$api = new LineAPI();
$api->processRequest();
?> 