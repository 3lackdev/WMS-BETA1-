<!DOCTYPE html>
<html lang="en">

<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title><?php echo isset($pageTitle) ? $pageTitle : 'WorkFlow Engine'; ?></title>

    <!-- Tailwind CSS -->
    <script src="https://cdn.tailwindcss.com"></script>

    <!-- Include Bootstrap 5, jQuery and Summernote CSS and JS -->
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.1.3/dist/css/bootstrap.min.css" rel="stylesheet">
    <link href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/5.15.4/css/all.min.css" rel="stylesheet">
    <link href="https://cdn.jsdelivr.net/npm/summernote@0.9.1/dist/summernote-bs5.min.css" rel="stylesheet">
    <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.1.3/dist/js/bootstrap.bundle.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/summernote@0.9.1/dist/summernote-bs5.min.js"></script>


    <!-- Additional CSS -->
    <?php if (isset($additionalCss)): ?>
        <?php foreach ($additionalCss as $css): ?>
            <link rel="stylesheet" href="<?php echo $css; ?>">
        <?php endforeach; ?>
    <?php endif; ?>

    <!-- Additional JavaScript -->
    <?php if (isset($additionalJs)): ?>
        <?php foreach ($additionalJs as $js): ?>
            <script src="<?php echo $js; ?>"></script>
        <?php endforeach; ?>
    <?php endif; ?>

    <!-- Tailwind Config -->
    <script>
        tailwind.config = {
            theme: {
                extend: {
                    colors: {
                        primary: '#3490dc',
                        secondary: '#ffed4a',
                        danger: '#e3342f',
                        success: '#38c172',
                        warning: '#f6993f',
                        info: '#6cb2eb',
                    }
                }
            }
        }
    </script>

    <style>
        /* CSS for the dashboard module layout */
        .dashboard-module {
            @apply inline-block w-30 h-30 text-center p-3 m-3 border border-gray-300 rounded-lg bg-white hover:bg-gray-100 hover:shadow-md transition-all;
        }

        .dashboard-module img {
            @apply mb-2 mx-auto;
        }

        .dashboard-module span {
            @apply block text-sm;
        }

        .grid_7 {
            @apply p-3;
        }
    </style>
</head>

<body class="bg-gray-50">
    <!-- Navigation Bar -->
    <nav class="bg-gray-800 text-white shadow-md">
        <div class="container-fluid px-4 py-2">
            <div class="flex flex-wrap items-center justify-between">
                <a class="text-xl font-bold" href="index.php">WorkFlow Engine</a>
                <button class="lg:hidden border border-gray-600 px-2 py-1 rounded" type="button" data-toggle="collapse" data-target="#navbarNav" aria-controls="navbarNav" aria-expanded="false" aria-label="Toggle navigation">
                    <span class="navbar-toggler-icon"></span>
                </button>
                <div class="collapse navbar-collapse" id="navbarNav">
                    <ul class="navbar-nav mr-auto">
                        <li class="nav-item">
                            <a class="nav-link hover:text-gray-300" href="index.php">Home</a>
                        </li>
                        <li class="nav-item">
                            <a class="nav-link hover:text-gray-300" href="workflowlist.php">Workflows</a>
                        </li>
                        <?php if (isset($_SESSION['user_role']) && $_SESSION['user_role'] == 'admin'): ?>
                            <li class="nav-item dropdown">
                                <a class="nav-link dropdown-toggle" href="#" id="adminDropdown" role="button" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">
                                    Admin
                                </a>
                                <div class="dropdown-menu" aria-labelledby="adminDropdown">
                                    <a class="dropdown-item" href="template.php">Templates</a>
                                    <a class="dropdown-item" href="Position.php">Positions</a>
                                    <a class="dropdown-item" href="Databind.php">Data Binding</a>
                                </div>
                            </li>
                        <?php endif; ?>
                    </ul>
                    <ul class="navbar-nav">
                        <?php if (isset($_SESSION['user_id'])): ?>
                            <li class="nav-item dropdown">
                                <a class="nav-link dropdown-toggle" href="#" id="userDropdown" role="button" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">
                                    <i class="fa fa-user"></i> <?php echo htmlspecialchars($_SESSION['username'] ?? 'User'); ?>
                                </a>
                                <div class="dropdown-menu dropdown-menu-right" aria-labelledby="userDropdown">
                                    <a class="dropdown-item" href="profile.php">Profile</a>
                                    <div class="dropdown-divider"></div>
                                    <a class="dropdown-item" href="logout.php">Logout</a>
                                </div>
                            </li>
                        <?php else: ?>
                            <li class="nav-item">
                                <a class="nav-link" href="login.php">Login</a>
                            </li>
                        <?php endif; ?>
                    </ul>
                </div>
            </div>
        </div>
    </nav>

    <!-- Navigation Breadcrumb -->
    <div class="bg-gray-100 py-2 shadow-sm">
        <div class="container-fluid">
            <div class="row">
                <div class="col-12">
                    <nav aria-label="breadcrumb">
                        <ol class="breadcrumb mb-0 bg-transparent">
                            <li class="breadcrumb-item">
                                <?php
                                // Display the main navigation link (from master page)
                                if (isset($pageNavigation)) {
                                    echo $pageNavigation;
                                } else {
                                    echo '<a href="index.php" class="text-blue-600 hover:text-blue-800">หน้าแรกโปรแกรม</a>';
                                }
                                ?>
                            </li>
                            <?php if (isset($pageHeader)): ?>
                                <li class="breadcrumb-item active" aria-current="page"><?php echo $pageHeader; ?></li>
                            <?php endif; ?>
                        </ol>
                    </nav>
                </div>
            </div>
        </div>
    </div>

    <!-- Main Content Container -->
    <div class="container-fluid py-4"><?php if (isset($pageHeader)): ?>
            <div class="row">
                <div class="col-12">
                    <h1 class="text-2xl font-bold mb-3"><?php echo $pageHeader; ?></h1>
                </div>
            </div>
        <?php endif; ?>