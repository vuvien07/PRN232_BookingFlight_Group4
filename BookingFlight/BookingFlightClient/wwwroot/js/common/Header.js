// Language switching functionality - Updated to use LanguageUtils
let currentLang = 'vn';

// Legacy onload handler - replaced by DOMContentLoaded
// This is kept for compatibility but the main logic is now in initializeAuthentication()
window.onload = () => {
    // Additional initialization if needed
    console.log('Window loaded - authentication should already be initialized');
};

// Make toggleLanguage global
window.toggleLanguage = function() {
    console.log('toggleLanguage called');
    const dropdown = document.getElementById('languageDropdown');
    const selector = document.querySelector('.language-selector');
    
    if (dropdown && selector) {
        dropdown.classList.toggle('show');
        selector.classList.toggle('active');
        console.log('Dropdown toggled, show class:', dropdown.classList.contains('show'));
    } else {
        console.error('Elements not found - dropdown:', dropdown, 'selector:', selector);
    }
}

function switchLanguage(lang) {
    console.log('Header switchLanguage called with:', lang);
    
    // Use LanguageUtils if available
    if (window.LanguageUtils) {
        console.log('Using LanguageUtils for language switching');
        window.LanguageUtils.switchLanguage(lang);
        currentLang = lang;
        
        // Update current language display
        const currentLangSpan = document.getElementById('currentLang');
        if (currentLangSpan) {
            currentLangSpan.textContent = lang.toUpperCase();
        }
        
        // Update lang options active state
        updateLanguageOptionsState(lang);
        
        // Close dropdown
        closeLanguageDropdown();
    } else {        console.log('LanguageUtils not available, using fallback');
        // Fallback implementation
        currentLang = lang;
        
        // Update current language display
        const currentLangSpan = document.getElementById('currentLang');
        if (currentLangSpan) {
            currentLangSpan.textContent = lang.toUpperCase();
            console.log('Updated current language display to:', lang.toUpperCase());
        } else {
            console.error('currentLang element not found');
        }
          // Update all text elements with language data attributes
        const elements = document.querySelectorAll('[data-vn][data-en], [data-vn][data-en][data-zh]');
        console.log('Found elements with language data:', elements.length);
        
        elements.forEach((element, index) => {
            const text = element.getAttribute(`data-${lang}`);
            if (text && text.trim()) {
                const oldText = element.textContent;
                // Simple text update
                element.textContent = text;
                console.log(`Element ${index}:`, oldText, '->', text);
            }
        });
        
        // Update placeholder attributes for input elements
        const inputElements = document.querySelectorAll('input[placeholder]');
        console.log('Found input elements:', inputElements.length);
        inputElements.forEach(input => {
            const placeholderText = input.getAttribute(`data-placeholder-${lang}`);
            if (placeholderText) {
                input.placeholder = placeholderText;
                console.log('Updated placeholder for input:', placeholderText);
            }
        });
          // Update select option text
        const selectOptions = document.querySelectorAll('option[data-vn][data-en], option[data-vn][data-en][data-zh]');
        console.log('Found select options:', selectOptions.length);
        selectOptions.forEach(option => {
            const text = option.getAttribute(`data-${lang}`);
            if (text && text.trim()) {
                option.textContent = text;
            }
        });
        
        // Update lang options active state
        updateLanguageOptionsState(lang);
        
        // Update document language attribute
        document.documentElement.lang = lang === 'vn' ? 'vi' : 'en';
        
        // Close dropdown
        closeLanguageDropdown();
        
        // Save language preference
        try {
            localStorage.setItem('preferred-language', lang);
            console.log('Saved language preference:', lang);
        } catch (error) {
            console.error('Failed to save language preference:', error);
        }
        
        // Trigger custom event
        window.dispatchEvent(new CustomEvent('languageChanged', { 
            detail: { language: lang } 
        }));
    }
}

function closeLanguageDropdown() {
    console.log('closeLanguageDropdown called');
    const dropdown = document.getElementById('languageDropdown');
    const selector = document.querySelector('.language-selector');
    if (dropdown) {
        dropdown.classList.remove('show');
        console.log('Removed show class from dropdown');
    }
    if (selector) {
        selector.classList.remove('active');
        console.log('Removed active class from selector');
    }
}

