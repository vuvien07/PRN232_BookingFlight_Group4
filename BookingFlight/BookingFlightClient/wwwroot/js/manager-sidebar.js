// Manager Sidebar JavaScript
document.addEventListener('DOMContentLoaded', function() {
    initializeSidebar();
    initializeResponsiveBehavior();
    initializeMenuItemAnimations();
});

function initializeSidebar() {
    console.log('Initializing manager sidebar...');
    
    // Set active menu item based on current URL
    setActiveMenuItem();
    
    // Load sidebar collapse state from localStorage
    const isCollapsed = localStorage.getItem('sidebarCollapsed') === 'true';
    if (isCollapsed) {
        toggleSidebar(false);
    }
    
    // Initialize tooltips for collapsed sidebar
    initializeTooltips();
}

function toggleSidebar(save = true) {
    const sidebar = document.getElementById('managerSidebar');
    const content = document.querySelector('.manager-content');
    
    if (!sidebar) return;
    
    const isCollapsed = sidebar.classList.contains('collapsed');
    
    if (isCollapsed) {
        // Expand sidebar
        sidebar.classList.remove('collapsed');
        if (content) {
            content.style.marginLeft = '280px';
        }
        if (save) {
            localStorage.setItem('sidebarCollapsed', 'false');
        }
    } else {
        // Collapse sidebar
        sidebar.classList.add('collapsed');
        if (content) {
            content.style.marginLeft = '70px';
        }
        if (save) {
            localStorage.setItem('sidebarCollapsed', 'true');
        }
    }
    
    // Trigger custom event for other components
    window.dispatchEvent(new CustomEvent('sidebarToggled', {
        detail: { collapsed: !isCollapsed }
    }));
}

function setActiveMenuItem() {
    const currentPath = window.location.pathname.toLowerCase();
    const menuLinks = document.querySelectorAll('.nav-menu .nav-link');
    
    menuLinks.forEach(link => {
        link.classList.remove('active');
        
        const linkPath = link.getAttribute('href');
        if (linkPath && currentPath.includes(linkPath.toLowerCase())) {
            link.classList.add('active');
        }
    });
}

function initializeResponsiveBehavior() {
    const sidebar = document.getElementById('managerSidebar');
    const content = document.querySelector('.manager-content');
    
    function handleResize() {
        const isMobile = window.innerWidth <= 768;
        
        if (isMobile) {
            // Mobile behavior
            sidebar?.classList.remove('collapsed');
            if (content) {
                content.style.marginLeft = '0';
            }
        } else {
            // Desktop behavior
            const isCollapsed = localStorage.getItem('sidebarCollapsed') === 'true';
            if (isCollapsed) {
                sidebar?.classList.add('collapsed');
                if (content) {
                    content.style.marginLeft = '70px';
                }
            } else {
                sidebar?.classList.remove('collapsed');
                if (content) {
                    content.style.marginLeft = '280px';
                }
            }
        }
    }
    
    window.addEventListener('resize', handleResize);
    handleResize(); // Initial call
    
    // Close mobile sidebar when clicking outside
    document.addEventListener('click', function(e) {
        if (window.innerWidth <= 768) {
            const sidebar = document.getElementById('managerSidebar');
            const isClickInsideSidebar = sidebar?.contains(e.target);
            const isToggleButton = e.target.closest('.mobile-sidebar-toggle') || e.target.closest('.sidebar-toggle');
            
            if (!isClickInsideSidebar && !isToggleButton && sidebar?.classList.contains('mobile-open')) {
                sidebar.classList.remove('mobile-open');
            }
        }
    });
}

function toggleMobileSidebar() {
    const sidebar = document.getElementById('managerSidebar');
    if (sidebar) {
        sidebar.classList.toggle('mobile-open');
    }
}

function initializeMenuItemAnimations() {
    const menuItems = document.querySelectorAll('.nav-menu .nav-link');
    
    menuItems.forEach((item, index) => {
        // Add staggered animation delay
        item.style.setProperty('--animation-delay', `${index * 0.1}s`);
        
        // Add click effect
        item.addEventListener('click', function(e) {
            // Add loading state for navigation
            if (!this.classList.contains('active')) {
                this.classList.add('loading');
                
                // Remove loading state after navigation (fallback)
                setTimeout(() => {
                    this.classList.remove('loading');
                }, 2000);
            }
        });
        
        // Add hover sound effect (optional)
        item.addEventListener('mouseenter', function() {
            // You can add sound effects here if needed
            this.style.transform = 'translateX(5px)';
        });
        
        item.addEventListener('mouseleave', function() {
            this.style.transform = 'translateX(0)';
        });
    });
}

