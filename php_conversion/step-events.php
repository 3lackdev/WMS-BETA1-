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

// Initialize database and helper
$db = new Database();
$helper = new WorkflowHelper($db);

// Get current user
$currentUser = $auth->getCurrentUser();
$currentUsername = $currentUser['username'] ?? '';

// Get parameters
$workflowId = $_GET['WFRowId'] ?? '';
$stepId = $_GET['WFRowId'] ?? '';
$action = $_GET['action'] ?? 'list';

// Page variables
$pageTitle = "Workflow Step Events - WorkFlow Engine";
$pageHeader = "Workflow Step Events";
$pageNavigation = "<a href=\"workflowlist.php\" class=\"text-blue-600 hover:text-blue-800\">Workflow List</a>";


// Variables for views
$steps = [];
$workflowName = '';
$stepName = '';
$message = '';
$showMessage = false;
$activeView = 0; // 0 = list, 1 = edit events, 2 = new event

// Get workflow details
if (!empty($workflowId)) {
    try {
        $sql = "SELECT WFName FROM WorkFlow WHERE WFRowId = '" . $db->escapeString($workflowId) . "'";
        $result = $db->executeQuery($sql);
        
        if (count($result) > 0) {
            $workflowName = $result[0]['WFName'];
        }
    } catch (Exception $e) {
        $errorMessage = "Error getting workflow details: " . $e->getMessage();
    }
    
    // Get steps for this workflow
    try {
        $sql = "SELECT 
                s.StepRowId,
                s.WFRowId,
                s.Seq,
                s.StepName,
                s.GroupName,
                s.DefaultStart,
                s.EnableSave,
                s.EnableApprove,
                s.EnableReject,
                s.TriggerSave,
                s.TriggerApprove,
                s.TriggerReject,
                s.EventBeforeSaveCallJavascriptFunction,
                s.EventBeforeApproveCallJavascriptFunction,
                s.EventBeforeRejectCallJavascriptFunction,
                s1.StepName as aftersave,
                s2.StepName as afterapprove,
                s3.StepName as afterreject,
                s.EventAfterSaveGotoStep,
                s.EventAfterApproveGotoStep,
                s.EventAfterRejectGotoStep,
                w.WFName
            FROM WorkFlowStep s
            LEFT JOIN WorkFlowStep s1 ON s.EventAfterSaveGotoStep = s1.StepRowId
            LEFT JOIN WorkFlowStep s2 ON s.EventAfterApproveGotoStep = s2.StepRowId
            LEFT JOIN WorkFlowStep s3 ON s.EventAfterRejectGotoStep = s3.StepRowId
            LEFT JOIN WorkFlow w ON s.WFRowId = w.WFRowId
            WHERE s.WFRowId = '" . $db->escapeString($workflowId) . "'
            ORDER BY s.Seq ASC";
                
        $steps = $db->executeQuery($sql);
    } catch (Exception $e) {
        $errorMessage = "Error getting workflow steps: " . $e->getMessage();
    }
}

// Handle specific actions
if ($action == 'edit' && !empty($stepId)) {
    $activeView = 1;
    
    // Get step details
    try {
        $sql = "SELECT * FROM WorkFlowStep WHERE StepRowId = '" . $db->escapeString($stepId) . "'";
        $result = $db->executeQuery($sql);
        
        if (count($result) > 0) {
            $stepDetails = $result[0];
            $stepName = $stepDetails['StepName'];
        } else {
            header("Location: step-events.php?WFRowId=$workflowId&error=step_not_found");
            exit;
        }
    } catch (Exception $e) {
        $errorMessage = "Error getting step details: " . $e->getMessage();
    }
} elseif ($action == 'new') {
    $activeView = 2;
}

// Include header
include 'includes/header.php';

// Custom CSS for this page
?>
<style type="text/css">
    .style3 {
        height: 17px;
    }
    .style4 {
   
    }
    
    .tbrow tr {
        height: 25px;
    }
    .style5 {
        height: 249px;
    }
    
    .warenone {
        display: none;
    }
    
    #overlay {
        position: fixed;
        display: none;
        width: 100%;
        height: 100%;
        top: 0;
        left: 0;
        right: 0;
        bottom: 0;
        background: #cccccc;
        z-index: 10;
        cursor: pointer;
    }
    .textover {
        position: absolute;
        z-index: 1000000;
        right: 10px;
        width: 900px !important;
    }