// Initialize when DOM is loaded
document.addEventListener('DOMContentLoaded', function() {
    console.log('DOM loaded, initializing language selector...');
    
    // Test if elements exist
    const dropdown = document.getElementById('languageDropdown');
    const selector = document.querySelector('.language-selector');
    const currentLangSpan = document.getElementById('currentLang');
    
    console.log('Elements found:');
    console.log('- dropdown:', dropdown);
    console.log('- selector:', selector);
    console.log('- currentLangSpan:', currentLangSpan);
    
    // Initialize language options click handlers
    const langOptions = document.querySelectorAll('.lang-option');
    console.log('Found language options:', langOptions.length);
    
    langOptions.forEach((option, index) => {
        const lang = option.getAttribute('data-lang');
        console.log(`Language option ${index}:`, lang);
        
        option.addEventListener('click', function(e) {
            e.stopPropagation();
            console.log('Language option clicked:', lang);
            if (lang) {
                switchLanguage(lang);
            }
        });
    });
    
    // Load saved language preference (default to Vietnamese)
    let savedLang;
    try {
        savedLang = localStorage.getItem('preferred-language') || 'vn';
        console.log('Loading saved language:', savedLang);
    } catch (error) {
        console.error('Failed to load saved language:', error);
        savedLang = 'vn';
    }
    
    // Apply the saved language immediately
    switchLanguage(savedLang);
    
    // Test the toggle function
    console.log('Testing toggleLanguage function...');
    if (typeof window.toggleLanguage === 'function') {
        console.log('toggleLanguage function is available');
    } else {
        console.error('toggleLanguage function is NOT available');
    }
});

function updateLanguageOptionsState(activeLang) {
    console.log('updateLanguageOptionsState called with:', activeLang);
    document.querySelectorAll('.lang-option').forEach(option => {
        option.classList.remove('active');
        if (option.getAttribute('data-lang') === activeLang) {
            option.classList.add('active');
            console.log('Set active language option:', activeLang);
        }
    });
}

// Close dropdown when clicking outside
document.addEventListener('click', function(e) {
    if (!e.target.closest('.language-selector')) {
        console.log('Click outside language selector, closing dropdown');
        closeLanguageDropdown();
    }
});

// Handle escape key to close dropdown
document.addEventListener('keydown', function(e) {
    if (e.key === 'Escape') {
        console.log('Escape key pressed, closing dropdown');
        closeLanguageDropdown();
    }
});

// Expose utility functions globally
window.getCurrentLanguage = function() {
    return currentLang;
};

window.getAvailableLanguages = function() {
    return {
        'vn': { code: 'VN', name: 'Tiếng Việt', flag: 'vn' },
        'en': { code: 'EN', name: 'English', flag: 'us' },
        'zh': { code: 'ZH', name: '中文', flag: 'cn' }
    };
};

// Header scroll effect
let lastScrollTop = 0;
window.addEventListener('scroll', function() {
    const header = document.querySelector('.main-header');
    if (!header) return;
    
    const scrollTop = window.pageYOffset || document.documentElement.scrollTop;
    
    if (scrollTop > lastScrollTop && scrollTop > 100) {
        header.style.transform = 'translateY(-100%)';
    } else {
        header.style.transform = 'translateY(0)';
    }
    
    if (scrollTop > 50) {
        header.classList.add('scrolled');
    } else {
        header.classList.remove('scrolled');
    }
    
    lastScrollTop = scrollTop;
});

// Mobile menu toggle
function toggleMobileMenu() {
    const navMenu = document.getElementById('navMenu');
    if (navMenu) {
        navMenu.classList.toggle('active');
    }
}

// Authentication related functions
function login() {
    window.location.href = '/Login';
}

function signup() {
    window.location.href = '/Register';
}

function logout() {
    localStorage.removeItem('token');
    window.location.href = '/Home?isLogout=true';
}

