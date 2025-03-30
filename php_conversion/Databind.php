<?php
require_once 'includes/Database.php';
require_once 'includes/Authentication.php';

// Initialize authentication
$auth = new Authentication();

// Check if user is authenticated
if (!$auth->isAuthenticated()) {
    // Redirect to login page if not authenticated
    header("Location: login.php");
    exit;
}

// Initialize database
$db = new Database();

// Get data bind values
$dataBind = $db->executeQuery("SELECT * FROM DataBindValue ORDER BY BindName");

// Set page title and header
$pageTitle = "Data Binding - WorkFlow Engine";
$pageHeader = "Data Binding";
$pageNavigation = "<a href=\"index.php\">หน้าแรกโปรแกรม</a>";

// Include header
include 'includes/header.php';
?>

<div class="row mb-4">
    <div class="col-md-12">
        <div class="card">
            <div class="card-header">
                <div class="row">
                    <div class="col-md-8">
                        <h5>Data Bind Values</h5>
                    </div>
                    <div class="col-md-4 text-right">
                        <button type="button" class="btn btn-success" data-toggle="modal" data-target="#addDataBindModal">
                            <i class="fa fa-plus"></i> Add New
                        </button>
                    </div>
                </div>
            </div>
            <div class="card-body">
                <?php if (is_array($dataBind) && count($dataBind) > 0): ?>
                <div class="table-responsive">
                    <table class="table table-striped table-bordered">
                        <thead>
                            <tr>
                                <th>Bind Name</th>
                                <th>Value</th>
                                <th>Description</th>
                                <th>SQL</th>
                                <th>Actions</th>
                            </tr>
                        </thead>
                        <tbody>
                            <?php foreach ($dataBind as $item): ?>
                            <tr>
                                <td><?php echo htmlspecialchars($item['BindName'] ?? ''); ?></td>
                                <td><?php echo htmlspecialchars($item['Value'] ?? ''); ?></td>
                                <td><?php echo htmlspecialchars($item['Description'] ?? ''); ?></td>
                                <td>
                                    <?php if (!empty($item['SqlStatement'] ?? '')): ?>
                                    <button type="button" class="btn btn-sm btn-info" data-toggle="modal" data-target="#viewSqlModal" 
                                           data-sql="<?php echo htmlspecialchars($item['SqlStatement'] ?? ''); ?>">
                                        <i class="fa fa-code"></i> View SQL
                                    </button>
                                    <?php else: ?>
                                    <span class="text-muted">No SQL</span>
                                    <?php endif; ?>
                                </td>
                                <td>
                                    <button type="button" class="btn btn-sm btn-warning" data-toggle="modal" data-target="#editDataBindModal"
                                           data-id="<?php echo $item['ID'] ?? 0; ?>"
                                           data-name="<?php echo htmlspecialchars($item['BindName'] ?? ''); ?>"
                                           data-value="<?php echo htmlspecialchars($item['Value'] ?? ''); ?>"
                                           data-description="<?php echo htmlspecialchars($item['Description'] ?? ''); ?>"
                                           data-sql="<?php echo htmlspecialchars($item['SqlStatement'] ?? ''); ?>">
                                        <i class="fa fa-edit"></i> Edit
                                    </button>
                                    <button type="button" class="btn btn-sm btn-danger" data-toggle="modal" data-target="#deleteDataBindModal"
                                           data-id="<?php echo $item['ID'] ?? 0; ?>"
                                           data-name="<?php echo htmlspecialchars($item['BindName'] ?? ''); ?>">
                                        <i class="fa fa-trash"></i> Delete
                                    </button>
                                </td>
                            </tr>
                            <?php endforeach; ?>
                        </tbody>
                    </table>
                </div>
                <?php else: ?>
                <div class="alert alert-info">
                    <i class="fa fa-info-circle"></i> No data bind values found.
                </div>
                <?php endif; ?>
            </div>
        </div>
    </div>
</div>

<!-- View SQL Modal -->
<div class="modal fade" id="viewSqlModal" tabindex="-1" role="dialog" aria-labelledby="viewSqlModalLabel" aria-hidden="true">
    <div class="modal-dialog modal-lg" role="document">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title" id="viewSqlModalLabel">SQL Statement</h5>
                <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                    <span aria-hidden="true">&times;</span>
                </button>
            </div>
            <div class="modal-body">
                <pre id="sqlContent" class="p-3 bg-light"></pre>
            </div>
            <div class="modal-footer">
                <button type="button" class="btn btn-secondary" data-dismiss="modal">Close</button>
            </div>
        </div>
    </div>
</div>

<?php
// Add JavaScript for modal functionality
$pageScript = <<<JS
$(document).ready(function() {
    // Handle View SQL modal
    $('#viewSqlModal').on('show.bs.modal', function (event) {
        var button = $(event.relatedTarget);
        var sql = button.data('sql');
        $('#sqlContent').text(sql);
    });
});
JS;

// Include footer
include 'includes/footer.php';
?> 