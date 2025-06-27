document.addEventListener("DOMContentLoaded", function () {
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
    try {
        e.preventDefault();
        let labels = [
            'Username',
            'Password',]
        const res = await fetch(`http://${host}:5077/api/login`, {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
            },
            body: JSON.stringify({
                "Username": document.querySelector('input[name="Username"]').value,
                "Password": document.querySelector('input[name="Password"]').value
            }),
            credentials: "include"
        });
        if (!res.ok) {
            const result = await res.json();
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
            window.location.href = "/Home?isLoggingIn=true";

            //localStorage.setItem("token", json.token);
            //let parseToken = parseJwtToken(json.token);
            
            //// Get role information using enhanced function
            //const roleInfo = getRoleInfo(parseToken);
            
            //console.log('Login successful - Role Info:', roleInfo);
            
            //// First call AfterLogin to update session
            //await updateSessionAndRedirect(roleInfo);
        }
    } catch (error) {
        console.log(error);
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

// Function to redirect user based on role
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
        case 2:
        case 3:
        case 4:
            // Other roles (Manager, Staff, Customer) - redirect to Home
            console.log('Redirecting user to home');
            window.location.href = "/Home?isLoggingIn=true";
            break;
        default:
            // Fallback for unknown roles
            console.warn('Unknown roleId:', numericRoleId, 'redirecting to home');
            
            // Try to check by role name as fallback
            if (roleName && roleName.toLowerCase() === 'admin') {
                window.location.href = "/Admin/Dashboard?isLoggingIn=true";
            } else {
                window.location.href = "/Home?isLoggingIn=true";
            }
            break;
    }
}

// Enhanced role mapping for better role handling
function getRoleInfo(parseToken) {
    const role = parseToken["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"];
    const roleId = parseToken["RoleId"] || 
                  parseToken["roleid"] || 
                  parseToken["role_id"] ||
                  parseToken["http://schemas.microsoft.com/ws/2008/06/identity/claims/roleId"];
    
    // Role mapping based on common role names
    const roleMapping = {
        'admin': 1,
        'administrator': 1,
        'manager': 2,
        'staff': 3,
        'customer': 4,
        'user': 4
    };
    
    // If no roleId found, try to infer from role name
    let finalRoleId = roleId;
    if (!finalRoleId && role) {
        finalRoleId = roleMapping[role.toLowerCase()] || 4; // Default to customer
    }
    
    return {
        roleId: finalRoleId,
        roleName: role,
        isAdmin: finalRoleId == 1 || (role && role.toLowerCase() === 'admin')
    };
}

// Function to force session update and then redirect
async function updateSessionAndRedirect(roleInfo) {
    try {
        // Call AfterLogin to update session
        const updateResponse = await fetch(`http://${host}:5189/api/AfterLogin?role=${roleInfo.roleName}`, {
            method: 'GET',
            credentials: 'include', // Include cookies/session
            headers: {
                'Accept': 'application/json',
                'Authorization': `Bearer ${localStorage.getItem('token')}`
            }
        });
        
        if (!updateResponse.ok) {
            throw new Error(`Session update failed: ${updateResponse.status}`);
        }
        
        const result = await updateResponse.json();
        console.log('Session updated successfully:', result);
        
        // Wait for session to be persisted
        await new Promise(resolve => setTimeout(resolve, 2000)); // Increased wait time
        
        // For debugging - check current session state
        console.log('Redirecting user with roleId:', roleInfo.roleId, 'roleName:', roleInfo.roleName);
        
        // Force a page reload to ensure session is read properly
        if (roleInfo.roleId == 1 || roleInfo.roleName?.toLowerCase() === 'admin') {
            console.log('Redirecting to Admin Dashboard');
            window.location.replace("/Admin/Dashboard?isLoggingIn=true");
        } else {
            console.log('Redirecting to Home');
            window.location.replace("/Home?isLoggingIn=true");
        }
        
    } catch (error) {
        console.error('Error updating session:', error);
        // Fallback redirect with role info in URL for debugging
        const roleParam = roleInfo.roleName ? `&role=${roleInfo.roleName}` : '';
        window.location.href = `/Home?isLoggingIn=true${roleParam}`;
    }
}
