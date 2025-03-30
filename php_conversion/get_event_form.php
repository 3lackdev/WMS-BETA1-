<?php
/**
 * File: get_event_form.php
 * Description: ไฟล์สำหรับโหลดฟอร์มแก้ไขและตั้งค่าเหตุการณ์ผ่าน AJAX
 */

// Include required files
require_once 'includes/Database.php';
require_once 'includes/Authentication.php';

// Check if user is logged in
session_start();
if (!isset($_SESSION['user_id'])) {
    echo json_encode(['error' => 'Authentication required']);
    exit();
}

// Get parameters
$action = $_GET['action'] ?? '';
$workflowId = $_GET['WorkflowId'] ?? '';
$stepId = $_GET['WFRowId'] ?? '';

// Validate required parameters
if (empty($action) || empty($workflowId) || empty($stepId)) {
    echo '<div class="alert alert-danger">Missing required parameters</div>';
    exit();
}

// Connect to database
$db = new Database();

// Get step details
try {
    $stepId = $db->escapeString($stepId);
    $sql = "SELECT s.*, w.WFName as WorkflowName 
            FROM WorkFlowStep s
            JOIN WorkFlow w ON s.WFRowId = w.WFRowId
            WHERE s.StepRowId = '$stepId'";
    $result = $db->executeQuery($sql);
    
    if (count($result) > 0) {
        $stepDetails = $result[0];
    } else {
        echo '<div class="alert alert-danger">Step not found</div>';
        exit();
    }
} catch (Exception $e) {
    echo '<div class="alert alert-danger">Error: ' . htmlspecialchars($e->getMessage()) . '</div>';
    exit();
}

$stepName = $stepDetails['StepName'] ?? '';
$workflowName = $stepDetails['WorkflowName'] ?? '';