</style>

<script>
    var currentobj = "";
    function on(obj) {
        try {
            document.getElementById("overlay").style.display = "block";
            currentobj = obj.id;
            $('#' + obj.id).addClass('textover');
            $('#' + obj.id).focus();
            $('#' + obj.id).val($('#' + obj.id).val());
        } catch (ex) { }
    }

    function off(obj) {
        try {
            document.getElementById("overlay").style.display = "none";
            $('#' + currentobj).removeClass('textover');
            currentobj = '';
            $('#' + currentobj).css('width', '');
        } catch (ex) { }
    }

    $(document).keypress(function (e) {
        if (e.which == 13) {
            if (e.target.name.toString().indexOf("find_txt") >= 0) {
                $("#find_field").click();
                return false;
            }

            if (e.target.name.toString().indexOf("txt_tt") >= 0) {
                $("#ss").click();
                $("#ss").focus();
                return false;
            }
        }
    });

    $(document).keydown(function (event) {
        //19 for Mac Command+S
        if (!(String.fromCharCode(event.which).toLowerCase() == 's' && event.ctrlKey) && !(event.which == 19)) return true;

        try {
            if ($("#btn_save1").val() == 'Save' || $("#btn_save1").val() == 'Save All') {
                $("#btn_save1").focus();
                $("#btn_save1").click();
            }
        } catch (ex) { }

        event.preventDefault();
        return false;
    });

    function hilightclass(obj) {
        if (obj != "") {
            $("." + obj).css("background-color", "gray");
            $("." + obj).css("color", "#ffffff");
        }
    }

    function clarehilightclass(obj) {
        if (obj != "") {
            $("." + obj).css("background-color", "");
            $("." + obj).css("color", "");
        }
    }

    function findfield() {
        var txt = $('#find_txt').val();

        if (txt != '') {
            var i = 0;
            $('#cb_fieldlist label').each(function () {
                $(this).parent().parent().show();
                $('#cb_fieldrequire_' + i).parent().parent().show();
                if ($(this).text().indexOf(txt) <= 0) {
                    $(this).parent().parent().hide();
                    $('#cb_fieldrequire_' + i).parent().parent().hide();
                    try {
                        $('#Table1 tr').eq(i).hide();
                        $('#default-' + i).parent().parent().hide();
                    } catch (ex) { }
                }
                i++;
            });
        }
        else {
            $('.tbrow').each(function () {
                $(this).children().children().show();
            });
        }
    }

    function ckall() {
        $('#cb_fieldlist input[type=checkbox]').each(function () {
            if ($(this).parent().parent().css('DISPLAY') != 'none') {
                $(this).prop('checked', 'checked');
            }
        });
    }
</script>

