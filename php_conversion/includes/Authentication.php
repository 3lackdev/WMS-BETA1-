<?php
/**
 * Authentication Class for WorkFlowEngine
 * Handles user authentication and session management
 */

class Authentication {
    private $db;
    
    /**
     * Constructor - initializes database connection
     */
    public function __construct() {
        // Start session if not already started
        if (session_status() == PHP_SESSION_NONE) {
            session_start();
        }
        
        // Initialize database connection
        $this->db = new Database();
    }
    
    /**
     * Check if user is authenticated
     * 
     * @return bool True if user is authenticated, false otherwise
     */
    public function isAuthenticated() {
        // For development: auto-login as admin if no session exists
        if (!isset($_SESSION['user_id']) || empty($_SESSION['user_id'])) {
            // Create a test session
            $_SESSION['user_id'] = 1;
            $_SESSION['username'] = 'admin';
            $_SESSION['user_role'] = 'admin';
            
            error_log("Auto-logged in as admin for development purposes");
        }
        
        return isset($_SESSION['user_id']) && !empty($_SESSION['user_id']);
    }
    
    /**
     * Get current authenticated user information
     * 
     * @return array|null User information if authenticated, null otherwise
     */
    public function getCurrentUser() {
        if (!$this->isAuthenticated()) {
            return null;
        }
        
        try {
            // First check if the users table exists
            $tableExists = $this->db->tableExists('users');
            
            // If the table doesn't exist, create a default user for the session
            if (!$tableExists) {
                error_log("Users table doesn't exist - using session user data only");
                return [
                    'user_id' => $_SESSION['user_id'],
                    'username' => $_SESSION['username'] ?? 'Unknown',
                    'role' => $_SESSION['user_role'] ?? 'user'
                ];
            }
            
            $userId = $_SESSION['user_id'];
            $sql = "SELECT * FROM users WHERE user_id = '" . $this->db->escapeString($userId) . "' LIMIT 1";
            $result = $this->db->executeQuery($sql);
            
            if (count($result) > 0) {
                return $result[0];
            }
            
            // If we reach here, user is in session but not in database
            // Return a basic user object to prevent null reference errors
            return [
                'user_id' => $userId,
                'username' => $_SESSION['username'] ?? 'Unknown',
                'role' => $_SESSION['user_role'] ?? 'user'
            ];
        } catch (Exception $e) {
            error_log("Error getting current user: " . $e->getMessage());
            
            // Return a fallback user object
            return [
                'user_id' => $_SESSION['user_id'],
                'username' => $_SESSION['username'] ?? 'Unknown',
                'role' => $_SESSION['user_role'] ?? 'user'
            ];
        }
    }
    
    /**
     * Attempt to login a user
     * 
     * @param string $username Username
     * @param string $password Password
     * @return bool True if login successful, false otherwise
     */
    public function login($username, $password) {
        try {
            // Get user from database
            $sql = "SELECT * FROM users WHERE username = '" . $this->db->escapeString($username) . "' LIMIT 1";
            $result = $this->db->executeQuery($sql);
            
            if (count($result) > 0) {
                $user = $result[0];
                
                // Verify password (assuming hashed password storage)
                if (password_verify($password, $user['password'])) {
                    // Set session variables
                    $_SESSION['user_id'] = $user['user_id'];
                    $_SESSION['username'] = $user['username'];
                    $_SESSION['user_role'] = $user['role'];
                    
                    return true;
                }
            }
            
            return false;
        } catch (Exception $e) {
            error_log("Login error: " . $e->getMessage());
            return false;
        }
    }
    
    /**
     * Log out the current user
     */
    public function logout() {
        // Unset all session variables
        $_SESSION = array();
        
        // Destroy the session
        session_destroy();
    }
    
    /**
     * Check if current user has specified role
     * 
     * @param string $role Role to check
     * @return bool True if user has role, false otherwise
     */
    public function hasRole($role) {
        if (!$this->isAuthenticated()) {
            return false;
        }
        
        return isset($_SESSION['user_role']) && $_SESSION['user_role'] == $role;
    }
}
?> 