const host = window.location.hostname;
function parseJwtToken(token) {
    var base64Url = token.split('.')[1];
    var base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
    var jsonPayload = decodeURIComponent(window.atob(base64).split('').map(function (c) {
        return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
    }).join(''));
    return JSON.parse(jsonPayload);
}

function CheckAccess(roles) {
    let token = localStorage.getItem('token');
    if (!token) {
        return false;
    } else {
        var decodedToken = parseJwtToken(token);
        return roles.includes(decodedToken['http://schemas.microsoft.com/ws/2008/06/identity/claims/role']);
    }
}

async function isSucessRequestFreshToken() {
    let isSucess = true;
    try {
        let refreshTokenRes = await fetch(`http://${host}:5077/api/Login/RefreshToken`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                RefreshToken: localStorage.getItem('refreshToken')
            })
        });
        if (!refreshTokenRes.ok) {
            isSucess = false;
        } else {
            var data = await refreshTokenRes.json();
            localStorage.setItem('token', data);
        }
    } catch (e) {
        isSucess = false;
    }
    return isSucess;
}