<div class="container-fluid">
<?php if ($activeView == 0): // List View ?>
    <div class="card">
        <div class="card-header d-flex justify-content-between align-items-center">
            <div>
                <h3 class="mb-0">Workflow Steps</h3>
                <p class="mb-0">Workflow ID: <?php echo htmlspecialchars($workflowId); ?> - <?php echo htmlspecialchars($workflowName); ?></p>
            </div>
            <button class="btn btn-success" id="btnNewEvent">New Event</button>
        </div>
        <div class="card-body">
            <div class="row mb-3">
                <div class="col-md-8">
                    <h4 class="text-primary"><?php echo htmlspecialchars($workflowName); ?></h4>
                    <button class="btn btn-secondary" onclick="window.location='workflowlist.php'">Back</button>
                </div>
                <div class="col-md-4">
                    <div class="input-group">
                        <input type="text" name="txt_tt" id="txt_tt" class="form-control" placeholder="Search...">
                        <button class="btn btn-outline-secondary" type="button" id="ss">Search</button>
                    </div>
                </div>
            </div>
            
            <div class="table-responsive">
                <table class="table table-striped table-bordered">
                    <thead>
                        <tr>
                            <th>Seq</th>
                            <th>Name</th>
                            <th>Group</th>
                            <th>Default Start</th>
                            <th>After Save</th>
                            <th>After Approve</th>
                            <th>After Reject</th>
                            <th>Actions</th>
                        </tr>
                    </thead>
                    <tbody>
                        <?php foreach ($steps as $step): ?>
                            <tr>
                                <td><?php echo htmlspecialchars($step['Seq']); ?></td>
                                <td><?php echo htmlspecialchars($step['StepName']); ?></td>
                                <td><?php echo htmlspecialchars($step['GroupName']); ?></td>
                                <td><?php echo $step['DefaultStart'] ? 'Yes' : 'No'; ?></td>
                                <td class="<?php echo htmlspecialchars($step['EventAfterSaveGotoStep'] ?? ''); ?>"><?php echo htmlspecialchars($step['aftersave'] ?? ''); ?></td>
                                <td class="<?php echo htmlspecialchars($step['EventAfterApproveGotoStep'] ?? ''); ?>"><?php echo htmlspecialchars($step['afterapprove'] ?? ''); ?></td>
                                <td class="<?php echo htmlspecialchars($step['EventAfterRejectGotoStep'] ?? ''); ?>"><?php echo htmlspecialchars($step['afterreject'] ?? ''); ?></td>
                                <td>
                                    <a href="javascript:void(0);" class="btn-edit-event" data-stepid="<?php echo $step['StepRowId']; ?>">Edit</a> | 
                                    <a href="javascript:void(0);" class="btn-setup-events" data-stepid="<?php echo $step['StepRowId']; ?>">Setup</a> | 
                                    <a href="javascript:void(0);" class="btn-copy-step" data-stepid="<?php echo $step['StepRowId']; ?>" data-wfid="<?php echo $workflowId; ?>">Copy</a> | 
                                    <a href="javascript:void(0);" class="btn-delete-step" data-stepid="<?php echo $step['StepRowId']; ?>" data-wfid="<?php echo $workflowId; ?>" data-stepname="<?php echo htmlspecialchars($step['StepName']); ?>">Delete</a>
                                </td>
                            </tr>
                        <?php endforeach; ?>
                    </tbody>
                </table>
            </div>
            
            <div class="text-center mt-3">
                <button class="btn btn-secondary" onclick="window.location='workflowlist.php'">Back</button>
            </div>
        </div>
    </div>
