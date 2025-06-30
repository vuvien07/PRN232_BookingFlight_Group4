// Define host variable
const host = window.location.hostname;

console.log('Login.js file loading...');
console.log('Host variable set to:', host);

document.addEventListener("DOMContentLoaded", function () {
    console.log('Login.js DOMContentLoaded event fired');
    // Xử lý sự kiện change trên checkbox có class 'rememberMe'
    document.addEventListener("change", function (event) {
        if (event.target && event.target.classList.contains("rememberMe")) {
            var checkbox = event.target;
            var rememberMeInput = document.getElementById("rememberMe");
            if (rememberMeInput) {
                rememberMeInput.value = checkbox.checked ? true : false;
            }
        }
    });

    // Xử lý sự kiện submit form có id 'loginForm'
    var loginForm = document.getElementById("loginForm");
    if (loginForm) {
        loginForm.addEventListener("submit", function (event) {
            var rememberCheckbox = loginForm.querySelector(".rememberMe");
            var isRememberInput = loginForm.querySelector('input[name="IsRememberMe"]');

            if (rememberCheckbox && isRememberInput) {
                isRememberInput.value = rememberCheckbox.checked ? true : false;
            }
        });
    }
});

async function LoginToSystem(e) {
    console.log('=== LoginToSystem function called ===');
    console.log('Event object:', e);
    console.log('Event type:', e ? e.type : 'no event');
    
    console.log('LoginToSystem function called with event:', e);
    try {
        e.preventDefault();
        
        console.log('Login attempt started...');
        console.log('Host:', host);
        
        // Determine the correct protocol and API URL
        const protocol = window.location.protocol; // 'http:' or 'https:'
        const apiUrl = `${protocol}//${host}:5077/api/login`;
        console.log('API URL:', apiUrl);
        
        let labels = [
            'Username',
            'Password',]
            
        const username = document.querySelector('input[name="Username"]').value;
        const password = document.querySelector('input[name="Password"]').value;
        
        console.log('Username:', username);
        console.log('Password length:', password.length);
        
        const res = await fetch(apiUrl, {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
            },
            body: JSON.stringify({
                "Username": username,
                "Password": password
            }),
            credentials: "include"
        });
        
        console.log('Response status:', res.status);
        console.log('Response headers:', Object.fromEntries(res.headers));
        
        if (!res.ok) {
            console.log('Login failed with status:', res.status);
            let result;
            try {
                result = await res.json();
                console.log('Error response:', result);
            } catch (jsonError) {
                console.error('Failed to parse error response as JSON:', jsonError);
                const textResponse = await res.text();
                console.log('Raw error response:', textResponse);
                await showSnackbar("Server error occurred", "error");
                return;
            }
            
            if (result.errors) {
                let errors = result.errors;
                for (let i = 0; i < labels.length; i++) {
                    DisplayError(labels[i], errors);
                }
            }
            if (result.message) {
                await showSnackbar(result.message, "error");
            }
        } else {
            // Expecting token in response
            const json = await res.json();
            console.log('Login response:', json); // Debug log
            
            if (json.token) {
                // Store token first
                localStorage.setItem("token", json.token);
                
                // Also store token in cookie for server-side authentication
                document.cookie = `X-Access-Token=${json.token}; path=/; max-age=86400`; // 24 hours
                
                // Parse token to get user info
                let parseToken = parseJwtToken(json.token);
                console.log('Parsed token:', parseToken); // Debug log
                
                // Get role info
                const roleInfo = getRoleInfo(parseToken);
                console.log('Role info:', roleInfo); // Debug log
                
                // Show success message first
                await showSnackbar("Đăng nhập thành công!", "success");
                
                // Small delay to ensure localStorage is written
                await new Promise(resolve => setTimeout(resolve, 100));
                
                // Refresh header authentication UI immediately
                if (window.refreshAuthentication) {
                    console.log('Refreshing header authentication...');
                    window.refreshAuthentication();
                } else {
                    console.warn('refreshAuthentication function not available');
                }
                
                // Wait a moment for UI update before redirect
                await new Promise(resolve => setTimeout(resolve, 300));
                
                // Debug: Check role info
                console.log('About to redirect - roleId:', roleInfo.roleId, 'type:', typeof roleInfo.roleId);
                console.log('About to redirect - roleName:', roleInfo.roleName);
                console.log('Is admin check:', roleInfo.roleId == 1, roleInfo.roleId === 1, roleInfo.roleId === "1");
                console.log('Is manager check (roleId=4):', roleInfo.roleId == 4, roleInfo.roleId === 4, roleInfo.roleId === "4");
                console.log('Is supporter check (roleId=2):', roleInfo.roleId == 2, roleInfo.roleId === 2, roleInfo.roleId === "2");
                console.log('Role name check:', roleInfo.roleName?.toLowerCase());
                console.log('Token RoleId field:', parseToken["RoleId"]);
                console.log('Token role field:', parseToken["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"]);
                
                // Direct redirect based on role (CORRECTED based on actual database)
                // Database mapping: 1=Admin, 2=Supporter, 3=Customer, 4=Manager
                if (roleInfo.roleId == 1 || roleInfo.roleId === 1 || roleInfo.roleId === "1") {
                    console.log('Admin user detected (roleId = 1), redirecting to admin dashboard');
                    window.location.href = "/Admin/Dashboard?isLoggingIn=true";
                } else if (roleInfo.roleName && roleInfo.roleName.toLowerCase() === 'admin') {
                    console.log('Admin user detected by role name, redirecting to admin dashboard');
                    window.location.href = "/Admin/Dashboard?isLoggingIn=true";
                } else if (roleInfo.roleId == 4 || roleInfo.roleId === 4 || roleInfo.roleId === "4") {
                    console.log('Manager user detected (roleId = 4), redirecting to manager dashboard');
                    window.location.href = "/Manager/Dashboard?isLoggingIn=true";
                } else if (roleInfo.roleId == 2 || roleInfo.roleId === 2 || roleInfo.roleId === "2") {
                    console.log('Supporter user detected (roleId = 2), redirecting to manager dashboard');
                    window.location.href = "/Manager/Dashboard?isLoggingIn=true";
                } else if (roleInfo.roleName && (roleInfo.roleName.toLowerCase() === 'manager' || roleInfo.roleName.toLowerCase() === 'supporter')) {
                    console.log('Manager/Supporter user detected by role name, redirecting to manager dashboard');
                    window.location.href = "/Manager/Dashboard?isLoggingIn=true";
                } else {
                    console.log('Regular user detected, redirecting to home');
                    window.location.href = "/Home?isLoggingIn=true";
                }
            } else {
                // Fallback: redirect to home if no token
                console.warn('No token received, redirecting to home');
                window.location.href = "/Home?isLoggingIn=true";
            }
        }
    } catch (error) {
        console.error('Login error:', error);
        console.error('Error details:', {
            name: error.name,
            message: error.message,
            stack: error.stack
        });
        
        // Show user-friendly error message
        if (error.name === 'TypeError' && error.message.includes('fetch')) {
            await showSnackbar("Không thể kết nối đến server. Vui lòng kiểm tra kết nối mạng.", "error");
        } else if (error.name === 'TypeError' && error.message.includes('NetworkError')) {
            await showSnackbar("Lỗi mạng. Vui lòng thử lại.", "error");
        } else {
            await showSnackbar("Đã xảy ra lỗi không mong muốn. Vui lòng thử lại.", "error");
        }
    }
}