function goToProfile() {
    window.location.href = '/Profile';
}

function parseJwtToken(token) {
    try {
        const base64Url = token.split('.')[1];
        const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
        const jsonPayload = decodeURIComponent(atob(base64).split('').map(function(c) {
            return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
        }).join(''));
        return JSON.parse(jsonPayload);
    } catch (e) {
        console.error('Error parsing JWT token:', e);
        return null;
    }
}

function showSnackbar(message, type = 'info') {
    // Create snackbar element
    const snackbar = document.createElement('div');
    snackbar.className = `snackbar snackbar-${type}`;
    snackbar.textContent = message;
    
    // Add to DOM
    document.body.appendChild(snackbar);
    
    // Show snackbar
    setTimeout(() => snackbar.classList.add('show'), 100);
    
    // Hide and remove snackbar
    setTimeout(() => {
        snackbar.classList.remove('show');
        setTimeout(() => document.body.removeChild(snackbar), 300);
    }, 3000);
}

// User dropdown menu functions
function toggleUserMenu() {
    const dropdown = document.getElementById('userDropdownMenu');
    const userBtn = document.querySelector('.user-btn');
    
    if (dropdown && userBtn) {
        dropdown.classList.toggle('show');
        userBtn.classList.toggle('active');
    }
}

function closeUserMenu() {
    const dropdown = document.getElementById('userDropdownMenu');
    const userBtn = document.querySelector('.user-btn');
    
    if (dropdown) {
        dropdown.classList.remove('show');
    }
    if (userBtn) {
        userBtn.classList.remove('active');
    }
}

function goToMyFlights() {
    window.location.href = '/MyFlights';
}

// Additional navigation functions
function goToBookingHistory() {
    window.location.href = '/BookingHistory';
}

function goToSettings() {
    window.location.href = '/Settings';
}

// Close user menu when clicking outside
document.addEventListener('click', function(e) {
    if (!e.target.closest('.user-dropdown')) {
        closeUserMenu();
    }
});

// Initialize authentication UI when DOM is ready
document.addEventListener('DOMContentLoaded', function() {
    console.log('Header DOM loaded, initializing authentication...');
    
    // Check if header dropdown menu exists
    const headerDropdown = document.getElementById('headerDropdownMenu');
    if (!headerDropdown) {
        console.error('Header dropdown menu not found!');
        return;
    }
    
    // Initialize authentication UI
    initializeAuthentication();
});