function initializeTooltips() {
    const menuItems = document.querySelectorAll('.nav-menu .nav-link');
    
    menuItems.forEach(item => {
        const text = item.querySelector('span')?.textContent;
        if (text) {
            item.setAttribute('title', text);
            item.setAttribute('data-bs-toggle', 'tooltip');
            item.setAttribute('data-bs-placement', 'right');
        }
    });
    
    // Initialize Bootstrap tooltips if available
    if (typeof bootstrap !== 'undefined' && bootstrap.Tooltip) {
        const tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
        tooltipTriggerList.map(function (tooltipTriggerEl) {
            return new bootstrap.Tooltip(tooltipTriggerEl);
        });
    }
}

// Role-based menu customization
function customizeMenuForRole(roleId) {
    const menuItems = document.querySelectorAll('.nav-menu .nav-item');
    
    menuItems.forEach(item => {
        const link = item.querySelector('.nav-link');
        if (!link) return;
        
        // Add role-specific data attributes
        if (roleId === 4) { // Manager
            item.setAttribute('data-role', 'manager');
        } else if (roleId === 2) { // Supporter
            item.setAttribute('data-role', 'supporter');
        }
        
        // Add role-specific styling or behavior
        if (roleId === 4 && link.textContent.includes('Support')) {
            item.style.order = '999'; // Move support items to bottom for managers
        }
    });
}

// Sidebar search functionality
function initializeSidebarSearch() {
    const searchContainer = document.createElement('div');
    searchContainer.className = 'sidebar-search';
    searchContainer.innerHTML = `
        <div class="search-input-container">
            <input type="text" id="sidebarSearch" placeholder="Search menu..." class="form-control">
            <i class="fas fa-search search-icon"></i>
        </div>
    `;
    
    const sidebarContent = document.querySelector('.sidebar-content');
    if (sidebarContent) {
        sidebarContent.insertBefore(searchContainer, sidebarContent.firstChild);
    }
    
    const searchInput = document.getElementById('sidebarSearch');
    if (searchInput) {
        searchInput.addEventListener('input', function(e) {
            const query = e.target.value.toLowerCase();
            filterMenuItems(query);
        });
    }
}

function filterMenuItems(query) {
    const menuItems = document.querySelectorAll('.nav-menu .nav-item');
    
    menuItems.forEach(item => {
        const link = item.querySelector('.nav-link');
        const text = link?.textContent.toLowerCase() || '';
        
        if (text.includes(query) || query === '') {
            item.style.display = 'block';
        } else {
            item.style.display = 'none';
        }
    });
}

// Utility functions
function highlightActiveSection(sectionName) {
    const menuLinks = document.querySelectorAll('.nav-menu .nav-link');
    menuLinks.forEach(link => {
        link.classList.remove('active');
        if (link.textContent.toLowerCase().includes(sectionName.toLowerCase())) {
            link.classList.add('active');
        }
    });
}

function addNotificationBadge(menuItemText, count) {
    const menuItems = document.querySelectorAll('.nav-menu .nav-link');
    menuItems.forEach(link => {
        if (link.textContent.includes(menuItemText)) {
            let badge = link.querySelector('.badge');
            if (!badge) {
                badge = document.createElement('span');
                badge.className = 'badge';
                link.appendChild(badge);
            }
            badge.textContent = count;
            badge.style.display = count > 0 ? 'inline-block' : 'none';
        }
    });
}

// Export functions for global use
window.toggleSidebar = toggleSidebar;
window.toggleMobileSidebar = toggleMobileSidebar;
window.highlightActiveSection = highlightActiveSection;
window.addNotificationBadge = addNotificationBadge;
window.customizeMenuForRole = customizeMenuForRole;

// Auto-hide mobile sidebar on navigation
window.addEventListener('beforeunload', function() {
    const sidebar = document.getElementById('managerSidebar');
    if (sidebar && window.innerWidth <= 768) {
        sidebar.classList.remove('mobile-open');
    }
});

console.log('Manager sidebar JavaScript loaded successfully');