function DisplayError(id, errors) {
    if (!errors || !errors[id]) {
        document.getElementById(id).innerText = '';
        return;
    };
    document.getElementById(id).innerText = errors[id];
}

// Utility functions for Login page
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
    // Create snackbar element if it doesn't exist
    let snackbar = document.getElementById('snackbar');
    if (!snackbar) {
        snackbar = document.createElement('div');
        snackbar.id = 'snackbar';
        snackbar.className = 'snackbar';
        document.body.appendChild(snackbar);
    }
    
    // Set message and type
    snackbar.textContent = message;
    snackbar.className = `snackbar snackbar-${type}`;
    
    // Show snackbar
    snackbar.classList.add('show');
    
    // Hide snackbar after 3 seconds
    setTimeout(() => {
        snackbar.classList.remove('show');
    }, 3000);
}

// Add basic snackbar styles if not already present
if (!document.getElementById('snackbar-styles')) {
    const style = document.createElement('style');
    style.id = 'snackbar-styles';
    style.textContent = `
        .snackbar {
            visibility: hidden;
            min-width: 250px;
            margin-left: -125px;
            background-color: #333;
            color: #fff;
            text-align: center;
            border-radius: 2px;
            padding: 16px;
            position: fixed;
            z-index: 1001;
            left: 50%;
            bottom: 30px;
            font-size: 14px;
            border-radius: 10px;
            box-shadow: 0 4px 12px rgba(0,0,0,0.15);
            transition: all 0.3s ease;
        }
        
        .snackbar.show {
            visibility: visible;
            animation: fadein 0.5s, fadeout 0.5s 2.5s;
        }
        
        .snackbar-success {
            background: linear-gradient(135deg, #10b981, #059669);
        }
        
        .snackbar-error {
            background: linear-gradient(135deg, #ef4444, #dc2626);
        }
        
        .snackbar-info {
            background: linear-gradient(135deg, #3b82f6, #2563eb);
        }
        
        @keyframes fadein {
            from {bottom: 0; opacity: 0;}
            to {bottom: 30px; opacity: 1;}
        }
        
        @keyframes fadeout {
            from {bottom: 30px; opacity: 1;}
            to {bottom: 0; opacity: 0;}
        }
    `;
    document.head.appendChild(style);
}