<?php elseif ($activeView == 1): // Edit Events View ?>
    <div class="card">
        <div class="card-header bg-primary text-white">
            <h3 class="mb-0">Step & Events</h3>
        </div>
        <div class="card-body">
            <h4 class="text-primary"><?php echo htmlspecialchars($stepName); ?></h4>
            <p>Step RowID: <?php echo htmlspecialchars($stepId); ?></p>
            
            <div class="mb-3">
                <button class="btn btn-secondary" onclick="window.location='step-events.php?WFRowId=<?php echo $workflowId; ?>'">Back</button>
                <button class="btn btn-primary" id="btn_save1" onclick="saveEvents('all')">Save All</button>
                <button class="btn btn-info" id="btn_save2" onclick="saveEvents('left')">Save leftpart only</button>
                <?php if (isset($_GET['success'])): ?>
                    <span class="text-success fw-bold">Data saved successfully</span>
                <?php elseif (isset($_GET['error'])): ?>
                    <span class="text-danger fw-bold">Error: <?php echo htmlspecialchars($_GET['error']); ?></span>
                <?php endif; ?>
            </div>
            
            <hr>
            
            <!-- Event Form -->
            <form id="event_form" method="post" action="process-step-events.php">
                <input type="hidden" name="action" value="update">
                <input type="hidden" name="WFRowId" value="<?php echo htmlspecialchars($stepId); ?>">
                <input type="hidden" name="wfid" value="<?php echo htmlspecialchars($workflowId); ?>">
                <input type="hidden" name="save_type" id="save_type" value="all">
                
                <!-- Form content will be implemented here -->
                <div class="row">
                    <div class="col-md-8">
                        <!-- Left part of the form -->
                        <div class="card mb-3">
                            <div class="card-header bg-info text-white">
                                <h5 class="mb-0">Setup Options</h5>
                            </div>
                            <div class="card-body">
                                <div class="mb-3 row">
                                    <label class="col-sm-3 col-form-label">EnableSave</label>
                                    <div class="col-sm-9">
                                        <div class="row">
                                            <div class="col-md-3">
                                                <div class="form-check">
                                                    <input type="checkbox" class="form-check-input" id="cb_enablesave" name="cb_enablesave" value="1" <?php echo (isset($stepDetails) && $stepDetails['EnableSave'] == 1) ? 'checked' : ''; ?>>
                                                    <label class="form-check-label" for="cb_enablesave">Enable</label>
                                                </div>
                                            </div>
                                            <div class="col-md-4">
                                                <label for="AliasNameSave" class="form-label">AliasName</label>
                                                <input type="text" class="form-control" id="AliasNameSave" name="AliasNameSave" value="<?php echo isset($stepDetails) ? htmlspecialchars($stepDetails['AliasNameSave'] ?? '') : ''; ?>">
                                            </div>
                                            <div class="col-md-5">
                                                <label for="TriggerSave" class="form-label">Trigger condition (Auto Press)</label>
                                                <input type="text" class="form-control" id="TriggerSave" name="TriggerSave" value="<?php echo isset($stepDetails) ? htmlspecialchars($stepDetails['TriggerSave'] ?? '') : ''; ?>">
                                                <div class="form-check">
                                                    <input type="checkbox" class="form-check-input" id="HideButtonSave" name="HideButtonSave" value="1" <?php echo (isset($stepDetails) && $stepDetails['HideButtonSave'] == 1) ? 'checked' : ''; ?>>
                                                    <label class="form-check-label" for="HideButtonSave">HideBtn</label>
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
                                                    <input type="checkbox" class="form-check-input" id="cb_enableapprove" name="cb_enableapprove" value="1" <?php echo (isset($stepDetails) && $stepDetails['EnableApprove'] == 1) ? 'checked' : ''; ?>>
                                                    <label class="form-check-label" for="cb_enableapprove">Enable</label>
                                                </div>
                                            </div>
                                            <div class="col-md-4">
                                                <label for="AliasNameApprove" class="form-label">AliasName</label>
                                                <input type="text" class="form-control" id="AliasNameApprove" name="AliasNameApprove" value="<?php echo isset($stepDetails) ? htmlspecialchars($stepDetails['AliasNameApprove'] ?? '') : ''; ?>">
                                            </div>
                                            <div class="col-md-5">
                                                <label for="TriggerApprove" class="form-label">Trigger condition (Auto Press)</label>
                                                <input type="text" class="form-control" id="TriggerApprove" name="TriggerApprove" value="<?php echo isset($stepDetails) ? htmlspecialchars($stepDetails['TriggerApprove'] ?? '') : ''; ?>">
                                                <div class="form-check">
                                                    <input type="checkbox" class="form-check-input" id="HideButtonApprove" name="HideButtonApprove" value="1" <?php echo (isset($stepDetails) && $stepDetails['HideButtonApprove'] == 1) ? 'checked' : ''; ?>>
                                                    <label class="form-check-label" for="HideButtonApprove">HideBtn</label>
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
                                                    <input type="checkbox" class="form-check-input" id="cb_enablereject" name="cb_enablereject" value="1" <?php echo (isset($stepDetails) && $stepDetails['EnableReject'] == 1) ? 'checked' : ''; ?>>
                                                    <label class="form-check-label" for="cb_enablereject">Enable</label>
                                                </div>
                                            </div>
                                            <div class="col-md-4">
                                                <label for="AliasNameReject" class="form-label">AliasName</label>
                                                <input type="text" class="form-control" id="AliasNameReject" name="AliasNameReject" value="<?php echo isset($stepDetails) ? htmlspecialchars($stepDetails['AliasNameReject'] ?? '') : ''; ?>">
                                            </div>
                                            <div class="col-md-5">
                                                <label for="TriggerReject" class="form-label">Trigger condition (Auto Press)</label>
                                                <input type="text" class="form-control" id="TriggerReject" name="TriggerReject" value="<?php echo isset($stepDetails) ? htmlspecialchars($stepDetails['TriggerReject'] ?? '') : ''; ?>">
                                                <div class="form-check">
                                                    <input type="checkbox" class="form-check-input" id="HideButtonReject" name="HideButtonReject" value="1" <?php echo (isset($stepDetails) && $stepDetails['HideButtonReject'] == 1) ? 'checked' : ''; ?>>
                                                    <label class="form-check-label" for="HideButtonReject">HideBtn</label>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                
                                <div class="mb-3 row">
                                    <label class="col-sm-3 col-form-label">DefaultStart</label>
                                    <div class="col-sm-9">
                                        <div class="form-check">
                                            <input type="checkbox" class="form-check-input" id="cb_defaultstart" name="cb_defaultstart" value="1" <?php echo (isset($stepDetails) && $stepDetails['DefaultStart'] == 1) ? 'checked' : ''; ?>>
                                            <label class="form-check-label" for="cb_defaultstart">Default to 1st Step for Start WF</label>
                                        </div>
                                    </div>
                                </div>
                                
                                <div class="mb-3 row">
                                    <label class="col-sm-3 col-form-label">Dynamic Key</label>
                                    <div class="col-sm-9">
                                        <input type="text" class="form-control" id="DinamicKey" name="DinamicKey" value="<?php echo isset($stepDetails) ? htmlspecialchars($stepDetails['DinamicKeyValue'] ?? '') : ''; ?>">
                                    </div>
                                </div>
                                
                                <!-- Additional form fields can be added here -->
                            </div>
                        </div>
                    </div>
                    
                    <div class="col-md-4">
                        <!-- Right part of the form -->
                        <div class="card">
                            <div class="card-header bg-info text-white">
                                <h5 class="mb-0">Objects</h5>
                            </div>
                            <div class="card-body">
                                <div class="mb-2">
                                    <input type="text" id="find_txt" name="find_txt" class="form-control form-control-sm" placeholder="Find...">
                                    <button type="button" id="find_field" class="btn btn-sm btn-outline-primary mt-1" onclick="findfield()">Find</button>
                                </div>
                                
                                <hr>
                                
                                <h6>Enable Input: Define which controls to allow key values</h6>
                                
                                <div class="table-responsive">
                                    <table class="table table-sm">
                                        <thead>
                                            <tr>
                                                <th><a href="javascript:void(0)" onclick="ckall()">Select Enable Object</a></th>
                                                <th>Require Field</th>
                                                <th>Default Value</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                            <!-- This will be populated with dynamic data -->
                                            <tr>
                                                <td colspan="3">No objects available</td>
                                            </tr>
                                        </tbody>
                                    </table>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                
                <div class="text-center mt-3">
                    <button type="submit" class="btn btn-primary">Save</button>
                    <button type="button" class="btn btn-secondary" onclick="window.location='step-events.php?WFRowId=<?php echo $workflowId; ?>'">Back</button>
                </div>
            </form>
            
            <hr>
        </div>
    </div>
