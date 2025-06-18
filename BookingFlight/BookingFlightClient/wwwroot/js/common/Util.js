const host = window.location.hostname;
function parseJwtToken(token) {
    var base64Url = token.split('.')[1];
    var base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
    var jsonPayload = decodeURIComponent(window.atob(base64).split('').map(function (c) {
        return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
    }).join(''));
    var result = JSON.parse(jsonPayload);
    return result;
}

async function CheckAccess() {
    const token = localStorage.getItem('token');
    const headers = {};
    if (token) {
        headers['Authorization'] = 'Bearer ' + token;
    }
    headers['RequestPath'] = window.location.pathname;
    await fetch(`http://${host}:5189/api/auth`, {
        method: 'GET',
        headers: headers
    });
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

function formatDate(date) {
    const day = String(date.getDate()).padStart(2, '0');
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const year = date.getFullYear();
    return `${year}-${month}-${day}`;
}

function formatTime(dateString) {
    let date = new Date(dateString);
    const hours = String(date.getHours()).padStart(2, '0');
    const minutes = String(date.getMinutes()).padStart(2, '0');
    return `${hours}:${minutes}`;
}
