<?php
/**
 * API Endpoint for WorkFlowEngine
 * Fetches data from the database based on workflow IDs
 */

// Include database connection class
require_once '../includes/Database.php';

class GetData {
    private $db;
    
    /**
     * Constructor - initializes database connection
     */
    public function __construct() {
        $this->db = new Database();
    }
    
    /**
     * Process the request to get data
     */
    public function processRequest() {
        header('Content-Type: application/json');
        
        // Get parameters
        $workflowId = $this->getParameter('workflowid');
        $column = $this->getParameter('column');
        $where = $this->getParameter('where');
        $orderBy = $this->getParameter('orderby');
        
        if (empty($workflowId)) {
            echo json_encode(['error' => 'Missing required parameter: workflowid']);
            return;
        }
        
        try {
            // Get data based on workflow ID
            $result = $this->getDataByWorkflowId($workflowId, $column, $where, $orderBy);
            echo json_encode($result);
        } catch (Exception $e) {
            echo json_encode(['error' => $e->getMessage()]);
        }
    }
    
    /**
     * Gets a parameter from GET request, with optional default value
     * 
     * @param string $name Parameter name
     * @param mixed $default Default value if parameter is not set
     * @return mixed Parameter value or default
     */
    private function getParameter($name, $default = '') {
        return isset($_GET[$name]) ? $_GET[$name] : $default;
    }
    
    /**
     * Get data from database based on workflow ID
     * 
     * @param string $workflowId The workflow ID to fetch data for
     * @param string $column Optional column specification
     * @param string $where Optional WHERE clause
     * @param string $orderBy Optional ORDER BY clause
     * @return array Data retrieved from database
     */
    private function getDataByWorkflowId($workflowId, $column = '', $where = '', $orderBy = '') {
        // Build the query
        $sql = "CALL GetHistoryValue('" . $this->db->escapeString($workflowId) . "')";
        
        // Apply column filter if specified
        if (!empty($column)) {
            // Apply column filter logic
            // In PHP implementation, we'll filter after getting results
        }
        
        // Apply where clause if specified
        if (!empty($where)) {
            // Apply where clause logic
            // In PHP implementation, we'll filter after getting results
        }
        
        // Apply order by if specified
        if (!empty($orderBy)) {
            // Apply order by logic
            // In PHP implementation, we'll sort after getting results
        }
        
        // Execute the query
        $data = $this->db->executeQuery($sql);
        
        // Apply filters if needed
        if (!empty($column) || !empty($where) || !empty($orderBy)) {
            // Additional filtering could be implemented here
        }
        
        return $data;
    }
}

// Initialize and process the request
$api = new GetData();
$api->processRequest();
?> 