<?php elseif ($activeView == 2): // New Event View ?>
    <div class="card">
        <div class="card-header bg-primary text-white">
            <h3 class="mb-0">New Event</h3>
        </div>
        <div class="card-body">
            <form id="newEventForm" method="post" action="process-step-events.php">
                <input type="hidden" name="action" value="create">
                <input type="hidden" name="WorkflowId" value="<?php echo htmlspecialchars($workflowId); ?>">
                
                <div class="mb-3 row">
                    <label for="txt_eventname" class="col-sm-2 col-form-label">Step Name</label>
                    <div class="col-sm-10">
                        <input type="text" class="form-control" id="txt_eventname" name="txt_eventname" required>
                    </div>
                </div>
                
                <div class="mb-3 row">
                    <label for="txt_seq" class="col-sm-2 col-form-label">Sequence</label>
                    <div class="col-sm-10">
                        <input type="number" class="form-control" id="txt_seq" name="txt_seq" required>
                    </div>
                </div>
                
                <div class="mb-3 row">
                    <label for="txt_group" class="col-sm-2 col-form-label">Group Name</label>
                    <div class="col-sm-10">
                        <input type="text" class="form-control" id="txt_group" name="txt_group">
                    </div>
                </div>
                
                <div class="mt-3">
                    <button type="submit" class="btn btn-primary">Save</button>
                    <button type="button" class="btn btn-secondary" onclick="window.location='step-events.php?WFRowId=<?php echo $workflowId; ?>'">Back</button>
                </div>
            </form>
        </div>
    </div>
