// Manager Dashboard JavaScript

document.addEventListener('DOMContentLoaded', function() {
    console.log('Manager Dashboard initialized');
    
    // Initialize dashboard components
    initializeDashboard();
    
    // Auto-close user dropdown when clicking outside
    document.addEventListener('click', function(event) {
        const userDropdown = document.getElementById('userDropdown');
        const userMenuBtn = document.querySelector('.user-menu-btn');
        
        if (userDropdown && !userMenuBtn.contains(event.target)) {
            userDropdown.classList.remove('show');
        }
    });
});

// Toggle sidebar for mobile
function toggleSidebar() {
    const sidebar = document.getElementById('managerSidebar');
    if (sidebar) {
        sidebar.classList.toggle('show');
    }
}

// Toggle user dropdown menu
function toggleUserMenu() {
    const dropdown = document.getElementById('userDropdown');
    if (dropdown) {
        dropdown.classList.toggle('show');
    }
}

// Logout function
function logout() {
    if (confirm('Are you sure you want to logout?')) {
        // Clear any stored authentication data
        localStorage.removeItem('authToken');
        localStorage.removeItem('userInfo');
        sessionStorage.clear();
        
        // Redirect to login page
        window.location.href = '/Authentication/Login';
    }
}

// Initialize dashboard
function initializeDashboard() {
    // Add any initialization logic here
    console.log('Dashboard components initialized');
    
    // Example: Load notifications
    loadNotifications();
    
    // Example: Update dashboard stats
    updateDashboardStats();
}

// Load notifications
function loadNotifications() {
    // Simulate loading notifications
    console.log('Loading notifications...');
}

// Update dashboard statistics
function updateDashboardStats() {
    // This would typically fetch real data from the server
    console.log('Updating dashboard statistics...');
}

// Utility function to show toast notifications
function showToast(message, type = 'info') {
    // Create toast element
    const toast = document.createElement('div');
    toast.className = `toast toast-${type}`;
    toast.innerHTML = `
        <div class="toast-content">
            <i class="fas fa-${getToastIcon(type)}"></i>
            <span>${message}</span>
        </div>
        <button class="toast-close" onclick="this.parentElement.remove()">
            <i class="fas fa-times"></i>
        </button>
    `;
    
    // Add toast to container or create one
    let toastContainer = document.getElementById('toastContainer');
    if (!toastContainer) {
        toastContainer = document.createElement('div');
        toastContainer.id = 'toastContainer';
        toastContainer.className = 'toast-container';
        document.body.appendChild(toastContainer);
    }
    
    toastContainer.appendChild(toast);
    
    // Auto remove after 5 seconds
    setTimeout(() => {
        if (toast.parentElement) {
            toast.remove();
        }
    }, 5000);
}

// Get icon for toast type
function getToastIcon(type) {
    switch (type) {
        case 'success': return 'check-circle';
        case 'error': return 'exclamation-circle';
        case 'warning': return 'exclamation-triangle';
        default: return 'info-circle';
    }
}

// Handle responsive sidebar
function handleResponsiveSidebar() {
    const sidebar = document.getElementById('managerSidebar');
    const content = document.querySelector('.manager-content');
    
    if (window.innerWidth <= 768) {
        sidebar?.classList.remove('show');
    }
}

// Listen for window resize
window.addEventListener('resize', handleResponsiveSidebar);

// Export functions for global access
window.toggleSidebar = toggleSidebar;
window.toggleUserMenu = toggleUserMenu;
window.logout = logout;
window.showToast = showToast;
