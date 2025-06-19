// Language switching functionality - Updated to use LanguageUtils
let currentLang = 'vn';

// User authentication handling
window.onload = () => {
    const token = localStorage.getItem('token');
    const params = new URLSearchParams(window.location.search);
    if (!token) {
        if(params.get('isLogout')) {
            showSnackbar("Logout success", "success");
            const newUrl = `${window.location.pathname}`;
            window.history.replaceState({}, '', newUrl);
        }
        document.getElementById('headerDropdownMenu').
            innerHTML = `
            <div class="dropdown-menu" id="dropdownMenu"> 
                                  <button class="yellow-btn">Chuyến bay của tôi</button> 
                                     <button class="menu-item" onclick="signup()"> 
                                       <i class="fas fa-user-plus"></i> 
                                         Đăng ký 
                                     </button> 
                                  <button class="menu-item" onclick="login()"> 
                                        <i class="fas fa-sign-in-alt"></i> 
                                        Đăng nhập 
                                     </button> 
                                </div> 
            `;
    } else {
        if (params.get('isLoggingIn')) {
            showSnackbar("Login success", "success");
        }
        let decodedToken = parseJwtToken(token);
        if (decodedToken['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] == "Customer") {
            document.getElementById('headerDropdownMenu').
                innerHTML = `
            <div class="dropdown-menu" id="dropdownMenu">
                                    <button class="yellow-btn">Chuyến bay của tôi</button>
                                    <button class="menu-item" onclick="goToProfile()">
                                        <i class="fas fa-user"></i>
                                        Profile
                                    </button>
                                    <button class="menu-item" onclick="logout()">
                                        <i class="fas fa-sign-out-alt"></i>
                                        Đăng xuất
                                    </button>
                                </div>
            `;
        }
    }
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