<?php endif; ?>
</div>

<!-- Modal สำหรับ New Event -->
<div class="modal fade" id="newEventModal" tabindex="-1" aria-labelledby="newEventModalLabel" aria-hidden="true">
  <div class="modal-dialog modal-lg">
    <div class="modal-content">
      <div class="modal-header">
        <h5 class="modal-title" id="newEventModalLabel">Create New Event</h5>
        <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
      </div>
      <div class="modal-body">
        <form id="newEventForm" action="process-step-events.php" method="POST">
          <input type="hidden" name="action" value="create">
          <input type="hidden" name="WorkflowId" value="<?php echo htmlspecialchars($workflowId); ?>">
          
          <!-- แสดงข้อมูล workflow เพื่อให้ผู้ใช้เห็นว่ากำลังสร้าง event ให้กับ workflow ไหน -->
          <div class="mb-3">
            <label class="form-label">Workflow: <strong id="workflow-info-name"><?php echo htmlspecialchars($workflowName); ?></strong></label>
            <div class="form-text">Workflow ID: <span id="workflow-info-id"><?php echo htmlspecialchars($workflowId); ?></span></div>
          </div>
          
          <div class="mb-3">
            <label for="txt_eventname" class="form-label">Step Name</label>
            <input type="text" class="form-control" id="txt_eventname" name="txt_eventname" required>
          </div>
          
          <div class="mb-3">
            <label for="txt_seq" class="form-label">Sequence</label>
            <input type="number" class="form-control" id="txt_seq" name="txt_seq" required>
          </div>
          
          <div class="mb-3">
            <label for="txt_group" class="form-label">Group Name</label>
            <input type="text" class="form-control" id="txt_group" name="txt_group">
          </div>
          
          <div class="modal-footer">
            <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancel</button>
            <button type="submit" class="btn btn-primary">Save</button>
          </div>
        </form>
      </div>
    </div>
  </div>
</div>

<!-- Modal สำหรับ Edit Event -->
<div class="modal fade" id="editEventModal" tabindex="-1" aria-labelledby="editEventModalLabel" aria-hidden="true">
  <div class="modal-dialog modal-lg">
    <div class="modal-content">
      <div class="modal-header">
        <h5 class="modal-title" id="editEventModalLabel">Edit Event</h5>
        <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
      </div>
      <div class="modal-body" id="editModalContent">
        <!-- Content will be loaded here via AJAX -->
      </div>
    </div>
  </div>
</div>

<!-- Modal สำหรับ Setup Events -->
<div class="modal fade" id="setupEventsModal" tabindex="-1" aria-labelledby="setupEventsModalLabel" aria-hidden="true">
  <div class="modal-dialog modal-lg">
    <div class="modal-content">
      <div class="modal-header">
        <h5 class="modal-title" id="setupEventsModalLabel">Setup Events</h5>
        <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
      </div>
      <div class="modal-body" id="setupEventsModalContent">
        <!-- Content will be loaded here via AJAX -->
      </div>
    </div>
  </div>
</div>

<!-- Confirmation Modal for Delete -->
<div class="modal fade" id="deleteConfirmModal" tabindex="-1" aria-labelledby="deleteConfirmModalLabel" aria-hidden="true">
  <div class="modal-dialog">
    <div class="modal-content">
      <div class="modal-header">
        <h5 class="modal-title" id="deleteConfirmModalLabel">Confirm Delete</h5>
        <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
      </div>
      <div class="modal-body">
        Are you sure you want to delete the step "<span id="deleteStepName"></span>"?
      </div>
      <div class="modal-footer">
        <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancel</button>
        <a href="#" id="deleteConfirmBtn" class="btn btn-danger">Delete</a>
      </div>
    </div>
  </div>
