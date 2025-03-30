<?php
/**
 * Database Connection Class for WorkFlowEngine
 * Handles connections to MySQL database and query execution
 */

class Database {
    private $host = 'localhost';
    private $username = 'wf';
    private $password = 'password';
    private $database = 'WorkFlowEngine';
    private $conn;
    
    /**
     * Constructor - establishes database connection
     */
    public function __construct() {
        try {
            $this->conn = new mysqli($this->host, $this->username, $this->password, $this->database);
            
            if ($this->conn->connect_error) {
                throw new Exception("Connection failed: " . $this->conn->connect_error);
            }
            
            $this->conn->set_charset("utf8");
        } catch (Exception $e) {
            error_log("Database connection error: " . $e->getMessage());
            throw $e;
        }
    }
    
    /**
     * Executes a query and returns results as an associative array
     * 
     * @param string $query The SQL query to execute
     * @param string $connectionType The type of connection to use (ignored in this implementation)
     * @return array The query results as an associative array
     */
    public function executeQuery($query, $connectionType = null) {
        try {
            $result = $this->conn->query($query);
            
            if (!$result) {
                throw new Exception("Query failed: " . $this->conn->error);
            }
            
            $data = [];
            while ($row = $result->fetch_assoc()) {
                $data[] = $row;
            }
            
            $result->free();
            return $data;
        } catch (Exception $e) {
            error_log("Query execution error: " . $e->getMessage());
            throw $e;
        }
    }
    
    /**
     * Executes a non-query SQL statement (INSERT, UPDATE, DELETE)
     * 
     * @param string $query The SQL query to execute
     * @param string $connectionType The type of connection to use (ignored in this implementation)
     * @return bool True if successful
     */
    public function executeNonQuery($query, $connectionType = null) {
        try {
            $result = $this->conn->query($query);
            
            if (!$result) {
                throw new Exception("Query failed: " . $this->conn->error);
            }
            
            return true;
        } catch (Exception $e) {
            error_log("Non-query execution error: " . $e->getMessage());
            throw $e;
        }
    }
    
    /**
     * Checks if a table exists in the database
     * 
     * @param string $tableName Table name to check
     * @return bool True if the table exists, false otherwise
     */
    public function tableExists($tableName) {
        try {
            $sql = "SHOW TABLES LIKE '" . $this->escapeString($tableName) . "'";
            $result = $this->conn->query($sql);
            
            return ($result && $result->num_rows > 0);
        } catch (Exception $e) {
            error_log("Table check error: " . $e->getMessage());
            return false;
        }
    }
    
    /**
     * Get actual table name with correct case
     * 
     * @param string $tableName Case-insensitive table name to find
     * @return string|null Actual table name with correct case, or null if not found
     */
    public function getCorrectTableName($tableName) {
        try {
            $sql = "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES 
                    WHERE TABLE_SCHEMA = '" . $this->database . "' 
                    AND LOWER(TABLE_NAME) = LOWER('" . $this->escapeString($tableName) . "')";
            
            $result = $this->conn->query($sql);
            
            if ($result && $result->num_rows > 0) {
                $row = $result->fetch_assoc();
                return $row['TABLE_NAME'];
            }
            
            return null;
        } catch (Exception $e) {
            error_log("Table name check error: " . $e->getMessage());
            return null;
        }
    }
    
    /**
     * Escapes special characters in a string for use in SQL statements
     * Handles null values safely by converting them to empty strings
     * 
     * @param mixed $value The value to escape (string or null)
     * @return string The escaped string
     */
    public function escapeString($value) {
        // If value is null, return empty string
        if ($value === null) {
            return '';
        }
        
        // Convert non-string values to string
        if (!is_string($value)) {
            $value = (string)$value;
        }
        
        return $this->conn->real_escape_string($value);
    }
    
    /**
     * Destructor - closes the database connection
     */
    public function __destruct() {
        if ($this->conn) {
            $this->conn->close();
        }
    }

    /**
     * Begin a database transaction
     */
    public function beginTransaction() {
        if ($this->conn) {
            $this->conn->begin_transaction();
        }
    }

    /**
     * Commit a database transaction
     */
    public function commitTransaction() {
        if ($this->conn) {
            $this->conn->commit();
        }
    }

    /**
     * Rollback a database transaction
     */
    public function rollbackTransaction() {
        if ($this->conn) {
            $this->conn->rollback();
        }
    }

    /**
     * Execute an INSERT query and return the last inserted ID
     * 
     * @param string $query The SQL query to execute
     * @param array $params Parameters to bind to the query (optional)
     * @return int The ID of the last inserted row or 0 if the operation failed
     */
    public function executeInsert($query, $params = []) {
        try {
            $stmt = $this->prepareAndExecute($query, $params);
            
            if ($stmt) {
                $insertId = $this->conn->insert_id;
                $stmt->close();
                return $insertId;
            }
        } catch (Exception $e) {
            error_log("Database error in executeInsert: " . $e->getMessage());
        }
        
        return 0;
    }

    /**
     * Execute an UPDATE query and return the number of affected rows
     * 
     * @param string $query The SQL query to execute
     * @param array $params Parameters to bind to the query (optional)
     * @return int The number of affected rows or 0 if the operation failed
     */
    public function executeUpdate($query, $params = []) {
        try {
            $stmt = $this->prepareAndExecute($query, $params);
            
            if ($stmt) {
                $affectedRows = $stmt->affected_rows;
                $stmt->close();
                return $affectedRows;
            }
        } catch (Exception $e) {
            error_log("Database error in executeUpdate: " . $e->getMessage());
        }
        
        return 0;
    }

    /**
     * Prepare and execute a statement with parameters
     * 
     * @param string $query The SQL query to prepare
     * @param array $params Parameters to bind to the query
     * @return mysqli_stmt|false The prepared statement or false on failure
     */
    private function prepareAndExecute($query, $params = []) {
        if (!$this->conn) {
            return false;
        }
        
        $stmt = $this->conn->prepare($query);
        
        if (!$stmt) {
            error_log("Database prepare error: " . $this->conn->error);
            return false;
        }
        
        if (!empty($params)) {
            $types = '';
            $bindParams = [];
            
            // Build the types string and bind params array
            foreach ($params as $param) {
                if (is_int($param)) {
                    $types .= 'i';
                } elseif (is_float($param)) {
                    $types .= 'd';
                } elseif (is_string($param)) {
                    $types .= 's';
                } else {
                    $types .= 'b';
                }
                
                $bindParams[] = $param;
            }
            
            // Add types as the first element of the array
            array_unshift($bindParams, $types);
            
            // Bind parameters using call_user_func_array
            call_user_func_array([$stmt, 'bind_param'], $this->refValues($bindParams));
        }
        
        // Execute the statement
        if (!$stmt->execute()) {
            error_log("Database execute error: " . $stmt->error);
            $stmt->close();
            return false;
        }
        
        return $stmt;
    }

    /**
     * Convert values to references for bind_param
     * 
     * @param array $arr The array to convert
     * @return array Array with values converted to references
     */
    private function refValues($arr) {
        $refs = [];
        
        foreach ($arr as $key => $value) {
            $refs[$key] = &$arr[$key];
        }
        
        return $refs;
    }
}
?> 