// Return appropriate form based on action
if ($action == 'edit') {
    // Form for editing event
    ?>
    <form id="editForm" action="process-step-events.php" method="POST">
        <input type="hidden" name="action" value="update">
        <input type="hidden" name="WFRowId" value="<?php echo htmlspecialchars($stepId); ?>">
        <input type="hidden" name="WorkflowId" value="<?php echo htmlspecialchars($workflowId); ?>">
        
        <div class="mb-3">
            <label class="form-label">Workflow: <strong><?php echo htmlspecialchars($workflowName); ?></strong></label>
            <div class="form-text">Workflow ID: <?php echo htmlspecialchars($workflowId); ?></div>
        </div>
        
        <div class="mb-3">
            <label for="StepName" class="form-label">ชื่อขั้นตอน</label>
            <input type="text" class="form-control" id="StepName" name="StepName" value="<?php echo htmlspecialchars($stepName); ?>" required>
        </div>
        
        <div class="mb-3">
            <label for="Seq" class="form-label">ลำดับ</label>
            <input type="number" class="form-control" id="Seq" name="Seq" value="<?php echo htmlspecialchars($stepDetails['Seq'] ?? ''); ?>" required>
        </div>
        
        <div class="mb-3">
            <label for="GroupName" class="form-label">ชื่อกลุ่ม</label>
            <input type="text" class="form-control" id="GroupName" name="GroupName" value="<?php echo htmlspecialchars($stepDetails['GroupName'] ?? ''); ?>">
        </div>
        
        <div class="modal-footer">
            <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">ยกเลิก</button>
            <button type="submit" class="btn btn-primary">บันทึก</button>
        </div>
    </form>
    <?php
} elseif ($action == 'setup') {
    // Form for setting up events
    ?>
    <form id="setupEventsForm" action="process-step-events.php" method="POST">
        <input type="hidden" name="action" value="update">
        <input type="hidden" name="WFRowId" value="<?php echo htmlspecialchars($stepId); ?>">
        <input type="hidden" name="WorkflowId" value="<?php echo htmlspecialchars($workflowId); ?>">
        <input type="hidden" name="save_type" value="all">
        
        <div class="mb-3">
            <label class="form-label">Workflow: <strong><?php echo htmlspecialchars($workflowName); ?></strong></label>
            <div class="form-text">Workflow ID: <?php echo htmlspecialchars($workflowId); ?></div>
        </div>
        
        <h5 class="text-primary"><?php echo htmlspecialchars($stepName); ?></h5>
        <p>Step RowID: <?php echo htmlspecialchars($stepId); ?></p>
        
        <div class="card mb-3">
            <div class="card-header bg-info text-white">
                <h5 class="mb-0">ตั้งค่าใช้งานปุ่มและค่าเริ่มต้น WF</h5>
            </div>
            <div class="card-body">
                <div class="mb-3 row">
                    <label class="col-sm-3 col-form-label">EnableSave</label>
                    <div class="col-sm-9">
                        <div class="row">
                            <div class="col-md-3">
                                <div class="form-check">
                                    <input type="checkbox" class="form-check-input" id="cb_enablesave" name="cb_enablesave" value="1" <?php echo ($stepDetails['EnableSave'] == 1) ? 'checked' : ''; ?>>
                                    <label class="form-check-label" for="cb_enablesave">Enable</label>
                                </div>
                            </div>
                            <div class="col-md-4">
                                <label for="AliasNameSave" class="form-label">AliasName</label>
                                <input type="text" class="form-control" id="AliasNameSave" name="AliasNameSave" value="<?php echo htmlspecialchars($stepDetails['AliasNameSave'] ?? ''); ?>">
                            </div>
                            <div class="col-md-5">
                                <label for="TriggerSave" class="form-label">Trigger condition</label>
                                <input type="text" class="form-control" id="TriggerSave" name="TriggerSave" value="<?php echo htmlspecialchars($stepDetails['TriggerSave'] ?? ''); ?>">
                                <div class="form-check">
                                    <input type="checkbox" class="form-check-input" id="HideButtonSave" name="HideButtonSave" value="1" <?php echo (isset($stepDetails['HideButtonSave']) && $stepDetails['HideButtonSave'] == 1) ? 'checked' : ''; ?>>
                                    <label class="form-check-label" for="HideButtonSave">Hide Button</label>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                
                <div class="mb-3 row">
                    <label class="col-sm-3 col-form-label">EnableApprove</label>
                    <div class="col-sm-9">
                        <div class="row">
                            <div class="col-md-3">
                                <div class="form-check">
                                    <input type="checkbox" class="form-check-input" id="cb_enableapprove" name="cb_enableapprove" value="1" <?php echo ($stepDetails['EnableApprove'] == 1) ? 'checked' : ''; ?>>
                                    <label class="form-check-label" for="cb_enableapprove">Enable</label>
                                </div>
                            </div>
                            <div class="col-md-4">
                                <label for="AliasNameApprove" class="form-label">AliasName</label>
                                <input type="text" class="form-control" id="AliasNameApprove" name="AliasNameApprove" value="<?php echo htmlspecialchars($stepDetails['AliasNameApprove'] ?? ''); ?>">
                            </div>
                            <div class="col-md-5">
                                <label for="TriggerApprove" class="form-label">Trigger condition</label>
                                <input type="text" class="form-control" id="TriggerApprove" name="TriggerApprove" value="<?php echo htmlspecialchars($stepDetails['TriggerApprove'] ?? ''); ?>">
                                <div class="form-check">
                                    <input type="checkbox" class="form-check-input" id="HideButtonApprove" name="HideButtonApprove" value="1" <?php echo (isset($stepDetails['HideButtonApprove']) && $stepDetails['HideButtonApprove'] == 1) ? 'checked' : ''; ?>>
                                    <label class="form-check-label" for="HideButtonApprove">Hide Button</label>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                
                <div class="mb-3 row">
                    <label class="col-sm-3 col-form-label">EnableReject</label>
                    <div class="col-sm-9">
                        <div class="row">
                            <div class="col-md-3">
                                <div class="form-check">
                                    <input type="checkbox" class="form-check-input" id="cb_enablereject" name="cb_enablereject" value="1" <?php echo ($stepDetails['EnableReject'] == 1) ? 'checked' : ''; ?>>
                                    <label class="form-check-label" for="cb_enablereject">Enable</label>
                                </div>
                            </div>
                            <div class="col-md-4">
                                <label for="AliasNameReject" class="form-label">AliasName</label>
                                <input type="text" class="form-control" id="AliasNameReject" name="AliasNameReject" value="<?php echo htmlspecialchars($stepDetails['AliasNameReject'] ?? ''); ?>">
                            </div>
                            <div class="col-md-5">
                                <label for="TriggerReject" class="form-label">Trigger condition</label>
                                <input type="text" class="form-control" id="TriggerReject" name="TriggerReject" value="<?php echo htmlspecialchars($stepDetails['TriggerReject'] ?? ''); ?>">
                                <div class="form-check">
                                    <input type="checkbox" class="form-check-input" id="HideButtonReject" name="HideButtonReject" value="1" <?php echo (isset($stepDetails['HideButtonReject']) && $stepDetails['HideButtonReject'] == 1) ? 'checked' : ''; ?>>
                                    <label class="form-check-label" for="HideButtonReject">Hide Button</label>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                
                <div class="mb-3 row">
                    <label class="col-sm-3 col-form-label">DefaultStart</label>
                    <div class="col-sm-9">
                        <div class="form-check">
                            <input type="checkbox" class="form-check-input" id="cb_defaultstart" name="cb_defaultstart" value="1" <?php echo ($stepDetails['DefaultStart'] == 1) ? 'checked' : ''; ?>>
                            <label class="form-check-label" for="cb_defaultstart">Default to 1st Step for Start WF</label>
                        </div>
                    </div>
                </div>
                
                <div class="mb-3 row">
                    <label class="col-sm-3 col-form-label">Dynamic Key</label>
                    <div class="col-sm-9">
                        <input type="text" class="form-control" id="DinamicKey" name="DinamicKey" value="<?php echo htmlspecialchars($stepDetails['DinamicKeyValue'] ?? ''); ?>">
                    </div>
                </div>
            </div>
        </div>
        
        <div class="modal-footer">
            <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">ยกเลิก</button>
            <button type="submit" class="btn btn-primary">บันทึก</button>
        </div>
    </form>
    <?php
} else {
    echo '<div class="alert alert-danger">Invalid action</div>';
}
?> 