// Function to redirect user based on role (legacy function - kept for compatibility)
function redirectBasedOnRole(roleId, roleName) {
    // Convert roleId to number if it's a string
    const numericRoleId = parseInt(roleId);
    
    console.log('Redirecting based on roleId:', numericRoleId, 'roleName:', roleName);
    
    switch (numericRoleId) {
        case 1:
            // Admin - redirect to Admin Dashboard
            console.log('Redirecting admin to dashboard');
            window.location.href = "/Admin/Dashboard?isLoggingIn=true";
            break;
        case 4:
            // Manager (roleId = 4) - redirect to Manager Dashboard
            console.log('Redirecting manager to manager dashboard');
            window.location.href = "/Manager/Dashboard?isLoggingIn=true";
            break;
        case 2:
            // Supporter (roleId = 2) - redirect to Manager Dashboard (shared with manager)
            console.log('Redirecting supporter to manager dashboard');
            window.location.href = "/Manager/Dashboard?isLoggingIn=true";
            break;
        case 3:
        case 5:
            // Other roles (Customer, etc.) - redirect to Home
            console.log('Redirecting user to home');
            window.location.href = "/Home?isLoggingIn=true";
            break;
        default:
            // Fallback for unknown roles
            console.warn('Unknown roleId:', numericRoleId, 'redirecting based on role name');
            
            // Try to check by role name as fallback
            if (roleName && roleName.toLowerCase() === 'admin') {
                console.log('Admin detected by role name, redirecting to dashboard');
                window.location.href = "/Admin/Dashboard?isLoggingIn=true";
            } else if (roleName && (roleName.toLowerCase() === 'manager' || roleName.toLowerCase() === 'supporter' || roleName.toLowerCase() === 'supporterd')) {
                console.log('Manager/Supporter detected by role name, redirecting to manager dashboard');
                window.location.href = "/Manager/Dashboard?isLoggingIn=true";
            } else {
                console.log('Defaulting to home page');
                window.location.href = "/Home?isLoggingIn=true";
            }
            break;
    }
}

// Enhanced role mapping for better role handling
function getRoleInfo(parseToken) {
    console.log('Getting role info from token:', parseToken);
    
    // Log all possible role-related fields
    console.log('All token fields:');
    Object.keys(parseToken).forEach(key => {
        console.log(`  ${key}:`, parseToken[key]);
    });
    
    // Extract role name from token
    const role = parseToken["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"];
    
    // Extract role ID from token - JWT service adds "RoleId" claim
    const roleId = parseToken["RoleId"] || 
                  parseToken["roleid"] || 
                  parseToken["role_id"] ||
                  parseToken["RoleID"] ||
                  parseToken["http://schemas.microsoft.com/ws/2008/06/identity/claims/roleId"];
    
    console.log('Extracted role:', role, 'roleId:', roleId, 'type of roleId:', typeof roleId);
    console.log('Available fields in token:', Object.keys(parseToken));
    console.log('Specific RoleId field value:', parseToken["RoleId"]);
    
    // Role mapping based on actual database structure
    // Database mapping: 1=Admin, 2=Supporter, 3=Customer, 4=Manager
    const roleMapping = {
        'admin': 1,
        'administrator': 1,
        'supporter': 2,      // Role ID 2 is Supporter
        'supporterd': 2,     // Handle the typo in backend Constants
        'customer': 3,       // Role ID 3 is Customer
        'user': 3,
        'manager': 4,        // Role ID 4 is Manager
        'staff': 3           // Default staff to customer role
    };
    
    // Use extracted roleId first, fall back to role name mapping
    let finalRoleId = roleId;
    if (!finalRoleId && role) {
        finalRoleId = roleMapping[role.toLowerCase()] || 3; // Default to customer (role 3) if not found
    }
    
    // Convert to number if it's a string
    if (finalRoleId && typeof finalRoleId === 'string') {
        finalRoleId = parseInt(finalRoleId);
    }
    
    const result = {
        roleId: finalRoleId,
        roleName: role,
        isAdmin: finalRoleId == 1 || (role && role.toLowerCase() === 'admin')
    };
    
    console.log('Final role info:', result);
    return result;
}

// Make sure LoginToSystem is available globally
console.log('Making LoginToSystem available globally...');
window.LoginToSystem = LoginToSystem;
console.log('LoginToSystem is now available as:', typeof window.LoginToSystem);

// Also expose other functions that might be needed
window.parseJwtToken = parseJwtToken;
window.showSnackbar = showSnackbar;
window.getRoleInfo = getRoleInfo;

console.log('Login.js file loaded completely');
console.log('All exposed functions:', {
    LoginToSystem: typeof window.LoginToSystem,
    parseJwtToken: typeof window.parseJwtToken,
    showSnackbar: typeof window.showSnackbar,
    getRoleInfo: typeof window.getRoleInfo
});