</div>

<!-- Confirmation Modal for Copy -->
<div class="modal fade" id="copyConfirmModal" tabindex="-1" aria-labelledby="copyConfirmModalLabel" aria-hidden="true">
  <div class="modal-dialog">
    <div class="modal-content">
      <div class="modal-header">
        <h5 class="modal-title" id="copyConfirmModalLabel">Confirm Copy</h5>
        <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
      </div>
      <div class="modal-body">
        Are you sure you want to create a copy of this step?
      </div>
      <div class="modal-footer">
        <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancel</button>
        <a href="#" id="copyConfirmBtn" class="btn btn-primary">Copy</a>
      </div>
    </div>
  </div>
</div>

<div id="overlay" onclick="off(this)"></div>

<script>
// Updated script for form handling
function saveEvents(type) {
    document.getElementById('save_type').value = type;
    document.getElementById('event_form').submit();
}

// Toast notification function
function showToast(message, title = 'Notification', type = 'info') {
    const toast = new bootstrap.Toast(document.getElementById('liveToast'));
    $('#toast-title').text(title);
    $('#toast-message').text(message);
    
    // Reset classes
    $('#liveToast').removeClass('bg-success bg-danger bg-info');
    
    // Set the appropriate class based on type
    if (type === 'success') {
        $('#liveToast').addClass('bg-success text-white');
    } else if (type === 'error') {
        $('#liveToast').addClass('bg-danger text-white');
    } else {
        $('#liveToast').addClass('bg-info text-white');
    }
    
    toast.show();
}

function loadEditForm(stepId) {
    $('#editModalContent').html('<div class="text-center"><div class="spinner-border" role="status"><span class="visually-hidden">Loading...</span></div></div>');
    $('#editEventModal').modal('show');
    
    $.ajax({
        url: 'get-step-form.php',
        type: 'GET',
        data: {
            action: 'edit',
            stepId: stepId,
            workflowId: '<?php echo $workflowId; ?>'
        },
        success: function(response) {
            $('#editModalContent').html(response);
        },
        error: function(xhr, status, error) {
            $('#editModalContent').html('<div class="alert alert-danger">Error loading form: ' + error + '</div>');
            showToast('Error loading form: ' + error, 'Error', 'error');
        }
    });
}

function loadSetupEventsForm(stepId) {
    $('#setupEventsModalContent').html('<div class="text-center"><div class="spinner-border" role="status"><span class="visually-hidden">Loading...</span></div></div>');
    $('#setupEventsModal').modal('show');
    
    $.ajax({
        url: 'get-step-form.php',
        type: 'GET',
        data: {
            action: 'setup',
            stepId: stepId,
            workflowId: '<?php echo $workflowId; ?>'
        },
        success: function(response) {
            $('#setupEventsModalContent').html(response);
        },
        error: function(xhr, status, error) {
            $('#setupEventsModalContent').html('<div class="alert alert-danger">Error loading form: ' + error + '</div>');
            showToast('Error loading form: ' + error, 'Error', 'error');
        }
    });
}