function initializeAuthentication() {
    const token = localStorage.getItem('token');
    const params = new URLSearchParams(window.location.search);
    
    console.log('Initializing authentication, token:', token ? 'exists' : 'not found');
    
    if (!token) {
        // User is not logged in - show simple login button
        if(params.get('isLogout')) {
            showSnackbar("Đăng xuất thành công", "success");
            const newUrl = `${window.location.pathname}`;
            window.history.replaceState({}, '', newUrl);
        }
        
        const headerDropdown = document.getElementById('headerDropdownMenu');
        if (headerDropdown) {
            headerDropdown.innerHTML = `
                <a href="/Login" class="login-btn">
                    <i class="fas fa-sign-in-alt"></i>
                    <span data-vn="Đăng nhập" data-en="Login" data-zh="登录">Đăng nhập</span>
                </a>
            `;
            
            // Apply current language to new elements
            if (window.LanguageUtils) {
                const currentLang = window.LanguageUtils.getCurrentLanguage();
                window.LanguageUtils.updateDOMElements(currentLang);
            }
        }
    } else {
        // User is logged in - show user menu
        if (params.get('isLoggingIn')) {
            showSnackbar("Đăng nhập thành công", "success");
            const newUrl = `${window.location.pathname}`;
            window.history.replaceState({}, '', newUrl);
        }
        
        let decodedToken = parseJwtToken(token);
        console.log('Decoded token:', decodedToken);
        
        if (decodedToken) {
            // Get username from token
            const username = decodedToken['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'] || 
                           decodedToken['sub'] || 
                           decodedToken['username'] || 
                           decodedToken['http://schemas.microsoft.com/ws/2008/06/identity/claims/name'] ||
                           'User';
            
            const role = decodedToken['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] || 'Guest';
            
            console.log('Username:', username, 'Role:', role);
            
            const headerDropdown = document.getElementById('headerDropdownMenu');
            if (headerDropdown) {
                headerDropdown.innerHTML = `
                    <div class="user-dropdown">
                        <button class="user-btn" onclick="toggleUserMenu()">
                            <div class="user-avatar">
                                <i class="fas fa-user-circle"></i>
                            </div>
                            <div class="user-info">
                                <span class="user-name">${username}</span>
                                <span class="user-role" data-vn="${getRoleDisplayName(role, 'vn')}" data-en="${getRoleDisplayName(role, 'en')}" data-zh="${getRoleDisplayName(role, 'zh')}">${getRoleDisplayName(role, 'vn')}</span>
                            </div>
                            <i class="fas fa-chevron-down dropdown-icon"></i>
                        </button>
                        <div class="user-dropdown-menu" id="userDropdownMenu">
                            <div class="dropdown-header">
                                <div class="user-avatar-large">
                                    <i class="fas fa-user-circle"></i>
                                </div>
                                <div class="user-details">
                                    <span class="user-name-large">${username}</span>
                                    <span class="user-email" data-vn="${getRoleDisplayName(role, 'vn')}" data-en="${getRoleDisplayName(role, 'en')}" data-zh="${getRoleDisplayName(role, 'zh')}">${getRoleDisplayName(role, 'vn')}</span>
                                </div>
                            </div>
                            <div class="dropdown-divider"></div>
                            <button class="menu-item" onclick="goToProfile()">
                                <i class="fas fa-user"></i>
                                <span data-vn="Hồ sơ cá nhân" data-en="Profile" data-zh="个人资料">Hồ sơ cá nhân</span>
                            </button>
                            <button class="menu-item" onclick="goToMyFlights()">
                                <i class="fas fa-plane"></i>
                                <span data-vn="Chuyến bay của tôi" data-en="My Flights" data-zh="我的航班">Chuyến bay của tôi</span>
                            </button>
                            <button class="menu-item" onclick="goToBookingHistory()">
                                <i class="fas fa-history"></i>
                                <span data-vn="Lịch sử đặt vé" data-en="Booking History" data-zh="预订历史">Lịch sử đặt vé</span>
                            </button>
                            <button class="menu-item" onclick="goToSettings()">
                                <i class="fas fa-cog"></i>
                                <span data-vn="Cài đặt" data-en="Settings" data-zh="设置">Cài đặt</span>
                            </button>
                            <div class="dropdown-divider"></div>
                            <button class="menu-item logout-item" onclick="logout()">
                                <i class="fas fa-sign-out-alt"></i>
                                <span data-vn="Đăng xuất" data-en="Logout" data-zh="登出">Đăng xuất</span>
                            </button>
                        </div>
                    </div>
                `;
                
                // Apply current language to new elements
                if (window.LanguageUtils) {
                    const currentLang = window.LanguageUtils.getCurrentLanguage();
                    window.LanguageUtils.updateDOMElements(currentLang);
                }
            }
        } else {
            // Token is invalid, remove it and show login button
            console.error('Invalid token, removing from localStorage');
            localStorage.removeItem('token');
            initializeAuthentication(); // Recursive call to show login button
        }
    }
}

// Helper function to get role display name
function getRoleDisplayName(role, lang) {
    const roleMap = {
        'Customer': {
            vn: 'Khách hàng',
            en: 'Customer',
            zh: '客户'
        },
        'Admin': {
            vn: 'Quản trị viên',
            en: 'Administrator',
            zh: '管理员'
        },
        'Manager': {
            vn: 'Quản lý',
            en: 'Manager', 
            zh: '经理'
        },
        'Staff': {
            vn: 'Nhân viên',
            en: 'Staff',
            zh: '员工'
        },
        'Guest': {
            vn: 'Khách',
            en: 'Guest',
            zh: '客人'
        }
    };
    
    return roleMap[role] ? roleMap[role][lang] : role;
}