// Edit buttons and links to open modals instead of changing pages
$(document).ready(function() {
    // Check and display workflowId and workflow name in console for verification
    console.log("Workflow ID: <?php echo $workflowId; ?>");
    console.log("Workflow Name: <?php echo $workflowName; ?>");
    
    // For New Event
    $('#btnNewEvent').click(function(e) {
        e.preventDefault();
        $('#newEventModal').modal('show');
    });
    
    // For Edit Event
    $(document).on('click', '.btn-edit-event', function(e) {
        e.preventDefault();
        var stepId = $(this).data('stepid');
        loadEditForm(stepId);
    });
    
    // For Setup Events
    $(document).on('click', '.btn-setup-events', function(e) {
        e.preventDefault();
        var stepId = $(this).data('stepid');
        loadSetupEventsForm(stepId);
    });
    
    // Submit New Event form with AJAX
    $(document).on('submit', '#newEventForm', function(e) {
        e.preventDefault();
        
        // Check workflowId before sending AJAX request
        var wfid = $('input[name="WorkflowId"]', this).val();
        console.log("Submitting form with wfid:", wfid);
        
        if (!wfid) {
            showToast("Workflow ID not found. Please refresh the page and try again.", "Error", "error");
            return;
        }
        
        $.ajax({
            url: $(this).attr('action'),
            type: 'POST',
            data: $(this).serialize(),
            dataType: 'json',
            success: function(response) {
                if (response.success) {
                    // Show success message
                    showToast(response.message || 'Data saved successfully', 'Success', 'success');
                    // Close modal
                    $('#newEventModal').modal('hide');
                    // Reload page to show new data
                    setTimeout(function() {
                        location.reload();
                    }, 1000);
                } else {
                    // Show error message
                    showToast(response.message || 'Error saving data', 'Error', 'error');
                }
            },
            error: function() {
                showToast('Error connecting to server', 'Error', 'error');
            }
        });
    });
    
    // Submit Edit Event form with AJAX
    $(document).on('submit', '#editForm', function(e) {
        e.preventDefault();
        $.ajax({
            url: $(this).attr('action'),
            type: 'POST',
            data: $(this).serialize(),
            dataType: 'json',
            success: function(response) {
                if (response.success) {
                    // Show success message
                    showToast(response.message || 'Data saved successfully', 'Success', 'success');
                    // Close modal
                    $('#editEventModal').modal('hide');
                    // Reload page to show updated data
                    setTimeout(function() {
                        location.reload();
                    }, 1000);
                } else {
                    // Show error message
                    showToast(response.message || 'Error saving data', 'Error', 'error');
                }
            },
            error: function() {
                showToast('Error connecting to server', 'Error', 'error');
            }
        });
    });
    
    // Submit Setup Events form with AJAX
    $(document).on('submit', '#setupEventsForm', function(e) {
        e.preventDefault();
        $.ajax({
            url: $(this).attr('action'),
            type: 'POST',
            data: $(this).serialize(),
            dataType: 'json',
            success: function(response) {
                if (response.success) {
                    // Show success message
                    showToast(response.message || 'Data saved successfully', 'Success', 'success');
                    // Close modal
                    $('#setupEventsModal').modal('hide');
                    // Reload page to show updated data
                    setTimeout(function() {
                        location.reload();
                    }, 1000);
                } else {
                    // Show error message
                    showToast(response.message || 'Error saving data', 'Error', 'error');
                }
            },
            error: function() {
                showToast('Error connecting to server', 'Error', 'error');
            }
        });
    });

    // For Copy button
    $(document).on('click', '.btn-copy-step', function(e) {
        e.preventDefault();
        var stepId = $(this).data('stepid');
        var workflowId = $(this).data('wfid');
        
        // Set up the confirmation button URL
        $('#copyConfirmBtn').attr('href', 'process-step-events.php?action=copy&WFRowId=' + stepId + '&WorkflowId=' + workflowId);
        
        // Show the modal
        $('#copyConfirmModal').modal('show');
    });
    
    // For Delete button
    $(document).on('click', '.btn-delete-step', function(e) {
        e.preventDefault();
        var stepId = $(this).data('stepid');
        var workflowId = $(this).data('wfid');
        var stepName = $(this).data('stepname');
        
        // Set the step name in the confirmation message
        $('#deleteStepName').text(stepName);
        
        // Set up the confirmation button URL
        $('#deleteConfirmBtn').attr('href', 'process-step-events.php?action=delete&WFRowId=' + stepId + '&WorkflowId=' + workflowId);
        
        // Show the modal
        $('#deleteConfirmModal').modal('show');
    });
});
</script>

<!-- Add this toast container before the closing body tag -->
<div class="position-fixed bottom-0 end-0 p-3" style="z-index: 11">
  <div id="liveToast" class="toast" role="alert" aria-live="assertive" aria-atomic="true">
    <div class="toast-header">
      <strong class="me-auto" id="toast-title">Notification</strong>
      <button type="button" class="btn-close" data-bs-dismiss="toast" aria-label="Close"></button>
    </div>
    <div class="toast-body" id="toast-message">
      Message goes here.
    </div>
  </div>
</div>

<?php
// Include footer
include 'includes/footer.php';
?>

</body>
</html>
